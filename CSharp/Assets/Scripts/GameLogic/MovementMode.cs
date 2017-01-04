namespace Assets.Scripts.GameLogic
{
    using System;

    public abstract class MovementMode : BaseState
    {
        protected PlayerMovement Movement;

        public MovementMode(PlayerMovement InMovement)
        {
            this.Movement = InMovement;
        }

        public virtual VInt3 GetTargetPosition()
        {
            return this.Movement.SelectedTargetLocation;
        }

        public virtual void Move(VInt3 delta)
        {
            this.Movement.actor.location += delta;
        }

        public virtual bool ShouldStop()
        {
            return false;
        }
    }
}

