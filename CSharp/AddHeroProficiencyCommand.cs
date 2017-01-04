using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "熟练度经验值", new object[] {  }), CheatCommand("英雄/属性/AddHeroProficiency", "增加英雄熟练度经验值", 0x31), ArgumentDescription(0, typeof(int), "ID", new object[] {  })]
internal class AddHeroProficiencyCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        int num2 = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        CheatCmdRef.stAddHeroProficiency = new CSDT_CHEAT_HEROVAL();
        CheatCmdRef.stAddHeroProficiency.dwHeroID = (uint) num;
        CheatCmdRef.stAddHeroProficiency.iValue = num2;
        return CheatCommandBase.Done;
    }
}

