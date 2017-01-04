namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_GuideTip")]
    public class AreaEventTriggerGuideTip : AreaEventTrigger
    {
        [FriendlyName("新手提示ID")]
        public int GuideTipId;
        [FriendlyName("离开时新手提示ID")]
        public int GuideTipIdLeaving;

        protected override void BuildTriggerWrapper()
        {
            base.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerGuideTip);
            base.PresetActWrapper.EnterUniqueId = this.GuideTipId;
            base.PresetActWrapper.LeaveUniqueId = this.GuideTipIdLeaving;
            base.PresetActWrapper.Init();
        }
    }
}

