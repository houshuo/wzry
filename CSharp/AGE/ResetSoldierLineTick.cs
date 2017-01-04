namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class ResetSoldierLineTick : TickEvent
    {
        public override BaseEvent Clone()
        {
            return ClassObjPool<ResetSoldierLineTick>.Get();
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (Singleton<BattleLogic>.GetInstance().mapLogic != null)
            {
                Singleton<BattleLogic>.GetInstance().mapLogic.ResetSoldierRegion();
            }
        }
    }
}

