using CSProtocol;
using System;

[CheatCommand("英雄/属性修改/经验等级/SetAcntLvl", "设置等级", 3), ArgumentDescription(typeof(int), "等级", new object[] {  })]
internal class SetAcntLvlCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetAcntLvl = new CSDT_CHEAT_COMVAL();
        CheatCmdRef.stSetAcntLvl.iValue = InValue;
    }
}

