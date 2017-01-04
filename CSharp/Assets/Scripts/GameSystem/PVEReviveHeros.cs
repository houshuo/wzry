namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class PVEReviveHeros : Singleton<PVEReviveHeros>
    {
        private int m_CurDifficulType;
        private CUIFormScript m_form;
        private int m_iLevelID;
        private const int m_iMaxBuffCount = 3;
        private static int m_iRemainReviveNum = 3;
        public BUFF_ITEM m_iSelectedBuffItem;
        private int m_iTimer = -1;
        private enPayType m_relivePayType;
        private uint m_relivePayValue;
        private const int m_ReviveNumMax = 3;
        public uint[] m_WishBuffId = new uint[3];
        public static readonly string PATH_REVIVEHERO = "UGUI/Form/System/PvE/Settle/Form_ReviveHero.prefab";

        public static  event SetTriggerAllDeathResult SetReviveResult;

        public bool CheckAndPopReviveForm(SetTriggerAllDeathResult SetReviveResultFunc, bool bDelayOpenForm = true)
        {
            if (this.IsCareCondition())
            {
                if (m_iRemainReviveNum <= 0)
                {
                    return false;
                }
                if (SetReviveResultFunc == null)
                {
                    return false;
                }
                SetReviveResult = SetReviveResultFunc;
                if (!bDelayOpenForm)
                {
                    this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_REVIVEHERO, false, true);
                    if (this.m_form != null)
                    {
                        this.InitFromView(this.m_form);
                        this.PauseGame(true);
                        return true;
                    }
                    return false;
                }
                if (this.m_iTimer == -1)
                {
                    this.m_iTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0x7d0, -1, new CTimer.OnTimeUpHandler(this.OnOpenFormTimeUpHandler));
                    if (this.m_iTimer != -1)
                    {
                        return true;
                    }
                }
                else
                {
                    Singleton<CTimerManager>.GetInstance().ResetTimer(this.m_iTimer);
                    Singleton<CTimerManager>.GetInstance().ResumeTimer(this.m_iTimer);
                    return true;
                }
            }
            return false;
        }

        public void ClearTimeOutTimer()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveTimeout, new CUIEventManager.OnUIEventHandler(this.OnReviveTimeout));
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnSelectBuff0, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem0));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnSelectBuff1, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnSelectBuff2, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnReviveBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickReviveBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnExitBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickExitBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnConfirmRevive, new CUIEventManager.OnUIEventHandler(this.OnConfirmRevive));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnReviveFailed, new CUIEventManager.OnUIEventHandler(this.OnReviveHerosFailed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReviveHero_OnReviveTimeout, new CUIEventManager.OnUIEventHandler(this.OnReviveTimeout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
        }

        private void InitBuffItem(CUIFormScript formScript, SLevelContext curLevelContext)
        {
            this.m_iLevelID = curLevelContext.m_mapID;
            this.m_CurDifficulType = curLevelContext.m_levelDifficulty;
            for (int i = 0; i < 3; i++)
            {
                string name = "buffInfoPanel/buffItem" + i;
                GameObject gameObject = this.m_form.transform.Find(name).gameObject;
                this.UpdateBuffItemChooseStat(i, i == this.m_iSelectedBuffItem);
                uint key = curLevelContext.m_reviveInfo[curLevelContext.m_levelDifficulty].ReviveBuff[i];
                this.m_WishBuffId[i] = key;
                ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey(key);
                if (dataByKey != null)
                {
                    if (dataByKey.szIconPath[0] != '\0')
                    {
                        Image component = gameObject.transform.Find("imageIcon").GetComponent<Image>();
                        GameObject prefab = CUIUtility.GetSpritePrefeb(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath)), true, true);
                        component.SetSprite(prefab);
                    }
                    gameObject.transform.Find("buffNameText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref dataByKey.szSkillCombineDesc);
                }
            }
        }

        private void InitFromView(CUIFormScript formScript)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            this.InitBuffItem(formScript, curLvelContext);
            this.m_relivePayType = CMallSystem.ResBuyTypeToPayType(curLvelContext.m_reviveInfo[curLvelContext.m_levelDifficulty].astReviveCost[3 - m_iRemainReviveNum].bCostType);
            this.m_relivePayValue = curLvelContext.m_reviveInfo[curLvelContext.m_levelDifficulty].astReviveCost[3 - m_iRemainReviveNum].dwCostPrice;
            string str = string.Format(Singleton<CTextManager>.GetInstance().GetText("ReliveMessage"), Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[(int) this.m_relivePayType]));
            GameObject widget = formScript.GetWidget(0);
            if (widget != null)
            {
                Text component = widget.GetComponent<Text>();
                if (component != null)
                {
                    component.text = str;
                }
            }
            GameObject obj3 = formScript.GetWidget(1);
            if (obj3 != null)
            {
                Image image = obj3.GetComponent<Image>();
                if (image != null)
                {
                    image.SetSprite(CMallSystem.GetPayTypeIconPath(this.m_relivePayType), formScript, true, false, false);
                }
            }
            GameObject obj4 = formScript.GetWidget(2);
            if (obj4 != null)
            {
                Text text2 = obj4.GetComponent<Text>();
                if (text2 != null)
                {
                    text2.text = this.m_relivePayValue.ToString();
                }
            }
            GameObject gameObject = this.m_form.transform.Find("buffInfoPanel").gameObject;
            if (gameObject != null)
            {
                GameObject obj6 = gameObject.transform.Find("ReviveText/NumText").gameObject;
                if (obj6 != null)
                {
                    obj6.GetComponent<Text>().text = m_iRemainReviveNum.ToString();
                    CUITimerScript script = gameObject.transform.Find("Timer").GetComponent<CUITimerScript>();
                    byte reviveTimeMax = curLvelContext.m_reviveTimeMax;
                    script.SetTotalTime((float) reviveTimeMax);
                }
            }
        }

        private bool IsCareCondition()
        {
            if (!Singleton<BattleLogic>.instance.isFighting)
            {
                return false;
            }
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            return (!curLvelContext.IsMobaMode() && curLvelContext.IsGameTypeAdventure());
        }

        private void OnBuyDianQuanPanelClose(CUIEvent uiEvent)
        {
            if (this.IsCareCondition())
            {
                this.CheckAndPopReviveForm(SetReviveResult, false);
            }
        }

        private void OnClickExitBtn(CUIEvent uiEvent)
        {
            this.ReviveHerosFailed();
        }

        private void OnClickReviveBtn(CUIEvent uiEvent)
        {
            stUIEventParams confirmEventParams = new stUIEventParams {
                commonUInt32Param1 = (uint) m_iRemainReviveNum
            };
            CMallSystem.TryToPay(enPayPurpose.Relive, string.Empty, this.m_relivePayType, this.m_relivePayValue, enUIEventID.ReviveHero_OnConfirmRevive, ref confirmEventParams, enUIEventID.ReviveHero_OnReviveFailed, true, true, false);
        }

        private void OnConfirmRevive(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10cf);
            msg.stPkgData.stPveReviveReq.bReviveNo = (byte) ((3 - m_iRemainReviveNum) + 1);
            Singleton<BattleLogic>.instance.m_bIsPayStat = true;
            if (!Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true))
            {
                string text = Singleton<CTextManager>.instance.GetText("Common_Network_Err");
                Singleton<CUIManager>.GetInstance().OpenMessageBox(text, enUIEventID.ReviveHero_OnReviveFailed, false);
            }
        }

        private void OnFightStart(ref DefaultGameEventParam prm)
        {
            this.ResetReviveCondition();
        }

        private void OnGameEnd(ref DefaultGameEventParam prm)
        {
            if (this.IsCareCondition() && (this.m_form != null))
            {
                this.m_form.Close();
            }
        }

        private void OnOpenFormTimeUpHandler(int timerSequence)
        {
            Singleton<CTimerManager>.instance.PauseTimer(timerSequence);
            if (this.IsCareCondition())
            {
                this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_REVIVEHERO, false, true);
                if (this.m_form != null)
                {
                    this.InitFromView(this.m_form);
                    this.PauseGame(true);
                }
                else
                {
                    this.ReviveHerosFailed();
                }
            }
        }

        [MessageHandler(0x10d0)]
        public static void OnReviveHeroRsp(CSPkg msg)
        {
            Singleton<BattleLogic>.instance.m_bIsPayStat = false;
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if ((msg.stPkgData.stPveReviveRsp.iErrCode == 0) && (((msg.stPkgData.stPveReviveRsp.bDifficultType == Singleton<PVEReviveHeros>.instance.CurDifficulType) || (msg.stPkgData.stPveReviveRsp.iLevelID == Singleton<PVEReviveHeros>.instance.CurLevelId)) || (msg.stPkgData.stPveReviveRsp.bReviveNo == Singleton<PVEReviveHeros>.instance.CurReviveNum)))
            {
                ReviveAllHeros();
            }
            else
            {
                string text = Singleton<CTextManager>.instance.GetText("PVE_Revive_Data_Error");
                Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
                Singleton<PVEReviveHeros>.instance.ReviveHerosFailed();
            }
        }

        private void OnReviveHerosFailed(CUIEvent uiEvent)
        {
            this.ReviveHerosFailed();
        }

        private void OnReviveTimeout(CUIEvent uiEvent)
        {
            if (this.m_form != null)
            {
                this.m_form.Close();
            }
            this.ReviveHerosFailed();
        }

        private void OnSelectBuffItem0(CUIEvent uiEvent)
        {
            if (this.m_iSelectedBuffItem != BUFF_ITEM.BUFF_ITEM_0)
            {
                this.UpdateBuffItemChooseStat((int) this.m_iSelectedBuffItem, false);
                this.m_iSelectedBuffItem = BUFF_ITEM.BUFF_ITEM_0;
                this.UpdateBuffItemChooseStat((int) this.m_iSelectedBuffItem, true);
            }
        }

        private void OnSelectBuffItem1(CUIEvent uiEvent)
        {
            if (this.m_iSelectedBuffItem != BUFF_ITEM.BUFF_ITEM_1)
            {
                this.UpdateBuffItemChooseStat((int) this.m_iSelectedBuffItem, false);
                this.m_iSelectedBuffItem = BUFF_ITEM.BUFF_ITEM_1;
                this.UpdateBuffItemChooseStat((int) this.m_iSelectedBuffItem, true);
            }
        }

        private void OnSelectBuffItem2(CUIEvent uiEvent)
        {
            if (this.m_iSelectedBuffItem != BUFF_ITEM.BUFF_ITEM_2)
            {
                this.UpdateBuffItemChooseStat((int) this.m_iSelectedBuffItem, false);
                this.m_iSelectedBuffItem = BUFF_ITEM.BUFF_ITEM_2;
                this.UpdateBuffItemChooseStat((int) this.m_iSelectedBuffItem, true);
            }
        }

        private void PauseGame(bool bPause)
        {
            Singleton<FrameSynchr>.instance.SetSynchrRunning(!bPause);
        }

        private void ResetReviveCondition()
        {
            m_iRemainReviveNum = 3;
        }

        public static void ReviveAllHeros()
        {
            int inSkillCombineId = (int) Singleton<PVEReviveHeros>.instance.m_WishBuffId[(int) Singleton<PVEReviveHeros>.instance.m_iSelectedBuffItem];
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.ActorControl.SetReviveContext(0, 0x2710, false, false, false);
                hostPlayer.Captain.handle.ActorControl.Revive(false);
                SkillUseParam inParam = new SkillUseParam();
                inParam.SetOriginator(hostPlayer.Captain);
                hostPlayer.Captain.handle.SkillControl.SpawnBuff(hostPlayer.Captain, ref inParam, inSkillCombineId, true);
                ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = hostPlayer.GetAllHeroes();
                int count = allHeroes.Count;
                for (int i = 0; i < count; i++)
                {
                    if (((allHeroes[i] != 0) && (allHeroes[i] != hostPlayer.Captain)) && (allHeroes[i] != 0))
                    {
                        PoolObjHandle<ActorRoot> handle = allHeroes[i];
                        handle.handle.ActorControl.SetReviveContext(0, 0x2710, false, false, false);
                        PoolObjHandle<ActorRoot> handle2 = allHeroes[i];
                        handle2.handle.ActorControl.Revive(false);
                        inParam.SetOriginator(allHeroes[i]);
                        PoolObjHandle<ActorRoot> handle3 = allHeroes[i];
                        handle3.handle.SkillControl.SpawnBuff(allHeroes[i], ref inParam, inSkillCombineId, true);
                    }
                }
                m_iRemainReviveNum--;
                Singleton<PVEReviveHeros>.instance.ReviveHerosSucess();
            }
        }

        private void ReviveHerosFailed()
        {
            this.PauseGame(false);
            if (SetReviveResult != null)
            {
                SetReviveResult(false);
            }
        }

        private void ReviveHerosSucess()
        {
            this.PauseGame(false);
            if (SetReviveResult != null)
            {
                SetReviveResult(true);
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnSelectBuff0, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem0));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnSelectBuff1, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem1));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnSelectBuff2, new CUIEventManager.OnUIEventHandler(this.OnSelectBuffItem2));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickReviveBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnExitBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickExitBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnConfirmRevive, new CUIEventManager.OnUIEventHandler(this.OnConfirmRevive));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveFailed, new CUIEventManager.OnUIEventHandler(this.OnReviveHerosFailed));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReviveHero_OnReviveTimeout, new CUIEventManager.OnUIEventHandler(this.OnReviveTimeout));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
        }

        private void UpdateBuffItemChooseStat(int iItemIndex, bool bChoose)
        {
            if ((iItemIndex >= 0) && (iItemIndex < 3))
            {
                string name = "buffInfoPanel/buffItem" + iItemIndex;
                this.m_form.transform.Find(name).gameObject.transform.Find("chooseImage").gameObject.CustomSetActive(bChoose);
            }
        }

        public int CurDifficulType
        {
            get
            {
                return this.m_CurDifficulType;
            }
        }

        public int CurLevelId
        {
            get
            {
                return this.m_iLevelID;
            }
        }

        public int CurReviveNum
        {
            get
            {
                return ((3 - m_iRemainReviveNum) + 1);
            }
        }

        public delegate void SetTriggerAllDeathResult(bool bReviveOk);
    }
}

