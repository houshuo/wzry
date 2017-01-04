namespace Assets.Scripts.GameLogic.GameKernal
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Player
    {
        private readonly List<PoolObjHandle<ActorRoot>> _heroes = new List<PoolObjHandle<ActorRoot>>();
        private readonly List<uint> _heroIds = new List<uint>();
        private CrypticInt32 _level = 1;
        public SelectEnemyType AttackTargetMode = SelectEnemyType.SelectLowHp;
        public bool bCommonAttackLockMode = true;
        private bool bUseAdvanceCommonAttack = true;
        public int CampPos;
        public PoolObjHandle<ActorRoot> Captain;
        public uint CaptainId = 0;
        public uint ClassOfRank;
        public bool Computer;
        public uint GradeOfRank;
        public int HeadIconId;
        public int HonorId;
        public int HonorLevel;
        public bool isGM;
        public int LogicWrold;
        public bool m_bMoved;
        public string Name;
        public string OpenId;
        public COM_PLAYERCAMP PlayerCamp;
        public uint PlayerId;
        public ulong PlayerUId;
        private OperateMode useOperateMode;
        public uint VipLv;

        public Player()
        {
            this.Level = 1;
        }

        public void AddHero(uint heroCfgId)
        {
            if ((heroCfgId != 0) && !this._heroIds.Contains(heroCfgId))
            {
                if (this._heroIds.Count >= 3)
                {
                    DebugHelper.Assert(false, "已经给player添加了三个英雄但是还在尝试继续添加");
                }
                else
                {
                    this._heroIds.Add(heroCfgId);
                    if (!this.Computer)
                    {
                        if (Singleton<GamePlayerCenter>.instance.HostPlayerId == this.PlayerId)
                        {
                            object[] inParameters = new object[] { this.PlayerId, heroCfgId };
                            DebugHelper.CustomLog("Player {0} adds Hero {1}", inParameters);
                        }
                    }
                    else if (string.IsNullOrEmpty(this.Name))
                    {
                        ActorStaticData actorData = new ActorStaticData();
                        ActorMeta actorMeta = new ActorMeta();
                        IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
                        actorMeta.PlayerId = this.PlayerId;
                        actorMeta.ActorType = ActorTypeDef.Actor_Type_Hero;
                        actorMeta.ConfigId = (int) heroCfgId;
                        string str = !actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData) ? null : actorData.TheResInfo.Name;
                        this.Name = string.Format("{0}[{1}]", str, Singleton<CTextManager>.GetInstance().GetText("PVP_NPC"));
                    }
                    if (this._heroIds.Count == 1)
                    {
                        this.CaptainId = heroCfgId;
                        if (Singleton<GamePlayerCenter>.instance.HostPlayerId == this.PlayerId)
                        {
                            object[] objArray2 = new object[] { this.CaptainId };
                            DebugHelper.CustomLog("Set Captain ID {0}", objArray2);
                        }
                    }
                }
            }
        }

        public void ClearHeroes()
        {
            if (Singleton<GamePlayerCenter>.instance.HostPlayerId == this.PlayerId)
            {
                object[] inParameters = new object[] { this.PlayerId, this.PlayerCamp };
                DebugHelper.CustomLog("Clear heros for playerid={0} camp={1}", inParameters);
            }
            this._heroIds.Clear();
            this._heroes.Clear();
            this.CaptainId = 0;
            this.Captain = new PoolObjHandle<ActorRoot>();
        }

        public void ConnectHeroActorRoot(ref PoolObjHandle<ActorRoot> hero)
        {
            if (hero == 0)
            {
                DebugHelper.Assert(false, "Failed ConnectHeroActorRoot, hero is null");
            }
            else if (!this._heroIds.Contains((uint) hero.handle.TheActorMeta.ConfigId))
            {
                object[] inParameters = new object[] { hero.handle.TheActorMeta.ConfigId, this.PlayerCamp, this.PlayerId };
                DebugHelper.Assert(false, "Failed ConnectHeroActorRoot, id is not valid = {0} Camp={1} playerid={2}", inParameters);
            }
            else
            {
                this._heroes.Add(hero);
                if (hero.handle.TheActorMeta.ConfigId == this.CaptainId)
                {
                    if (Singleton<GamePlayerCenter>.instance.HostPlayerId == this.PlayerId)
                    {
                        object[] objArray2 = new object[] { this.CaptainId, this.PlayerCamp, this.PlayerId };
                        DebugHelper.CustomLog("Set valid Captain with id = {0} Camp={1} player id={2}", objArray2);
                    }
                    this.Captain = hero;
                }
            }
        }

        public int GetAllHeroCombatEft()
        {
            int num = 0;
            for (int i = 0; i < this._heroes.Count; i++)
            {
                num += GetHeroCombatEft(this._heroes[i]);
            }
            return num;
        }

        public ReadonlyContext<PoolObjHandle<ActorRoot>> GetAllHeroes()
        {
            return new ReadonlyContext<PoolObjHandle<ActorRoot>>(this._heroes);
        }

        public ReadonlyContext<uint> GetAllHeroIds()
        {
            return new ReadonlyContext<uint>(this._heroIds);
        }

        private static int GetHeroCombatEft(PoolObjHandle<ActorRoot> actor)
        {
            if (actor == 0)
            {
                return 0;
            }
            int num = 0;
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
            ActorServerData actorData = new ActorServerData();
            if (actorDataProvider.GetActorServerData(ref actor.handle.TheActorMeta, ref actorData))
            {
                num += CHeroInfo.GetCombatEftByStarLevel((int) actorData.Level, (int) actorData.Star);
                num += CSkinInfo.GetCombatEft((uint) actorData.TheActorMeta.ConfigId, actorData.SkinId);
                ActorServerRuneData runeData = new ActorServerRuneData();
                for (int i = 0; i < 30; i++)
                {
                    if (actorDataProvider.GetActorServerRuneData(ref actor.handle.TheActorMeta, (ActorRunelSlot) i, ref runeData))
                    {
                        ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(runeData.RuneId);
                        if (dataByKey != null)
                        {
                            num += dataByKey.iCombatEft;
                        }
                    }
                }
            }
            return num;
        }

        public int GetHeroTeamPosIndex(uint heroCfgId)
        {
            return this._heroIds.IndexOf(heroCfgId);
        }

        public OperateMode GetOperateMode()
        {
            return this.useOperateMode;
        }

        public bool IsAllHeroesDead()
        {
            for (int i = 0; i < this._heroes.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = this._heroes[i];
                if (!handle.handle.ActorControl.IsDeadState)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsAtMyTeam(ref ActorMeta actorMeta)
        {
            return ((actorMeta.PlayerId == this.PlayerId) && this._heroIds.Contains((uint) actorMeta.ConfigId));
        }

        public bool IsMyTeamOutOfBattle()
        {
            for (int i = 0; i < this._heroes.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = this._heroes[i];
                if (handle.handle.ActorControl.IsInBattle)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsUseAdvanceCommonAttack()
        {
            return this.bUseAdvanceCommonAttack;
        }

        public void SetCaptain(uint configId)
        {
            <SetCaptain>c__AnonStorey65 storey = new <SetCaptain>c__AnonStorey65 {
                configId = configId
            };
            this.CaptainId = storey.configId;
            this.Captain = this._heroes.Find(new Predicate<PoolObjHandle<ActorRoot>>(storey.<>m__42));
            if (this.PlayerId == Singleton<GamePlayerCenter>.instance.HostPlayerId)
            {
                object[] objArray1 = new object[] { this.CaptainId };
                DebugHelper.CustomLog("Set Captain id={0}", objArray1);
            }
            object[] inParameters = new object[] { storey.configId };
            DebugHelper.Assert((bool) this.Captain, "Failed SetCaptain id={0}", inParameters);
        }

        public void SetOperateMode(OperateMode _mode)
        {
            if (_mode == OperateMode.DefaultMode)
            {
                if ((this.Captain != 0) && (this.Captain.handle.LockTargetAttackModeControl != null))
                {
                    this.Captain.handle.LockTargetAttackModeControl.ClearTargetID();
                }
            }
            else if ((this.Captain != 0) && (this.Captain.handle.DefaultAttackModeControl != null))
            {
                this.Captain.handle.DefaultAttackModeControl.ClearCommonAttackTarget();
            }
            this.useOperateMode = _mode;
            if (ActorHelper.IsHostCtrlActor(ref this.Captain))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, (CommonAttactType) _mode);
            }
        }

        public void SetUseAdvanceCommonAttack(bool bFlag)
        {
            this.bUseAdvanceCommonAttack = bFlag;
        }

        public int heroCount
        {
            get
            {
                return this._heroIds.Count;
            }
        }

        public int Level
        {
            get
            {
                return (int) this._level;
            }
            set
            {
                this._level = value;
            }
        }

        [CompilerGenerated]
        private sealed class <SetCaptain>c__AnonStorey65
        {
            internal uint configId;

            internal bool <>m__42(PoolObjHandle<ActorRoot> hero)
            {
                return (hero.handle.TheActorMeta.ConfigId == this.configId);
            }
        }
    }
}

