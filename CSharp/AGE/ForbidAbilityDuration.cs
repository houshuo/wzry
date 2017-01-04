namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class ForbidAbilityDuration : DurationEvent
    {
        public bool abortFilterDamage;
        public bool abortFilterMove;
        public bool abortFilterSkill0;
        public bool abortFilterSkill1;
        public bool abortFilterSkill2;
        public bool abortFilterSkill3;
        public bool abortFilterSkill4;
        private PoolObjHandle<ActorRoot> actor_;
        [ObjectTemplate(new System.Type[] {  })]
        public int attackerId;
        public bool delaySkillAbort;
        public bool forbidEnergyRecover;
        public bool forbidFilterSkill0;
        public bool forbidFilterSkill1;
        public bool forbidFilterSkill2;
        public bool forbidFilterSkill3;
        public bool forbidFilterSkill4;
        public bool forbidFilterSkill5;
        public bool forbidFilterSkill6;
        public bool forbidFilterSkill7;
        public bool forbidMove;
        public bool forbidMoveRotate;
        public bool forbidSkill;
        public bool forbidSkillAbort = true;
        public bool ImmuneControl;
        public bool ImmuneNegative;

        private void ClearForbidSkill()
        {
            if (this.forbidSkill)
            {
                if (!this.forbidFilterSkill0)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_0);
                }
                if (!this.forbidFilterSkill1)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_1);
                }
                if (!this.forbidFilterSkill2)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_2);
                }
                if (!this.forbidFilterSkill3)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_3);
                }
                if (!this.forbidFilterSkill4)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_4);
                }
                if (!this.forbidFilterSkill5)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_5);
                }
                if (!this.forbidFilterSkill6)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_6);
                }
                if (!this.forbidFilterSkill7)
                {
                    this.actor_.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_7);
                }
            }
        }

        public override BaseEvent Clone()
        {
            ForbidAbilityDuration duration = ClassObjPool<ForbidAbilityDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ForbidAbilityDuration duration = src as ForbidAbilityDuration;
            this.attackerId = duration.attackerId;
            this.forbidMove = duration.forbidMove;
            this.forbidMoveRotate = duration.forbidMoveRotate;
            this.delaySkillAbort = duration.delaySkillAbort;
            this.ImmuneNegative = duration.ImmuneNegative;
            this.ImmuneControl = duration.ImmuneControl;
            this.forbidEnergyRecover = duration.forbidEnergyRecover;
            this.forbidSkill = duration.forbidSkill;
            this.forbidFilterSkill0 = duration.forbidFilterSkill0;
            this.forbidFilterSkill1 = duration.forbidFilterSkill1;
            this.forbidFilterSkill2 = duration.forbidFilterSkill2;
            this.forbidFilterSkill3 = duration.forbidFilterSkill3;
            this.forbidFilterSkill4 = duration.forbidFilterSkill4;
            this.forbidFilterSkill5 = duration.forbidFilterSkill5;
            this.forbidFilterSkill6 = duration.forbidFilterSkill6;
            this.forbidFilterSkill7 = duration.forbidFilterSkill7;
            this.forbidSkillAbort = duration.forbidSkillAbort;
            this.abortFilterSkill0 = duration.abortFilterSkill0;
            this.abortFilterSkill1 = duration.abortFilterSkill1;
            this.abortFilterSkill2 = duration.abortFilterSkill2;
            this.abortFilterSkill3 = duration.abortFilterSkill3;
            this.abortFilterSkill4 = duration.abortFilterSkill4;
            this.abortFilterMove = duration.abortFilterMove;
            this.abortFilterDamage = duration.abortFilterDamage;
            this.actor_ = duration.actor_;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.actor_ = _action.GetActorHandle(this.attackerId);
            if (this.actor_ != 0)
            {
                if (this.forbidMove)
                {
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                }
                if (this.delaySkillAbort && (this.actor_.handle.SkillControl.CurUseSkill != null))
                {
                    this.actor_.handle.SkillControl.CurUseSkill.bProtectAbortSkill = true;
                }
                if (this.forbidMoveRotate)
                {
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
                }
                if (this.forbidEnergyRecover)
                {
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_RecoverEnergy);
                }
                this.ForbidSkill();
                this.ForbidSkillAbort();
                this.ImmuneAbility(false);
            }
        }

        private void ForbidSkill()
        {
            if (this.forbidSkill)
            {
                if (!this.forbidFilterSkill0)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_0);
                }
                if (!this.forbidFilterSkill1)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_1);
                }
                if (!this.forbidFilterSkill2)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_2);
                }
                if (!this.forbidFilterSkill3)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_3);
                }
                if (!this.forbidFilterSkill4)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_4);
                }
                if (!this.forbidFilterSkill5)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_5);
                }
                if (!this.forbidFilterSkill6)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_6);
                }
                if (!this.forbidFilterSkill7)
                {
                    this.actor_.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_7);
                }
            }
        }

        private void ForbidSkillAbort()
        {
            if ((this.forbidSkillAbort && (this.actor_ != 0)) && (this.actor_.handle.SkillControl.CurUseSkill != null))
            {
                this.actor_.handle.SkillControl.CurUseSkill.skillAbort.InitAbort(false);
                if (this.abortFilterSkill0)
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_0);
                }
                if (this.abortFilterSkill1)
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_1);
                }
                if (this.abortFilterSkill2)
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_2);
                }
                if (this.abortFilterSkill3)
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_3);
                }
                if (this.abortFilterSkill4)
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_SKILL_4);
                }
                if (this.abortFilterMove)
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_MOVE);
                }
                if (this.abortFilterDamage)
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.SetAbort(SkillAbortType.TYPE_DAMAGE);
                }
            }
        }

        private void ImmuneAbility(bool bRemove)
        {
            if (this.ImmuneNegative)
            {
                if (!bRemove)
                {
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
                }
                else
                {
                    this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
                }
            }
            if (this.ImmuneControl && (this.actor_ != 0))
            {
                if (!bRemove)
                {
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
                }
                else
                {
                    this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.actor_ != 0)
            {
                if (this.forbidMove)
                {
                    this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                    if (!this.actor_.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move))
                    {
                        DefaultGameEventParam prm = new DefaultGameEventParam(this.actor_, this.actor_);
                        Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, ref prm);
                    }
                }
                if (this.forbidSkillAbort && (this.actor_.handle.SkillControl.CurUseSkill != null))
                {
                    this.actor_.handle.SkillControl.CurUseSkill.skillAbort.InitAbort(true);
                }
                if (this.forbidMoveRotate)
                {
                    this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate);
                }
                if (this.delaySkillAbort && (this.actor_.handle.SkillControl.CurUseSkill != null))
                {
                    this.actor_.handle.SkillControl.CurUseSkill.bProtectAbortSkill = false;
                    if (this.actor_.handle.SkillControl.CurUseSkill.bDelayAbortSkill)
                    {
                        this.actor_.handle.SkillControl.CurUseSkill.bDelayAbortSkill = false;
                        this.actor_.handle.SkillControl.ForceAbortCurUseSkill();
                    }
                }
                if (this.forbidEnergyRecover)
                {
                    this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_RecoverEnergy);
                }
                this.ImmuneAbility(true);
                this.ClearForbidSkill();
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.attackerId = 0;
            this.forbidMove = false;
            this.forbidMoveRotate = false;
            this.delaySkillAbort = false;
            this.ImmuneNegative = false;
            this.ImmuneControl = false;
            this.forbidEnergyRecover = false;
            this.forbidSkill = false;
            this.forbidFilterSkill0 = false;
            this.forbidFilterSkill1 = false;
            this.forbidFilterSkill2 = false;
            this.forbidFilterSkill3 = false;
            this.forbidFilterSkill4 = false;
            this.forbidFilterSkill5 = false;
            this.forbidFilterSkill6 = false;
            this.forbidFilterSkill7 = false;
            this.forbidSkillAbort = true;
            this.abortFilterSkill0 = false;
            this.abortFilterSkill1 = false;
            this.abortFilterSkill2 = false;
            this.abortFilterSkill3 = false;
            this.abortFilterSkill4 = false;
            this.abortFilterMove = false;
            this.abortFilterDamage = false;
            this.actor_.Release();
        }
    }
}

