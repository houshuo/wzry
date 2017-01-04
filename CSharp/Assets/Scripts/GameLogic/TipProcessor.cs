namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;

    public class TipProcessor : Singleton<TipProcessor>
    {
        private DialogueProcessor.SActorLineNode m_actorLines;
        private string m_contentGoName = "Txt_Dialog";
        private bool[] m_currentOpenNode = new bool[Enum.GetNames(typeof(enGuideTipFormWidget)).Length];
        private CUIFormScript m_curUiForm;
        private string m_imgGoName = "Pic_Npc";
        private string m_nameGoName = "CharacterName";

        private void EndDialogue(int inGroupId)
        {
            if (this.m_curUiForm != null)
            {
                ResGuideTipInfo dataByKey = GameDataMgr.guideTipDatabin.GetDataByKey((long) inGroupId);
                DebugHelper.Assert(dataByKey != null);
                if (dataByKey != null)
                {
                    DialogueProcessor.SActorLineNode outNode = new DialogueProcessor.SActorLineNode();
                    this.TranslateNodeFromRaw(ref outNode, ref dataByKey);
                    GameObject widget = this.m_curUiForm.GetWidget(outNode.DialogPos);
                    if (widget != null)
                    {
                        widget.CustomSetActive(false);
                    }
                    this.m_currentOpenNode[outNode.DialogPos] = false;
                    bool flag = true;
                    for (int i = 0; i < this.m_currentOpenNode.Length; i++)
                    {
                        if (this.m_currentOpenNode[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        Singleton<CUIManager>.GetInstance().CloseForm(this.m_curUiForm);
                        this.m_curUiForm = null;
                    }
                }
            }
        }

        public void EndDrama(int inGroupId)
        {
            this.EndDialogue(inGroupId);
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Battle_Guide_PanelMisson_1_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Battle_Guide_PanelMisson_2_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Battle_Guide_PanelMisson_3_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
        }

        private void OnGuidePanelMissonClose(CUIEvent cuiEvent)
        {
            if (cuiEvent.m_eventID == enUIEventID.Battle_Guide_PanelMisson_1_Close)
            {
                this.ShowOneWidget(enGuideTipFormWidget.Mission1_Panel, false);
            }
            else if (cuiEvent.m_eventID == enUIEventID.Battle_Guide_PanelMisson_2_Close)
            {
                this.ShowOneWidget(enGuideTipFormWidget.Mission2_Panel, false);
            }
            else if (cuiEvent.m_eventID == enUIEventID.Battle_Guide_PanelMisson_3_Close)
            {
                this.ShowOneWidget(enGuideTipFormWidget.Mission3_Panel, false);
            }
        }

        public void PlayDrama(int inGroupId, ActorRoot inSrc, ActorRoot inAtker)
        {
            this.StartDialogue(inGroupId);
        }

        public void PrepareFight()
        {
        }

        private string QueryDialogTempPath(int inDialogStyle)
        {
            return "UGUI/Form/System/Dialog/Form_GuideTip";
        }

        private CUIFormScript QueryUiForm(int inDialogStyle)
        {
            if (this.m_curUiForm == null)
            {
                string formPath = this.QueryDialogTempPath(inDialogStyle);
                this.m_curUiForm = Singleton<CUIManager>.GetInstance().OpenForm(formPath, true, true);
                DebugHelper.Assert(this.m_curUiForm != null);
                this.ShowAllWidgets(false);
                for (int i = 0; i < this.m_currentOpenNode.Length; i++)
                {
                    this.m_currentOpenNode[i] = false;
                }
            }
            return this.m_curUiForm;
        }

        private void ShowAllWidgets(bool bShow)
        {
            for (int i = 0; i < 12; i++)
            {
                GameObject widget = this.m_curUiForm.GetWidget(i);
                if (widget != null)
                {
                    widget.CustomSetActive(bShow);
                }
            }
        }

        private GameObject ShowOneWidget(enGuideTipFormWidget inWidgetType, bool bShow)
        {
            GameObject widget = this.m_curUiForm.GetWidget((int) inWidgetType);
            if (widget != null)
            {
                widget.CustomSetActive(bShow);
            }
            return widget;
        }

        private void StartDialogue(int inGroupId)
        {
            ResGuideTipInfo dataByKey = GameDataMgr.guideTipDatabin.GetDataByKey((long) inGroupId);
            if (dataByKey != null)
            {
                this.TranslateNodeFromRaw(ref this.m_actorLines, ref dataByKey);
                DialogueProcessor.SActorLineNode actorLines = this.m_actorLines;
                this.QueryUiForm(actorLines.DialogStyle);
                if (this.m_curUiForm != null)
                {
                    GameObject obj2 = this.ShowOneWidget((enGuideTipFormWidget) actorLines.DialogPos, true);
                    DebugHelper.Assert(obj2 != null);
                    obj2.transform.FindChild(this.m_contentGoName).gameObject.GetComponent<Text>().text = actorLines.DialogContent;
                    obj2.transform.FindChild(this.m_nameGoName).gameObject.GetComponent<Text>().text = actorLines.DialogTitle;
                    if (actorLines.PortraitImgPrefab.Object != null)
                    {
                        Transform transform = obj2.transform.FindChild(this.m_imgGoName);
                        if (transform != null)
                        {
                            transform.gameObject.GetComponent<Image>().SetSprite(actorLines.PortraitImgPrefab.Object);
                        }
                    }
                    if (!string.IsNullOrEmpty(actorLines.VoiceEvent))
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent(actorLines.VoiceEvent, null);
                    }
                    DebugHelper.Assert(actorLines.DialogPos < this.m_currentOpenNode.Length);
                    this.m_currentOpenNode[actorLines.DialogPos] = true;
                }
            }
        }

        private void TranslateNodeFromRaw(ref DialogueProcessor.SActorLineNode outNode, ref ResGuideTipInfo inRecord)
        {
            outNode.DialogContent = StringHelper.UTF8BytesToString(ref inRecord.szTipContent);
            outNode.DialogTitle = StringHelper.UTF8BytesToString(ref inRecord.szTipTitle);
            outNode.DialogPos = inRecord.bTipPos;
            outNode.VoiceEvent = StringHelper.UTF8BytesToString(ref inRecord.szTipVoice);
            string fullPathInResources = StringHelper.UTF8BytesToString(ref inRecord.szImagePath);
            fullPathInResources = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Head + fullPathInResources;
            outNode.PortraitImgPrefab.Object = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
            if (outNode.PortraitImgPrefab.Object == null)
            {
                fullPathInResources = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Head + "0000";
                outNode.PortraitImgPrefab.Object = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
            }
        }

        public void Uninit()
        {
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Battle_Guide_PanelMisson_1_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Battle_Guide_PanelMisson_2_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Battle_Guide_PanelMisson_3_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
        }

        public enum enGuideTipFormWidget
        {
            Up_Panel,
            Down_Panel,
            Atk_Panel,
            Recover_Panel,
            Skill1_Panel,
            Skill2_Panel,
            Skill3_Panel,
            Talent_Panel,
            Mission1_Panel,
            Mission2_Panel,
            Mission3_Panel,
            Setting_panel,
            Count
        }
    }
}

