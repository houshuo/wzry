using CSProtocol;
using System;

[CheatCommand("英雄/英雄排位赛/SetRankTotalWinCnt", "设置排位赛总胜利场次", 0x36), ArgumentDescription(typeof(int), "数值", new object[] {  })]
internal class SetRankTotalWinCntCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetRankTotalWinCnt = new CSDT_CHEAT_COMVAL();
        CheatCmdRef.stSetRankTotalWinCnt.iValue = InValue;
    }
}

