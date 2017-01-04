namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;

    public class HostPlayerLogic : IUpdateLogic
    {
        private AreaCheck _areaCheck = null;
        private Player _hostPlayer = null;

        private void ActorMarkProcess(PoolObjHandle<ActorRoot> inActor, AreaCheck.ActorAction action)
        {
            if (inActor != 0)
            {
                OrganWrapper actorControl = inActor.handle.ActorControl as OrganWrapper;
                if (actorControl != null)
                {
                    switch (action)
                    {
                        case AreaCheck.ActorAction.Enter:
                        case AreaCheck.ActorAction.Hover:
                        {
                            long iMaxAttackDistance = (long) inActor.handle.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_0).SkillObj.cfgData.iMaxAttackDistance;
                            if (action != AreaCheck.ActorAction.Enter)
                            {
                                VInt3 num3 = inActor.handle.location - this._hostPlayer.Captain.handle.location;
                                Place place = (num3.sqrMagnitudeLong2D < (iMaxAttackDistance * iMaxAttackDistance)) ? Place.In : Place.Near;
                                bool flag = actorControl.myTarget == this._hostPlayer.Captain;
                                actorControl.ShowAroundEffect((place != Place.Near) ? (!flag ? OrganAroundEffect.HostPlayerInNotHit : OrganAroundEffect.HostPlayerInAndHit) : OrganAroundEffect.HostPlayerNear, true, true, ((float) iMaxAttackDistance) / 10000f);
                                break;
                            }
                            actorControl.ShowAroundEffect(OrganAroundEffect.HostPlayerNear, true, true, ((float) iMaxAttackDistance) / 10000f);
                            break;
                        }
                        case AreaCheck.ActorAction.Leave:
                            actorControl.ShowAroundEffect(OrganAroundEffect.HostPlayerNear, false, true, 1f);
                            break;
                    }
                }
            }
        }

        private bool AroundTowerFilter(ref PoolObjHandle<ActorRoot> actor)
        {
            if ((((actor != 0) && (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)) && (((actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)) || (actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4))) && ((((this._hostPlayer != null) && (this._hostPlayer.Captain != 0)) && ((actor.handle.TheActorMeta.ActorCamp != this._hostPlayer.Captain.handle.TheActorMeta.ActorCamp) && !actor.handle.ActorControl.IsDeadState)) && !this._hostPlayer.Captain.handle.ActorControl.IsDeadState))
            {
                SkillSlot skillSlot = actor.handle.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_0);
                if (skillSlot != null)
                {
                    long num = (long) ((skillSlot.SkillObj.cfgData.iMaxAttackDistance * 0x87) / 100);
                    VInt3 num2 = actor.handle.location - this._hostPlayer.Captain.handle.location;
                    return (num2.sqrMagnitudeLong2D < (num * num));
                }
            }
            return false;
        }

        public void FightOver()
        {
            this._hostPlayer = null;
            this._areaCheck = null;
        }

        public void FightStart()
        {
            this._hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            this._areaCheck = new AreaCheck(new ActorFilterDelegate(this.AroundTowerFilter), new AreaCheck.ActorProcess(this.ActorMarkProcess), Singleton<GameObjMgr>.instance.OrganActors);
        }

        public void UpdateLogic(int delta)
        {
            if (this._areaCheck != null)
            {
                this._areaCheck.UpdateLogic(3);
            }
        }

        public enum Place
        {
            Far,
            Near,
            In
        }
    }
}

