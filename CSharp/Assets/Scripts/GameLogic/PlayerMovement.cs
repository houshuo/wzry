namespace Assets.Scripts.GameLogic
{
    using Pathfinding;
    using Pathfinding.RVO;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class PlayerMovement : Movement
    {
        private CrypticInt32 _MaxSpeed = 0x1770;
        public int Acceleration = 0xf423f;
        private bool bExcecuteMoving;
        private bool bFlying;
        private bool bLerpFlying;
        private bool bRotateImmediately;
        private bool bRotatingLock;
        private bool bStopMoving = true;
        public int DecelerateDistance = 0x3e8;
        private VInt3 Direction;
        public GravityMovement GravityMode;
        public int IgnoreDistance = 1;
        public uint m_uiMoveIntervalMax;
        public uint m_uiNonMoveTotalTime;
        public ulong m_ulLastMoveEndTime;
        public int MinDecelerateSpeed;
        public MoveDirectionState MoveDirState = new MoveDirectionState();
        private StateMachine MovingMode;
        private StateMachine MovingState;
        public int nLerpStep;
        private MPathfinding pathfinding;
        public int RotateSpeed = 720;
        private VInt3 TargetLocation;
        private uint uCmdID;

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            this._MaxSpeed = 0x1770;
            this.MovingState = new StateMachine();
            this.MovingState.RegisterState<IdleMovmentState>(new IdleMovmentState(this), "IdleMovmentState");
            this.MovingState.RegisterState<AccelerateMovementState>(new AccelerateMovementState(this), "AccelerateMovementState");
            this.MovingState.RegisterState<UniformMovementState>(new UniformMovementState(this), "UniformMovementState");
            this.MovingState.RegisterState<DecelerateMovementState>(new DecelerateMovementState(this), "DecelerateMovementState");
            this.MovingState.ChangeState("IdleMovmentState");
            this.MovingMode = new StateMachine();
            this.MovingMode.RegisterState<AutoMovementMode>(new AutoMovementMode(this), "AutoMovementMode");
            this.MovingMode.RegisterState<DirectionalMovementMode>(new DirectionalMovementMode(this), "DirectionalMovementMode");
            this.MovingMode.RegisterState<HoldonMovementMode>(new HoldonMovementMode(this), "HoldonMovementMode");
            this.MovingMode.ChangeState("HoldonMovementMode");
            this.GravityMode = new GravityMovement(this);
            this.CreateNavSearchAgent();
        }

        private void CreateNavSearchAgent()
        {
            if (base.actor.isMovable)
            {
                Seeker component = base.gameObject.GetComponent<Seeker>();
                if (component == null)
                {
                    component = base.gameObject.AddComponent<Seeker>();
                }
                FunnelModifier modifier = base.gameObject.GetComponent<FunnelModifier>();
                if (modifier == null)
                {
                    modifier = base.gameObject.AddComponent<FunnelModifier>();
                }
                component.startEndModifier.Priority = 3;
                modifier.Priority = 2;
                if ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                {
                    RVOController controller = base.gameObject.GetComponent<RVOController>();
                    if (controller == null)
                    {
                        controller = base.gameObject.AddComponent<RVOController>();
                    }
                    controller.maxSpeed = this.maxSpeed;
                    controller.enabled = true;
                    controller.EnsureActorAndSimulator();
                }
            }
        }

        public override void Deactive()
        {
            this.ResetVariables();
            this.MovingState.ChangeState("IdleMovmentState");
            this.MovingMode.ChangeState("HoldonMovementMode");
            base.Deactive();
        }

        public override void ExcuteMove()
        {
            this.bExcecuteMoving = true;
            this.bRotatingLock = false;
            this.bStopMoving = false;
            uint num2 = (uint) (Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_ulLastMoveEndTime);
            this.m_uiMoveIntervalMax = (this.m_uiMoveIntervalMax <= num2) ? num2 : this.m_uiMoveIntervalMax;
            this.m_uiNonMoveTotalTime += num2;
        }

        public override void Fight()
        {
            base.Fight();
            this.m_ulLastMoveEndTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
        }

        public override float GetDistance(uint nDelta)
        {
            return (((this.maxSpeed * nDelta) / ((ulong) 0x3e8L)) * 0.001f);
        }

        public void GotoState(string InStateName)
        {
            this.MovingState.ChangeState(InStateName);
        }

        public override void GravityModeLerp(uint nDelta, bool bReset)
        {
            if (this.GravityMode != null)
            {
                this.GravityMode.GravityMoveLerp((int) nDelta, bReset);
            }
        }

        public override void Init()
        {
            base.Init();
            this.GravityMode.Init();
            if (AstarPath.active != null)
            {
                this.pathfinding = new MPathfinding();
                if (!this.pathfinding.Init(base.actorPtr))
                {
                    this.pathfinding = null;
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.ResetVariables();
            this.pathfinding = null;
            this.MovingState = null;
            this.MovingMode = null;
            this.GravityMode = null;
        }

        public override void Reactive()
        {
            base.Reactive();
            if (this.GravityMode != null)
            {
                this.GravityMode.Reset();
            }
            if (this.pathfinding != null)
            {
                this.pathfinding.Reset();
                if (this.pathfinding.rvo != null)
                {
                    this.pathfinding.rvo.maxSpeed = this.maxSpeed;
                }
            }
        }

        private void ResetVariables()
        {
            this.Acceleration = 0xf423f;
            this._MaxSpeed = 0x1770;
            this.RotateSpeed = 720;
            this.DecelerateDistance = 0x3e8;
            this.IgnoreDistance = 1;
            this.MinDecelerateSpeed = 0;
            this.bStopMoving = true;
            this.bExcecuteMoving = false;
            this.bRotatingLock = false;
            this.bRotateImmediately = false;
            this.bFlying = false;
            this.bLerpFlying = false;
            this.nLerpStep = 0;
            this.TargetLocation = VInt3.zero;
            this.Direction = VInt3.zero;
            this.MoveDirState.Reset();
            this.uCmdID = 0;
            this.m_uiNonMoveTotalTime = 0;
            this.m_ulLastMoveEndTime = 0L;
            this.m_uiMoveIntervalMax = 0;
        }

        public override void SetMoveParam(VInt3 InVector, bool bDirection, bool bInRotateImmediately, uint cmdId = 0)
        {
            this.uCommandId = cmdId;
            this.bRotateImmediately = bInRotateImmediately;
            if (bDirection)
            {
                this.Direction = InVector;
                this.Direction.NormalizeTo(0x3e8);
                this.MoveDirState.SetNewDirection(ref this.Direction);
                if (this.pathfinding != null)
                {
                    this.pathfinding.enabled = false;
                    this.pathfinding.InvalidPath();
                }
                if (!this.isDirectionalMoveMode)
                {
                    this.MovingMode.ChangeState("DirectionalMovementMode");
                }
                ((MovementState) this.MovingState.TopState()).ChangeDirection();
            }
            else
            {
                this.TargetLocation = InVector;
                if ((Math.Abs(this.TargetLocation.x) <= 0x13880) && (Math.Abs(this.TargetLocation.z) > 0x13880))
                {
                }
                this.Direction = InVector - base.actor.location;
                this.Direction.NormalizeTo(0x3e8);
                this.MoveDirState.SetNewDirection(ref this.Direction);
                if (this.pathfinding != null)
                {
                    this.pathfinding.enabled = true;
                    this.pathfinding.SearchPath(this.TargetLocation);
                }
                if (!this.isAutoMoveMode)
                {
                    this.MovingMode.ChangeState("AutoMovementMode");
                }
                ((MovementState) this.MovingState.TopState()).ChangeTarget();
            }
        }

        public override void SetRotate(VInt3 InDirection, bool bInRotateImmediately)
        {
            if ((InDirection != VInt3.zero) && !base.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate))
            {
                InDirection = InDirection.NormalizeTo(0x3e8);
                base.actor.forward = InDirection;
                base.actor.ObjLinker.SetForward(InDirection, -1);
                base.actor.rotation = Quaternion.LookRotation((Vector3) InDirection);
                this.bRotateImmediately = bInRotateImmediately;
                this.bRotatingLock = true;
            }
        }

        public override void StopMove()
        {
            this.bStopMoving = true;
            this.bExcecuteMoving = false;
            this.TargetLocation = base.actor.location;
            if (this.pathfinding != null)
            {
                this.pathfinding.StopMove();
            }
            this.MovingMode.ChangeState("HoldonMovementMode");
            this.GotoState("IdleMovmentState");
            this.m_ulLastMoveEndTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
        }

        public override void UpdateLogic(int delta)
        {
            if (this.pathfinding != null)
            {
                this.pathfinding.UpdateLogic(delta);
            }
            if (this.bExcecuteMoving || this.bRotateImmediately)
            {
                ((MovementState) this.MovingState.TopState()).UpdateLogic(delta);
            }
            this.GravityMode.Move(delta);
        }

        public override int acceleration
        {
            get
            {
                return this.Acceleration;
            }
        }

        public VInt3 adjustedDirection
        {
            get
            {
                return this.Direction;
            }
        }

        public VInt3 direction
        {
            get
            {
                return this.Direction;
            }
        }

        public override bool isAccelerating
        {
            get
            {
                return (this.MovingState.TopState() is AccelerateMovementState);
            }
        }

        public override bool isAutoMoveMode
        {
            get
            {
                return (this.MovingMode.TopState() is AutoMovementMode);
            }
        }

        public override bool isDecelerate
        {
            get
            {
                return (this.MovingState.TopState() is DecelerateMovementState);
            }
        }

        public override bool isDirectionalMoveMode
        {
            get
            {
                return (this.MovingMode.TopState() is DirectionalMovementMode);
            }
        }

        public override bool isExcuteMoving
        {
            get
            {
                return this.bExcecuteMoving;
            }
        }

        public override bool isFinished
        {
            get
            {
                return ((MovementMode) this.MovingMode.TopState()).ShouldStop();
            }
        }

        public override bool isFlying
        {
            get
            {
                return this.bFlying;
            }
            set
            {
                this.bFlying = value;
            }
        }

        public override bool isLerpFlying
        {
            get
            {
                return this.bLerpFlying;
            }
            set
            {
                this.bLerpFlying = value;
            }
        }

        public override bool isMoving
        {
            get
            {
                return ((MovementState) this.MovingState.TopState()).IsMoving();
            }
        }

        public override bool isRotateImmediately
        {
            get
            {
                return this.bRotateImmediately;
            }
            set
            {
                this.bRotateImmediately = value;
            }
        }

        public override bool isRotatingLock
        {
            get
            {
                return this.bRotatingLock;
            }
        }

        public bool isStandOnTarget
        {
            get
            {
                return ((MovementMode) this.MovingMode.TopState()).ShouldStop();
            }
        }

        public bool isStopMoving
        {
            get
            {
                return this.bStopMoving;
            }
        }

        public override int maxSpeed
        {
            get
            {
                return (int) this._MaxSpeed;
            }
            set
            {
                this._MaxSpeed = value;
            }
        }

        public MovementMode movingMode
        {
            get
            {
                return (this.MovingMode.TopState() as MovementMode);
            }
        }

        public override MPathfinding Pathfinding
        {
            get
            {
                return this.pathfinding;
            }
        }

        public override int rotateSpeed
        {
            get
            {
                return this.RotateSpeed;
            }
        }

        public VInt3 SelectedTargetLocation
        {
            get
            {
                return this.TargetLocation;
            }
        }

        public override int speed
        {
            get
            {
                return ((MovementState) this.MovingState.TopState()).GetCurrentSpeed();
            }
        }

        public override VInt3 targetLocation
        {
            get
            {
                return ((MovementMode) this.MovingMode.TopState()).GetTargetPosition();
            }
        }

        public override uint uCommandId
        {
            get
            {
                return this.uCmdID;
            }
            set
            {
                this.uCmdID = value;
            }
        }

        public override VInt3 velocity
        {
            get
            {
                return ((MovementState) this.MovingState.TopState()).GetVelocity();
            }
        }
    }
}

