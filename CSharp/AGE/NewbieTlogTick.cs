namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using System;

    [EventCategory("MMGame/Newbie")]
    public class NewbieTlogTick : TickEvent
    {
        public bool bIsLastStep;
        public int iStepId;

        public override BaseEvent Clone()
        {
            NewbieTlogTick tick = ClassObjPool<NewbieTlogTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            NewbieTlogTick tick = src as NewbieTlogTick;
            this.iStepId = tick.iStepId;
            this.bIsLastStep = tick.bIsLastStep;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.ReqSetInBattleNewbieBit((uint) this.iStepId, this.bIsLastStep);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

