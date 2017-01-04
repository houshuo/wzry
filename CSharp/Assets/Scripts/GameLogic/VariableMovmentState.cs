namespace Assets.Scripts.GameLogic
{
    using System;

    internal class VariableMovmentState : MovingMovmentState
    {
        public VariableMovmentState(PlayerMovement InParent) : base(InParent)
        {
        }

        protected override void CalcSpeed(int delta)
        {
        }
    }
}

