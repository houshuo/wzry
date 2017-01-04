namespace Assets.Scripts.GameLogic
{
    using System;

    internal class AccelerateMovementState : VariableMovmentState
    {
        public AccelerateMovementState(PlayerMovement InParent) : base(InParent)
        {
        }

        protected override void CalcSpeed(int delta)
        {
            base.CurrentSpeed += (base.Parent.Acceleration * delta) / 0x3e8;
            if (base.CurrentSpeed >= base.Parent.maxSpeed)
            {
                base.CurrentSpeed = base.Parent.maxSpeed;
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            base.UpdateLogic(nDelta);
            if ((base.CurrentSpeed >= base.Parent.maxSpeed) && !this.IsOnTarget())
            {
                base.Parent.GotoState(UniformMovementState.StateName);
            }
        }
    }
}

