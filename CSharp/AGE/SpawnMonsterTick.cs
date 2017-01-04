namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class SpawnMonsterTick : TickEvent
    {
        public int ConfigID;
        public bool Invincible;
        public int LifeTime;
        public bool Moveable;
        public COM_PLAYERCAMP PlayerCamp;
        private PoolObjHandle<ActorRoot> tarActor;
        [ObjectTemplate(new System.Type[] {  })]
        public int TargetId = -1;
        private GameObject wayPoint;
        public int WayPointId = -1;

        public override BaseEvent Clone()
        {
            SpawnMonsterTick tick = ClassObjPool<SpawnMonsterTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SpawnMonsterTick tick = src as SpawnMonsterTick;
            this.TargetId = tick.TargetId;
            this.WayPointId = tick.WayPointId;
            this.ConfigID = tick.ConfigID;
            this.LifeTime = tick.LifeTime;
            this.tarActor = tick.tarActor;
            this.PlayerCamp = tick.PlayerCamp;
            this.Invincible = tick.Invincible;
            this.Moveable = tick.Moveable;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.tarActor.Release();
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            this.tarActor = _action.GetActorHandle(this.TargetId);
            if (this.tarActor == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                this.wayPoint = _action.GetGameObject(this.WayPointId);
                this.SpawnMonster(_action);
                this.tarActor.Release();
            }
        }

        private void SpawnMonster(AGE.Action _action)
        {
            ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(this.ConfigID);
            if (dataCfgInfoByCurLevelDiff != null)
            {
                string fullPathInResources = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo) + ".asset";
                CActorInfo content = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(CActorInfo), enResourceType.BattleScene, false, false).m_content as CActorInfo;
                if (content != null)
                {
                    ActorMeta actorMeta = new ActorMeta {
                        ConfigId = this.ConfigID,
                        ActorType = ActorTypeDef.Actor_Type_Monster,
                        ActorCamp = this.PlayerCamp,
                        EnCId = this.ConfigID
                    };
                    VInt3 location = this.tarActor.handle.location;
                    VInt3 forward = this.tarActor.handle.forward;
                    PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref actorMeta, location, forward, false, true);
                    if (actor != 0)
                    {
                        actor.handle.InitActor();
                        actor.handle.PrepareFight();
                        Singleton<GameObjMgr>.instance.AddActor(actor);
                        actor.handle.StartFight();
                        actor.handle.ObjLinker.Invincible = this.Invincible;
                        actor.handle.ObjLinker.CanMovable = this.Moveable;
                        MonsterWrapper actorControl = actor.handle.ActorControl as MonsterWrapper;
                        if (actorControl != null)
                        {
                            if (this.wayPoint != null)
                            {
                                actorControl.AttackAlongRoute(this.wayPoint.GetComponent<WaypointsHolder>());
                            }
                            if (this.LifeTime > 0)
                            {
                                actorControl.LifeTime = this.LifeTime;
                            }
                        }
                    }
                }
            }
        }
    }
}

