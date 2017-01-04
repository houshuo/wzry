namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveEvent(PassiveEventType.DistancePassiveEvent)]
    public class PassiveDistanceEvent : PassiveEvent
    {
        private bool bDashingState;
        private int curDistance;
        private int curTriggerCount;
        private int groupEffectID;
        private int layerEffectID;
        private int maxTriggerCount;
        private int minMoveSpeed;
        private int triggerDistance;

        private void AbortDashingState()
        {
            if (this.bDashingState)
            {
                this.bDashingState = false;
                if ((base.passiveSkill != null) && (base.passiveSkill.CurAction != 0))
                {
                    base.passiveSkill.CurAction.handle.Stop(false);
                }
                this.RemoveSkillEffectGroup(this.groupEffectID);
            }
        }

        private void AbortLastMove()
        {
            if (this.curTriggerCount > 0)
            {
                this.curDistance = 0;
                this.curTriggerCount = 0;
                this.RemoveSkillEffect(this.layerEffectID);
            }
        }

        public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
        {
            base.Init(_actor, _skill);
            this.curDistance = 0;
            this.curTriggerCount = 0;
            this.bDashingState = false;
            this.triggerDistance = base.localParams[0];
            this.maxTriggerCount = base.localParams[1];
            this.minMoveSpeed = base.localParams[2];
            this.layerEffectID = base.localParams[3];
            this.groupEffectID = base.localParams[4];
        }

        private bool IsLastMove()
        {
            if (!this.sourceActor.handle.MovementComponent.isExcuteMoving || this.sourceActor.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move))
            {
                return false;
            }
            return (!this.IsReduceSpeedState() || (this.sourceActor.handle.ValueComponent.actorMoveSpeed >= this.minMoveSpeed));
        }

        private bool IsReduceSpeedState()
        {
            return ((this.sourceActor.handle.BuffHolderComp != null) && this.sourceActor.handle.BuffHolderComp.IsExistSkillFuncType(7));
        }

        private void RemoveSkillEffect(int _skillCombineID)
        {
            if (base.sourceActor != 0)
            {
                this.sourceActor.handle.BuffHolderComp.RemoveBuff(_skillCombineID);
            }
        }

        private void RemoveSkillEffectGroup(int _groupID)
        {
            if (base.sourceActor != 0)
            {
                this.sourceActor.handle.BuffHolderComp.RemoveSkillEffectGroup(_groupID);
            }
        }

        private void SpawnSkillEffect(int _skillCombineID)
        {
            if (base.sourceActor != 0)
            {
                SkillUseParam inParam = new SkillUseParam();
                inParam.SetOriginator(base.sourceActor);
                this.sourceActor.handle.SkillControl.SpawnBuff(base.sourceActor, ref inParam, _skillCombineID, true);
            }
        }

        private void UpdateLastMove(int _delta)
        {
            if (base.sourceActor != 0)
            {
                if (!this.bDashingState)
                {
                    if (this.IsLastMove())
                    {
                        this.triggerDistance = base.localParams[0];
                        int actorMoveSpeed = this.sourceActor.handle.ValueComponent.actorMoveSpeed;
                        int num2 = (_delta * actorMoveSpeed) / 0x3e8;
                        this.curDistance += num2;
                        if ((this.curDistance >= this.triggerDistance) && (this.curTriggerCount < this.maxTriggerCount))
                        {
                            this.curTriggerCount++;
                            this.curDistance -= this.triggerDistance;
                            if (this.curTriggerCount < this.maxTriggerCount)
                            {
                                this.SpawnSkillEffect(this.layerEffectID);
                            }
                            else if (this.curTriggerCount == this.maxTriggerCount)
                            {
                                this.SpawnSkillEffect(this.layerEffectID);
                                this.bDashingState = true;
                                base.Reset();
                                base.Trigger();
                            }
                        }
                    }
                    else
                    {
                        this.AbortLastMove();
                    }
                }
                else if (!this.IsLastMove() || base.Fit())
                {
                    this.AbortLastMove();
                    this.AbortDashingState();
                }
            }
        }

        public override void UpdateLogic(int _delta)
        {
            base.UpdateLogic(_delta);
            this.UpdateLastMove(_delta);
        }
    }
}

