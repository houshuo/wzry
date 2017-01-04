using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "战队币", new object[] {  }), CheatCommand("通用/公会/SetGuildMemberCoin", "设置战队币", 0x3f)]
internal class SetGuildMemberCoin : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetGuildMemberCoin = new CSDT_CHEAT_COMVAL();
        CheatCmdRef.stSetGuildMemberCoin.iValue = InValue;
    }
}

