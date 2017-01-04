namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public abstract class MovementState : BaseState
    {
        protected PlayerMovement Parent;
        protected VInt3 Velocity;

        public MovementState(PlayerMovement InParent)
        {
            this.Parent = InParent;
        }

        public virtual void ChangeDirection()
        {
        }

        public virtual void ChangeTarget()
        {
        }

        public virtual int GetCurrentSpeed()
        {
            return 0;
        }

        public virtual VInt3 GetVelocity()
        {
            return this.Velocity;
        }

        public virtual bool IsMoving()
        {
            return false;
        }

        public virtual bool IsOnTarget()
        {
            return this.Parent.isStandOnTarget;
        }

        protected virtual void PointingAtTarget(int delta)
        {
            this.UpdateRotateDir(this.Parent.adjustedDirection, delta);
        }

        public virtual void UpdateLogic(int nDelta)
        {
            if (!this.Parent.isRotatingLock)
            {
                this.PointingAtTarget(nDelta);
            }
        }

        protected void UpdateRotateDir(VInt3 targetDir, int dt)
        {
            ActorRoot actor = this.Parent.actor;
            if (targetDir != VInt3.zero)
            {
                actor.ObjLinker.SetForward(targetDir, this.Parent.actor.ActorControl.curMoveSeq);
                if (!actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate))
                {
                    actor.forward = targetDir;
                    if (actor.InCamera)
                    {
                        Vector3 forward = this.Parent.gameObject.transform.forward;
                        Vector3 vector2 = (Vector3) targetDir;
                        vector2.y = 0f;
                        forward.y = 0f;
                        VFactor factor = VInt3.AngleInt(targetDir, VInt3.forward);
                        int num = (targetDir.x * VInt3.forward.z) - (VInt3.forward.x * targetDir.z);
                        if (num < 0)
                        {
                            factor = VFactor.twoPi - factor;
                        }
                        Quaternion to = Quaternion.AngleAxis(factor.single * 57.29578f, Vector3.up);
                        actor.rotation = Quaternion.RotateTowards(actor.rotation, to, (this.Parent.RotateSpeed * dt) * 0.001f);
                    }
                }
            }
        }
    }
}

