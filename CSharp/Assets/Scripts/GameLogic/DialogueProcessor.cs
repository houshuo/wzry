namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class DialogueProcessor : MonoSingleton<DialogueProcessor>
    {
        private const float AlphaMax = 1f;
        private const float AlphaMin = 0.2f;
        [NonSerialized]
        public float AutoNextPageTime = 3f;
        public bool bAutoNextPage;
        private static string[] BlackDialogBgPaths = new string[] { "ClickFg", "Txt_Dialog", "CharacterName" };
        private const float FadeInTime = 0.5f;
        private const float FadeOutTime = 0.5f;
        private static string[] ImageDialogBgPaths = new string[] { "Bg_Down", "Bg_Down/ImageLine", "Bg_Down/ImageArrow", "Txt_Dialog", "CharacterName" };
        private const string m_3dPortraitAnimDefault = "Idleshow";
        private GameObject m_3dPortraitName;
        private const string m_3dPortraitPanelName = "PanelCenter";
        private const string m_3dPortraitRawImgName = "3DImage";
        private SActorLineNode[] m_actorLines;
        private HashSet<object> m_actorLinesRaw;
        private string m_ageActName;
        private ListView<GameObject> m_bgGoList = new ListView<GameObject>();
        private bool m_bIsPlayingAutoNext;
        private const string m_contentGoName = "Txt_Dialog";
        private PoolObjHandle<AGE.Action> m_curAction = new PoolObjHandle<AGE.Action>();
        private int m_curIndex = -1;
        private CUIFormScript m_curUiForm;
        private CUIFormScript m_curUiFormPortrait;
        private Coroutine m_fadingCoroutine;
        private const string m_imgGoName = "Pic_Npc";
        private const string m_nameGoName = "CharacterName";
        private float m_nextPageProgressTime;
        private DictionaryView<int, CUIFormScript> m_uiFormMap = new DictionaryView<int, CUIFormScript>();
        private const float NextPageDelay = 0.2f;
        private DictionaryView<GameObject, Coroutine> Portrait3dAnimCoMap = new DictionaryView<GameObject, Coroutine>();
        private int PreDialogId;

        private void ClearUiForms()
        {
            DictionaryView<int, CUIFormScript>.Enumerator enumerator = this.m_uiFormMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, CUIFormScript> current = enumerator.Current;
                Singleton<CUIManager>.GetInstance().CloseForm(current.Value);
            }
            this.m_uiFormMap.Clear();
            this.m_curUiForm = null;
            if (this.m_curUiFormPortrait != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(this.m_curUiFormPortrait);
                this.m_curUiFormPortrait = null;
            }
        }

        private void DoNextPageInternal()
        {
            SActorLineNode node = this.m_actorLines[this.m_curIndex];
            SActorLineNode preNode = new SActorLineNode {
                CharCfgId = -1
            };
            if (this.m_curIndex > 0)
            {
                preNode = this.m_actorLines[this.m_curIndex - 1];
            }
            CUIFormScript script = null;
            if (node.DialogStyle >= 0)
            {
                script = this.QueryUiForm(node.DialogStyle);
            }
            if ((this.m_curUiForm != script) && (script != null))
            {
                if (this.m_curUiForm != null)
                {
                    this.m_curUiForm.Hide(enFormHideFlag.HideByCustom, true);
                    if (this.m_curUiFormPortrait != null)
                    {
                        this.m_curUiFormPortrait.Hide(enFormHideFlag.HideByCustom, true);
                    }
                }
                this.m_curUiForm = script;
                if (this.m_curUiForm != null)
                {
                    this.m_curUiForm.Appear(enFormHideFlag.HideByCustom, true);
                    if ((node.DialogStyle == 0) && (this.m_curUiFormPortrait != null))
                    {
                        this.m_curUiFormPortrait.Appear(enFormHideFlag.HideByCustom, true);
                    }
                }
            }
            if (this.m_curUiForm != null)
            {
                string[] strArray = (node.DialogStyle != 1) ? ImageDialogBgPaths : BlackDialogBgPaths;
                foreach (string str in strArray)
                {
                    Transform transform = this.m_curUiForm.gameObject.transform.FindChild(str);
                    if (transform != null)
                    {
                        this.m_bgGoList.Add(transform.gameObject);
                    }
                }
                this.m_curUiForm.gameObject.transform.Find("Txt_Dialog").gameObject.GetComponent<Text>().text = node.DialogContent;
                this.m_curUiForm.gameObject.transform.Find("CharacterName").gameObject.GetComponent<Text>().text = node.DialogTitle;
                if (node.b3dPortrait)
                {
                    this.UpdatePortrait3d(true, node, preNode);
                    this.UpdatePortraitImg(false, node, preNode);
                }
                else
                {
                    this.UpdatePortrait3d(false, node, preNode);
                    this.UpdatePortraitImg(true, node, preNode);
                }
            }
            if (node.bFadeIn)
            {
                this.m_fadingCoroutine = base.StartCoroutine(this.StartFadingIn());
            }
            else
            {
                this.UpdateBgImageAlpha(1f);
            }
        }

        private void EndDialogue()
        {
            if (this.m_actorLines != null)
            {
                this.m_actorLines = null;
            }
            if (this.m_curUiFormPortrait != null)
            {
                Transform transform = this.m_curUiFormPortrait.gameObject.transform.FindChild("PanelCenter");
                if (transform != null)
                {
                    Transform transform2 = transform.FindChild("3DImage");
                    if (transform2 != null)
                    {
                        transform2.gameObject.GetComponent<CUI3DImageScript>().RemoveGameObject(this.m_3dPortraitName);
                    }
                }
            }
            this.OnDestroyPortrait(this.m_3dPortraitName);
            this.m_3dPortraitName = null;
            this.Portrait3dAnimCoMap.Clear();
            this.m_actorLinesRaw = null;
            this.m_curIndex = -1;
            Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
        }

        private void EndDialogueComplete()
        {
            if (this.bAutoNextPage)
            {
                this.m_nextPageProgressTime = 0f;
                this.m_bIsPlayingAutoNext = false;
            }
            this.m_bgGoList.Clear();
            if (this.m_fadingCoroutine != null)
            {
                base.StopCoroutine(this.m_fadingCoroutine);
                this.m_fadingCoroutine = null;
            }
            this.EndDialogue();
            this.ClearUiForms();
        }

        protected override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Dialogue_NextPage, new CUIEventManager.OnUIEventHandler(this.NextPage));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Dialogue_SkipPages, new CUIEventManager.OnUIEventHandler(this.SkipPages));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        private bool IsFading()
        {
            return (this.m_fadingCoroutine != null);
        }

        public bool IsInDialog()
        {
            return (this.m_actorLines != null);
        }

        private void NextPage(CUIEvent inUiEvent)
        {
            if (!this.IsFading())
            {
                this.NextPageInternal();
            }
        }

        private bool NextPageInternal()
        {
            if (this.m_actorLines == null)
            {
                this.EndDialogueComplete();
                return true;
            }
            if (++this.m_curIndex >= this.m_actorLines.Length)
            {
                SActorLineNode node = this.m_actorLines[this.m_curIndex - 1];
                if (!node.bFadeOut)
                {
                    this.EndDialogueComplete();
                }
                else
                {
                    this.m_fadingCoroutine = base.StartCoroutine(this.StartFadingOut());
                }
                return true;
            }
            if (this.bAutoNextPage)
            {
                this.m_nextPageProgressTime = 0f;
            }
            SActorLineNode node2 = new SActorLineNode {
                CharCfgId = -1
            };
            if (this.m_curIndex > 0)
            {
                node2 = this.m_actorLines[this.m_curIndex - 1];
                if (node2.bFadeOut)
                {
                    this.m_fadingCoroutine = base.StartCoroutine(this.StartFadingOut());
                    return false;
                }
            }
            this.DoNextPageInternal();
            return false;
        }

        [DebuggerHidden]
        private IEnumerator NextPageNoTimeScale()
        {
            return new <NextPageNoTimeScale>c__IteratorB { <>f__this = this };
        }

        private void OnActionStoped(ref PoolObjHandle<AGE.Action> action)
        {
            if (action != 0)
            {
                action.handle.onActionStop -= new ActionStopDelegate(this.OnActionStoped);
                if (action == this.m_curAction)
                {
                    this.m_curAction.Release();
                    this.m_ageActName = null;
                }
            }
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            if ((prm.src.handle.CharInfo != null) && (prm.src.handle.CharInfo.DyingDialogGroupId > 0))
            {
                GameObject inAtker = (prm.orignalAtker == 0) ? null : prm.orignalAtker.handle.gameObject;
                this.PlayDrama(prm.src.handle.CharInfo.DyingDialogGroupId, prm.src.handle.gameObject, inAtker, false);
            }
        }

        protected override void OnDestroy()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Dialogue_NextPage, new CUIEventManager.OnUIEventHandler(this.NextPage));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Dialogue_SkipPages, new CUIEventManager.OnUIEventHandler(this.SkipPages));
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            base.OnDestroy();
        }

        private void OnDestroyPortrait(GameObject inPrefabObj)
        {
            if ((inPrefabObj != null) && this.Portrait3dAnimCoMap.ContainsKey(inPrefabObj))
            {
                base.StopCoroutine(this.Portrait3dAnimCoMap[inPrefabObj]);
                this.Portrait3dAnimCoMap.Remove(inPrefabObj);
            }
        }

        private void OnPortrait3dAnimComplete()
        {
            if ((this.m_curUiFormPortrait != null) && (this.m_curUiFormPortrait.gameObject.transform.FindChild("PanelCenter") != null))
            {
                GameObject key = this.m_3dPortraitName;
                if (key != null)
                {
                    Animation component = key.GetComponent<Animation>();
                    if (component != null)
                    {
                        Coroutine coroutine = base.StartCoroutine(PlayAnimNoTimeScale(component, "Idleshow", true, null));
                        if (this.Portrait3dAnimCoMap.ContainsKey(key))
                        {
                            this.Portrait3dAnimCoMap[key] = coroutine;
                        }
                        else
                        {
                            this.Portrait3dAnimCoMap.Add(key, coroutine);
                        }
                    }
                }
            }
        }

        public static AGE.Action PlayAgeAction(string inActionName, string inHelperName, GameObject inSrc, GameObject inAtker, ActionStopDelegate inCallback = null, int inHelperIndex = -1)
        {
            ActionHelperStorage storage = null;
            ActionHelper component = null;
            if (inSrc != null)
            {
                component = inSrc.GetComponent<ActionHelper>();
            }
            if ((component == null) && (inAtker != null))
            {
                component = inAtker.GetComponent<ActionHelper>();
            }
            if (((component != null) && (inHelperName != null)) && (inHelperName.Length > 0))
            {
                storage = component.GetAction(inHelperName);
            }
            if (((storage == null) && (inHelperIndex >= 0)) && (component != null))
            {
                storage = component.GetAction(inHelperIndex);
            }
            GameObject obj2 = inSrc;
            GameObject obj3 = inAtker;
            AGE.Action action = null;
            if (storage != null)
            {
                int num = (storage.targets.Length >= 2) ? storage.targets.Length : 2;
                GameObject[] objArray = new GameObject[num];
                if (storage.targets.Length == 0)
                {
                    objArray[0] = obj2;
                    objArray[1] = obj3;
                }
                else if (storage.targets.Length == 1)
                {
                    if (storage.targets[0] == null)
                    {
                        objArray[0] = obj2;
                    }
                    else
                    {
                        objArray[0] = storage.targets[0];
                    }
                    objArray[1] = obj3;
                }
                else
                {
                    if (storage.targets[0] == null)
                    {
                        objArray[0] = obj2;
                    }
                    else
                    {
                        objArray[0] = storage.targets[0];
                    }
                    if (storage.targets[1] == null)
                    {
                        objArray[1] = obj3;
                    }
                    else
                    {
                        objArray[1] = storage.targets[1];
                    }
                    for (int i = 2; i < num; i++)
                    {
                        objArray[i] = storage.targets[i];
                    }
                }
                storage.autoPlay = true;
                action = storage.PlayActionEx(objArray);
            }
            if (((action == null) && (inActionName != null)) && (inActionName.Length > 0))
            {
                GameObject[] objArray1 = new GameObject[] { obj2, obj3 };
                action = ActionManager.Instance.PlayAction(inActionName, true, false, objArray1);
            }
            if ((action != null) && (inCallback != null))
            {
                action.onActionStop += inCallback;
            }
            return action;
        }

        private bool PlayAgeActionInternal(GameObject inSrc, GameObject inAtker, int inGroupId)
        {
            if ((this.m_curAction != 0) && !ActionManager.Instance.IsActionValid((AGE.Action) this.m_curAction))
            {
                return false;
            }
            this.m_curAction = new PoolObjHandle<AGE.Action>(PlayAgeAction(this.m_ageActName, this.m_ageActName, inSrc, inAtker, new ActionStopDelegate(this.OnActionStoped), -1));
            if (this.m_curAction != 0)
            {
                this.m_curAction.handle.refParams.AddRefParam("DialogGroupIdRaw", inGroupId);
            }
            return (this.m_curAction != 0);
        }

        [DebuggerHidden]
        public static IEnumerator PlayAnimNoTimeScale(Animation animation, string clipName, bool bLoop, System.Action onComplete)
        {
            return new <PlayAnimNoTimeScale>c__IteratorC { animation = animation, clipName = clipName, bLoop = bLoop, onComplete = onComplete, <$>animation = animation, <$>clipName = clipName, <$>bLoop = bLoop, <$>onComplete = onComplete };
        }

        public void PlayDrama(int inGroupId, GameObject inSrc, GameObject inAtker, bool bDialogTriggerStart = false)
        {
            this.EndDialogue();
            this.m_actorLinesRaw = GameDataMgr.actorLinesDatabin.GetDataByKey(inGroupId);
            if (this.m_actorLinesRaw != null)
            {
                ResActorLinesInfo dataByKeySingle = GameDataMgr.actorLinesDatabin.GetDataByKeySingle((uint) inGroupId);
                if (dataByKeySingle != null)
                {
                    if (bDialogTriggerStart)
                    {
                        this.PreDialogId = inGroupId;
                    }
                    this.m_ageActName = StringHelper.UTF8BytesToString(ref dataByKeySingle.szAgeActionName);
                    if ((this.m_ageActName != null) && (this.m_ageActName.Length > 0))
                    {
                        this.PlayAgeActionInternal(inSrc, inAtker, inGroupId);
                    }
                    else
                    {
                        this.StartDialogue(inGroupId);
                    }
                }
            }
        }

        public bool PrepareFight()
        {
            return false;
        }

        private string QueryDialogTempPath(int inDialogStyle)
        {
            int num = inDialogStyle;
            if ((num != 0) && (num == 1))
            {
                return "UGUI/Form/System/Dialog/Form_DialogBlack";
            }
            return "UGUI/Form/System/Dialog/Form_NpcDialog";
        }

        private CUIFormScript QueryUiForm(int inDialogStyle)
        {
            if (this.m_uiFormMap.ContainsKey(inDialogStyle))
            {
                return this.m_uiFormMap[inDialogStyle];
            }
            string formPath = this.QueryDialogTempPath(inDialogStyle);
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(formPath, true, false);
            DebugHelper.Assert(script != null);
            if (script != null)
            {
                this.m_uiFormMap.Add(inDialogStyle, script);
                script.Hide(enFormHideFlag.HideByCustom, true);
                if (inDialogStyle == 0)
                {
                    this.m_curUiFormPortrait = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Dialog/Form_NpcDialogPortrait", true, false);
                    if (this.m_curUiFormPortrait != null)
                    {
                        this.m_curUiFormPortrait.Hide(enFormHideFlag.HideByCustom, true);
                    }
                }
            }
            return script;
        }

        private void SkipPages(CUIEvent inUiEvent)
        {
            this.EndDialogueComplete();
        }

        public void StartDialogue(int inGroupId)
        {
            string ageActName = this.m_ageActName;
            this.EndDialogue();
            this.m_ageActName = ageActName;
            this.m_actorLinesRaw = GameDataMgr.actorLinesDatabin.GetDataByKey(inGroupId);
            if (this.m_actorLinesRaw != null)
            {
                ResActorLinesInfo dataByKeySingle = GameDataMgr.actorLinesDatabin.GetDataByKeySingle((uint) inGroupId);
                if ((dataByKeySingle != null) && (!CSysDynamicBlock.bDialogBlock || (dataByKeySingle.bIOSHide == 0)))
                {
                    this.m_actorLines = new SActorLineNode[this.m_actorLinesRaw.Count];
                    SActorLineNode outNode = new SActorLineNode();
                    this.TranslateNodeFromRaw(ref outNode, ref dataByKeySingle);
                    this.m_actorLines[0] = outNode;
                    HashSet<object>.Enumerator enumerator = this.m_actorLinesRaw.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        for (int i = 1; enumerator.MoveNext(); i++)
                        {
                            ResActorLinesInfo current = enumerator.Current as ResActorLinesInfo;
                            SActorLineNode node2 = new SActorLineNode();
                            this.TranslateNodeFromRaw(ref node2, ref current);
                            this.m_actorLines[i] = node2;
                            outNode = node2;
                        }
                    }
                    Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, true);
                    this.m_curIndex = -1;
                    if (this.bAutoNextPage)
                    {
                        base.StartCoroutine(this.NextPageNoTimeScale());
                    }
                    else
                    {
                        this.NextPageInternal();
                    }
                    if ((this.PreDialogId != 0) && (inGroupId == this.PreDialogId))
                    {
                        PreDialogStartedEventParam prm = new PreDialogStartedEventParam(this.PreDialogId);
                        Singleton<GameEventSys>.instance.SendEvent<PreDialogStartedEventParam>(GameEventDef.Event_PreDialogStarted, ref prm);
                        this.PreDialogId = 0;
                    }
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator StartFadingIn()
        {
            return new <StartFadingIn>c__IteratorE { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator StartFadingOut()
        {
            return new <StartFadingOut>c__IteratorD { <>f__this = this };
        }

        private void TranslateNodeFromRaw(ref SActorLineNode outNode, ref ResActorLinesInfo inRecord)
        {
            outNode.DialogStyle = inRecord.iDialogStyle;
            string name = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Name;
            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown";
            }
            outNode.DialogContent = StringHelper.UTF8BytesToString(ref inRecord.szDialogContent);
            if (string.IsNullOrEmpty(outNode.DialogContent))
            {
                outNode.DialogContent = string.Empty;
            }
            else
            {
                outNode.DialogContent = outNode.DialogContent.Replace("[c]", name);
            }
            outNode.DialogTitle = StringHelper.UTF8BytesToString(ref inRecord.szDialogTitle);
            if (string.IsNullOrEmpty(outNode.DialogTitle))
            {
                outNode.DialogTitle = string.Empty;
            }
            else
            {
                outNode.DialogTitle = outNode.DialogTitle.Replace("[c]", name);
            }
            outNode.bFadeIn = inRecord.bFadeInType > 0;
            outNode.bFadeOut = inRecord.bFadeOutType > 0;
            outNode.b3dPortrait = inRecord.iUse3dPortrait > 0;
            outNode.ActorType = (ActorTypeDef) inRecord.iActorType;
            outNode.CharCfgId = inRecord.iCharacterCfgId;
            outNode.AnimName = StringHelper.UTF8BytesToString(ref inRecord.szAnimName);
            outNode.bAnimLoop = inRecord.iAnimLoop > 0;
            outNode.PortraitImgPrefab = new ObjData();
            if (outNode.b3dPortrait)
            {
                if (inRecord.iCharacterCfgId > 0)
                {
                    switch (inRecord.iActorType)
                    {
                        case 0:
                            outNode.PortraitImgPath = CUICommonSystem.GetHero3DObjPath((uint) inRecord.iCharacterCfgId, true);
                            break;

                        case 1:
                            outNode.PortraitImgPath = CUICommonSystem.GetMonster3DObjPath(inRecord.iCharacterCfgId, true);
                            break;

                        case 2:
                            outNode.PortraitImgPath = CUICommonSystem.GetOrgan3DObjPath(inRecord.iCharacterCfgId, true);
                            break;
                    }
                }
            }
            else
            {
                string fullPathInResources = StringHelper.UTF8BytesToString(ref inRecord.szImagePath);
                if (fullPathInResources == "9999")
                {
                    fullPathInResources = "90" + (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().HeadIconId + 1);
                    fullPathInResources = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Portrait + fullPathInResources;
                    outNode.PortraitImgPrefab.Object = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
                }
                else
                {
                    fullPathInResources = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Portrait + fullPathInResources;
                    outNode.PortraitImgPrefab.Object = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
                }
            }
        }

        public void Uninit()
        {
            this.EndDialogueComplete();
            this.PreDialogId = 0;
        }

        private void UpdateBgImageAlpha(float inAlpha)
        {
            ListView<GameObject>.Enumerator enumerator = this.m_bgGoList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    GameObject current = enumerator.Current;
                    Image component = current.GetComponent<Image>();
                    Text text = current.GetComponent<Text>();
                    if (component != null)
                    {
                        Color color = component.color;
                        component.color = new Color(color.r, color.g, color.b, inAlpha);
                    }
                    if (text != null)
                    {
                        Color color2 = text.color;
                        text.color = new Color(color2.r, color2.g, color2.b, inAlpha);
                    }
                }
            }
        }

        private void UpdatePortrait3d(bool bActive, SActorLineNode node, SActorLineNode preNode)
        {
            if (this.m_curUiFormPortrait != null)
            {
                Transform transform = this.m_curUiFormPortrait.gameObject.transform.FindChild("PanelCenter");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(bActive);
                    if (bActive)
                    {
                        if (((preNode.AnimName != node.AnimName) || (preNode.bAnimLoop != node.bAnimLoop)) || (((preNode.b3dPortrait != node.b3dPortrait) || (preNode.ActorType != node.ActorType)) || (preNode.CharCfgId != node.CharCfgId)))
                        {
                            CUI3DImageScript component = transform.FindChild("3DImage").gameObject.GetComponent<CUI3DImageScript>();
                            this.OnDestroyPortrait(this.m_3dPortraitName);
                            component.RemoveGameObject(this.m_3dPortraitName);
                            this.m_3dPortraitName = null;
                            CActorInfo actorInfo = node.PortraitImgPath.ActorInfo;
                            TransformConfig config = (actorInfo == null) ? null : actorInfo.GetTransformConfig(ETransformConfigUsage.NPCInStory);
                            GameObject key = null;
                            if (config != null)
                            {
                                Vector2 screenPosition = new Vector2(config.Offset.x, config.Offset.y);
                                screenPosition += component.GetPivotScreenPosition();
                                key = component.AddGameObject(node.PortraitImgPath.ObjectName, false, ref screenPosition, true, true, null);
                                if (key != null)
                                {
                                    key.transform.localScale = (Vector3) (key.transform.localScale * config.Scale);
                                }
                            }
                            else
                            {
                                key = component.AddGameObject(node.PortraitImgPath.ObjectName, false, true);
                            }
                            CResourcePackerInfo resourceBelongedPackerInfo = Singleton<CResourceManager>.GetInstance().GetResourceBelongedPackerInfo(node.PortraitImgPath.ObjectName);
                            if ((resourceBelongedPackerInfo != null) && resourceBelongedPackerInfo.IsAssetBundleLoaded())
                            {
                                resourceBelongedPackerInfo.UnloadAssetBundle(false);
                            }
                            this.m_3dPortraitName = key;
                            if (key != null)
                            {
                                Animation animation = key.GetComponent<Animation>();
                                if (animation != null)
                                {
                                    string clipName = "Idleshow";
                                    if ((node.AnimName != null) && !string.IsNullOrEmpty(node.AnimName))
                                    {
                                        clipName = node.AnimName;
                                    }
                                    Coroutine coroutine = base.StartCoroutine(PlayAnimNoTimeScale(animation, clipName, node.bAnimLoop, new System.Action(this.OnPortrait3dAnimComplete)));
                                    this.Portrait3dAnimCoMap.Add(key, coroutine);
                                }
                            }
                        }
                    }
                    else
                    {
                        CUI3DImageScript script2 = transform.FindChild("3DImage").gameObject.GetComponent<CUI3DImageScript>();
                        this.OnDestroyPortrait(this.m_3dPortraitName);
                        script2.RemoveGameObject(this.m_3dPortraitName);
                        this.m_3dPortraitName = null;
                    }
                }
            }
        }

        private void UpdatePortraitImg(bool bActive, SActorLineNode node, SActorLineNode preNode)
        {
            if (this.m_curUiForm != null)
            {
                Transform transform = this.m_curUiForm.gameObject.transform.Find("Pic_Npc");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(bActive);
                    if (bActive)
                    {
                        if (node.PortraitImgPrefab.Object != null)
                        {
                            transform.gameObject.GetComponent<Image>().SetSprite(node.PortraitImgPrefab.Object);
                        }
                        else
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <NextPageNoTimeScale>c__IteratorB : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <_timeAtCurrentFrame>__1;
            internal float <_timeAtLastFrame>__0;
            internal DialogueProcessor <>f__this;
            internal float <deltaTime>__2;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this.NextPageInternal();
                        this.<>f__this.m_bIsPlayingAutoNext = true;
                        this.<>f__this.m_nextPageProgressTime = 0f;
                        this.<_timeAtLastFrame>__0 = 0f;
                        this.<_timeAtCurrentFrame>__1 = 0f;
                        this.<deltaTime>__2 = 0f;
                        this.<_timeAtLastFrame>__0 = Time.realtimeSinceStartup;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_013E;
                }
                if (this.<>f__this.m_bIsPlayingAutoNext)
                {
                    if (!this.<>f__this.IsFading())
                    {
                        this.<_timeAtCurrentFrame>__1 = Time.realtimeSinceStartup;
                        this.<deltaTime>__2 = this.<_timeAtCurrentFrame>__1 - this.<_timeAtLastFrame>__0;
                        this.<_timeAtLastFrame>__0 = this.<_timeAtCurrentFrame>__1;
                        this.<>f__this.m_nextPageProgressTime += this.<deltaTime>__2;
                        if (this.<>f__this.m_nextPageProgressTime >= this.<>f__this.AutoNextPageTime)
                        {
                            this.<>f__this.m_bIsPlayingAutoNext = !this.<>f__this.NextPageInternal();
                            this.<>f__this.m_nextPageProgressTime = 0f;
                        }
                    }
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_013E:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <PlayAnimNoTimeScale>c__IteratorC : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Animation <$>animation;
            internal bool <$>bLoop;
            internal string <$>clipName;
            internal System.Action <$>onComplete;
            internal AnimationState <_currState>__0;
            internal float <_progressTime>__2;
            internal float <_timeAtCurrentFrame>__4;
            internal float <_timeAtLastFrame>__3;
            internal float <deltaTime>__5;
            internal bool <isPlaying>__1;
            internal Animation animation;
            internal bool bLoop;
            internal string clipName;
            internal System.Action onComplete;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        DebugHelper.Assert(this.animation != null);
                        DebugHelper.Assert(this.clipName != null);
                        DebugHelper.Assert(!string.IsNullOrEmpty(this.clipName));
                        this.<_currState>__0 = this.animation[this.clipName];
                        if (this.<_currState>__0 == null)
                        {
                            goto Label_01B9;
                        }
                        this.<_currState>__0.wrapMode = !this.bLoop ? WrapMode.Once : WrapMode.Loop;
                        this.<isPlaying>__1 = true;
                        this.<_progressTime>__2 = 0f;
                        this.<_timeAtLastFrame>__3 = 0f;
                        this.<_timeAtCurrentFrame>__4 = 0f;
                        this.<deltaTime>__5 = 0f;
                        this.animation.Play(this.clipName);
                        this.<_timeAtLastFrame>__3 = Time.realtimeSinceStartup;
                        break;

                    case 1:
                        break;

                    case 2:
                        if (this.onComplete != null)
                        {
                            this.onComplete();
                        }
                        this.$PC = -1;
                        goto Label_01E9;

                    default:
                        goto Label_01E9;
                }
                if (this.<isPlaying>__1)
                {
                    this.<_timeAtCurrentFrame>__4 = Time.realtimeSinceStartup;
                    this.<deltaTime>__5 = this.<_timeAtCurrentFrame>__4 - this.<_timeAtLastFrame>__3;
                    this.<_timeAtLastFrame>__3 = this.<_timeAtCurrentFrame>__4;
                    this.<_progressTime>__2 += this.<deltaTime>__5;
                    this.<_currState>__0.normalizedTime = this.<_progressTime>__2 / this.<_currState>__0.length;
                    this.animation.Sample();
                    if (this.<_progressTime>__2 >= this.<_currState>__0.length)
                    {
                        if (this.<_currState>__0.wrapMode != WrapMode.Loop)
                        {
                            this.<isPlaying>__1 = false;
                        }
                        else
                        {
                            this.<_progressTime>__2 = 0f;
                        }
                    }
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    goto Label_01EB;
                }
            Label_01B9:
                this.$current = null;
                this.$PC = 2;
                goto Label_01EB;
            Label_01E9:
                return false;
            Label_01EB:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <StartFadingIn>c__IteratorE : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <_progressTime>__1;
            internal float <_timeAtCurrentFrame>__3;
            internal float <_timeAtLastFrame>__2;
            internal DialogueProcessor <>f__this;
            internal float <bgAlpha>__5;
            internal float <deltaTime>__4;
            internal bool <isPlaying>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this.UpdateBgImageAlpha(0.2f);
                        this.<isPlaying>__0 = true;
                        this.<_progressTime>__1 = 0f;
                        this.<_timeAtLastFrame>__2 = 0f;
                        this.<_timeAtCurrentFrame>__3 = 0f;
                        this.<deltaTime>__4 = 0f;
                        this.<_timeAtLastFrame>__2 = Time.realtimeSinceStartup;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0137;
                }
                if (this.<isPlaying>__0)
                {
                    this.<_timeAtCurrentFrame>__3 = Time.realtimeSinceStartup;
                    this.<deltaTime>__4 = this.<_timeAtCurrentFrame>__3 - this.<_timeAtLastFrame>__2;
                    this.<_timeAtLastFrame>__2 = this.<_timeAtCurrentFrame>__3;
                    this.<_progressTime>__1 += this.<deltaTime>__4;
                    if (this.<_progressTime>__1 >= 0.5f)
                    {
                        this.<isPlaying>__0 = false;
                        this.<_progressTime>__1 = 0.5f;
                    }
                    this.<bgAlpha>__5 = ((this.<_progressTime>__1 / 0.5f) * 0.8f) + 0.2f;
                    this.<>f__this.UpdateBgImageAlpha(this.<bgAlpha>__5);
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this.m_fadingCoroutine = null;
                this.$PC = -1;
            Label_0137:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <StartFadingOut>c__IteratorD : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <_progressTime>__1;
            internal float <_timeAtCurrentFrame>__3;
            internal float <_timeAtLastFrame>__2;
            internal DialogueProcessor <>f__this;
            internal float <bgAlpha>__5;
            internal float <deltaTime>__4;
            internal bool <isPlaying>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this.UpdateBgImageAlpha(1f);
                        this.<isPlaying>__0 = true;
                        this.<_progressTime>__1 = 0f;
                        this.<_timeAtLastFrame>__2 = 0f;
                        this.<_timeAtCurrentFrame>__3 = 0f;
                        this.<deltaTime>__4 = 0f;
                        this.<_timeAtLastFrame>__2 = Time.realtimeSinceStartup;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0175;
                }
                if (this.<isPlaying>__0)
                {
                    this.<_timeAtCurrentFrame>__3 = Time.realtimeSinceStartup;
                    this.<deltaTime>__4 = this.<_timeAtCurrentFrame>__3 - this.<_timeAtLastFrame>__2;
                    this.<_timeAtLastFrame>__2 = this.<_timeAtCurrentFrame>__3;
                    this.<_progressTime>__1 += this.<deltaTime>__4;
                    if (this.<_progressTime>__1 >= 0.5f)
                    {
                        this.<isPlaying>__0 = false;
                        this.<_progressTime>__1 = 0.5f;
                    }
                    this.<bgAlpha>__5 = ((1f - (this.<_progressTime>__1 / 0.5f)) * 0.8f) + 0.2f;
                    this.<>f__this.UpdateBgImageAlpha(this.<bgAlpha>__5);
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this.m_fadingCoroutine = null;
                if (this.<>f__this.m_curIndex >= this.<>f__this.m_actorLines.Length)
                {
                    this.<>f__this.EndDialogueComplete();
                }
                else
                {
                    this.<>f__this.DoNextPageInternal();
                }
                this.$PC = -1;
            Label_0175:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SActorLineNode
        {
            public ObjData PortraitImgPrefab;
            public ObjNameData PortraitImgPath;
            public bool b3dPortrait;
            public int CharCfgId;
            public ActorTypeDef ActorType;
            public string AnimName;
            public bool bAnimLoop;
            public string DialogTitle;
            public string DialogContent;
            public int DialogStyle;
            public byte DialogPos;
            public string VoiceEvent;
            public bool bFadeIn;
            public bool bFadeOut;
        }
    }
}

