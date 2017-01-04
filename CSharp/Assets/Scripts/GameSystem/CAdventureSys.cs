namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    internal class CAdventureSys : Singleton<CAdventureSys>
    {
        public static string ADVENTURE_LEVEL_FORM = "UGUI/Form/System/PvE/Adv/Form_Adv_Level.prefab";
        public static string ADVENTURE_MOB_FORM = "UGUI/Form/System/PvE/Adv/Form_Mopup.prefab";
        public static string ADVENTURE_SELECT_FORM = "UGUI/Form/System/PvE/Adv/Form_Adv_Select.prefab";
        public byte bNewChapterId;
        public static uint CHALLENGE_BUYTIME_LIMIT = 1;
        public static int CHAPTER_NUM = 10;
        public int currentChapter;
        public int currentDifficulty;
        public int currentLevelId;
        public int currentLevelSeq;
        public static string EXLPORE_FORM_PATH = "UGUI/Form/System/PvE/Adv/Form_Explore_Select.prefab";
        public static int LEVEL_DIFFICULT_OPENED = 4;
        public static int LEVEL_PER_CHAPTER = 4;
        private byte[] m_chapterStatus;
        private byte[] m_difficultyStatus;
        public static uint MOPUP_TICKET_ID;
        public static uint MOPUP_TICKET_NUM_PER_LEVEL = 1;
        public static uint MOPUP_TICKET_PRICE_BY_DIAMOND = 1;
        private const string s_str_chapter = "Sgame_Chapter";
        private const string s_str_difficulty = "Sgame_Difficulty";
        private const string s_str_Level = "Sgame_Level";
        public static int STAR_PER_LEVEL = 3;

        private int CanBuyPlayTime(PVE_LEVEL_COMPLETE_INFO levelInfo, out byte CoinType, out uint Price, out string reason)
        {
            reason = string.Empty;
            int num = levelInfo.PlayLimit + 1;
            if (num <= CHALLENGE_BUYTIME_LIMIT)
            {
                Dictionary<long, object>.Enumerator enumerator = GameDataMgr.resShopInfoDatabin.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<long, object> current = enumerator.Current;
                    ResShopInfo info = (ResShopInfo) current.Value;
                    if ((info.iType == 6) && (info.iSubType == num))
                    {
                        CoinType = info.bCoinType;
                        Price = info.dwCoinPrice;
                        if (info.bCoinType == 2)
                        {
                            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().DianQuan >= info.dwCoinPrice)
                            {
                                return 0;
                            }
                            reason = Singleton<CTextManager>.GetInstance().GetText("Common_DianQuan_Not_Enough");
                            return -1;
                        }
                        if (info.bCoinType == 4)
                        {
                            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GoldCoin >= info.dwCoinPrice)
                            {
                                return 0;
                            }
                            reason = Singleton<CTextManager>.GetInstance().GetText("Common_GoldCoin_Not_Enough");
                            return -2;
                        }
                        object[] inParameters = new object[] { info.bCoinType };
                        DebugHelper.Assert(false, "Invalid coin type: {0}", inParameters);
                        reason = Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Money_Type_Error");
                        return -3;
                    }
                }
                CoinType = 0;
                Price = 0;
                reason = Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Can_Not_Find_Config");
                return -4;
            }
            CoinType = 0;
            Price = 0;
            reason = Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Level_Refresh_Max");
            return -5;
        }

        private EnterAdvError CanEnterLevel(int chapterId, int levelSeq, int difficulty)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return EnterAdvError.Other;
            }
            ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey((long) chapterId);
            DebugHelper.Assert(dataByKey != null, string.Format("chapterId[{0}]", chapterId));
            if (dataByKey == null)
            {
                return EnterAdvError.Other;
            }
            bool flag = Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock(dataByKey.dwChapterId);
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = masterRoleInfo.pveLevelDetail[difficulty - 1];
            PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = pve_adv_complete_info.ChapterDetailList[chapterId - 1];
            if ((pve_chapter_complete_info.LevelDetailList[levelSeq - 1].levelStatus != 0) && flag)
            {
                return EnterAdvError.None;
            }
            return EnterAdvError.Locked;
        }

        private bool CheckChapterRewardCondition()
        {
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
            if (pve_adv_complete_info == null)
            {
                return false;
            }
            PVE_CHAPTER_COMPLETE_INFO chapterInfo = pve_adv_complete_info.ChapterDetailList[this.currentChapter - 1];
            if (chapterInfo == null)
            {
                return false;
            }
            return ((chapterInfo.bIsGetBonus == 0) && (GetChapterTotalStar(chapterInfo) == (STAR_PER_LEVEL * LEVEL_PER_CHAPTER)));
        }

        private bool CheckFullItem(out string itemName)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) this.currentLevelId);
            object[] inParameters = new object[] { this.currentLevelId };
            DebugHelper.Assert(dataByKey != null, "level config is null with level ID: {0}", inParameters);
            itemName = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                if (dataByKey.astRewardShowDetail[i].dwRewardID != 0)
                {
                    CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    int useableStackCount = 0;
                    if (dataByKey.astRewardShowDetail[i].bRewardType == 2)
                    {
                        useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dataByKey.astRewardShowDetail[i].dwRewardID);
                        ResPropInfo info3 = GameDataMgr.itemDatabin.GetDataByKey(dataByKey.astRewardShowDetail[i].dwRewardID);
                        object[] objArray2 = new object[] { dataByKey.astRewardShowDetail[i].dwRewardID };
                        DebugHelper.Assert(info3 != null, "item is null with ID: {0}", objArray2);
                        itemName = StringHelper.UTF8BytesToString(ref info3.szName);
                        if (useableStackCount == info3.iOverLimit)
                        {
                            return false;
                        }
                    }
                    else if (dataByKey.astRewardShowDetail[i].bRewardType == 3)
                    {
                        useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, dataByKey.astRewardShowDetail[i].dwRewardID);
                        ResEquipInfo info4 = GameDataMgr.equipInfoDatabin.GetDataByKey(dataByKey.astRewardShowDetail[i].dwRewardID);
                        object[] objArray3 = new object[] { dataByKey.astRewardShowDetail[i].dwRewardID };
                        DebugHelper.Assert(info4 != null, "equip is null with ID: {0}", objArray3);
                        itemName = StringHelper.UTF8BytesToString(ref info4.szName);
                        if (useableStackCount == info4.iOverLimit)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckMopUpCondition(int time)
        {
            if (!this.HasEnoughMopupTicket(time))
            {
                int num;
                if (this.HasEnoughDiamondForTicket(time, out num))
                {
                    stUIEventParams par = new stUIEventParams {
                        tag = time
                    };
                    string[] args = new string[] { num.ToString() };
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Buy_Sweep_Ticket_Confirm", args), enUIEventID.Adv_ConfirmDiamondMopup, enUIEventID.None, par, false);
                    return false;
                }
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Not_Enough_Sweep_Ticket"), false);
                return false;
            }
            string itemName = string.Empty;
            if (!this.CheckFullItem(out itemName))
            {
                stUIEventParams params2 = new stUIEventParams {
                    tag = time
                };
                string[] textArray2 = new string[] { itemName.ToString() };
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Package_Item_Max_Tip_Sweep", textArray2), enUIEventID.Adv_ConfirmItemFullMopup, enUIEventID.None, params2, false);
                return false;
            }
            return true;
        }

        private bool CheckOpenChapterRewardCondition(out bool bCanGetReward, int chapterId)
        {
            bCanGetReward = false;
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
            if (pve_adv_complete_info != null)
            {
                PVE_CHAPTER_COMPLETE_INFO chapterInfo = pve_adv_complete_info.ChapterDetailList[chapterId - 1];
                if (chapterInfo == null)
                {
                    return true;
                }
                if (chapterInfo.bIsGetBonus != 0)
                {
                    return false;
                }
                if (GetChapterTotalStar(chapterInfo) == (STAR_PER_LEVEL * LEVEL_PER_CHAPTER))
                {
                    bCanGetReward = true;
                }
            }
            return true;
        }

        private bool CheckTeamNum()
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) this.currentLevelId);
            if (dataByKey == null)
            {
                object[] inParameters = new object[] { this.currentLevelId };
                DebugHelper.Assert(false, "Can't find Level Config -- LevelID: ", inParameters);
                return false;
            }
            COMDT_BATTLELIST_LIST comdt_battlelist_list = CHeroSelectBaseSystem.s_defaultBattleListInfo;
            int num = 0;
            if ((comdt_battlelist_list == null) || (comdt_battlelist_list.dwListNum == 0))
            {
                return false;
            }
            for (int i = 0; i < comdt_battlelist_list.dwListNum; i++)
            {
                if (comdt_battlelist_list.astBattleList[i].dwBattleListID == dataByKey.dwBattleListID)
                {
                    if (comdt_battlelist_list.astBattleList[i].stBattleList.wHeroCnt == 0)
                    {
                        return false;
                    }
                    for (int j = 0; j < comdt_battlelist_list.astBattleList[i].stBattleList.wHeroCnt; j++)
                    {
                        if (comdt_battlelist_list.astBattleList[i].stBattleList.BattleHeroList[j] > 0)
                        {
                            num++;
                        }
                    }
                    break;
                }
            }
            return (num == dataByKey.iHeroNum);
        }

        private void CheckUnlockTips()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if ((this.m_chapterStatus == null) || (this.m_difficultyStatus == null))
                {
                    this.m_chapterStatus = new byte[CHAPTER_NUM];
                    this.m_difficultyStatus = new byte[CHAPTER_NUM * LEVEL_DIFFICULT_OPENED];
                    for (int i = 0; i < CHAPTER_NUM; i++)
                    {
                        this.m_chapterStatus[i] = !Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint) (i + 1)) ? ((byte) 0) : ((byte) 1);
                    }
                    for (int j = 0; j < LEVEL_DIFFICULT_OPENED; j++)
                    {
                        for (int k = 0; k < CHAPTER_NUM; k++)
                        {
                            if ((this.m_chapterStatus[k] == 1) && (masterRoleInfo.pveLevelDetail[j].ChapterDetailList[k].LevelDetailList[0].levelStatus != 0))
                            {
                                this.m_difficultyStatus[(j * LEVEL_DIFFICULT_OPENED) + k] = 1;
                            }
                        }
                    }
                }
                else
                {
                    int id = -1;
                    int num5 = 0;
                    for (int m = 0; m < CHAPTER_NUM; m++)
                    {
                        if ((this.m_chapterStatus[m] == 0) && Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint) (m + 1)))
                        {
                            this.m_chapterStatus[m] = 1;
                            id = m;
                            this.bNewChapterId = (byte) (m + 1);
                        }
                    }
                    for (int n = 0; n < LEVEL_DIFFICULT_OPENED; n++)
                    {
                        for (int num8 = 0; num8 < CHAPTER_NUM; num8++)
                        {
                            if (((this.m_chapterStatus[num8] == 1) && (this.m_difficultyStatus[(n * LEVEL_DIFFICULT_OPENED) + num8] == 0)) && (masterRoleInfo.pveLevelDetail[n].ChapterDetailList[num8].LevelDetailList[0].levelStatus != 0))
                            {
                                num5 = n;
                                id = num8;
                                this.m_difficultyStatus[(n * LEVEL_DIFFICULT_OPENED) + num8] = 1;
                            }
                        }
                    }
                    if (id >= 0)
                    {
                        ResChapterInfo dataByIndex = GameDataMgr.chapterInfoDatabin.GetDataByIndex(id);
                        if (dataByIndex != null)
                        {
                            object[] replaceArr = new object[] { Utility.UTF8Convert(dataByIndex.szChapterName), Singleton<CTextManager>.instance.GetText(string.Format("Adventure_Level_{0}", num5 + 1)) };
                            Singleton<CUIManager>.instance.OpenTips("Adventure_Unlock_Tips", true, 1f, null, replaceArr);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            this.currentChapter = 0;
            this.currentDifficulty = 0;
            this.currentLevelSeq = 0;
        }

        private void CloseChapterRewardPanel(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
            if (form != null)
            {
                CAdventureView.CloseChapterRewardPanel(form.gameObject);
            }
        }

        private void ConfirmCheckTeamNum(CUIEvent uiEvent = null)
        {
            string itemName = string.Empty;
            if (!this.CheckFullItem(out itemName))
            {
                string[] args = new string[] { itemName };
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Package_Item_Max_Tip_Level", args), enUIEventID.Adv_ConfirmItemFullAdv, enUIEventID.None, false);
            }
            else
            {
                this.ConfirmItemFullAdv(null);
            }
        }

        private void ConfirmCheckTeamPower(CUIEvent uiEvent = null)
        {
            if ((this.currentChapter > 1) && !this.CheckTeamNum())
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Adv_TeamNum_Confirm"), enUIEventID.Adv_ConfirmTeamNumCheck, enUIEventID.None, false);
            }
            else
            {
                this.ConfirmCheckTeamNum(null);
            }
        }

        private void ConfirmItemFullAdv(CUIEvent uiEvent = null)
        {
            CHeroSelectBaseSystem.SendSingleGameStartMsgSkipHeroSelect(this.currentLevelId, this.currentDifficulty);
        }

        private void ConfirmItemFullMopup(CUIEvent uiEvent)
        {
            byte tag = (byte) uiEvent.m_eventParams.tag;
            this.ReqMopup(tag);
        }

        private byte[] GetCacheLevel()
        {
            byte[] buffer = new byte[3];
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                buffer[0] = (byte) PlayerPrefs.GetInt(string.Format("{0}_{1}", "Sgame_Chapter", masterRoleInfo.playerUllUID));
                buffer[1] = (byte) PlayerPrefs.GetInt(string.Format("{0}_{1}", "Sgame_Level", masterRoleInfo.playerUllUID));
                buffer[2] = (byte) PlayerPrefs.GetInt(string.Format("{0}_{1}", "Sgame_Difficulty", masterRoleInfo.playerUllUID));
            }
            return buffer;
        }

        private void GetChapterReward(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidget.GetComponent<Button>().interactable && this.CheckChapterRewardCondition())
            {
                this.ReqGetChapterReward();
            }
        }

        public static int GetChapterTotalStar(PVE_CHAPTER_COMPLETE_INFO chapterInfo)
        {
            int num = 0;
            for (int i = 0; i < chapterInfo.LevelDetailList.Length; i++)
            {
                if (chapterInfo.LevelDetailList[i] != null)
                {
                    num += GetStarNum(chapterInfo.LevelDetailList[i].bStarBits);
                }
            }
            return num;
        }

        public static int GetLastChapter(int difficulty = 1)
        {
            int num = 1;
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                for (int i = 0; i < CHAPTER_NUM; i++)
                {
                    if (!Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint) (i + 1)))
                    {
                        return num;
                    }
                    num = i + 1;
                }
            }
            return num;
        }

        public static int GetLastDifficulty(int chapterId)
        {
            int num = 1;
            if (Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint) chapterId))
            {
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
                {
                    return num;
                }
                int num2 = LEVEL_DIFFICULT_OPENED;
                for (int i = 0; i < num2; i++)
                {
                    if (IsDifOpen(chapterId, i + 1))
                    {
                        num = i + 1;
                    }
                }
            }
            return num;
        }

        public static int GetLastLevel(int chapterId, int difficulty)
        {
            int num = 1;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if ((chapterId <= 0) || (difficulty <= 0))
                {
                    return num;
                }
                if ((LEVEL_DIFFICULT_OPENED < difficulty) || (masterRoleInfo.pveLevelDetail[difficulty - 1] == null))
                {
                    return num;
                }
                if ((CHAPTER_NUM < chapterId) || (masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[chapterId - 1] == null))
                {
                    return num;
                }
                PVE_ADV_COMPLETE_INFO pve_adv_complete_info = masterRoleInfo.pveLevelDetail[difficulty - 1];
                PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = pve_adv_complete_info.ChapterDetailList[chapterId - 1];
                PVE_LEVEL_COMPLETE_INFO[] levelDetailList = pve_chapter_complete_info.LevelDetailList;
                for (int i = 0; i < levelDetailList.Length; i++)
                {
                    if (levelDetailList[i].levelStatus != 0)
                    {
                        num = i + 1;
                    }
                }
            }
            return num;
        }

        public static ResLevelCfgInfo GetLevelCfg(int ChapterId, int Seq, int Difficulty)
        {
            <GetLevelCfg>c__AnonStorey7F storeyf = new <GetLevelCfg>c__AnonStorey7F {
                ChapterId = ChapterId,
                Seq = Seq
            };
            return GameDataMgr.levelDatabin.FindIf(new Func<ResLevelCfgInfo, bool>(storeyf.<>m__7B));
        }

        public static int GetLevelId(int ChapterId, int Seq, int Difficulty)
        {
            <GetLevelId>c__AnonStorey80 storey = new <GetLevelId>c__AnonStorey80 {
                ChapterId = ChapterId,
                Seq = Seq,
                Difficulty = Difficulty
            };
            ResLevelCfgInfo info = GameDataMgr.levelDatabin.FindIf(new Func<ResLevelCfgInfo, bool>(storey.<>m__7C));
            return ((info == null) ? 0 : info.iCfgID);
        }

        public static int GetNextChapterId(int ChapterId, int Seq)
        {
            if (Seq < LEVEL_PER_CHAPTER)
            {
                return ChapterId;
            }
            return (ChapterId + 1);
        }

        public static int GetNextLevelId(int ChapterId, int Seq, int Difficulty)
        {
            if (Seq < LEVEL_PER_CHAPTER)
            {
                return GetLevelId(ChapterId, ++Seq, Difficulty);
            }
            return GetLevelId(++ChapterId, 1, Difficulty);
        }

        public static int GetStarNum(byte starMask)
        {
            return (((((starMask & 1) <= 0) ? 0 : 1) + (((starMask & 2) <= 0) ? 0 : 1)) + (((starMask & 4) <= 0) ? 0 : 1));
        }

        private bool HasEnoughAPMopup(int count)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) this.currentLevelId);
            DebugHelper.Assert(dataByKey != null);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null);
            return (masterRoleInfo.CurActionPoint >= ((dataByKey.dwEnterConsumeAP + dataByKey.dwFinishConsumeAP) * count));
        }

        private bool HasEnoughDiamondForTicket(int count, out int ReqDiamondNum)
        {
            int useableStackCount = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, MOPUP_TICKET_ID);
            DebugHelper.Assert(useableStackCount < (count * MOPUP_TICKET_NUM_PER_LEVEL));
            ReqDiamondNum = (int) (((count * MOPUP_TICKET_NUM_PER_LEVEL) - useableStackCount) * MOPUP_TICKET_PRICE_BY_DIAMOND);
            return (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().DianQuan >= ((long) ReqDiamondNum));
        }

        private bool HasEnoughMopupTicket(int count)
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, MOPUP_TICKET_ID) >= (count * MOPUP_TICKET_NUM_PER_LEVEL));
        }

        private bool HasMoreStar(byte newBits, byte oldBits)
        {
            return (GetStarNum(newBits) >= GetStarNum(oldBits));
        }

        public override void Init()
        {
            base.Init();
            CHAPTER_NUM = GameDataMgr.chapterInfoDatabin.Count();
            MOPUP_TICKET_ID = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x21).dwConfValue;
            MOPUP_TICKET_NUM_PER_LEVEL = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x22).dwConfValue;
            ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(MOPUP_TICKET_ID);
            object[] inParameters = new object[] { MOPUP_TICKET_ID };
            DebugHelper.Assert(dataByKey != null, "Can't find Mopup ticket config -- ID: {0}", inParameters);
            MOPUP_TICKET_PRICE_BY_DIAMOND = dataByKey.dwCouponsBuy;
            CHALLENGE_BUYTIME_LIMIT = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x2a).dwConfValue;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_CloseChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectLevel, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectLevel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectDifficult, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectDifficult));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectChapter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenLevelForm, new CUIEventManager.OnUIEventHandler(this.OnLevelDetail_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectPreChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectPreChapter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_SelectNextChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectNextChapter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenChooseHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChooseHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ChooseHeroReady, new CUIEventManager.OnUIEventHandler(this.OnChooseHeroReady));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_Mopup, new CUIEventManager.OnUIEventHandler(this.OnMopup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_MopupTenTimes, new CUIEventManager.OnUIEventHandler(this.OnMopupTenTimes));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmDiamondMopup, new CUIEventManager.OnUIEventHandler(this.OnConfirmDiamondMopup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.OpenChapterRewardPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_CloseChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.CloseChapterRewardPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_GetChapterReward, new CUIEventManager.OnUIEventHandler(this.GetChapterReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_BuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnBuyPlayTime));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmBuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyPlayTime));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmItemFullMopup, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullMopup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmItemFullAdv, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullAdv));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_CloseSettleForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSettleForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmTeamPowerCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamPower));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ConfirmTeamNumCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamNum));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Explore_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenExplore));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Explore_ClsoeForm, new CUIEventManager.OnUIEventHandler(this.OnCloseExplore));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ExploreScroll, new CUIEventManager.OnUIEventHandler(this.OnExploreListScroll));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_ExploreSelect, new CUIEventManager.OnUIEventHandler(this.OnExploreSelect));
            Singleton<EventRouter>.instance.AddEventHandler<CSPkg>("ShopBuyLvlChallengeTime", new Action<CSPkg>(this.OnBuyPlayTimeRsp));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.SINGLEGAME_ERR_FREEHERO, new System.Action(this.OnFreeHeroChanged));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new System.Action(CAdventureSys.ResetElitePlayNum));
        }

        public static bool IsChapterFullStar(int chapterNum, int difficulty)
        {
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[difficulty - 1];
            if (pve_adv_complete_info == null)
            {
                return false;
            }
            PVE_CHAPTER_COMPLETE_INFO chapterInfo = pve_adv_complete_info.ChapterDetailList[chapterNum - 1];
            if (chapterInfo == null)
            {
                return false;
            }
            return (GetChapterTotalStar(chapterInfo) == (STAR_PER_LEVEL * LEVEL_PER_CHAPTER));
        }

        public static bool IsDifOpen(int chapterId, int difficulty)
        {
            if (!Singleton<CFunctionUnlockSys>.instance.ChapterIsUnlock((uint) chapterId))
            {
                return false;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return false;
            }
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = masterRoleInfo.pveLevelDetail[difficulty - 1];
            PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = pve_adv_complete_info.ChapterDetailList[chapterId - 1];
            return (pve_chapter_complete_info.LevelDetailList[0].levelStatus != 0);
        }

        public bool IsLevelFinished(int inLevelId)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) inLevelId);
            object[] inParameters = new object[] { inLevelId };
            DebugHelper.Assert(dataByKey != null, "Can't find level config with ID: {0}", inParameters);
            DebugHelper.Assert(dataByKey.bLevelDifficulty > 0, "LevelDifficulty must > 0");
            PVE_ADV_COMPLETE_INFO pveInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[dataByKey.bLevelDifficulty - 1];
            if (pveInfo == null)
            {
                return false;
            }
            return this.IsLevelFinished(pveInfo, dataByKey.iChapterId, dataByKey.bLevelNo);
        }

        private bool IsLevelFinished(PVE_ADV_COMPLETE_INFO pveInfo, int ChapterNum, int LevelSeq)
        {
            return (this.IsLevelOpened(pveInfo, ChapterNum, LevelSeq) && (pveInfo.ChapterDetailList[ChapterNum - 1].LevelDetailList[LevelSeq - 1].levelStatus == 2));
        }

        public static bool IsLevelFullStar(int levelId)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) levelId);
            DebugHelper.Assert(dataByKey != null);
            DebugHelper.Assert(dataByKey.bLevelDifficulty > 0, "LevelDifficulty must > 0");
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[dataByKey.bLevelDifficulty - 1];
            if (pve_adv_complete_info == null)
            {
                return false;
            }
            PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = pve_adv_complete_info.ChapterDetailList[dataByKey.iChapterId - 1];
            if (pve_chapter_complete_info == null)
            {
                return false;
            }
            PVE_LEVEL_COMPLETE_INFO pve_level_complete_info = pve_chapter_complete_info.LevelDetailList[dataByKey.bLevelNo - 1];
            if (pve_level_complete_info == null)
            {
                return false;
            }
            return (GetStarNum(pve_level_complete_info.bStarBits) == STAR_PER_LEVEL);
        }

        public bool IsLevelOpen(int LevelId)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) LevelId);
            object[] inParameters = new object[] { LevelId };
            DebugHelper.Assert(dataByKey != null, "Can't find level config with ID: {0}", inParameters);
            DebugHelper.Assert(dataByKey.bLevelDifficulty > 0, "LevelDifficulty must > 0");
            PVE_ADV_COMPLETE_INFO pveInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[dataByKey.bLevelDifficulty - 1];
            if (pveInfo == null)
            {
                return false;
            }
            if (!Singleton<CFunctionUnlockSys>.GetInstance().ChapterIsUnlock((uint) dataByKey.iChapterId))
            {
                return false;
            }
            return this.IsLevelOpened(pveInfo, dataByKey.iChapterId, dataByKey.bLevelNo);
        }

        private bool IsLevelOpened(PVE_ADV_COMPLETE_INFO pveInfo, int ChapterNum, int LevelSeq)
        {
            return (pveInfo.ChapterDetailList[ChapterNum - 1].LevelDetailList[LevelSeq - 1].levelStatus != 0);
        }

        private bool IsLevelVaild(int chapterId, int levelSeq, int difficulty)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return false;
            }
            if (((chapterId <= 0) || (levelSeq <= 0)) || (difficulty <= 0))
            {
                return false;
            }
            if ((LEVEL_DIFFICULT_OPENED < difficulty) || (masterRoleInfo.pveLevelDetail[difficulty - 1] == null))
            {
                return false;
            }
            if ((CHAPTER_NUM < chapterId) || (masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[chapterId - 1] == null))
            {
                return false;
            }
            return ((masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[levelSeq - 1].LevelDetailList.Length >= levelSeq) && (masterRoleInfo.pveLevelDetail[difficulty - 1].ChapterDetailList[chapterId - 1].LevelDetailList[levelSeq - 1] != null));
        }

        public static bool IsStarGained(byte starMask, int Pos)
        {
            int num = (int) Math.Pow(2.0, (double) (Pos - 1));
            return ((starMask & num) > 0);
        }

        private void OnAdv_CloseForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(ADVENTURE_SELECT_FORM);
            Singleton<CResourceManager>.instance.UnloadUnusedAssets();
        }

        private void OnAdv_OpenForm(CUIEvent uiEvent)
        {
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(ADVENTURE_SELECT_FORM, true, true);
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                {
                    Singleton<CBattleGuideManager>.instance.bTrainingAdv = false;
                    if (!this.IsLevelVaild(this.currentChapter, this.currentLevelSeq, this.currentDifficulty))
                    {
                        byte[] cacheLevel = this.GetCacheLevel();
                        this.currentChapter = cacheLevel[0];
                        this.currentDifficulty = cacheLevel[2];
                        this.currentLevelSeq = cacheLevel[1];
                    }
                    if (!this.IsLevelVaild(this.currentChapter, this.currentLevelSeq, this.currentDifficulty))
                    {
                        this.currentChapter = 1;
                        this.currentDifficulty = 1;
                        this.currentLevelSeq = 1;
                    }
                    ResLevelCfgInfo info2 = GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    if (info2 != null)
                    {
                        this.currentLevelId = info2.iCfgID;
                        this.CheckUnlockTips();
                        CAdventureView.InitChapterForm(formScript, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                        MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.enterPveEntry, new uint[0]);
                    }
                }
            }
        }

        private void OnAdv_SelectChapter(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            if (tag != this.currentChapter)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
                if (form != null)
                {
                    this.currentChapter = tag;
                    this.currentDifficulty = GetLastDifficulty(this.currentChapter);
                    this.currentLevelSeq = GetLastLevel(this.currentChapter, this.currentDifficulty);
                    ResLevelCfgInfo info = GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    if (info == null)
                    {
                        return;
                    }
                    this.currentLevelId = info.iCfgID;
                    if (this.currentChapter == this.bNewChapterId)
                    {
                        this.bNewChapterId = 0;
                    }
                    CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                }
                this.SetCacheLevel((byte) this.currentChapter, (byte) this.currentLevelSeq, (byte) this.currentDifficulty);
            }
        }

        private void OnAdv_SelectDifficult(CUIEvent uiEvent)
        {
            if (this.currentDifficulty != uiEvent.m_eventParams.tag)
            {
                this.currentDifficulty = uiEvent.m_eventParams.tag;
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
                if (form != null)
                {
                    this.currentLevelSeq = Math.Min(GetLastLevel(this.currentChapter, this.currentDifficulty), this.currentLevelSeq);
                    ResLevelCfgInfo info = GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    if (info == null)
                    {
                        return;
                    }
                    this.currentLevelId = info.iCfgID;
                    CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                }
                this.SetCacheLevel((byte) this.currentChapter, (byte) this.currentLevelSeq, (byte) this.currentDifficulty);
            }
        }

        private void OnAdv_SelectLevel(CUIEvent uiEvent)
        {
            if (this.currentLevelSeq != uiEvent.m_eventParams.tag)
            {
                this.currentLevelSeq = uiEvent.m_eventParams.tag;
                ResLevelCfgInfo info = GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                if (info != null)
                {
                    this.currentLevelId = info.iCfgID;
                    CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
                    if (form != null)
                    {
                        CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                        CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                        CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    }
                    this.SetCacheLevel((byte) this.currentChapter, (byte) this.currentLevelSeq, (byte) this.currentDifficulty);
                }
            }
        }

        private void OnAdv_SelectNextChapter(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
            if ((form != null) && ((this.currentChapter + 1) <= CHAPTER_NUM))
            {
                this.currentChapter++;
                if (this.currentChapter == this.bNewChapterId)
                {
                    this.bNewChapterId = 0;
                }
                CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
            }
        }

        private void OnAdv_SelectPreChapter(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
            if ((form != null) && ((this.currentChapter - 1) > 0))
            {
                this.currentChapter--;
                if (this.currentChapter == this.bNewChapterId)
                {
                    this.bNewChapterId = 0;
                }
                CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
            }
        }

        private void OnBuyPlayTime(CUIEvent uiEvent)
        {
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
            DebugHelper.Assert(pve_adv_complete_info != null);
            PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = pve_adv_complete_info.ChapterDetailList[this.currentChapter - 1];
            DebugHelper.Assert(pve_chapter_complete_info != null);
            PVE_LEVEL_COMPLETE_INFO levelInfo = pve_chapter_complete_info.LevelDetailList[this.currentLevelSeq - 1];
            byte coinType = 0;
            uint price = 0;
            string reason = string.Empty;
            switch (this.CanBuyPlayTime(levelInfo, out coinType, out price, out reason))
            {
                case 0:
                {
                    string str2 = (coinType != 2) ? Singleton<CTextManager>.GetInstance().GetText("Money_Type_GoldCoin") : Singleton<CTextManager>.GetInstance().GetText("Money_Type_DianQuan");
                    string[] args = new string[] { price.ToString(), str2 };
                    string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Level_Refresh_Confirm", args), new object[0]);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Adv_ConfirmBuyPlayTime, enUIEventID.None, false);
                    break;
                }
                case -1:
                    CUICommonSystem.OpenDianQuanNotEnoughTip();
                    break;

                case -2:
                    CUICommonSystem.OpenGoldCoinNotEnoughTip();
                    break;

                default:
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Can_Not_Refresh_Level") + reason, false, 1.5f, null, new object[0]);
                    break;
            }
        }

        public void OnBuyPlayTimeRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) msg.stPkgData.stShopBuyRsp.stExtraInfo.dwLevelID);
            DebugHelper.Assert(dataByKey != null);
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[Singleton<CAdventureSys>.GetInstance().currentDifficulty - 1];
            DebugHelper.Assert(pve_adv_complete_info != null);
            PVE_LEVEL_COMPLETE_INFO pve_level_complete_info = pve_adv_complete_info.ChapterDetailList[dataByKey.iChapterId - 1].LevelDetailList[dataByKey.bLevelNo - 1];
            pve_level_complete_info.PlayLimit = msg.stPkgData.stShopBuyRsp.iBuySubType;
            pve_level_complete_info.PlayNum = 0;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_LEVEL_FORM);
            if (form != null)
            {
                CAdventureView.InitLevelForm(form, dataByKey.iChapterId, dataByKey.bLevelNo, this.currentDifficulty);
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
        }

        private void OnChooseHeroReady(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidget.GetComponent<Button>().interactable)
            {
                DebugHelper.Assert(this.currentLevelId != 0);
                DebugHelper.Assert(this.currentChapter != 0);
                DebugHelper.Assert(this.currentLevelSeq != 0);
                DebugHelper.Assert(GameDataMgr.levelDatabin.GetDataByKey((long) this.currentLevelId) != null);
                DebugHelper.Assert(Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null);
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                }
                else
                {
                    this.ConfirmCheckTeamPower(null);
                }
            }
        }

        private void OnCloseExplore(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.instance.CloseForm(EXLPORE_FORM_PATH);
        }

        private void OnCloseSettleForm(CUIEvent uiEvent)
        {
            CAdventureView.CheckMopupLevelUp();
        }

        private void OnConfirmBuyPlayTime(CUIEvent uiEvent)
        {
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[this.currentDifficulty - 1];
            DebugHelper.Assert(pve_adv_complete_info != null);
            PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = pve_adv_complete_info.ChapterDetailList[this.currentChapter - 1];
            DebugHelper.Assert(pve_chapter_complete_info != null);
            PVE_LEVEL_COMPLETE_INFO pve_level_complete_info = pve_chapter_complete_info.LevelDetailList[this.currentLevelSeq - 1];
            this.ReqBuyPlayTime(pve_level_complete_info.PlayLimit + 1);
        }

        private void OnConfirmDiamondMopup(CUIEvent uiEvent)
        {
            string itemName = string.Empty;
            byte tag = (byte) uiEvent.m_eventParams.tag;
            if (!this.CheckFullItem(out itemName))
            {
                stUIEventParams par = new stUIEventParams {
                    tag = tag
                };
                string[] args = new string[] { itemName };
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_Package_Item_Max_Tip_Sweep", args), enUIEventID.Adv_ConfirmItemFullMopup, enUIEventID.None, par, false);
            }
            else
            {
                this.ReqMopup(tag);
            }
        }

        private void OnDragStart(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.instance.PostEvent("UI_Add_Button", null);
        }

        private void OnExploreListScroll(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(EXLPORE_FORM_PATH);
            if (form != null)
            {
                CExploreView.OnExploreListScroll(form.gameObject);
            }
        }

        private void OnExploreSelect(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.instance.PostEvent("UI_Add_Button", null);
        }

        private void OnFreeHeroChanged()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_LEVEL_FORM);
            if (form != null)
            {
                CAdventureView.InitLevelForm(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
            }
        }

        [MessageHandler(0x421)]
        public static void OnGetChapterReward(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stGetChapterRewardRsp.iErrorCode == 0)
            {
                PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[msg.stPkgData.stGetChapterRewardRsp.bDifficultType - 1];
                object[] inParameters = new object[] { msg.stPkgData.stGetChapterRewardRsp.bDifficultType };
                DebugHelper.Assert(pve_adv_complete_info != null, "PVE info is NULL!!! -- Difficulty", inParameters);
                pve_adv_complete_info.ChapterDetailList[msg.stPkgData.stGetChapterRewardRsp.bChapterNo - 1].bIsGetBonus = 1;
                ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(msg.stPkgData.stGetChapterRewardRsp.stRewardDetail);
                Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(useableListFromReward), null, false, enUIEventID.None, false, false, "Form_Award");
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
                if (form != null)
                {
                    CAdventureView.InitChapterForm(form, msg.stPkgData.stGetChapterRewardRsp.bChapterNo, Singleton<CAdventureSys>.instance.currentLevelSeq, msg.stPkgData.stGetChapterRewardRsp.bDifficultType);
                    CAdventureView.CloseChapterRewardPanel(form.gameObject);
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("PVE_Level_All_Chapter_Starts_Error") + Utility.ProtErrCodeToStr(0x421, msg.stPkgData.stGetChapterRewardRsp.iErrorCode), false, 1.5f, null, new object[0]);
            }
        }

        private void OnLevelDetail_OpenForm(CUIEvent uiEvent)
        {
            ResLevelCfgInfo info = GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
            if (info != null)
            {
                DebugHelper.Assert(info != null);
                switch (this.CanEnterLevel(this.currentChapter, this.currentLevelSeq, this.currentDifficulty))
                {
                    case EnterAdvError.Other:
                        Singleton<CUIManager>.instance.OpenTips("Level_Error_Tips_2", true, 1.5f, null, new object[0]);
                        break;

                    case EnterAdvError.None:
                        CAdventureView.InitLevelForm(Singleton<CUIManager>.GetInstance().OpenForm(ADVENTURE_LEVEL_FORM, false, true), this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                        if (((this.currentChapter == 1) && (this.currentLevelSeq == 1)) && (this.currentDifficulty == 1))
                        {
                            Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.PVE_1_1_1_Enter;
                            MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(12, true, false);
                        }
                        break;

                    case EnterAdvError.Locked:
                        if (this.currentDifficulty == 1)
                        {
                            Singleton<CUIManager>.instance.OpenTips("Level_Error_Tips_1", true, 1.5f, null, new object[0]);
                        }
                        else
                        {
                            Singleton<CUIManager>.instance.OpenTips("Level_Error_Tips_3", true, 1.5f, null, new object[0]);
                        }
                        break;
                }
            }
        }

        private void OnMopup(CUIEvent uiEvent)
        {
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SAODANG))
            {
                if (uiEvent.m_srcWidget.GetComponent<Button>().interactable)
                {
                    if (this.CheckMopUpCondition(1))
                    {
                        this.ReqMopup(1);
                    }
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Mopup_Condition", true, 1.5f, null, new object[0]);
                }
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 7);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        private void OnMopupTenTimes(CUIEvent uiEvent)
        {
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SAODANG))
            {
                if (uiEvent.m_srcWidget.GetComponent<Button>().interactable)
                {
                    byte tag = (byte) uiEvent.m_eventParams.tag;
                    if (this.CheckMopUpCondition(tag))
                    {
                        this.ReqMopup(tag);
                    }
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Mopup10_Condition", true, 1.5f, null, new object[0]);
                }
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 7);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        private void OnOpenChooseHero(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidget.GetComponent<Button>().interactable)
            {
                object[] inParameters = new object[] { this.currentLevelId };
                DebugHelper.Assert(GameDataMgr.levelDatabin.GetDataByKey((long) this.currentLevelId) != null, "Can't find level config -- ID: {0}", inParameters);
                DebugHelper.Assert(Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null, "Master role info is NULL!");
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                }
                else
                {
                    ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) Singleton<CAdventureSys>.GetInstance().currentLevelId);
                    CSDT_SINGLE_GAME_OF_ADVENTURE reportInfo = new CSDT_SINGLE_GAME_OF_ADVENTURE {
                        iLevelID = dataByKey.iCfgID,
                        bChapterNo = (byte) dataByKey.iChapterId,
                        bLevelNo = dataByKey.bLevelNo,
                        bDifficultType = (byte) this.currentDifficulty
                    };
                    Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithAdventure(dataByKey.dwBattleListID, reportInfo, StringHelper.UTF8BytesToString(ref dataByKey.szName));
                    Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enPVE_Adventure, (byte) dataByKey.iHeroNum, 0, 0, 0);
                }
            }
        }

        private void OnOpenExplore(CUIEvent uiEvent)
        {
            Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.None;
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                CUIFormScript form = Singleton<CUIManager>.instance.OpenForm(EXLPORE_FORM_PATH, true, true);
                if (form != null)
                {
                    CExploreView.InitExloreList(form);
                }
            }
        }

        public void OpenAdvForm(int chapterId, int LevelNo, int difficulty)
        {
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(ADVENTURE_SELECT_FORM, true, true);
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                {
                    this.currentChapter = chapterId;
                    this.currentDifficulty = difficulty;
                    this.currentLevelSeq = LevelNo;
                    if (!this.IsLevelVaild(this.currentChapter, this.currentLevelSeq, this.currentDifficulty))
                    {
                        DebugHelper.Assert(false, string.Format("The Level is Invaild , chapterNo[{0}],levelNo[{1}],difficulty[{2}]", this.currentChapter, this.currentLevelSeq, this.currentDifficulty));
                        this.currentChapter = 1;
                        this.currentDifficulty = 1;
                        this.currentLevelSeq = 1;
                    }
                    ResLevelCfgInfo info2 = GetLevelCfg(this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    if (info2 != null)
                    {
                        this.currentLevelId = info2.iCfgID;
                        this.CheckUnlockTips();
                        CAdventureView.InitChapterForm(formScript, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                    }
                }
            }
        }

        private void OpenChapterRewardPanel(CUIEvent uiEvent)
        {
            bool bCanGetReward = false;
            int tag = uiEvent.m_eventParams.tag;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_SELECT_FORM);
            if (form != null)
            {
                this.currentChapter = tag;
                this.currentLevelSeq = GetLastLevel(this.currentChapter, this.currentDifficulty);
                CAdventureView.InitChapterList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitLevelList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitDifficultList(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                CAdventureView.InitChapterElement(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
                if (this.CheckOpenChapterRewardCondition(out bCanGetReward, tag))
                {
                    CAdventureView.OpenChapterRewardPanel(form, form.gameObject, tag, this.currentDifficulty, bCanGetReward);
                }
            }
        }

        private void RefreshLevelForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(ADVENTURE_LEVEL_FORM);
            if (form != null)
            {
                CAdventureView.InitLevelForm(form, this.currentChapter, this.currentLevelSeq, this.currentDifficulty);
            }
        }

        private void ReqBuyPlayTime(int BuyTime)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
            msg.stPkgData.stShopBuyReq.stExtraInfo.dwLevelID = (uint) this.currentLevelId;
            msg.stPkgData.stShopBuyReq.iBuyType = 6;
            msg.stPkgData.stShopBuyReq.iBuySubType = BuyTime;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void ReqGetChapterReward()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x420);
            msg.stPkgData.stGetChapterRewardReq.bChapterNo = (byte) this.currentChapter;
            msg.stPkgData.stGetChapterRewardReq.bDifficultType = (byte) this.currentDifficulty;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void ReqMopup(byte SweepCount)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x41e);
            msg.stPkgData.stSweepSingleGameReq.iLevelID = this.currentLevelId;
            msg.stPkgData.stSweepSingleGameReq.bGameType = 0;
            msg.stPkgData.stSweepSingleGameReq.dwSweepCnt = SweepCount;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ResetElitePlayNum()
        {
            PVE_ADV_COMPLETE_INFO pve_adv_complete_info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[1];
            if (pve_adv_complete_info != null)
            {
                for (int i = 0; i < pve_adv_complete_info.ChapterDetailList.Length; i++)
                {
                    if (pve_adv_complete_info.ChapterDetailList[i] != null)
                    {
                        PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = pve_adv_complete_info.ChapterDetailList[i];
                        for (int j = 0; j < pve_chapter_complete_info.LevelDetailList.Length; j++)
                        {
                            if (pve_chapter_complete_info.LevelDetailList[j] != null)
                            {
                                PVE_LEVEL_COMPLETE_INFO pve_level_complete_info = pve_chapter_complete_info.LevelDetailList[j];
                                pve_level_complete_info.PlayNum = 0;
                                pve_level_complete_info.PlayLimit = 0;
                            }
                        }
                    }
                }
                Singleton<CAdventureSys>.GetInstance().RefreshLevelForm();
                Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
            }
        }

        private void SetCacheLevel(byte chapterNo, byte levelNo, byte difficultyNo)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                PlayerPrefs.SetInt(string.Format("{0}_{1}", "Sgame_Chapter", masterRoleInfo.playerUllUID), chapterNo);
                PlayerPrefs.SetInt(string.Format("{0}_{1}", "Sgame_Level", masterRoleInfo.playerUllUID), levelNo);
                PlayerPrefs.SetInt(string.Format("{0}_{1}", "Sgame_Difficulty", masterRoleInfo.playerUllUID), difficultyNo);
            }
        }

        public void setDifficult(int indata)
        {
            this.currentDifficulty = indata;
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_OpenForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_CloseChapterForm, new CUIEventManager.OnUIEventHandler(this.OnAdv_CloseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectLevel, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectLevel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectDifficult, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectDifficult));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectChapter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectPreChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectPreChapter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_SelectNextChapter, new CUIEventManager.OnUIEventHandler(this.OnAdv_SelectNextChapter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenLevelForm, new CUIEventManager.OnUIEventHandler(this.OnLevelDetail_OpenForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenChooseHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChooseHero));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ChooseHeroReady, new CUIEventManager.OnUIEventHandler(this.OnChooseHeroReady));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_Mopup, new CUIEventManager.OnUIEventHandler(this.OnMopup));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_MopupTenTimes, new CUIEventManager.OnUIEventHandler(this.OnMopupTenTimes));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmDiamondMopup, new CUIEventManager.OnUIEventHandler(this.OnConfirmDiamondMopup));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.OpenChapterRewardPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_CloseChapterRewardPanel, new CUIEventManager.OnUIEventHandler(this.CloseChapterRewardPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_GetChapterReward, new CUIEventManager.OnUIEventHandler(this.GetChapterReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_BuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnBuyPlayTime));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmBuyPlayTime, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyPlayTime));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmItemFullMopup, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullMopup));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmItemFullAdv, new CUIEventManager.OnUIEventHandler(this.ConfirmItemFullAdv));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_CloseSettleForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSettleForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmTeamPowerCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamPower));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ConfirmTeamNumCheck, new CUIEventManager.OnUIEventHandler(this.ConfirmCheckTeamNum));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Explore_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenExplore));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Explore_ClsoeForm, new CUIEventManager.OnUIEventHandler(this.OnCloseExplore));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ExploreScroll, new CUIEventManager.OnUIEventHandler(this.OnExploreListScroll));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_ExploreSelect, new CUIEventManager.OnUIEventHandler(this.OnExploreSelect));
            Singleton<EventRouter>.instance.RemoveEventHandler<CSPkg>("ShopBuyLvlChallengeTime", new Action<CSPkg>(this.OnBuyPlayTimeRsp));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.SINGLEGAME_ERR_FREEHERO, new System.Action(this.OnFreeHeroChanged));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.GLOBAL_REFRESH_TIME, new System.Action(CAdventureSys.ResetElitePlayNum));
        }

        public bool UpdateAdvProgress(bool bWin)
        {
            PVE_ADV_COMPLETE_INFO[] pveLevelDetail = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail;
            PVE_ADV_COMPLETE_INFO pveInfo = pveLevelDetail[this.currentDifficulty - 1];
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext == null)
            {
                return false;
            }
            int chapterNo = curLvelContext.m_chapterNo;
            int levelNo = curLvelContext.m_levelNo;
            bool flag = false;
            if (bWin)
            {
                if (this.IsLevelFinished(pveInfo, chapterNo, levelNo))
                {
                    pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].levelStatus = 2;
                    PVE_LEVEL_COMPLETE_INFO pve_level_complete_info1 = pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1];
                    pve_level_complete_info1.PlayNum++;
                    byte starBits = Singleton<StarSystem>.GetInstance().GetStarBits();
                    if (this.HasMoreStar(starBits, pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].bStarBits))
                    {
                        pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].bStarBits = starBits;
                    }
                }
                else
                {
                    flag = true;
                    if (pveInfo.ChapterDetailList[chapterNo - 1] != null)
                    {
                        pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].levelStatus = 2;
                        PVE_LEVEL_COMPLETE_INFO pve_level_complete_info2 = pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1];
                        pve_level_complete_info2.PlayNum++;
                        pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo - 1].bStarBits = Singleton<StarSystem>.GetInstance().GetStarBits();
                    }
                    if (levelNo < 4)
                    {
                        if (pveInfo.ChapterDetailList[chapterNo - 1] != null)
                        {
                            pveInfo.ChapterDetailList[chapterNo - 1].LevelDetailList[levelNo].levelStatus = 1;
                            this.currentLevelSeq = levelNo + 1;
                        }
                    }
                    else if (this.currentDifficulty < LEVEL_DIFFICULT_OPENED)
                    {
                        PVE_ADV_COMPLETE_INFO pve_adv_complete_info2 = pveLevelDetail[this.currentDifficulty];
                        if (pve_adv_complete_info2.ChapterDetailList[chapterNo - 1] != null)
                        {
                            pve_adv_complete_info2.ChapterDetailList[chapterNo - 1].LevelDetailList[0].levelStatus = 1;
                            this.currentDifficulty++;
                            this.currentLevelSeq = 1;
                        }
                    }
                }
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
            return flag;
        }

        [CompilerGenerated]
        private sealed class <GetLevelCfg>c__AnonStorey7F
        {
            internal int ChapterId;
            internal int Seq;

            internal bool <>m__7B(ResLevelCfgInfo x)
            {
                return ((x.iChapterId == this.ChapterId) && (x.bLevelNo == this.Seq));
            }
        }

        [CompilerGenerated]
        private sealed class <GetLevelId>c__AnonStorey80
        {
            internal int ChapterId;
            internal int Difficulty;
            internal int Seq;

            internal bool <>m__7C(ResLevelCfgInfo x)
            {
                return (((x.iChapterId == this.ChapterId) && (x.bLevelNo == this.Seq)) && (x.bLevelDifficulty == this.Difficulty));
            }
        }
    }
}

