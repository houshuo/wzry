namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;

    [MessageHandlerClass]
    public class BattleMsgHandler
    {
        [MessageHandler(0x44b)]
        public static void OnBroadHangup(CSPkg msg)
        {
            Singleton<EventRouter>.instance.BroadCastEvent<HANGUP_TYPE, uint>(EventID.HangupNtf, (HANGUP_TYPE) msg.stPkgData.stHangUpNtf.bType, msg.stPkgData.stHangUpNtf.dwObjID);
        }

        [MessageHandler(0x43d)]
        public static void OnGameOverEvent(CSPkg msg)
        {
            if (!Singleton<BattleLogic>.instance.isWaitGameEnd)
            {
                if (msg.stPkgData.stNtfCltGameOver.bWinCamp > 0)
                {
                    COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
                    Singleton<BattleLogic>.instance.battleStat.iBattleResult = (((byte) playerCamp) != msg.stPkgData.stNtfCltGameOver.bWinCamp) ? 2 : 1;
                    COM_PLAYERCAMP oppositeCmp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                    if (Singleton<BattleLogic>.instance.battleStat.iBattleResult == 1)
                    {
                        oppositeCmp = BattleLogic.GetOppositeCmp(playerCamp);
                    }
                    else
                    {
                        oppositeCmp = playerCamp;
                    }
                    BattleLogic.ForceKillCrystal((int) oppositeCmp);
                }
                Singleton<CSurrenderSystem>.instance.DelayCloseSurrenderForm(5);
            }
        }

        [MessageHandler(0x416)]
        public static void onHangup(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Hangup_Warnning"), false);
        }

        [MessageHandler(0x414)]
        public static void onIsAccept_Aiplayer(CSPkg msg)
        {
            stUIEventParams par = new stUIEventParams {
                commonUInt32Param1 = msg.stPkgData.stIsAcceptAiPlayerReq.dwAiPlayerObjID
            };
            Singleton<CUIManager>.GetInstance().OpenSmallMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trusteeship_Ask"), true, enUIEventID.Trusteeship_Accept, enUIEventID.Trusteeship_Cancel, par, 30, enUIEventID.Trusteeship_Cancel, Singleton<CTextManager>.GetInstance().GetText("FollowMe"), Singleton<CTextManager>.GetInstance().GetText("StayHome"), false);
        }
    }
}

