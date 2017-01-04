namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class SetAttackDirDuration : DurationCondition
    {
        private PoolObjHandle<ActorRoot> actor_;
        [ObjectTemplate(new System.Type[] {  })]
        public int attackerId;
        private bool bRotate;
        private bool done_;
        private Quaternion fromRot = Quaternion.identity;
        private int rotTime_;
        private Quaternion toRot = Quaternion.identity;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.done_;
        }

        public override BaseEvent Clone()
        {
            SetAttackDirDuration duration = ClassObjPool<SetAttackDirDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetAttackDirDuration duration = src as SetAttackDirDuration;
            this.attackerId = duration.attackerId;
            this.done_ = duration.done_;
            this.rotTime_ = duration.rotTime_;
            this.fromRot = duration.fromRot;
            this.toRot = duration.toRot;
            this.actor_.Release();
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.done_ = false;
            this.Init(_action, _track);
            base.Enter(_action, _track);
        }

        private void Init(AGE.Action _action, Track _track)
        {
            this.actor_ = _action.GetActorHandle(this.attackerId);
            if (this.actor_ == 0)
            {
                this.done_ = true;
            }
            else
            {
                SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                if (refParamObject == null)
                {
                    this.done_ = true;
                }
                else
                {
                    VInt3 num;
                    if (!refParamObject.CalcAttackerDir(out num, this.actor_))
                    {
                        this.done_ = true;
                    }
                    else if ((num != VInt3.zero) && (this.actor_.handle.MovementComponent != null))
                    {
                        this.bRotate = true;
                        this.actor_.handle.MovementComponent.SetRotate(num, true);
                        this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
                        this.fromRot = this.actor_.handle.rotation;
                        this.toRot = Quaternion.LookRotation((Vector3) num);
                        if (base.length <= 30)
                        {
                            this.actor_.handle.rotation = this.toRot;
                            this.done_ = true;
                        }
                        else
                        {
                            float num2 = Quaternion.Angle(this.fromRot, this.toRot);
                            if (num2 > 180.1f)
                            {
                                DebugHelper.Assert(num2 <= 180.1f);
                            }
                            this.rotTime_ = Mathf.FloorToInt((num2 * base.length) / 180f);
                            DebugHelper.Assert(this.rotTime_ <= base.length);
                        }
                    }
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if ((this.actor_ != 0) && !this.done_)
            {
                this.actor_.handle.rotation = this.toRot;
            }
            if ((this.actor_ != 0) && this.bRotate)
            {
                this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
            }
            this.done_ = true;
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.attackerId = 0;
            this.done_ = false;
            this.rotTime_ = 0;
            this.bRotate = false;
            this.fromRot = Quaternion.identity;
            this.toRot = Quaternion.identity;
            this.actor_.Release();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if (!this.done_ && (this.actor_ != 0))
            {
                if (_localTime >= this.rotTime_)
                {
                    this.actor_.handle.rotation = this.toRot;
                    this.done_ = true;
                }
                else
                {
                    this.actor_.handle.rotation = Quaternion.Slerp(this.fromRot, this.toRot, ((float) _localTime) / ((float) this.rotTime_));
                }
                base.Process(_action, _track, _localTime);
            }
        }
    }
}

