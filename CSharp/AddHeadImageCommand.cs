using CSProtocol;
using System;

[CheatCommand("通用/头像框/AddHeadImage", "增加头像框", 0x41), ArgumentDescription(0, typeof(int), "ID", new object[] {  })]
internal class AddHeadImageCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        CheatCmdRef.stAddHeadImage = new CSDT_CHEAT_HEADIMAGE_ADD();
        CheatCmdRef.stAddHeadImage.dwHeadImgID = CheatCommandBase.SmartConvert<uint>(InArguments[0]);
        return CheatCommandBase.Done;
    }
}

