namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Drama")]
    public class TipProcessorDura : DurationEvent
    {
        public int GuideTipId;

        public override BaseEvent Clone()
        {
            TipProcessorDura dura = ClassObjPool<TipProcessorDura>.Get();
            dura.CopyData(this);
            return dura;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TipProcessorDura dura = src as TipProcessorDura;
            this.GuideTipId = dura.GuideTipId;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            int guideTipId = this.GuideTipId;
            if (guideTipId <= 0)
            {
                _action.refParams.GetRefParam("GuideTipIdRaw", ref guideTipId);
            }
            if (guideTipId > 0)
            {
                Singleton<TipProcessor>.GetInstance().PlayDrama(guideTipId, null, null);
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            int guideTipId = this.GuideTipId;
            if (guideTipId <= 0)
            {
                _action.refParams.GetRefParam("GuideTipIdRaw", ref guideTipId);
            }
            if (guideTipId > 0)
            {
                Singleton<TipProcessor>.GetInstance().EndDrama(guideTipId);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

