namespace Assets.Scripts.GameLogic
{
    using System;

    internal class IdleMovmentState : MovementState
    {
        public IdleMovmentState(PlayerMovement InParent) : base(InParent)
        {
        }

        public override void ChangeDirection()
        {
            base.Parent.GotoState("AccelerateMovementState");
        }

        public override void ChangeTarget()
        {
            base.Parent.GotoState("AccelerateMovementState");
        }

        public override int GetCurrentSpeed()
        {
            return 0;
        }

        public override bool IsMoving()
        {
            return false;
        }

        public override void OnStateEnter()
        {
        }
    }
}

