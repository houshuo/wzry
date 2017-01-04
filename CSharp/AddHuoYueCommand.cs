using CSProtocol;
using System;

[CheatCommand("工具/游戏/活跃度/Add_Huoyue", "加活跃值", 0x39), ArgumentDescription(typeof(int), "数量", new object[] {  })]
internal class AddHuoYueCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stHuoYueDuOpt = new CSDT_CHEAT_HUOYUEDU();
        CheatCmdRef.stHuoYueDuOpt.iValue = InValue;
        CheatCmdRef.stHuoYueDuOpt.bOpt = 1;
    }
}

