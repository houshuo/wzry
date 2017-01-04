using CSProtocol;
using System;

[CheatCommand("英雄/属性修改/其它/ChangeHonorPoint", "加荣誉(局内)点", 0x48), ArgumentDescription(1, typeof(int), "数量", new object[] {  }), ArgumentDescription(0, typeof(EHonorType), "荣誉类型", new object[] {  })]
internal class ChangeHonorPointCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        EHonorType type = CheatCommandBase.SmartConvert<EHonorType>(InArguments[0]);
        int num = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        CheatCmdRef.stChgHonorInfo = new CSDT_CHEAT_CHG_HONORINFO();
        CheatCmdRef.stChgHonorInfo.iHonorID = (int) type;
        CheatCmdRef.stChgHonorInfo.iAddValue = num;
        return CheatCommandBase.Done;
    }
}

