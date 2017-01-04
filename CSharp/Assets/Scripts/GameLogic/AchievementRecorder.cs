namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using System.Collections.Generic;

    public class AchievementRecorder
    {
        private bool bFristBlood;

        private KillDetailInfo _create(PoolObjHandle<ActorRoot> killer, PoolObjHandle<ActorRoot> victim, KillDetailInfoType type, KillDetailInfoType HeroMultiKillType, KillDetailInfoType HeroContiKillType, bool bSelfCamp, bool bAllDead, bool bPlayerSelf_KillOrKilled)
        {
            return new KillDetailInfo { Killer = killer, Victim = victim, Type = type, HeroMultiKillType = HeroMultiKillType, HeroContiKillType = HeroContiKillType, bSelfCamp = bSelfCamp, bAllDead = bAllDead, bPlayerSelf_KillOrKilled = bPlayerSelf_KillOrKilled };
        }

        public void Clear()
        {
            this.bFristBlood = false;
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
        }

        private bool IsAllDead(ref PoolObjHandle<ActorRoot> actorHandle)
        {
            if (actorHandle == 0)
            {
                return false;
            }
            List<Player> allCampPlayers = Singleton<GamePlayerCenter>.instance.GetAllCampPlayers(actorHandle.handle.TheActorMeta.ActorCamp);
            if (allCampPlayers.Count <= 1)
            {
                return false;
            }
            if (allCampPlayers != null)
            {
                for (int i = 0; i < allCampPlayers.Count; i++)
                {
                    Player player = allCampPlayers[i];
                    if ((player != null) && !player.IsAllHeroesDead())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void OnActorDeath(ref GameDeadEventParam prm)
        {
            if (((prm.src != 0) && (prm.orignalAtker != 0)) && !prm.bImmediateRevive)
            {
                bool flag = false;
                switch (prm.src.handle.ActorControl.GetActorSubSoliderType())
                {
                    case 8:
                    case 9:
                        flag = true;
                        break;
                }
                if (((prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster) || (prm.src.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId)) || flag)
                {
                    KillDetailInfo info = this.OnActorDeathd(ref prm);
                    if (info != null)
                    {
                        Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
                    }
                }
            }
        }

        private KillDetailInfo OnActorDeathd(ref GameDeadEventParam param)
        {
            KillDetailInfo info = null;
            KillDetailInfoType type = KillDetailInfoType.Info_Type_None;
            KillDetailInfoType heroMultiKillType = KillDetailInfoType.Info_Type_None;
            KillDetailInfoType heroContiKillType = KillDetailInfoType.Info_Type_None;
            PoolObjHandle<ActorRoot> victim = new PoolObjHandle<ActorRoot>();
            PoolObjHandle<ActorRoot> killer = new PoolObjHandle<ActorRoot>();
            DefaultGameEventParam prm = new DefaultGameEventParam(param.src, param.atker, ref param.orignalAtker);
            HeroWrapper actorControl = null;
            HeroWrapper wrapper2 = null;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            DebugHelper.Assert(hostPlayer != null, "Fatal error in OnActorDeadthd, HostPlayer is null!");
            if (hostPlayer != null)
            {
                DebugHelper.Assert((bool) hostPlayer.Captain, "Fatal error in OnActorDeadthd, Captain is null!");
            }
            bool bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
            bool flag8 = false;
            uint objID = hostPlayer.Captain.handle.ObjID;
            flag8 = (objID == prm.src.handle.ObjID) || (objID == prm.orignalAtker.handle.ObjID);
            flag = prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero;
            flag2 = prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ;
            flag3 = (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (prm.src.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId);
            byte actorSubSoliderType = prm.src.handle.ActorControl.GetActorSubSoliderType();
            bool flag9 = false;
            bool flag10 = false;
            switch (actorSubSoliderType)
            {
                case 8:
                    flag9 = true;
                    flag3 = false;
                    break;

                case 9:
                    flag10 = true;
                    flag3 = false;
                    break;
            }
            victim = prm.src;
            killer = prm.orignalAtker;
            if (flag)
            {
                actorControl = prm.src.handle.ActorControl as HeroWrapper;
                wrapper2 = prm.orignalAtker.handle.ActorControl as HeroWrapper;
                if (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    if (prm.orignalAtker.handle.ObjID == objID)
                    {
                        flag8 = true;
                    }
                    flag4 = true;
                    bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
                }
                else if (actorControl.IsKilledByHero())
                {
                    flag4 = true;
                    killer = actorControl.LastHeroAtker;
                    wrapper2 = actorControl.LastHeroAtker.handle.ActorControl as HeroWrapper;
                    if (killer.handle.ObjID == objID)
                    {
                        flag8 = true;
                    }
                    bSelfCamp = hostPlayer.PlayerCamp == killer.handle.TheActorMeta.ActorCamp;
                }
                else if (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    flag5 = true;
                    flag4 = false;
                    bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
                }
                else if (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    flag6 = true;
                    flag4 = false;
                    bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
                }
                if (flag && flag4)
                {
                    wrapper2.ContiDeadNum = 0;
                    wrapper2.ContiKillNum++;
                    if (wrapper2.IsInMultiKill())
                    {
                        wrapper2.MultiKillNum++;
                    }
                    else
                    {
                        wrapper2.MultiKillNum = 1;
                    }
                    wrapper2.UpdateLastKillTime();
                }
            }
            if (flag && flag4)
            {
                bool flag11 = false;
                if (wrapper2.MultiKillNum == 2)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_DoubleKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, ref prm);
                }
                else if (wrapper2.MultiKillNum == 3)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_TripleKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_TripleKill, ref prm);
                }
                else if (wrapper2.MultiKillNum == 4)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_QuataryKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, ref prm);
                }
                else if (wrapper2.MultiKillNum >= 5)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_PentaKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_PentaKill, ref prm);
                }
                if (flag11 && (info == null))
                {
                    info = this._create(killer, victim, type, heroMultiKillType, type, bSelfCamp, false, flag8);
                }
            }
            if ((flag && flag4) && (actorControl.ContiKillNum >= 3))
            {
                if (actorControl.ContiKillNum >= 7)
                {
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, ref prm);
                }
                if (info == null)
                {
                    info = this._create(killer, victim, KillDetailInfoType.Info_Type_StopMultiKill, heroMultiKillType, type, bSelfCamp, false, flag8);
                }
            }
            if ((flag && flag4) && !this.bFristBlood)
            {
                this.bFristBlood = true;
                return this._create(killer, victim, KillDetailInfoType.Info_Type_First_Kill, heroMultiKillType, type, bSelfCamp, false, flag8);
            }
            if (flag2 && ((prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4)))
            {
                KillDetailInfo info2 = this._create(killer, victim, KillDetailInfoType.Info_Type_DestroyTower, type, type, bSelfCamp, false, flag8);
                if (prm.src.handle.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0)
                {
                    Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info2);
                    return this._create(killer, victim, KillDetailInfoType.Info_Type_Soldier_Boosted, type, type, bSelfCamp, false, flag8);
                }
                return info2;
            }
            if (flag && flag4)
            {
                bool flag12 = false;
                if (wrapper2.ContiKillNum == 3)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_MonsterKill;
                }
                else if (wrapper2.ContiKillNum == 4)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_DominateBattle;
                }
                else if (wrapper2.ContiKillNum == 5)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_Legendary;
                }
                else if (wrapper2.ContiKillNum == 6)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_TotalAnnihilat;
                }
                else if (wrapper2.ContiKillNum >= 7)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_Odyssey;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_Odyssey, ref prm);
                }
                if (flag12 && (info == null))
                {
                    info = this._create(killer, victim, type, type, heroContiKillType, bSelfCamp, false, flag8);
                }
            }
            if (info != null)
            {
                return info;
            }
            if (flag && ((flag4 || flag5) || flag6))
            {
                return this._create(killer, victim, KillDetailInfoType.Info_Type_Kill, type, type, bSelfCamp, false, flag8);
            }
            if (flag && this.IsAllDead(ref prm.src))
            {
                KillDetailInfo info3 = this._create(killer, victim, type, type, type, bSelfCamp, true, flag8);
                Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info3);
            }
            if (flag3)
            {
                return this._create(killer, victim, KillDetailInfoType.Info_Type_KillDragon, type, type, bSelfCamp, false, flag8);
            }
            if (flag10)
            {
                return this._create(killer, victim, KillDetailInfoType.Info_Type_KillBIGDRAGON, type, type, bSelfCamp, false, flag8);
            }
            if (flag9)
            {
                return this._create(killer, victim, KillDetailInfoType.Info_Type_KillBARON, type, type, bSelfCamp, false, flag8);
            }
            return null;
        }

        public void StartRecord()
        {
            this.Clear();
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
        }
    }
}

