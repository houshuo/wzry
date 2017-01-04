using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommandEntry("工具"), MessageHandlerClass]
internal class CheatCommandCommonEntryCommon
{
    [CheatCommandEntryMethod("一键毕业", true, false)]
    public static string FinishSchool()
    {
        string[] inArgs = new string[] { string.Empty };
        Singleton<CheatCommandsRepository>.instance.ExecuteCommand("FinishLevel", inArgs);
        string[] textArray2 = new string[] { string.Empty };
        Singleton<CheatCommandsRepository>.instance.ExecuteCommand("UnlockAllLevel", textArray2);
        string[] textArray3 = new string[] { string.Empty };
        Singleton<CheatCommandsRepository>.instance.ExecuteCommand("ClearStoreLimit", textArray3);
        string[] textArray4 = new string[] { "0" };
        Singleton<CheatCommandsRepository>.instance.ExecuteCommand("AddHero", textArray4);
        string[] textArray5 = new string[] { "0" };
        Singleton<CheatCommandsRepository>.instance.ExecuteCommand("UnlockPvPHero", textArray5);
        string[] textArray6 = new string[] { string.Empty };
        Singleton<CheatCommandsRepository>.instance.ExecuteCommand("SetNewbieGuideState", textArray6);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("服务器/获取服务器时间", true, false)]
    public static string GetServerTime()
    {
        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xfa1);
        Singleton<NetworkModule>.instance.SendLobbyMsg(ref msg, false);
        return "Wait server rsp.";
    }

    [MessageHandler(0xfa2)]
    public static void OnServerTimeRSP(CSPkg msg)
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
        if (masterRoleInfo != null)
        {
            CRoleInfo.SetServerTime((int) msg.stPkgData.stServerTimeRsp.dwUTCSec);
            masterRoleInfo.SetGlobalRefreshTimer(msg.stPkgData.stServerTimeRsp.dwUTCSec, true);
        }
        object[] args = new object[] { msg.stPkgData.stServerTimeRsp.iYear, msg.stPkgData.stServerTimeRsp.iMonth, msg.stPkgData.stServerTimeRsp.iDay, msg.stPkgData.stServerTimeRsp.iHour, msg.stPkgData.stServerTimeRsp.iMin, msg.stPkgData.stServerTimeRsp.iSec };
        MonoSingleton<ConsoleWindow>.instance.AddMessage(string.Format("{0}.{1}.{2} {3}:{4}:{5}", args));
        Singleton<EventRouter>.instance.BroadCastEvent(EventID.EDITOR_REFRESH_GM_PANEL);
    }
}

