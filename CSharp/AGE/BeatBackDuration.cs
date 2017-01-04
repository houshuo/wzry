namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class BeatBackDuration : DurationCondition
    {
        public int accelerateSpeed = 0x3e8;
        private PoolObjHandle<ActorRoot> actor_;
        [ObjectTemplate(new System.Type[] {  })]
        public int attackerId;
        public VInt3 attackerPos = VInt3.zero;
        public int atteDistance;
        public BeatBackDirType dirType;
        private bool done_;
        public bool enableRotate = true;
        private Quaternion fromRot = Quaternion.identity;
        public int initSpeed = 0x3e8;
        private int lastTime_;
        private AccelerateMotionControler motionControler;
        private VInt3 moveDirection = VInt3.zero;
        public int rotationTime;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;
        private Quaternion toRot = Quaternion.identity;

        private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
        {
            if ((actor != null) && !this.done_)
            {
                VInt num2;
                int motionLerpDistance = this.motionControler.GetMotionLerpDistance((int) nDelta);
                VInt3 delta = (VInt3) ((this.moveDirection * motionLerpDistance) / 1000f);
                Vector3 position = actor.gameObject.transform.position;
                VInt3 num4 = PathfindingUtility.MoveLerp(actor, (VInt3) position, delta, out num2);
                if (actor.MovementComponent.isFlying)
                {
                    float y = position.y;
                    position += (Vector3) num4;
                    Vector3 vector2 = position;
                    vector2.y = y;
                    actor.gameObject.transform.position = vector2;
                }
                else
                {
                    Transform transform = actor.gameObject.transform;
                    transform.position += (Vector3) num4;
                }
            }
        }

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.done_;
        }

        public override BaseEvent Clone()
        {
            BeatBackDuration duration = ClassObjPool<BeatBackDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BeatBackDuration duration = src as BeatBackDuration;
            this.attackerId = duration.attackerId;
            this.targetId = duration.targetId;
            this.attackerPos = duration.attackerPos;
            this.initSpeed = duration.initSpeed;
            this.accelerateSpeed = duration.accelerateSpeed;
            this.enableRotate = duration.enableRotate;
            this.rotationTime = duration.rotationTime;
            this.dirType = duration.dirType;
            this.atteDistance = duration.atteDistance;
            this.done_ = duration.done_;
            this.moveDirection = duration.moveDirection;
            this.fromRot = duration.fromRot;
            this.toRot = duration.toRot;
            this.lastTime_ = duration.lastTime_;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = new PoolObjHandle<ActorRoot>(null);
            this.done_ = false;
            this.lastTime_ = 0;
            base.Enter(_action, _track);
            VInt3 attackerPos = this.attackerPos;
            if (this.attackerId != -1)
            {
                if (_action.GetGameObject(this.attackerId) == null)
                {
                    return;
                }
                actorHandle = _action.GetActorHandle(this.attackerId);
                if (actorHandle == 0)
                {
                    return;
                }
                attackerPos = actorHandle.handle.location;
            }
            this.actor_ = _action.GetActorHandle(this.targetId);
            if (this.actor_ != 0)
            {
                if (!this.actor_.handle.isMovable)
                {
                    this.actor_.Release();
                    this.done_ = true;
                }
                else
                {
                    if (this.dirType == BeatBackDirType.Position)
                    {
                        VInt3 num2 = this.actor_.handle.location - attackerPos;
                        num2.y = 0;
                        this.moveDirection = num2.NormalizeTo(0x3e8);
                    }
                    else if (this.dirType == BeatBackDirType.Directional)
                    {
                        if (actorHandle == 0)
                        {
                            this.done_ = true;
                            return;
                        }
                        this.moveDirection = actorHandle.handle.forward;
                    }
                    if (this.enableRotate)
                    {
                        this.fromRot = this.actor_.handle.rotation;
                        this.actor_.handle.MovementComponent.SetRotate(-this.moveDirection, true);
                        if (this.rotationTime > 0)
                        {
                            this.toRot = Quaternion.LookRotation((Vector3) this.actor_.handle.forward);
                        }
                        else
                        {
                            this.actor_.handle.rotation = Quaternion.LookRotation((Vector3) this.actor_.handle.forward);
                        }
                    }
                    int initSpeed = this.initSpeed;
                    this.motionControler = new AccelerateMotionControler();
                    if (this.atteDistance > 0)
                    {
                        VInt3 num4 = this.actor_.handle.location - attackerPos;
                        VInt num5 = num4.magnitude2D;
                        if (num5.i > this.atteDistance)
                        {
                            initSpeed = 0;
                        }
                        else
                        {
                            initSpeed = ((this.atteDistance - num5.i) * this.initSpeed) / this.atteDistance;
                        }
                    }
                    this.motionControler.InitMotionControler(initSpeed, this.accelerateSpeed);
                    this.actor_.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.actor_ != 0)
            {
                this.actor_.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
                this.done_ = true;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.attackerId = 0;
            this.targetId = 0;
            this.attackerPos = VInt3.zero;
            this.initSpeed = 0x3e8;
            this.accelerateSpeed = 0x3e8;
            this.enableRotate = true;
            this.rotationTime = 0;
            this.dirType = BeatBackDirType.Position;
            this.atteDistance = 0;
            this.done_ = false;
            this.moveDirection = VInt3.zero;
            this.fromRot = Quaternion.identity;
            this.toRot = Quaternion.identity;
            this.lastTime_ = 0;
            this.motionControler = null;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if (this.actor_ != 0)
            {
                bool flag = this.lastTime_ < this.rotationTime;
                int num = _localTime - this.lastTime_;
                this.lastTime_ = _localTime;
                if (flag && this.enableRotate)
                {
                    float t = Mathf.Min(1f, (float) (_localTime / this.rotationTime));
                    Quaternion quaternion = Quaternion.Slerp(this.fromRot, this.toRot, t);
                    this.actor_.handle.rotation = quaternion;
                }
                if (!this.done_)
                {
                    VInt num4;
                    int motionDeltaDistance = this.motionControler.GetMotionDeltaDistance(num);
                    VInt3 delta = (VInt3) ((this.moveDirection * motionDeltaDistance) / 1000f);
                    VInt3 num6 = PathfindingUtility.Move(this.actor_.handle, delta, out num4, null);
                    if (this.actor_.handle.MovementComponent.isFlying)
                    {
                        int y = this.actor_.handle.location.y;
                        ActorRoot handle = this.actor_.handle;
                        handle.location += num6;
                        VInt3 location = this.actor_.handle.location;
                        location.y = y;
                        this.actor_.handle.location = location;
                    }
                    else
                    {
                        ActorRoot local2 = this.actor_.handle;
                        local2.location += num6;
                    }
                    this.actor_.handle.groundY = num4;
                    if ((delta.x != num6.x) || (delta.z != num6.z))
                    {
                        this.done_ = true;
                    }
                    base.Process(_action, _track, _localTime);
                }
            }
        }
    }
}

