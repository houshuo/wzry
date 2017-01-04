using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("关卡/SetNewbieGuideStateToOldPlayer", "成为老玩家", 0x1c)]
internal class SetNewbieGuideStateToOldPlayer : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
        if ((masterRoleInfo != null) && Singleton<LobbyLogic>.GetInstance().isLogin)
        {
            CheatCmdRef.stDyeNewbieBit = new CSDT_CHEAT_DYE_NEWBIE_BIT();
            CheatCmdRef.stDyeNewbieBit.bOpenOrClose = 1;
            CheatCmdRef.stDyeNewbieBit.bIsAll = 0;
            CheatCmdRef.stDyeNewbieBit.dwApntBit = 0;
            MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteNewbieGuideAll(true);
            MonoSingleton<NewbieGuideManager>.GetInstance().ForceSetWeakGuideCompleteAll(false);
            masterRoleInfo.SyncNewbieAchieveToSvr(true);
            return CheatCommandBase.Done;
        }
        return "undone";
    }
}

