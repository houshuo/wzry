namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Pathfinding.RVO;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class FreezeActorDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        private string curAnimName;
        public int freezeHeight;
        private RVOController rovController;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            FreezeActorDuration duration = ClassObjPool<FreezeActorDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            FreezeActorDuration duration = src as FreezeActorDuration;
            this.targetId = duration.targetId;
            this.freezeHeight = duration.freezeHeight;
            this.actorObj = duration.actorObj;
            this.curAnimName = duration.curAnimName;
            this.rovController = duration.rovController;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            if (this.actorObj != 0)
            {
                ObjWrapper actorControl = this.actorObj.handle.ActorControl;
                if (actorControl != null)
                {
                    this.PauseAnimation();
                    actorControl.TerminateMove();
                    actorControl.ClearMoveCommand();
                    actorControl.ForceAbortCurUseSkill();
                    this.actorObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                    this.actorObj.handle.ActorControl.AddDisableSkillFlag(SkillSlotType.SLOT_SKILL_COUNT);
                    if ((this.freezeHeight > 0) && this.actorObj.handle.isMovable)
                    {
                        VInt3 location = this.actorObj.handle.location;
                        location.y += this.freezeHeight;
                        this.actorObj.handle.location = location;
                    }
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (this.actorObj != 0)
            {
                this.RecoverAnimation();
                this.actorObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                this.actorObj.handle.ActorControl.RmvDisableSkillFlag(SkillSlotType.SLOT_SKILL_COUNT);
                if ((this.freezeHeight > 0) && this.actorObj.handle.isMovable)
                {
                    VInt groundY = 0;
                    VInt3 location = this.actorObj.handle.location;
                    location.y -= this.freezeHeight;
                    PathfindingUtility.GetGroundY(location, out groundY);
                    if (location.y < groundY.i)
                    {
                        location.y = groundY.i;
                    }
                    this.actorObj.handle.location = location;
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.freezeHeight = 0;
            this.actorObj.Release();
            this.curAnimName = null;
            this.rovController = null;
        }

        private void PauseAnimation()
        {
            AnimPlayComponent animControl = this.actorObj.handle.AnimControl;
            if (animControl != null)
            {
                this.curAnimName = animControl.GetCurAnimName();
                if ((this.actorObj.handle.ActorMesh != null) && (this.actorObj.handle.ActorMeshAnimation != null))
                {
                    AnimationState state = this.actorObj.handle.ActorMeshAnimation[this.curAnimName];
                    if (state != null)
                    {
                        state.speed = 0f;
                    }
                    animControl.bPausePlay = true;
                }
            }
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }

        private void RecoverAnimation()
        {
            AnimPlayComponent animControl = this.actorObj.handle.AnimControl;
            if ((this.actorObj.handle.ActorMesh != null) && (this.actorObj.handle.ActorMeshAnimation != null))
            {
                AnimationState state = this.actorObj.handle.ActorMeshAnimation[this.curAnimName];
                if (state != null)
                {
                    state.speed = 1f;
                }
                if (animControl != null)
                {
                    animControl.bPausePlay = false;
                    animControl.UpdatePlay();
                }
            }
        }
    }
}

