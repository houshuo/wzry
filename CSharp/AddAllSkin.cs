using CSProtocol;
using System;

[CheatCommand("英雄/解锁/AddAllSkin", "获取已拥有英雄的所有皮肤", 0x38)]
internal class AddAllSkin : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        CheatCmdRef.stAddAllSkin = new CSDT_CHEAT_COMVAL();
        return CheatCommandBase.Done;
    }
}

