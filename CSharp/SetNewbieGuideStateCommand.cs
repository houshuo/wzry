using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("关卡/SetNewbieGuideState", "完成所有新手引导", 0x1c)]
internal class SetNewbieGuideStateCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
        if ((masterRoleInfo == null) || !Singleton<LobbyLogic>.GetInstance().isLogin)
        {
            return "undone";
        }
        CheatCmdRef.stDyeNewbieBit = new CSDT_CHEAT_DYE_NEWBIE_BIT();
        CheatCmdRef.stDyeNewbieBit.bOpenOrClose = 1;
        CheatCmdRef.stDyeNewbieBit.bIsAll = 1;
        CheatCmdRef.stDyeNewbieBit.dwApntBit = 0;
        for (int i = 0; i < 0x6a; i++)
        {
            masterRoleInfo.SetGuidedStateSet(i, true);
        }
        MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteNewbieGuideAll(true);
        MonoSingleton<NewbieGuideManager>.GetInstance().ForceSetWeakGuideCompleteAll(false);
        masterRoleInfo.SyncNewbieAchieveToSvr(true);
        return CheatCommandBase.Done;
    }
}

