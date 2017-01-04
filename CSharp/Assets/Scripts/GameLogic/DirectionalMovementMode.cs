namespace Assets.Scripts.GameLogic
{
    using System;

    internal class DirectionalMovementMode : MovementMode
    {
        public static string StateName = typeof(DirectionalMovementMode).Name;

        public DirectionalMovementMode(PlayerMovement InMovement) : base(InMovement)
        {
        }

        public override VInt3 GetTargetPosition()
        {
            return (base.Movement.actor.location + base.Movement.direction);
        }

        public override void Move(VInt3 delta)
        {
            VInt num;
            ActorRoot actor = base.Movement.actor;
            actor.location += PathfindingUtility.Move(actor, delta, out num, out actor.hasReachedNavEdge, base.Movement.MoveDirState);
            actor.groundY = num;
        }

        public override void OnStateEnter()
        {
            base.Movement.MoveDirState.Reset();
        }

        public override void OnStateLeave()
        {
        }

        public override bool ShouldStop()
        {
            return base.Movement.isStopMoving;
        }
    }
}

