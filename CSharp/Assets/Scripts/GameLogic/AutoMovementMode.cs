namespace Assets.Scripts.GameLogic
{
    using System;

    internal class AutoMovementMode : MovementMode
    {
        public AutoMovementMode(PlayerMovement InMovement) : base(InMovement)
        {
        }

        public override bool ShouldStop()
        {
            VInt3 num = base.Movement.actor.location - base.Movement.SelectedTargetLocation;
            return ((num == VInt3.zero) || (((base.Movement.Pathfinding != null) && base.Movement.Pathfinding.targetReached) && (base.Movement.Pathfinding.targetSearchPos == base.Movement.SelectedTargetLocation)));
        }
    }
}

