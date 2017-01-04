namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class Horizon : IUpdateLogic
    {
        private bool _enabled = false;
        private bool _fighting = false;
        private static uint _globalSight;
        private static int exposeDurationHero;
        private static int exposeDurationNormal;
        private static int exposeRadius;
        public const byte UPDATE_CYCLE = 8;

        public void FightOver()
        {
            this.Enabled = false;
            _globalSight = 0;
        }

        public void FightStart()
        {
            this._fighting = true;
            _globalSight = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x38).dwConfValue;
            this._enabled = Singleton<BattleLogic>.instance.GetCurLvelContext().m_horizonEnableMethod == EnableMethod.EnableAll;
        }

        public static int QueryExposeDurationHero()
        {
            if (exposeDurationHero == 0)
            {
                if (GameDataMgr.globalInfoDatabin != null)
                {
                    ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xe0);
                    if (dataByKey != null)
                    {
                        exposeDurationHero = (int) dataByKey.dwConfValue;
                    }
                }
                if (exposeDurationHero == 0)
                {
                    exposeDurationHero = 0x1388;
                }
            }
            return exposeDurationHero;
        }

        public static int QueryExposeDurationNormal()
        {
            if (exposeDurationNormal == 0)
            {
                if (GameDataMgr.globalInfoDatabin != null)
                {
                    ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xdf);
                    if (dataByKey != null)
                    {
                        exposeDurationNormal = (int) dataByKey.dwConfValue;
                    }
                }
                if (exposeDurationNormal == 0)
                {
                    exposeDurationNormal = 0x1388;
                }
            }
            return exposeDurationNormal;
        }

        public static int QueryExposeRadius()
        {
            if (exposeRadius == 0)
            {
                if (GameDataMgr.globalInfoDatabin != null)
                {
                    ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xde);
                    if (dataByKey != null)
                    {
                        exposeRadius = (int) dataByKey.dwConfValue;
                    }
                }
                if (exposeRadius == 0)
                {
                    exposeRadius = 0x1388;
                }
            }
            return exposeRadius;
        }

        public static uint QueryGlobalSight()
        {
            if (_globalSight == 0)
            {
                if (GameDataMgr.globalInfoDatabin != null)
                {
                    ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x38);
                    if (dataByKey != null)
                    {
                        _globalSight = dataByKey.dwConfValue;
                    }
                }
                if (_globalSight == 0)
                {
                    _globalSight = 0x2710;
                }
            }
            return _globalSight;
        }

        public void UpdateLogic(int delta)
        {
            if (this._enabled && this._fighting)
            {
                uint num = Singleton<FrameSynchr>.instance.CurFrameNum % 8;
                List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
                int count = gameActors.Count;
                for (int i = 0; i < count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = gameActors[i];
                    ActorRoot root = handle.handle;
                    if (((root.ObjID % 8) == num) && !root.ActorControl.IsDeadState)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (j != root.TheActorMeta.ActorCamp)
                            {
                                COM_PLAYERCAMP actorCamp = root.TheActorMeta.ActorCamp;
                                List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP) j);
                                int num5 = campActors.Count;
                                for (int k = 0; k < num5; k++)
                                {
                                    PoolObjHandle<ActorRoot> handle2 = campActors[k];
                                    ActorRoot root2 = handle2.handle;
                                    if (!root2.HorizonMarker.IsSightVisited(actorCamp))
                                    {
                                        long sightRadius;
                                        if (root.HorizonMarker.SightRadius != 0)
                                        {
                                            sightRadius = root.HorizonMarker.SightRadius;
                                        }
                                        else
                                        {
                                            sightRadius = _globalSight;
                                        }
                                        VInt3 num8 = root2.location - root.location;
                                        if (num8.sqrMagnitudeLong2D < (sightRadius * sightRadius))
                                        {
                                            root2.HorizonMarker.VisitSight(actorCamp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                if (value != this._enabled)
                {
                    this._enabled = value;
                    List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
                    int count = gameActors.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PoolObjHandle<ActorRoot> handle = gameActors[i];
                        handle.handle.HorizonMarker.SetEnabled(this._enabled);
                    }
                }
            }
        }

        public enum EnableMethod
        {
            DisableAll,
            EnableAll,
            EnableMarkNoMist,
            INVALID
        }
    }
}

