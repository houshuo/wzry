using CSProtocol;
using System;

[CheatCommand("英雄/属性修改/钱币/AddArenaCoin", "加竞技币", 0x2a), ArgumentDescription(typeof(int), "数量", new object[] {  })]
internal class AddArenaCoinCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stAddArenaCoin = new CSDT_CHEAT_COMVAL();
        CheatCmdRef.stAddArenaCoin.iValue = InValue;
    }
}

