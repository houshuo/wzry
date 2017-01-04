namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class SoldierWave
    {
        private bool bInIdleState;
        private int curTick;
        private int firstTick = -1;
        private int idleTick;
        private bool isCannonNotified;
        private int preSpawnTick;
        public int repeatCount = 1;
        public SoldierSelector Selector = new SoldierSelector();

        public SoldierWave(SoldierRegion InRegion, ResSoldierWaveInfo InWaveInfo, int InIndex)
        {
            this.Region = InRegion;
            this.WaveInfo = InWaveInfo;
            this.Index = InIndex;
            DebugHelper.Assert((this.Region != null) && (InWaveInfo != null));
            this.isCannonNotified = false;
            this.Reset();
        }

        public void CloneState(Assets.Scripts.GameLogic.SoldierWave sw)
        {
            this.curTick = sw.curTick;
            this.preSpawnTick = sw.preSpawnTick;
            this.firstTick = sw.firstTick;
            this.bInIdleState = sw.bInIdleState;
            this.idleTick = sw.idleTick;
            this.bInIdleState = sw.bInIdleState;
            this.isCannonNotified = sw.isCannonNotified;
        }

        public void Reset()
        {
            this.curTick = this.preSpawnTick = 0;
            this.firstTick = -1;
            this.repeatCount = 1;
            this.bInIdleState = false;
            this.idleTick = 0;
            this.Selector.Reset(this.WaveInfo);
        }

        private void SpawnSoldier(uint SoldierID)
        {
            ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff((int) SoldierID);
            if (dataCfgInfoByCurLevelDiff != null)
            {
                if (CActorInfo.GetActorInfo(StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo), enResourceType.BattleScene) != null)
                {
                    Transform transform = this.Region.transform;
                    COM_PLAYERCAMP campType = this.Region.CampType;
                    ActorMeta actorMeta = new ActorMeta {
                        ConfigId = (int) SoldierID,
                        ActorType = ActorTypeDef.Actor_Type_Monster,
                        ActorCamp = campType
                    };
                    VInt3 position = (VInt3) transform.position;
                    VInt3 forward = (VInt3) transform.forward;
                    PoolObjHandle<ActorRoot> actor = new PoolObjHandle<ActorRoot>();
                    if (!Singleton<GameObjMgr>.GetInstance().TryGetFromCache(ref actor, ref actorMeta))
                    {
                        actor = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref actorMeta, position, forward, false, true);
                        if (actor != 0)
                        {
                            actor.handle.InitActor();
                            actor.handle.PrepareFight();
                            Singleton<GameObjMgr>.instance.AddActor(actor);
                            actor.handle.StartFight();
                        }
                    }
                    else
                    {
                        ActorRoot handle = actor.handle;
                        handle.TheActorMeta.ActorCamp = actorMeta.ActorCamp;
                        handle.ReactiveActor(position, forward);
                    }
                    if (actor != 0)
                    {
                        if (this.Region.AttackRoute != null)
                        {
                            actor.handle.ActorControl.AttackAlongRoute(this.Region.AttackRoute.GetComponent<WaypointsHolder>());
                        }
                        else if (this.Region.finalTarget != null)
                        {
                            FrameCommand<AttackPositionCommand> cmd = FrameCommandFactory.CreateFrameCommand<AttackPositionCommand>();
                            cmd.cmdId = 1;
                            cmd.cmdData.WorldPos = new VInt3(this.Region.finalTarget.transform.position);
                            actor.handle.ActorControl.CmdAttackMoveToDest(cmd, cmd.cmdData.WorldPos);
                        }
                        if (!this.isCannonNotified && (this.WaveInfo.bType == 1))
                        {
                            KillNotify theKillNotify = Singleton<CBattleSystem>.GetInstance().TheKillNotify;
                            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                            if ((theKillNotify != null) && (hostPlayer != null))
                            {
                                bool bSrcAllies = hostPlayer.PlayerCamp == actor.handle.TheActorMeta.ActorCamp;
                                if (bSrcAllies)
                                {
                                    KillInfo killInfo = new KillInfo((hostPlayer.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? KillNotify.red_cannon_icon : KillNotify.blue_cannon_icon, null, KillDetailInfoType.Info_Type_Cannon_Spawned, bSrcAllies, false, ActorTypeDef.Invalid);
                                    theKillNotify.AddKillInfo(killInfo);
                                    this.isCannonNotified = true;
                                }
                            }
                        }
                    }
                }
                if (this.Region.bTriggerEvent)
                {
                    SoldierWaveParam prm = new SoldierWaveParam(this.Index, this.repeatCount, this.Region.GetNextRepeatTime(false));
                    Singleton<GameEventSys>.instance.SendEvent<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, ref prm);
                }
            }
        }

        public SoldierSpawnResult Update(int delta)
        {
            this.firstTick = (this.firstTick != -1) ? this.firstTick : this.curTick;
            this.curTick += delta;
            if (this.bInIdleState)
            {
                if ((this.curTick - this.idleTick) < this.WaveInfo.dwIntervalTick)
                {
                    return SoldierSpawnResult.ShouldWaitInterval;
                }
                this.bInIdleState = false;
                this.Selector.Reset(this.WaveInfo);
                this.repeatCount++;
            }
            if (this.curTick < this.WaveInfo.dwStartWatiTick)
            {
                return SoldierSpawnResult.ShouldWaitStart;
            }
            if ((((this.curTick - this.firstTick) >= this.WaveInfo.dwRepeatTimeTick) && (this.WaveInfo.dwRepeatTimeTick > 0)) && (!this.Region.bForceCompleteSpawn || (this.Region.bForceCompleteSpawn && this.Selector.isFinished)))
            {
                return SoldierSpawnResult.Finish;
            }
            if ((this.curTick - this.preSpawnTick) >= MonoSingleton<GlobalConfig>.instance.SoldierWaveInterval)
            {
                if (this.Selector.isFinished)
                {
                    if ((this.WaveInfo.dwRepeatNum != 0) && (this.repeatCount >= this.WaveInfo.dwRepeatNum))
                    {
                        return SoldierSpawnResult.Finish;
                    }
                    this.bInIdleState = true;
                    this.idleTick = this.curTick;
                    return SoldierSpawnResult.ShouldWaitInterval;
                }
                uint soldierID = this.Selector.NextSoldierID();
                DebugHelper.Assert(soldierID != 0);
                this.SpawnSoldier(soldierID);
                this.preSpawnTick = this.curTick;
            }
            return SoldierSpawnResult.ShouldWaitSoldierInterval;
        }

        public int Index { get; protected set; }

        public bool IsInIdle
        {
            get
            {
                return this.bInIdleState;
            }
        }

        public SoldierRegion Region { get; protected set; }

        public ResSoldierWaveInfo WaveInfo { get; protected set; }
    }
}

