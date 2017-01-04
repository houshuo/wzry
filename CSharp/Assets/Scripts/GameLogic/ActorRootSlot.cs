namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class ActorRootSlot
    {
        private PoolObjHandle<ActorRoot> childActorRoot;
        private VInt distance;
        private VInt3 translation;

        public ActorRootSlot(PoolObjHandle<ActorRoot> _child)
        {
            this.translation = VInt3.zero;
            this.distance = 0;
            this.childActorRoot = _child;
        }

        public ActorRootSlot(PoolObjHandle<ActorRoot> _child, VInt3 _trans)
        {
            this.translation = VInt3.zero;
            this.distance = 0;
            this.translation = _trans;
            this.distance = this.translation.magnitude;
            this.childActorRoot = _child;
        }

        public void Update(ActorRoot _parent)
        {
            if (this.childActorRoot != 0)
            {
                VInt3 num = _parent.location + this.translation;
                if ((this.translation.x != 0) || (this.translation.z != 0))
                {
                    VInt3 forward = VInt3.forward;
                    VFactor radians = VInt3.AngleInt(_parent.forward, forward);
                    int num4 = (_parent.forward.x * forward.z) - (forward.x * _parent.forward.z);
                    if (num4 < 0)
                    {
                        radians = VFactor.twoPi - radians;
                    }
                    VInt3 num2 = this.translation.RotateY(ref radians);
                    num = _parent.location + num2.NormalizeTo(this.distance.i);
                    num.y += this.translation.y;
                }
                this.childActorRoot.handle.location = num;
                this.childActorRoot.handle.forward = _parent.forward;
            }
        }
    }
}

