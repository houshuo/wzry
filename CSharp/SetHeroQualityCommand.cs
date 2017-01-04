using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "品质", new object[] {  }), ArgumentDescription(2, typeof(int), "品阶", new object[] {  }), CheatCommand("英雄/属性修改/数值/SetHeroQyality", "设置品质", 0x19), ArgumentDescription(0, typeof(int), "ID", new object[] {  })]
internal class SetHeroQualityCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        int num2 = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        int num3 = CheatCommandBase.SmartConvert<int>(InArguments[2]);
        CheatCmdRef.stSetHeroQuality = new CSDT_CHEAT_SETHEROQUALITY();
        CheatCmdRef.stSetHeroQuality.dwHeroID = (uint) num;
        CheatCmdRef.stSetHeroQuality.stQuality.wQuality = (ushort) num2;
        CheatCmdRef.stSetHeroQuality.stQuality.wSubQuality = (ushort) num3;
        return CheatCommandBase.Done;
    }
}

