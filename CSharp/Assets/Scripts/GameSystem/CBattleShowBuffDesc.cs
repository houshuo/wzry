namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CBattleShowBuffDesc
    {
        private BuffCDString BufStringProvider = new BuffCDString();
        private BuffInfo[] m_ArrShowBuffSkill = new BuffInfo[5];
        private GameObject m_BuffBtn0;
        private GameObject m_BuffDescNodeObj;
        private GameObject m_BuffDescTxtObj;
        private GameObject m_BuffSkillPanel;
        private PoolObjHandle<ActorRoot> m_curActor;
        private int m_CurShowBuffCount;
        private Image m_imgSkillFrame;
        private const int m_iShowBuffNumMax = 5;
        private PoolObjHandle<BuffSkill> m_selectBuff;
        private Image m_textBgImage;

        public void Init(GameObject root)
        {
            if (root != null)
            {
                this.m_BuffSkillPanel = root;
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    this.m_curActor = hostPlayer.Captain;
                }
                else
                {
                    this.m_curActor = new PoolObjHandle<ActorRoot>(null);
                }
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_BuffSkillBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnDown));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_BuffSkillBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnUp));
                Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
                Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
                this.InitShowBuffDesc();
            }
        }

        private bool InitShowBuffDesc()
        {
            this.m_CurShowBuffCount = 0;
            this.m_selectBuff = new PoolObjHandle<BuffSkill>();
            for (int i = 0; i < 5; i++)
            {
                this.m_ArrShowBuffSkill[i] = new BuffInfo();
                string path = string.Format("BuffSkillBtn_{0}", i);
                GameObject obj2 = Utility.FindChild(this.m_BuffSkillPanel, path);
                if (obj2 != null)
                {
                    if (i == 0)
                    {
                        this.m_BuffBtn0 = obj2;
                        this.m_imgSkillFrame = Utility.GetComponetInChild<Image>(this.m_BuffBtn0, "SkillFrame");
                    }
                    obj2.CustomSetActive(false);
                }
            }
            this.m_BuffDescNodeObj = Utility.FindChild(this.m_BuffSkillPanel, "BuffDesc");
            if (this.m_BuffDescNodeObj == null)
            {
                return false;
            }
            this.m_BuffDescNodeObj.CustomSetActive(false);
            this.m_textBgImage = Utility.GetComponetInChild<Image>(this.m_BuffDescNodeObj, "bg");
            if (this.m_textBgImage == null)
            {
                return false;
            }
            this.m_textBgImage.gameObject.CustomSetActive(true);
            return true;
        }

        private void OnBuffSkillBtnDown(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidget != null)
            {
                int index = int.Parse(uiEvent.m_srcWidget.name.Substring(uiEvent.m_srcWidget.name.IndexOf("_") + 1));
                if ((index < 5) && ((this.m_ArrShowBuffSkill != null) && (index < this.m_ArrShowBuffSkill.Length)))
                {
                    this.m_selectBuff = this.m_ArrShowBuffSkill[index].stBuffSkill;
                    if (this.m_selectBuff != 0)
                    {
                        uint iCfgID = (uint) this.m_selectBuff.handle.cfgData.iCfgID;
                        this.ShowBuffSkillDesc(iCfgID, uiEvent.m_srcWidget.transform.localPosition);
                    }
                }
            }
        }

        private void OnBuffSkillBtnUp(CUIEvent uiEvent)
        {
            this.m_selectBuff.Release();
            this.m_BuffDescNodeObj.CustomSetActive(false);
        }

        private void OnCaptainSwitched(ref DefaultGameEventParam prm)
        {
            this.SwitchActor(ref prm.src);
        }

        private void OnPlayerBuffChange(ref BuffChangeEventParam prm)
        {
            if (Singleton<CBattleSystem>.GetInstance().IsFormOpen && (this.m_curActor == prm.target))
            {
                this.m_curActor = prm.target;
                if ((prm.stBuffSkill != 0) && (prm.stBuffSkill.handle.cfgData.bIsShowBuff == 1))
                {
                    this.UpdateShowBuffList(ref prm.stBuffSkill, prm.bIsAdd);
                    this.UpdateShowBuff();
                }
            }
        }

        private void ShowBuffSkillDesc(uint uiBuffId, Vector3 BtnPos)
        {
            ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey(uiBuffId);
            if (((dataByKey != null) && !string.IsNullOrEmpty(dataByKey.szSkillCombineDesc)) && (this.m_BuffSkillPanel != null))
            {
                if (this.m_BuffDescTxtObj == null)
                {
                    this.m_BuffDescTxtObj = Utility.FindChild(this.m_BuffDescNodeObj, "Text");
                    if (this.m_BuffDescTxtObj == null)
                    {
                        return;
                    }
                }
                Text component = this.m_BuffDescTxtObj.GetComponent<Text>();
                component.text = dataByKey.szSkillCombineDesc;
                float preferredHeight = component.preferredHeight;
                Vector2 sizeDelta = this.m_textBgImage.rectTransform.sizeDelta;
                preferredHeight += ((this.m_textBgImage.gameObject.transform.localPosition.y - component.gameObject.transform.localPosition.y) * 2f) + 10f;
                this.m_textBgImage.rectTransform.sizeDelta = new Vector2(sizeDelta.x, preferredHeight);
                Vector3 vector2 = BtnPos;
                RectTransform transform = this.m_BuffBtn0.GetComponent<RectTransform>();
                vector2.y += (((this.m_BuffBtn0.transform.localScale.y * transform.rect.height) / 2f) + (preferredHeight / 2f)) + 15f;
                this.m_BuffDescNodeObj.transform.localPosition = vector2;
                this.m_BuffDescNodeObj.CustomSetActive(true);
            }
        }

        public void SwitchActor(ref PoolObjHandle<ActorRoot> actor)
        {
            if (((actor != 0) && (actor.handle.BuffHolderComp != null)) && (actor != this.m_curActor))
            {
                this.m_curActor = actor;
                this.m_CurShowBuffCount = 0;
                List<BuffSkill> spawnedBuffList = actor.handle.BuffHolderComp.SpawnedBuffList;
                if ((spawnedBuffList != null) && (spawnedBuffList.Count != 0))
                {
                    int count = spawnedBuffList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (spawnedBuffList[i].cfgData.bIsShowBuff == 1)
                        {
                            PoolObjHandle<BuffSkill> stBuffSkill = new PoolObjHandle<BuffSkill>(spawnedBuffList[i]);
                            this.UpdateShowBuffList(ref stBuffSkill, true);
                        }
                    }
                }
                this.UpdateShowBuff();
            }
        }

        public void UnInit()
        {
            if (this.m_curActor != 0)
            {
                this.m_curActor.Release();
            }
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_BuffSkillBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_BuffSkillBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnUp));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
            this.m_BuffSkillPanel = null;
            this.m_BuffDescTxtObj = null;
            this.m_BuffDescNodeObj = null;
            this.m_BuffBtn0 = null;
            this.m_imgSkillFrame = null;
            this.m_textBgImage = null;
        }

        public void UpdateBuffCD(int delta)
        {
            int num = 0;
            for (int i = 0; i < this.m_CurShowBuffCount; i++)
            {
                if ((((this.m_ArrShowBuffSkill != null) && (this.m_ArrShowBuffSkill[i].stBuffSkill != 0)) && ((num = this.m_ArrShowBuffSkill[i].iBufCD - delta) > 0)) && (this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData != null))
                {
                    int iDuration = this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.iDuration;
                    if (iDuration != -1)
                    {
                        this.m_ArrShowBuffSkill[i].iBufCD = num;
                        string path = this.BufStringProvider.GetString(i);
                        GameObject obj2 = Utility.FindChild(this.m_BuffSkillPanel, path);
                        if (obj2 != null)
                        {
                            Image component = obj2.GetComponent<Image>();
                            if (component != null)
                            {
                                float num4 = ((float) num) / ((float) iDuration);
                                component.CustomFillAmount(num4);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateShowBuff()
        {
            if (this.m_BuffSkillPanel != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    string path = string.Format("BuffSkillBtn_{0}", i);
                    GameObject p = Utility.FindChild(this.m_BuffSkillPanel, path);
                    if (p == null)
                    {
                        return;
                    }
                    if (i < this.m_CurShowBuffCount)
                    {
                        if (((this.m_ArrShowBuffSkill[i].stBuffSkill != 0) && (this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData != null)) && !string.IsNullOrEmpty(this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.szIconPath))
                        {
                            Image component = Utility.FindChild(p, "BuffImg").GetComponent<Image>();
                            GameObject prefab = CUIUtility.GetSpritePrefeb(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.szIconPath)), true, true);
                            component.SetSprite(prefab);
                            Image image = Utility.FindChild(p, "BuffImgMask").GetComponent<Image>();
                            image.SetSprite(prefab);
                            image.CustomFillAmount(0f);
                            GameObject obj4 = Utility.FindChild(p, "OverlyingNumTxt");
                            if (obj4 != null)
                            {
                                Text text = obj4.GetComponent<Text>();
                                if (this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.dwOverlayMax > 1)
                                {
                                    text.text = this.m_ArrShowBuffSkill[i].stBuffSkill.handle.GetOverlayCount().ToString();
                                    obj4.CustomSetActive(true);
                                }
                                else
                                {
                                    obj4.CustomSetActive(false);
                                }
                            }
                        }
                        p.CustomSetActive(true);
                    }
                    else
                    {
                        p.CustomSetActive(false);
                    }
                }
            }
        }

        private void UpdateShowBuffList(ref PoolObjHandle<BuffSkill> stBuffSkill, bool bIsAdd)
        {
            if (((stBuffSkill != 0) && (stBuffSkill.handle.cfgData != null)) && (stBuffSkill.handle.cfgData.bIsShowBuff != 0))
            {
                if (!bIsAdd)
                {
                    for (int i = 0; i < this.m_CurShowBuffCount; i++)
                    {
                        if ((this.m_ArrShowBuffSkill[i].stBuffSkill != 0) && (this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.iCfgID == stBuffSkill.handle.cfgData.iCfgID))
                        {
                            this.m_ArrShowBuffSkill[i].iCacheCount--;
                            if (this.m_ArrShowBuffSkill[i].iCacheCount == 0)
                            {
                                if ((((this.m_selectBuff != 0) && (this.m_selectBuff.handle.cfgData != null)) && ((this.m_ArrShowBuffSkill[i].stBuffSkill != 0) && (this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData != null))) && (this.m_selectBuff.handle.cfgData.iCfgID == this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.iCfgID))
                                {
                                    this.m_BuffDescNodeObj.CustomSetActive(false);
                                }
                                this.m_CurShowBuffCount--;
                                for (int j = i; j < this.m_CurShowBuffCount; j++)
                                {
                                    this.m_ArrShowBuffSkill[j] = this.m_ArrShowBuffSkill[j + 1];
                                }
                            }
                            break;
                        }
                    }
                }
                else if (this.m_CurShowBuffCount == 0)
                {
                    this.m_ArrShowBuffSkill[0].stBuffSkill = stBuffSkill;
                    this.m_ArrShowBuffSkill[0].iCacheCount = 1;
                    this.m_ArrShowBuffSkill[0].iBufCD = stBuffSkill.handle.cfgData.iDuration;
                    this.m_CurShowBuffCount++;
                }
                else
                {
                    int index = 0;
                    bool flag = true;
                    int num2 = -1;
                    while (index < this.m_CurShowBuffCount)
                    {
                        DebugHelper.Assert(this.m_ArrShowBuffSkill[index].stBuffSkill == 1, "UpdateShowBuffList: bad m_ArrShowBuffSkill[i].stBuffSkill");
                        if ((this.m_ArrShowBuffSkill[index].stBuffSkill != 0) && (this.m_ArrShowBuffSkill[index].stBuffSkill.handle.cfgData.bShowBuffPriority > stBuffSkill.handle.cfgData.bShowBuffPriority))
                        {
                            for (int k = this.m_CurShowBuffCount; k > index; k--)
                            {
                                if (k != 5)
                                {
                                    this.m_ArrShowBuffSkill[k] = this.m_ArrShowBuffSkill[k - 1];
                                }
                            }
                            break;
                        }
                        if ((this.m_ArrShowBuffSkill[index].stBuffSkill != 0) && (this.m_ArrShowBuffSkill[index].stBuffSkill.handle.cfgData.bShowBuffPriority == stBuffSkill.handle.cfgData.bShowBuffPriority))
                        {
                            if (this.m_ArrShowBuffSkill[index].stBuffSkill.handle.cfgData.iCfgID != stBuffSkill.handle.cfgData.iCfgID)
                            {
                                goto Label_0293;
                            }
                            this.m_ArrShowBuffSkill[index].stBuffSkill = stBuffSkill;
                            this.m_ArrShowBuffSkill[index].iBufCD = stBuffSkill.handle.cfgData.iDuration;
                            this.m_ArrShowBuffSkill[index].iCacheCount++;
                            flag = false;
                            num2 = -1;
                            break;
                        }
                        if (this.m_ArrShowBuffSkill[index].stBuffSkill == 0)
                        {
                            num2 = index;
                            flag = false;
                        }
                        else if ((index >= (this.m_CurShowBuffCount - 1)) && (this.m_CurShowBuffCount == 5))
                        {
                            flag = false;
                        }
                    Label_0293:
                        index++;
                    }
                    if (flag && (index < 5))
                    {
                        this.m_ArrShowBuffSkill[index].stBuffSkill = stBuffSkill;
                        this.m_ArrShowBuffSkill[index].iBufCD = stBuffSkill.handle.cfgData.iDuration;
                        this.m_ArrShowBuffSkill[index].iCacheCount = 1;
                        if (this.m_CurShowBuffCount < 5)
                        {
                            this.m_CurShowBuffCount++;
                        }
                    }
                    else if ((!flag && (num2 >= 0)) && (num2 < this.m_CurShowBuffCount))
                    {
                        this.m_ArrShowBuffSkill[num2].stBuffSkill = stBuffSkill;
                        this.m_ArrShowBuffSkill[num2].iBufCD = stBuffSkill.handle.cfgData.iDuration;
                        this.m_ArrShowBuffSkill[num2].iCacheCount = 1;
                    }
                }
            }
        }

        private class BuffCDString
        {
            private string[] CachedString = new string[4];

            public BuffCDString()
            {
                for (int i = 0; i < this.CachedString.Length; i++)
                {
                    this.CachedString[i] = this.QueryString(i);
                }
            }

            public string GetString(int Index)
            {
                if ((Index >= 0) && (Index < this.CachedString.Length))
                {
                    return this.CachedString[Index];
                }
                return this.QueryString(Index);
            }

            private string QueryString(int Index)
            {
                return string.Format("BuffSkillBtn_{0}/BuffImgMask", Index);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BuffInfo
        {
            public int iCacheCount;
            public PoolObjHandle<BuffSkill> stBuffSkill;
            public int iBufCD;
        }
    }
}

