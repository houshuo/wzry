using CSProtocol;
using System;

[CheatCommand("工具/游戏/任务/Reset_Task", "重置任务", 0x39), ArgumentDescription(typeof(int), "任务ID", new object[] {  })]
internal class ResetTaskCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stTaskDone = new CSDT_CHEAT_TASKDONE();
        CheatCmdRef.stTaskDone.dwTaskID = (uint) InValue;
        CheatCmdRef.stTaskDone.bReset = 1;
    }
}

