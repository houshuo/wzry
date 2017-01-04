namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class EnableSoldierLineTick : TickEvent
    {
        public bool bEnable;
        public int SoldierWaveId;

        public override BaseEvent Clone()
        {
            EnableSoldierLineTick tick = ClassObjPool<EnableSoldierLineTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            EnableSoldierLineTick tick = src as EnableSoldierLineTick;
            this.bEnable = tick.bEnable;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (Singleton<BattleLogic>.GetInstance().mapLogic != null)
            {
                Singleton<BattleLogic>.GetInstance().mapLogic.EnableSoldierRegion(this.bEnable, this.SoldierWaveId);
            }
        }
    }
}

