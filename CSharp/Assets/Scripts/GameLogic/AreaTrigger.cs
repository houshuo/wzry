namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_BuffSkill")]
    public class AreaTrigger : AreaEventTrigger
    {
        [FriendlyName("离开时拔除")]
        public bool bStopWhenLeaving = true;
        [FriendlyName("作用于进入者")]
        public bool bToActor = true;
        [FriendlyName("作用于肇事者")]
        public bool bToAtker;
        [FriendlyName("进入时挂的BUFF")]
        public int BuffID;
        [FriendlyName("离开时挂的BUFF")]
        public int LeaveBuffID;
        [FriendlyName("轮询探测时挂的BUFF")]
        public int UpdateBuffID;

        protected override void BuildTriggerWrapper()
        {
            base.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerBuff);
            base.PresetActWrapper.EnterUniqueId = this.BuffID;
            base.PresetActWrapper.UpdateUniqueId = this.UpdateBuffID;
            base.PresetActWrapper.LeaveUniqueId = this.LeaveBuffID;
            base.PresetActWrapper.bStopWhenLeaving = this.bStopWhenLeaving;
            base.PresetActWrapper.bSrc = this.bToActor;
            base.PresetActWrapper.bAtker = this.bToAtker;
            base.PresetActWrapper.Init();
        }
    }
}

