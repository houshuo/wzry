using CSProtocol;
using System;

[ArgumentDescription(0, typeof(int), "ID", new object[] {  }), CheatCommand("英雄/属性修改/经验等级/SetHeroStar", "设置英雄星级", 0x18), ArgumentDescription(1, typeof(int), "星级", new object[] {  })]
internal class SetHeroStarCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        int num2 = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        CheatCmdRef.stSetHeroStar = new CSDT_CHEAT_SETHEROSTAR();
        CheatCmdRef.stSetHeroStar.dwHeroID = (uint) num;
        CheatCmdRef.stSetHeroStar.dwStar = (uint) num2;
        return CheatCommandBase.Done;
    }
}

