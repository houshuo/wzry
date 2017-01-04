using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "公会活跃", new object[] {  }), CheatCommand("通用/公会/SetGuildActive", "设置公会活跃", 40)]
internal class SetGuildActiveCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetGuildInfo = new CSDT_CHEAT_SET_GUILD_INFO();
        CheatCmdRef.stSetGuildInfo.iActive = InValue;
        CheatCmdRef.stSetGuildInfo.iMoney = -1;
    }
}

