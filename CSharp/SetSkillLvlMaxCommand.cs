using CSProtocol;
using System;

[ArgumentDescription(0, typeof(int), "HeroID", new object[] {  }), CheatCommand("英雄/属性/SetSkillLvlMax", "升满技能等级", 0x2b)]
internal class SetSkillLvlMaxCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        CheatCmdRef.stSetSkillLvlMax = new CSDT_CHEAT_SET_SKILLLVL_MAX();
        CheatCmdRef.stSetSkillLvlMax.dwHeroID = (uint) num;
        return CheatCommandBase.Done;
    }
}

