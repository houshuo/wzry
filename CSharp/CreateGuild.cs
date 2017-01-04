using CSProtocol;
using System;

[CheatCommand("通用/公会/CreateGuild", "一键创建战队", 0x4a)]
internal class CreateGuild : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        CheatCmdRef.stCreateGuild = new CSDT_CREATE_GUILD();
        CheatCmdRef.stCreateGuild.ullGuildID = 0L;
        return CheatCommandBase.Done;
    }
}

