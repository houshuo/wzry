namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SkillEnergyCostTriggerTick : TickEvent
    {
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SkillEnergyCostTriggerTick tick = ClassObjPool<SkillEnergyCostTriggerTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillEnergyCostTriggerTick tick = src as SkillEnergyCostTriggerTick;
            this.targetId = tick.targetId;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                SkillComponent skillControl = actorHandle.handle.SkillControl;
                if (skillControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    SkillSlot slot;
                    SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                    if (((refParamObject != null) && skillControl.TryGetSkillSlot(refParamObject.SlotType, out slot)) && (slot != null))
                    {
                        slot.CurSkillEnergyCostTick();
                    }
                }
            }
        }
    }
}

