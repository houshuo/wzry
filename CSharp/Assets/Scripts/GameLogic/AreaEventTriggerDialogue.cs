namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_Dialogue")]
    public class AreaEventTriggerDialogue : AreaEventTrigger
    {
        [FriendlyName("剧情对话组ID")]
        public int DialogueGroupId;
        [FriendlyName("离开时剧情对话组ID")]
        public int DialogueGroupIdLeaving;

        protected override void BuildTriggerWrapper()
        {
            base.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerDialogue);
            base.PresetActWrapper.EnterUniqueId = this.DialogueGroupId;
            base.PresetActWrapper.LeaveUniqueId = this.DialogueGroupIdLeaving;
            base.PresetActWrapper.Init();
        }
    }
}

