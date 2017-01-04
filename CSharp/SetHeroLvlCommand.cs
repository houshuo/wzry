using CSProtocol;
using System;

[ArgumentDescription(0, typeof(int), "ID", new object[] {  }), CheatCommand("英雄/属性/SetHeroLvl", "设置英雄等级", 10), ArgumentDescription(1, typeof(int), "等级", new object[] {  })]
internal class SetHeroLvlCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        int num2 = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        CheatCmdRef.stSetHeroLvl = new CSDT_CHEAT_HEROVAL();
        CheatCmdRef.stSetHeroLvl.dwHeroID = (uint) num;
        CheatCmdRef.stSetHeroLvl.iValue = num2;
        return CheatCommandBase.Done;
    }
}

