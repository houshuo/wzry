using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "ID", new object[] {  }), ArgumentDescription(0, typeof(EPropTypeType), "物品类型", new object[] {  }), ArgumentDescription(2, typeof(int), "数量", new object[] {  }), CheatCommand("英雄/属性/AddItem", "添加物品", 7)]
internal class AddItemCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        EPropTypeType type = CheatCommandBase.SmartConvert<EPropTypeType>(InArguments[0]);
        int num = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        int num2 = CheatCommandBase.SmartConvert<int>(InArguments[2]);
        CheatCmdRef.stAddItem = new CSDT_CHEAT_ITEMINFO();
        CheatCmdRef.stAddItem.wItemType = (ushort) type;
        CheatCmdRef.stAddItem.dwItemID = (uint) num;
        CheatCmdRef.stAddItem.wItemCnt = (ushort) num2;
        return CheatCommandBase.Done;
    }
}

