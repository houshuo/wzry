namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_Age")]
    public class AreaEventTriggerAge : AreaEventTrigger
    {
        [SerializeField]
        public AreaEventTrigger.STimingAction[] TimingActionsInter = new AreaEventTrigger.STimingAction[0];

        protected override void BuildTriggerWrapper()
        {
            base.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerAge);
            base.PresetActWrapper.TimingActionsInter = this.TimingActionsInter;
            base.PresetActWrapper.Init();
        }
    }
}

