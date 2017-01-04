using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("英雄/属性修改/钱币/GiveCoupons", "赠送点券", 0x2e), ArgumentDescription(typeof(int), "数量", new object[] {  })]
internal class GiveDiamondCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
        if ((masterRoleInfo != null) && (masterRoleInfo.UInt32ChgAdjust((uint) masterRoleInfo.DianQuan, InValue) > 0x7fffffffL))
        {
            DebugHelper.Assert(false, "超过点券最大值Int32.MaxValue！");
        }
        else
        {
            CheatCmdRef.stGiveCoupons = new CSDT_CHEAT_COMVAL();
            CheatCmdRef.stGiveCoupons.iValue = InValue;
        }
    }
}

