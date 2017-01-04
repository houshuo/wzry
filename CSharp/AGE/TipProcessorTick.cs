namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Drama")]
    public class TipProcessorTick : TickEvent
    {
        public bool bPlayTip = true;
        public int GuideTipId;

        public override BaseEvent Clone()
        {
            TipProcessorTick tick = ClassObjPool<TipProcessorTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TipProcessorTick tick = src as TipProcessorTick;
            this.bPlayTip = tick.bPlayTip;
            this.GuideTipId = tick.GuideTipId;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            int guideTipId = this.GuideTipId;
            if (guideTipId <= 0)
            {
                _action.refParams.GetRefParam("GuideTipIdRaw", ref guideTipId);
            }
            if (guideTipId > 0)
            {
                if (this.bPlayTip)
                {
                    Singleton<TipProcessor>.GetInstance().PlayDrama(guideTipId, null, null);
                }
                else
                {
                    Singleton<TipProcessor>.GetInstance().EndDrama(guideTipId);
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

