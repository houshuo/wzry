namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_BlockSwitcher")]
    public class AreaEventTriggerBlockSwitcher : AreaEventTrigger
    {
        [FriendlyName("开启")]
        public bool bEnable;
        [FriendlyName("离开时拔除")]
        public bool bStopWhenLeaving;
        public GameObject[] DynamicBlockList = new GameObject[0];

        protected override void BuildTriggerWrapper()
        {
            base.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerDynamicBlock);
            base.PresetActWrapper.bEnable = this.bEnable;
            base.PresetActWrapper.bStopWhenLeaving = this.bStopWhenLeaving;
            base.PresetActWrapper.RefObjList = this.DynamicBlockList;
            base.PresetActWrapper.Init();
        }
    }
}

