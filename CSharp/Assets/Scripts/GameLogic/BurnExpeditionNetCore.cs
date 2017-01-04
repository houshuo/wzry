namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;

    [MessageHandlerClass]
    public class BurnExpeditionNetCore
    {
        public static void Clear_ResetBurning_Limit()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3f4);
            msg.stPkgData.stCheatCmd.iCheatCmd = 0x23;
            msg.stPkgData.stCheatCmd.stCheatCmdDetail = new CSDT_CHEATCMD_DETAIL();
            msg.stPkgData.stCheatCmd.stCheatCmdDetail.stClrBurningLimit = new CSDT_CHEAT_CLR_BURNING_LIMIT();
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void Clear_ResetBurning_Power()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3f4);
            msg.stPkgData.stCheatCmd.iCheatCmd = 0x27;
            msg.stPkgData.stCheatCmd.stCheatCmdDetail = new CSDT_CHEATCMD_DETAIL();
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        [MessageHandler(0xa8d)]
        public static void On_GET_BURNING_PROGRESS_RSP(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Burn_GET_BURNING_PROGRESS_RSP", msg);
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        [MessageHandler(0xa8f)]
        public static void On_GET_BURNING_REWARD_RSP(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Burn_GET_BURNING_REWARD_RSP", msg);
        }

        [MessageHandler(0xa91)]
        public static void On_RESET_BURNING_PROGRESS_RSP(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Burn_RESET_BURNING_PROGRESS_RSP", msg);
        }

        public static void Send_Get_BURNING_PROGRESS_REQ()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa8c);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_GET_BURNING_REWARD_REQ(byte levelNo, int levelID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa8e);
            msg.stPkgData.stGetBurningRewardReq.bLevelNo = levelNo;
            msg.stPkgData.stGetBurningRewardReq.iLevelID = levelID;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_RESET_BURNING_PROGRESS_REQ(byte value)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa90);
            msg.stPkgData.stResetBurningProgressReq.bDifficultType = value;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }
    }
}

