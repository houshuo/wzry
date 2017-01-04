namespace Assets.Scripts.GameLogic
{
    using System;

    internal class UniformMovementState : MovingMovmentState
    {
        public static string StateName = typeof(UniformMovementState).Name;

        public UniformMovementState(PlayerMovement InParent) : base(InParent)
        {
        }

        protected override void CalcSpeed(int delta)
        {
            base.CurrentSpeed = base.Parent.maxSpeed;
        }
    }
}

