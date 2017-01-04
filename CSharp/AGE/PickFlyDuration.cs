namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    internal class PickFlyDuration : DurationCondition
    {
        private PoolObjHandle<ActorRoot> actor_;
        private bool bMotionControl;
        private bool done_;
        public int gravity = -10;
        public int height;
        private int lastTime_;
        private AccelerateMotionControler motionControler;
        private PlayerMovement movement;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;
        private int totalTime;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.done_;
        }

        public override BaseEvent Clone()
        {
            PickFlyDuration duration = ClassObjPool<PickFlyDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PickFlyDuration duration = src as PickFlyDuration;
            this.targetId = duration.targetId;
            this.height = duration.height;
            this.gravity = duration.gravity;
            this.actor_ = duration.actor_;
            this.done_ = duration.done_;
            this.lastTime_ = duration.lastTime_;
            this.totalTime = duration.totalTime;
            this.bMotionControl = duration.bMotionControl;
            this.movement = duration.movement;
            this.motionControler = duration.motionControler;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.done_ = false;
            base.Enter(_action, _track);
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
                    this.actor_.handle.ActorControl.TerminateMove();
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                    this.lastTime_ = 0;
                    this.movement = this.actor_.handle.MovementComponent as PlayerMovement;
                    this.motionControler = new AccelerateMotionControler();
                    if (this.movement != null)
                    {
                        int motionControlerCount = this.movement.GravityMode.GetMotionControlerCount();
                        if (motionControlerCount < 3)
                        {
                            if (motionControlerCount == 0)
                            {
                                this.motionControler.InitMotionControler(base.length, 0, this.gravity);
                            }
                            else if (motionControlerCount == 1)
                            {
                                this.motionControler.InitMotionControler(base.length, 0, IntMath.Divide((int) (this.gravity * 6), 10));
                            }
                            else if (motionControlerCount == 2)
                            {
                                this.motionControler.InitMotionControler(base.length, 0, IntMath.Divide((int) (this.gravity * 4), 10));
                            }
                            this.movement.GravityMode.AddMotionControler(this.motionControler);
                            this.movement.isFlying = true;
                            this.bMotionControl = true;
                        }
                    }
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.actor_ != 0)
            {
                this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                this.done_ = true;
                if ((this.movement != null) && this.bMotionControl)
                {
                    this.movement.GravityMode.RemoveMotionControler(this.motionControler);
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.height = 0;
            this.gravity = -10;
            this.actor_.Release();
            this.done_ = false;
            this.lastTime_ = 0;
            this.totalTime = 0;
            this.bMotionControl = false;
            this.movement = null;
            this.motionControler = null;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }
    }
}

