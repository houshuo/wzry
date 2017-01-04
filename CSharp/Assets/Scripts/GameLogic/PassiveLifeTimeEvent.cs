namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [PassiveEvent(PassiveEventType.LifeTimePassiveEvent)]
    public class PassiveLifeTimeEvent : PassiveTimeEvent
    {
        public override void UpdateLogic(int _delta)
        {
            if ((base.sourceActor != 0) && !this.sourceActor.handle.ActorControl.IsDeadState)
            {
                base.UpdateLogic(_delta);
            }
            else
            {
                base.startTime = 0;
                base.Reset();
            }
        }
    }
}

