using CSProtocol;
using System;

[CheatCommand("英雄/属性/AddItemEx", "添加物品旧版", 7), ArgumentDescription(0, typeof(int), "物品类型", new object[] {  }), ArgumentDescription(1, typeof(int), "ID", new object[] {  }), ArgumentDescription(2, typeof(int), "数量", new object[] {  })]
internal class AddItemExCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        int num2 = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        int num3 = CheatCommandBase.SmartConvert<int>(InArguments[2]);
        CheatCmdRef.stAddItem = new CSDT_CHEAT_ITEMINFO();
        CheatCmdRef.stAddItem.wItemType = (ushort) num;
        CheatCmdRef.stAddItem.dwItemID = (uint) num2;
        CheatCmdRef.stAddItem.wItemCnt = (ushort) num3;
        return CheatCommandBase.Done;
    }
}

