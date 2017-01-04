namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [MessageHandlerClass]
    internal class CPvPHeroShop : Singleton<CPvPHeroShop>
    {
        private static int ReGetFreeHeroTimer;

        public void GetFreeHero(int seq = 0)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x716);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        [MessageHandler(0x717)]
        public static void OnGetFreeHero(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (ReGetFreeHeroTimer != 0)
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimer(ReGetFreeHeroTimer);
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.SetFreeHeroInfo(msg.stPkgData.stFreeHeroRsp.stFreeHero);
                masterRoleInfo.SetFreeHeroSymbol(msg.stPkgData.stFreeHeroRsp.stFreeHeroSymbol);
                if (CRoleInfo.GetCurrentUTCTime() >= msg.stPkgData.stFreeHeroRsp.stFreeHero.dwDeadline)
                {
                    ReGetFreeHeroTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0xdbba0, 1, new CTimer.OnTimeUpHandler(Singleton<CPvPHeroShop>.GetInstance().GetFreeHero));
                }
            }
            else
            {
                DebugHelper.Assert(false, "Master RoleInfo is NULL!!!");
            }
        }

        public override void UnInit()
        {
            if (ReGetFreeHeroTimer != 0)
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimer(ReGetFreeHeroTimer);
            }
            base.UnInit();
        }
    }
}

