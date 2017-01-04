namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine.UI;

    public class CFunctionUnlockSys : Singleton<CFunctionUnlockSys>
    {
        public static readonly string FUC_UNLOCK_FORM_PATH = "UGUI/Form/System/FunctionUnlock/Form_FucUnlockTip.prefab";
        private ulong m_tipUnlockMask;

        public bool ChapterIsUnlock(uint ChapterId)
        {
            ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey(ChapterId);
            object[] inParameters = new object[] { ChapterId };
            DebugHelper.Assert(dataByKey != null, "ChapterIsUnlock : ChapterId[{0}] can not be find.", inParameters);
            if (dataByKey == null)
            {
                return false;
            }
            uint dwUnlockLevel = dataByKey.dwUnlockLevel;
            if ((dwUnlockLevel > 0) && !this.CheckUnlock(dwUnlockLevel))
            {
                return false;
            }
            return true;
        }

        public bool CheckUnlock(uint id)
        {
            bool flag = false;
            ResUnlockCondition dataByKey = GameDataMgr.unlockConditionDatabin.GetDataByKey(id);
            object[] inParameters = new object[] { id };
            DebugHelper.Assert(dataByKey != null, "ResUnlockCondition[{0}] can not be find.", inParameters);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "roleinfo can't be null in CheckUnlock");
            if ((masterRoleInfo != null) && (dataByKey != null))
            {
                switch (dataByKey.wUnlockType)
                {
                    case 1:
                        return (masterRoleInfo.PvpLevel >= dataByKey.UnlockParam[0]);

                    case 2:
                    {
                        int num = (int) dataByKey.UnlockParam[0];
                        int num2 = (int) dataByKey.UnlockParam[1];
                        int index = 0;
                        ResLevelCfgInfo info2 = GameDataMgr.levelDatabin.GetDataByKey((long) num);
                        object[] objArray2 = new object[] { num };
                        DebugHelper.Assert(info2 != null, "can't find level = {0}", objArray2);
                        flag = false;
                        if (((info2 != null) && (info2 != null)) && ((masterRoleInfo.pveLevelDetail[index] != null) && (masterRoleInfo.pveLevelDetail[index].ChapterDetailList[info2.iChapterId - 1] != null)))
                        {
                            PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = masterRoleInfo.pveLevelDetail[index].ChapterDetailList[info2.iChapterId - 1];
                            for (int i = 0; i < pve_chapter_complete_info.LevelDetailList.Length; i++)
                            {
                                PVE_LEVEL_COMPLETE_INFO pve_level_complete_info = pve_chapter_complete_info.LevelDetailList[i];
                                if ((pve_level_complete_info != null) && (pve_level_complete_info.iLevelID == num))
                                {
                                    flag = (pve_level_complete_info.levelStatus == 1) && (num2 <= CAdventureSys.GetStarNum(pve_level_complete_info.bStarBits));
                                    break;
                                }
                            }
                        }
                        return true;
                    }
                    case 3:
                        return flag;

                    case 4:
                    {
                        uint num5 = 0;
                        if (masterRoleInfo.pvpDetail != null)
                        {
                            num5 = (masterRoleInfo.pvpDetail.stOneVsOneInfo.dwTotalNum + masterRoleInfo.pvpDetail.stTwoVsTwoInfo.dwTotalNum) + masterRoleInfo.pvpDetail.stThreeVsThreeInfo.dwTotalNum;
                        }
                        return ((dataByKey.UnlockParam[0] <= num5) && (dataByKey.UnlockParam[1] <= masterRoleInfo.PvpLevel));
                    }
                }
            }
            return flag;
        }

        public bool FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) type);
            if (dataByKey == null)
            {
                return true;
            }
            if (dataByKey.bIsAnd == 1)
            {
                for (int j = 0; j < dataByKey.UnlockArray.Length; j++)
                {
                    uint id = dataByKey.UnlockArray[j];
                    if ((id > 0) && !this.CheckUnlock(id))
                    {
                        return false;
                    }
                }
                return true;
            }
            bool flag = false;
            for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
            {
                uint num4 = dataByKey.UnlockArray[i];
                if (num4 > 0)
                {
                    flag = true;
                }
                if ((num4 > 0) && this.CheckUnlock(num4))
                {
                    return true;
                }
            }
            return !flag;
        }

        public uint[] GetConditionParam(RES_SPECIALFUNCUNLOCK_TYPE type, RES_UNLOCKCONDITION_TYPE conditionType)
        {
            uint[] numArray = new uint[0];
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) type);
            if ((dataByKey != null) && (masterRoleInfo != null))
            {
                for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
                {
                    uint key = dataByKey.UnlockArray[i];
                    if (key > 0)
                    {
                        ResUnlockCondition condition = GameDataMgr.unlockConditionDatabin.GetDataByKey(key);
                        if ((condition != null) && (((RES_UNLOCKCONDITION_TYPE) condition.wUnlockType) == conditionType))
                        {
                            return condition.UnlockParam;
                        }
                    }
                }
            }
            return numArray;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.FucUnlock_TimerUp, new CUIEventManager.OnUIEventHandler(this.OnUnlockTipsTimeUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.FucUnlock_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseUnlockTip));
            Singleton<EventRouter>.instance.AddEventHandler("FucUnlockConditionChanged", new System.Action(this.UnlockConditionChanged));
            base.Init();
        }

        private bool IsBottomBtn(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            return ((type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO) || ((type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL) || ((type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG) || ((type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK) || ((type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION) || ((type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND) || (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL)))))));
        }

        public bool IsTypeHasCondition(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) type);
            if (dataByKey != null)
            {
                for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
                {
                    uint num2 = dataByKey.UnlockArray[i];
                    if (num2 > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnCloseUnlockTip(CUIEvent cuiEvent)
        {
            Singleton<CUIManager>.instance.CloseForm(FUC_UNLOCK_FORM_PATH);
            this.UnlockConditionChanged();
        }

        public void OnSetUnlockTipsMask(SCPKG_CMD_GAMELOGINRSP rsp)
        {
            this.m_tipUnlockMask = rsp.ullFuncUnlockFlag;
        }

        private void OnUnlockTipsTimeUp(CUIEvent cuiEvent)
        {
            this.OnCloseUnlockTip(null);
        }

        public void OpenUnlockTip(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) type);
            DebugHelper.Assert(dataByKey != null);
            if (dataByKey != null)
            {
                string tips = Utility.UTF8Convert(dataByKey.szUnlockTip);
                string icon = Utility.UTF8Convert(dataByKey.szUnlockTipIcon);
                this.OpenUnlockTip(tips, icon);
                Singleton<CUIManager>.instance.GetForm(FUC_UNLOCK_FORM_PATH).GetComponent<CUITimerScript>().m_eventParams[1].tag = (int) type;
            }
        }

        public void OpenUnlockTip(string tips, string icon)
        {
            CUIFormScript formScript = Singleton<CUIManager>.instance.OpenForm(FUC_UNLOCK_FORM_PATH, false, false);
            if (formScript != null)
            {
                Text component = formScript.transform.FindChild("TipContentTxt").GetComponent<Text>();
                Image image = formScript.transform.FindChild("TipIconImg").GetComponent<Image>();
                formScript.GetComponent<CUIEventScript>().enabled = false;
                component.text = tips;
                image.SetSprite(CUIUtility.s_Sprite_Dynamic_FucUnlock_Dir + icon, formScript, true, false, false);
                image.SetNativeSize();
                component.gameObject.CustomSetActive(true);
            }
        }

        public void ReqUnlockTipsMask()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x57a);
            msg.stPkgData.stFuncUnlockReq.ullUnlockFlag = this.m_tipUnlockMask;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SetUnlockTipsMask(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            int num = (int) type;
            ulong num2 = ((ulong) 1L) << num;
            this.m_tipUnlockMask |= num2;
        }

        public bool TipsHasShow(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            int num = (int) type;
            ulong num2 = ((ulong) 1L) << num;
            return ((this.m_tipUnlockMask & num2) > 0L);
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.FucUnlock_TimerUp, new CUIEventManager.OnUIEventHandler(this.OnUnlockTipsTimeUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.FucUnlock_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseUnlockTip));
            Singleton<EventRouter>.instance.RemoveEventHandler("FucUnlockConditionChanged", new System.Action(this.UnlockConditionChanged));
            base.UnInit();
        }

        public void UnlockConditionChanged()
        {
            if (Singleton<GameStateCtrl>.instance.GetCurrentState() is LobbyState)
            {
                int num = 1;
                int num2 = 0x1c;
                for (int i = num; i < num2; i++)
                {
                    ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) i);
                    RES_SPECIALFUNCUNLOCK_TYPE type = (RES_SPECIALFUNCUNLOCK_TYPE) i;
                    if (((dataByKey != null) && !this.TipsHasShow(type)) && ((dataByKey.bIsShowUnlockTip == 1) && this.FucIsUnlock(type)))
                    {
                        if (this.IsBottomBtn(type))
                        {
                            Singleton<CLobbySystem>.instance.Play_UnLock_Animation(type);
                            this.SetUnlockTipsMask(type);
                            this.ReqUnlockTipsMask();
                            return;
                        }
                        this.SetUnlockTipsMask(type);
                        this.ReqUnlockTipsMask();
                    }
                }
            }
        }

        private byte ChapterUnlockMask
        {
            get
            {
                return (byte) (this.m_tipUnlockMask >> 0x38);
            }
        }
    }
}

