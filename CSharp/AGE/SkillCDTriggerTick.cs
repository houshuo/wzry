namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SkillCDTriggerTick : TickEvent
    {
        private SkillComponent skillControl;
        public SkillSlotType slotType;
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;
        public bool useSlotType;

        public override BaseEvent Clone()
        {
            SkillCDTriggerTick tick = ClassObjPool<SkillCDTriggerTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillCDTriggerTick tick = src as SkillCDTriggerTick;
            this.targetId = tick.targetId;
            this.useSlotType = tick.useSlotType;
            this.slotType = tick.slotType;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            this.targetActor = _action.GetActorHandle(this.targetId);
            if (this.targetActor == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                this.skillControl = this.targetActor.handle.SkillControl;
                if (this.skillControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else if (!this.useSlotType)
                {
                    this.StartSkillContextCD(_action);
                }
                else
                {
                    this.StartSkillSlotCD();
                }
            }
        }

        private void StartSkillContextCD(AGE.Action _action)
        {
            SkillSlot slot;
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            if (((refParamObject != null) && this.skillControl.TryGetSkillSlot(refParamObject.SlotType, out slot)) && (slot != null))
            {
                slot.StartSkillCD();
            }
        }

        private void StartSkillSlotCD()
        {
            SkillSlot slot;
            if (this.skillControl.TryGetSkillSlot(this.slotType, out slot) && (slot != null))
            {
                slot.StartSkillCD();
            }
        }
    }
}

