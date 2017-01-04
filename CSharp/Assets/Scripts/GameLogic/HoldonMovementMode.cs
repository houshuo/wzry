namespace Assets.Scripts.GameLogic
{
    using System;

    internal class HoldonMovementMode : MovementMode
    {
        public HoldonMovementMode(PlayerMovement InMovement) : base(InMovement)
        {
        }

        public override void OnStateEnter()
        {
            base.Movement.isRotateImmediately = false;
        }

        public override bool ShouldStop()
        {
            return true;
        }
    }
}

