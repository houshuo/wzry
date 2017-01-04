namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    public class BaseAttackMode : LogicComponent
    {
        public virtual bool CancelCommonAttackMode()
        {
            return false;
        }

        public virtual uint CommonAttackSearchEnemy(int srchR)
        {
            return 0;
        }

        protected bool IsValidTargetID(uint selectID)
        {
            bool flag = false;
            if (selectID > 0)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(selectID);
                flag = ((((actor != 0) && !actor.handle.ObjLinker.Invincible) && (!actor.handle.ActorControl.IsDeadState && !base.actor.IsSelfCamp((ActorRoot) actor))) && actor.handle.HorizonMarker.IsVisibleFor(base.actor.TheActorMeta.ActorCamp)) && actor.handle.AttackOrderReady;
                if (!flag)
                {
                    return flag;
                }
                Skill nextSkill = base.actor.ActorControl.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
                if (nextSkill == null)
                {
                    return flag;
                }
                long iMaxSearchDistance = nextSkill.cfgData.iMaxSearchDistance;
                if (((actor == 0) || (actor.handle.shape == null)) || ((actor.handle.ActorAgent == null) || (nextSkill.cfgData == null)))
                {
                    return false;
                }
                iMaxSearchDistance += actor.handle.shape.AvgCollisionRadius;
                iMaxSearchDistance *= iMaxSearchDistance;
                VInt3 num2 = base.actor.ActorControl.actorLocation - actor.handle.location;
                if (num2.sqrMagnitudeLong2D > iMaxSearchDistance)
                {
                    return false;
                }
            }
            return flag;
        }

        public virtual void OnDead()
        {
        }

        public virtual VInt3 SelectSkillDirection(SkillSlot _slot)
        {
            return VInt3.one;
        }

        public virtual bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
        {
            _position = VInt3.zero;
            return false;
        }

        public virtual uint SelectSkillTarget(SkillSlot _slot)
        {
            return 0;
        }
    }
}

