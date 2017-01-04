using CSProtocol;
using System;

[CheatCommand("英雄/英雄排位赛/SetScoreOfRank", "排位赛段位积分（范围0-100）", 0x13), ArgumentDescription(typeof(int), "积分", new object[] {  })]
internal class SetScoreOfRankCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetScoreOfRank = new CSDT_CHEAT_COMVAL();
        CheatCmdRef.stSetScoreOfRank.iValue = InValue;
    }
}

