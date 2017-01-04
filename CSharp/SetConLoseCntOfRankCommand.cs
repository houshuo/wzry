using CSProtocol;
using System;

[CheatCommand("英雄/英雄排位赛/SetConLoseCntOfRank", "排位赛连败场数", 0x15), ArgumentDescription(typeof(int), "数值", new object[] {  })]
internal class SetConLoseCntOfRankCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetConLoseCntOfRank = new CSDT_CHEAT_COMVAL();
        CheatCmdRef.stSetConLoseCntOfRank.iValue = InValue;
    }
}

