namespace Assets.Scripts.GameLogic
{
    using System;

    internal abstract class MovingMovmentState : MovementState
    {
        private CrypticInt32 _CurrentSpeed;

        public MovingMovmentState(PlayerMovement InParent) : base(InParent)
        {
            this._CurrentSpeed = 0;
        }

        protected abstract void CalcSpeed(int delta);
        public override int GetCurrentSpeed()
        {
            return (int) this._CurrentSpeed;
        }

        public override bool IsMoving()
        {
            return true;
        }

        protected virtual void MoveToTarget(int delta)
        {
            this.CalcSpeed(delta);
            MPathfinding pathfinding = base.Parent.Pathfinding;
            if ((pathfinding != null) && pathfinding.enabled)
            {
                VInt3 num;
                pathfinding.speed = this.CurrentSpeed;
                pathfinding.Move(out num, delta);
                base.UpdateRotateDir(num, delta);
            }
            else
            {
                VInt3 location = base.Parent.actor.location;
                VInt3 num3 = VInt3.MoveTowards(location, base.Parent.targetLocation, (this.CurrentSpeed * delta) / 0x3e8);
                base.Parent.movingMode.Move(num3 - location);
                if (base.Parent.isRotateImmediately)
                {
                    this.PointingAtTarget(delta);
                }
            }
            base.Velocity = (VInt3) ((base.Parent.actor.forward * this.CurrentSpeed) / 1000f);
            if (this.IsOnTarget())
            {
                base.Parent.GotoState("IdleMovmentState");
            }
            base.Parent.nLerpStep = 2;
        }

        public override void OnStateLeave()
        {
            this._CurrentSpeed = 0;
        }

        public override void UpdateLogic(int nDelta)
        {
            if (base.Parent.isExcuteMoving)
            {
                this.MoveToTarget(nDelta);
            }
            else
            {
                base.UpdateLogic(nDelta);
            }
        }

        public int CurrentSpeed
        {
            get
            {
                return (int) this._CurrentSpeed;
            }
            set
            {
                this._CurrentSpeed = value;
            }
        }
    }
}

