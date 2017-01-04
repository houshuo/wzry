namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [PassiveEvent(PassiveEventType.LifeRandomPassiveEvent)]
    public class PassiveLifeRandomEvent : PassiveRandomEvent
    {
        public override void UpdateLogic(int _delta)
        {
            if ((base.sourceActor != 0) && !this.sourceActor.handle.ActorControl.IsDeadState)
            {
                base.UpdateLogic(_delta);
            }
        }
    }
}

