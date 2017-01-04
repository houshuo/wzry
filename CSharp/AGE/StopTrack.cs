namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("Utility")]
    public class StopTrack : TickEvent
    {
        public int trackId = -1;

        public override BaseEvent Clone()
        {
            StopTrack track = ClassObjPool<StopTrack>.Get();
            track.CopyData(this);
            return track;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            StopTrack track = src as StopTrack;
            this.trackId = track.trackId;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.trackId = -1;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            Track track = _action.GetTrack(this.trackId);
            if (track != null)
            {
                track.Stop(_action);
            }
        }
    }
}

