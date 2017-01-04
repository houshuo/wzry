namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class KillSoldierLineTick : TickEvent
    {
        public override BaseEvent Clone()
        {
            KillSoldierLineTick tick = ClassObjPool<KillSoldierLineTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            Singleton<GameObjMgr>.GetInstance().KillSoldiers();
        }
    }
}

