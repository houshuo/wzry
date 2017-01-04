namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class CSurrenderSystem : Singleton<CSurrenderSystem>
    {
        private int m_curCnt;
        private bool m_haveRights = true;
        private uint m_lastSurrenderTime;
        private int m_maxCnt;
        private System.Random m_random = new System.Random();
        private byte m_result;
        private int m_timerSeq = -1;
        public static string s_surrenderForm = "UGUI/Form/Battle/Form_Surrender.prefab";
        private static int SURRENDER_TIME_CD;
        private static int SURRENDER_TIME_START;
        private static int SURRENDER_TIME_VALID;

        public bool CanSurrender()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            return ((curLvelContext != null) && curLvelContext.IsMultilModeWithWarmBattle());
        }

        public void CloseSurrenderForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_surrenderForm);
        }

        private byte ConstrcutData(int maxCnt)
        {
            int num = 0;
            for (int i = 0; i < maxCnt; i++)
            {
                if (i != (maxCnt - 1))
                {
                    num |= ((int) 1) << i;
                }
                else if (this.m_random.Next(0, 2) == 0)
                {
                    num |= ((int) 1) << i;
                }
            }
            return (byte) num;
        }

        public void DelayCloseSurrenderForm(int delay)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_surrenderForm);
            if (form != null)
            {
                CUITimerScript component = form.transform.GetChild(0).GetComponent<CUITimerScript>();
                if ((component != null) && (component.GetCurrentTime() > delay))
                {
                    component.SetTotalTime((float) delay);
                    component.ReStartTimer();
                }
            }
        }

        private int GetSurrenderCDTime()
        {
            if (SURRENDER_TIME_CD == 0)
            {
                SURRENDER_TIME_CD = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x8b).dwConfValue;
            }
            return SURRENDER_TIME_CD;
        }

        private int GetSurrenderStartTime()
        {
            if (SURRENDER_TIME_START == 0)
            {
                SURRENDER_TIME_START = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x89).dwConfValue;
            }
            return SURRENDER_TIME_START;
        }

        private int GetSurrenderVaildTime()
        {
            if (SURRENDER_TIME_VALID == 0)
            {
                SURRENDER_TIME_VALID = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x8a).dwConfValue;
            }
            return SURRENDER_TIME_VALID;
        }

        private int GetTotalAcnt()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext == null)
            {
                return 0;
            }
            return (curLvelContext.m_pvpPlayerNum / 2);
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_Confirm, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_Against, new CUIEventManager.OnUIEventHandler(this.OnSurrenderAgainst));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_CountDown, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCountDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_TimeStart, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeStart));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
        }

        public bool InSurrenderCD()
        {
            if ((this.m_lastSurrenderTime == 0) && (((int) Singleton<FrameSynchr>.instance.LogicFrameTick) >= (this.GetSurrenderStartTime() * 0x3e8)))
            {
                return false;
            }
            if ((this.m_lastSurrenderTime > 0) && (CRoleInfo.GetCurrentUTCTime() >= (this.m_lastSurrenderTime + this.GetSurrenderCDTime())))
            {
                return false;
            }
            return true;
        }

        public bool InSurrenderCD(out uint time)
        {
            time = 0;
            if (this.m_lastSurrenderTime == 0)
            {
                if (((int) Singleton<FrameSynchr>.instance.LogicFrameTick) >= (this.GetSurrenderStartTime() * 0x3e8))
                {
                    return false;
                }
                time = ((uint) this.GetSurrenderStartTime()) - (((uint) Singleton<FrameSynchr>.instance.LogicFrameTick) / 0x3e8);
                return true;
            }
            if (this.m_lastSurrenderTime > 0)
            {
                if (CRoleInfo.GetCurrentUTCTime() >= (this.m_lastSurrenderTime + this.GetSurrenderCDTime()))
                {
                    return false;
                }
                time = (uint) ((this.m_lastSurrenderTime + this.GetSurrenderCDTime()) - CRoleInfo.GetCurrentUTCTime());
            }
            return true;
        }

        private bool IsWarmBattle()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext == null)
            {
                return false;
            }
            return curLvelContext.m_isWarmBattle;
        }

        private void OnFightOver(ref DefaultGameEventParam prm)
        {
            this.CloseSurrenderForm();
        }

        private void OnSurrenderAgainst(CUIEvent cuiEvent)
        {
            this.SendMsgSurrender(0);
        }

        private void OnSurrenderConfirm(CUIEvent cuiEvent)
        {
            if (!this.IsWarmBattle())
            {
                this.SendMsgSurrender(1);
            }
            else
            {
                this.m_lastSurrenderTime = (uint) CRoleInfo.GetCurrentUTCTime();
                this.m_maxCnt = this.GetTotalAcnt();
                this.m_curCnt = 1;
                if (this.m_maxCnt == 1)
                {
                    BattleLogic.ForceKillCrystal((int) Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
                }
                else
                {
                    this.m_haveRights = false;
                    this.m_result = this.ConstrcutData(this.m_maxCnt);
                    this.m_timerSeq = Singleton<CTimerManager>.instance.AddTimer(this.m_random.Next(500, 0xbb8), 1, new CTimer.OnTimeUpHandler(this.OnTimerWarmBattle));
                    this.OpenSurrenderForm(this.m_maxCnt, this.m_curCnt, this.m_result);
                }
            }
        }

        private void OnSurrenderCountDown(CUIEvent cuiEvent)
        {
            GameObject srcWidget = cuiEvent.m_srcWidget;
            CUITimerScript srcWidgetScript = cuiEvent.m_srcWidgetScript as CUITimerScript;
            if ((srcWidget != null) && (srcWidgetScript != null))
            {
                float num = srcWidgetScript.GetCurrentTime() / ((float) this.GetSurrenderVaildTime());
                Utility.GetComponetInChild<Slider>(srcWidget, "CountDownBar/Bar").value = num;
            }
        }

        [MessageHandler(0x1326)]
        public static void OnSurrenderNtf(CSPkg msg)
        {
            SCPKG_SURRENDER_NTF stSurrenderNtf = msg.stPkgData.stSurrenderNtf;
            Singleton<CSurrenderSystem>.instance.OnSurrenderNtf(stSurrenderNtf);
        }

        public void OnSurrenderNtf(SCPKG_SURRENDER_NTF surrenderNtf)
        {
            this.m_lastSurrenderTime = surrenderNtf.dwSurrenderTime;
            this.OpenSurrenderForm(surrenderNtf.bSurrenderValidCnt, surrenderNtf.bSurrenderCnt, surrenderNtf.bSurrenderData);
        }

        [MessageHandler(0x1325)]
        public static void OnSurrenderRsp(CSPkg msg)
        {
            Singleton<CSurrenderSystem>.instance.OnSurrenderRsp(msg.stPkgData.stSurrenderRsp);
        }

        public void OnSurrenderRsp(SCPKG_SURRENDER_RSP msg)
        {
            this.m_haveRights = false;
            this.m_lastSurrenderTime = msg.dwSurrenderTime;
        }

        private void OnSurrenderTimeStart(CUIEvent cuiEvent)
        {
            GameObject srcWidget = cuiEvent.m_srcWidget;
            CUITimerScript srcWidgetScript = cuiEvent.m_srcWidgetScript as CUITimerScript;
            if ((srcWidget != null) && (srcWidgetScript != null))
            {
                float num = srcWidgetScript.GetCurrentTime() / ((float) this.GetSurrenderVaildTime());
                Utility.GetComponetInChild<Slider>(srcWidget, "CountDownBar/Bar").value = num;
            }
        }

        private void OnSurrenderTimeUp(CUIEvent cuiEvent)
        {
            this.CloseSurrenderForm();
            this.m_haveRights = true;
        }

        private void OnTimerWarmBattle(int timerSequence)
        {
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSeq);
            this.m_timerSeq = -1;
            if (this.m_curCnt != this.m_maxCnt)
            {
                this.m_curCnt++;
                this.m_timerSeq = Singleton<CTimerManager>.instance.AddTimer(this.m_random.Next(500, 0xbb8), 1, new CTimer.OnTimeUpHandler(this.OnTimerWarmBattle));
                this.OpenSurrenderForm(this.m_maxCnt, this.m_curCnt, this.m_result);
            }
            else
            {
                this.m_haveRights = true;
                BattleLogic.ForceKillCrystal((int) Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
            }
        }

        public void OpenSurrenderForm(int maxNum, int totalNum, byte data)
        {
            bool flag = Singleton<CUIManager>.GetInstance().GetForm(s_surrenderForm) == null;
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_surrenderForm, false, true);
            GameObject gameObject = null;
            GameObject obj3 = null;
            GameObject obj4 = null;
            bool flag2 = false;
            bool bActive = false;
            if (script != null)
            {
                gameObject = script.transform.GetChild(0).gameObject;
                obj3 = gameObject.transform.Find("SurrenderElement").gameObject;
                for (int i = 0; i < 5; i++)
                {
                    obj4 = obj3.transform.GetChild(i).gameObject;
                    flag2 = i < maxNum;
                    obj4.SetActive(flag2);
                    if (flag2)
                    {
                        if (i < totalNum)
                        {
                            bActive = (data & (((int) 1) << i)) > 0;
                            obj4.transform.GetChild(0).gameObject.CustomSetActive(bActive);
                            obj4.transform.GetChild(1).gameObject.CustomSetActive(!bActive);
                        }
                        else
                        {
                            obj4.transform.GetChild(0).gameObject.CustomSetActive(false);
                            obj4.transform.GetChild(1).gameObject.CustomSetActive(false);
                        }
                    }
                }
                gameObject.transform.Find("ButtonGroup/Button_Surrender").gameObject.SetActive(this.m_haveRights);
                gameObject.transform.Find("ButtonGroup/Button_Reject").gameObject.SetActive(this.m_haveRights);
                gameObject.transform.Find("ButtonGroup/SurrenderResult_Txt").gameObject.SetActive(!this.m_haveRights);
                if (flag)
                {
                    CUITimerScript component = gameObject.GetComponent<CUITimerScript>();
                    component.SetTotalTime((float) this.GetSurrenderVaildTime());
                    component.ReStartTimer();
                }
                if (maxNum == totalNum)
                {
                    this.DelayCloseSurrenderForm(5);
                }
            }
        }

        public void Reset()
        {
            this.m_lastSurrenderTime = 0;
            this.m_haveRights = true;
            if (this.m_timerSeq != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSeq);
            }
            this.m_timerSeq = -1;
            this.m_maxCnt = 0;
            this.m_curCnt = 0;
            this.m_result = 0;
        }

        public void SendMsgSurrender(byte bSurrender)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1324);
            msg.stPkgData.stSurrenderReq.bSurrender = bSurrender;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_Confirm, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_Against, new CUIEventManager.OnUIEventHandler(this.OnSurrenderAgainst));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_CountDown, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCountDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_TimeStart, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeStart));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
        }
    }
}

