namespace AGE
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class NewbieShowDragonBuffTick : TickEvent
    {
        public override BaseEvent Clone()
        {
            NewbieShowDragonBuffTick tick = ClassObjPool<NewbieShowDragonBuffTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                Singleton<CBattleSystem>.GetInstance().FightForm.SetDragonUINum(COM_PLAYERCAMP.COM_PLAYERCAMP_1, 3);
            }
        }
    }
}

