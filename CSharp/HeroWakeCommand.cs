using CSProtocol;
using System;

[CheatCommand("英雄/解锁/HeroWake", "一键英雄觉醒", 0x33), ArgumentDescription(typeof(int), "英雄id", new object[] {  })]
internal class HeroWakeCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stHeroWake = new CSDT_CHEAT_HERO();
        CheatCmdRef.stHeroWake.dwHeroID = (uint) InValue;
    }
}

