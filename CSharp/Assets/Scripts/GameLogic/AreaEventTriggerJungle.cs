namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_Jungle")]
    public class AreaEventTriggerJungle : AreaEventTrigger
    {
        protected override void BuildTriggerWrapper()
        {
            base.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerJungle);
            base.PresetActWrapper.Init();
        }

        public override void UpdateLogic(int delta)
        {
            base.UpdateLogic(delta);
        }
    }
}

