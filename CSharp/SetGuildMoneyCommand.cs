using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "公会资金", new object[] {  }), CheatCommand("通用/公会/SetGuildMoney", "设置公会资金", 40)]
internal class SetGuildMoneyCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetGuildInfo = new CSDT_CHEAT_SET_GUILD_INFO();
        CheatCmdRef.stSetGuildInfo.iMoney = InValue;
        CheatCmdRef.stSetGuildInfo.iActive = -1;
    }
}

