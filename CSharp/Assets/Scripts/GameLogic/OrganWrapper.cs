namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using Pathfinding;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class OrganWrapper : ObjWrapper
    {
        private GameObject[] _aroundEffects;
        private int _aroundEnemyMonsterCount;
        private LineRenderer _attackLinker;
        private int _attackOneTargetCounter;
        private int _attackOneTargetCounterLast;
        private PoolObjHandle<ActorRoot> _myLastTarget = new PoolObjHandle<ActorRoot>();
        private static Vector3 _myselfHeightVec = new Vector3(0f, 3f, 0f);
        private AreaCheck attackAreaCheck;
        public const string AttackLinkerPrefab = "AttackLinker";
        private OrganHitEffect HitEffect;
        private bool m_bFirstAttacked;
        [NonSerialized, HideInInspector]
        private NavmeshCut navmeshCut;
        private int nOutCombatHpRecoveryTick;
        public const string OrganAroundEffectHomePath = "Prefab_Characters/Prefab_Organ/AroundEffect/";

        private bool ActorMarkFilter(ref PoolObjHandle<ActorRoot> _inActor)
        {
            if (((_inActor != 0) && (((base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 2)) || (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4))) && (!_inActor.handle.ActorControl.IsDeadState && !base.actor.ActorControl.IsDeadState))
            {
                SkillSlot skillSlot = base.actor.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_0);
                if (skillSlot != null)
                {
                    long iMaxAttackDistance = (long) skillSlot.SkillObj.cfgData.iMaxAttackDistance;
                    VInt3 num3 = base.actor.location - _inActor.handle.location;
                    if (num3.sqrMagnitudeLong2D <= (iMaxAttackDistance * iMaxAttackDistance))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ActorMarkProcess(PoolObjHandle<ActorRoot> _inActor, AreaCheck.ActorAction _action)
        {
            if (_inActor != 0)
            {
                if (_inActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    if (_action == AreaCheck.ActorAction.Enter)
                    {
                        if (++this._aroundEnemyMonsterCount == 1)
                        {
                            ValueDataInfo info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT] - base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
                            info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT] - base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
                        }
                    }
                    else if ((_action == AreaCheck.ActorAction.Leave) && (--this._aroundEnemyMonsterCount == 0))
                    {
                        ValueDataInfo info2 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
                        info2 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
                    }
                }
                else if (_inActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    if (_action == AreaCheck.ActorAction.Enter)
                    {
                        _inActor.handle.HorizonMarker.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.ShowMark.Skill, 1);
                    }
                    else if (_action == AreaCheck.ActorAction.Leave)
                    {
                        _inActor.handle.HorizonMarker.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.ShowMark.Skill, -1);
                    }
                }
            }
        }

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            base.actor.isMovable = false;
            base.actor.isRotatable = false;
            VCollisionShape.InitActorCollision(base.actor);
            this.navmeshCut = base.gameObject.GetComponent<NavmeshCut>();
            if (this.navmeshCut != null)
            {
                this.navmeshCut.enabled = true;
            }
            this._aroundEffects = new GameObject[3];
            this.attackAreaCheck = new AreaCheck(new ActorFilterDelegate(this.ActorMarkFilter), new AreaCheck.ActorProcess(this.ActorMarkProcess), Singleton<GameObjMgr>.GetInstance().GetCampActors(base.actor.GiveMyEnemyCamp()));
            this._aroundEnemyMonsterCount = 0;
            this.cfgInfo = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(base.actor.TheActorMeta.ConfigId);
            if (this.cfgInfo != null)
            {
                base.actorSubType = this.cfgInfo.bOrganType;
            }
        }

        private void DrawAttackLinker()
        {
            if (((base.myTarget != 0) && !base.myTarget.handle.ActorControl.IsDeadState) && !base.actor.ActorControl.IsDeadState)
            {
                if (null == this._attackLinker)
                {
                    GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Prefab_Organ/AroundEffect/AttackLinker", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
                    if (content != null)
                    {
                        this._attackLinker = ((GameObject) UnityEngine.Object.Instantiate(content)).GetComponent<LineRenderer>();
                        if (this._attackLinker != null)
                        {
                            this._attackLinker.transform.SetParent(base.actor.gameObject.transform);
                            this._attackLinker.SetVertexCount(2);
                            this._attackLinker.useWorldSpace = true;
                            this._attackLinker.SetPosition(0, base.actor.gameObject.transform.position + _myselfHeightVec);
                        }
                    }
                }
                if (null != this._attackLinker)
                {
                    float y = base.myTarget.handle.CharInfo.iBulletHeight * 0.001f;
                    this._attackLinker.SetPosition(1, base.myTarget.handle.gameObject.transform.position + new Vector3(0f, y, 0f));
                    if (!this._attackLinker.gameObject.activeSelf)
                    {
                        this._attackLinker.gameObject.SetActive(true);
                    }
                }
            }
            else if ((null != this._attackLinker) && this._attackLinker.gameObject.activeSelf)
            {
                this._attackLinker.gameObject.SetActive(false);
            }
        }

        public override void Fight()
        {
            base.Fight();
            this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
            this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit);
            FrameCommand<AttackPositionCommand> cmd = FrameCommandFactory.CreateFrameCommand<AttackPositionCommand>();
            cmd.cmdId = 1;
            cmd.cmdData.WorldPos = base.actor.location;
            base.CmdAttackMoveToDest(cmd, base.actor.location);
            if (this.isTower)
            {
                this.HitEffect.Reset(this);
            }
            base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE].baseValue = base.actor.TheStaticData.TheOrganOnlyInfo.PhyArmorHurtRate;
            this._aroundEnemyMonsterCount = 0;
            ValueDataInfo info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
            info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
            if (base.actor.HorizonMarker != null)
            {
                base.actor.HorizonMarker.SightRadius = base.actor.TheStaticData.TheOrganOnlyInfo.HorizonRadius;
            }
        }

        public int GetAttackCounter(PoolObjHandle<ActorRoot> inActor)
        {
            int num = (this._attackOneTargetCounter <= 0) ? (!(inActor == this._myLastTarget) ? 0 : this._attackOneTargetCounterLast) : this._attackOneTargetCounter;
            this._attackOneTargetCounterLast = 0;
            return num;
        }

        public override string GetTypeName()
        {
            return "OrganWrapper";
        }

        public override void Init()
        {
            base.Init();
        }

        public override void LateUpdate(int delta)
        {
            try
            {
                this.DrawAttackLinker();
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "exception in DrawAttackLinker:{0}, \n {1}", inParameters);
            }
        }

        protected override void OnDead()
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("Set_Theme", null);
            if (base.actor.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0)
            {
                SoldierRegion soldierRegion = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldierRegion(base.actor.GiveMyEnemyCamp(), base.actor.TheStaticData.TheOrganOnlyInfo.AttackRouteID);
                if (soldierRegion != null)
                {
                    soldierRegion.SwitchWave(base.actor.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier);
                }
            }
            base.OnDead();
        }

        protected override void OnHpChange()
        {
            if (this.isTower)
            {
                this.HitEffect.OnHpChanged(this);
            }
            base.OnHpChange();
        }

        public override void OnMyTargetSwitch()
        {
            this._myLastTarget = base.myTarget;
            this._attackOneTargetCounterLast = this._attackOneTargetCounter;
            this._attackOneTargetCounter = 0;
        }

        protected override void OnRevive()
        {
            base.OnRevive();
            base.EnableRVO(true);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.navmeshCut = null;
            this._attackOneTargetCounter = 0;
            this._attackOneTargetCounterLast = 0;
            this._myLastTarget = new PoolObjHandle<ActorRoot>();
            this.nOutCombatHpRecoveryTick = 0;
            this._aroundEffects = null;
            this._attackLinker = null;
            _myselfHeightVec = new Vector3(0f, 3f, 0f);
            this.attackAreaCheck = null;
            this._aroundEnemyMonsterCount = 0;
            this.m_bFirstAttacked = false;
            this.HitEffect = new OrganHitEffect();
            this.cfgInfo = null;
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            for (int i = 0; i < 3; i++)
            {
                preloadTab.AddParticle("Prefab_Characters/Prefab_Organ/AroundEffect/" + ((OrganAroundEffect) i).ToString());
            }
            preloadTab.AddParticle("Prefab_Characters/Prefab_Organ/AroundEffect/AttackLinker");
        }

        public override bool RealUseSkill(SkillSlotType InSlot)
        {
            if (base.RealUseSkill(InSlot))
            {
                this._attackOneTargetCounter++;
                this._attackOneTargetCounterLast = 0;
                return true;
            }
            return false;
        }

        public override void Revive(bool auto)
        {
            base.Revive(auto);
            base.SetObjBehaviMode(ObjBehaviMode.Attack_Move);
        }

        public void ShowAroundEffect(OrganAroundEffect oae, bool showOrHide, bool hideOther, float scale = 1f)
        {
            if (this._aroundEffects != null)
            {
                int index = (int) oae;
                GameObject obj2 = this._aroundEffects[index];
                if (showOrHide && (null == obj2))
                {
                    GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Prefab_Organ/AroundEffect/" + oae.ToString(), typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
                    if (content != null)
                    {
                        obj2 = (GameObject) UnityEngine.Object.Instantiate(content);
                        DebugHelper.Assert(obj2 != null);
                        if (obj2 != null)
                        {
                            Transform transform = obj2.transform;
                            if (transform != null)
                            {
                                transform.SetParent(base.actor.gameObject.transform);
                                transform.localPosition = Vector3.zero;
                                transform.localScale = Vector3.one;
                                transform.localRotation = Quaternion.identity;
                            }
                            ParticleScaler[] componentsInChildren = obj2.GetComponentsInChildren<ParticleScaler>(true);
                            for (int i = 0; i < componentsInChildren.Length; i++)
                            {
                                componentsInChildren[i].particleScale = scale;
                                componentsInChildren[i].CheckAndApplyScale();
                            }
                        }
                        this._aroundEffects[index] = obj2;
                    }
                }
                if ((null != obj2) && (obj2.activeSelf != showOrHide))
                {
                    obj2.SetActive(showOrHide);
                    if (showOrHide && (oae == OrganAroundEffect.HostPlayerInAndHit))
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_Prompt_fangyuta_atk", null);
                    }
                }
                if (hideOther)
                {
                    for (int j = 0; j < this._aroundEffects.Length; j++)
                    {
                        if (((j != index) && (null != this._aroundEffects[j])) && this._aroundEffects[j].activeSelf)
                        {
                            this._aroundEffects[j].SetActive(false);
                        }
                    }
                }
            }
        }

        public override int TakeDamage(ref HurtDataInfo hurt)
        {
            if (!this.m_bFirstAttacked)
            {
                this.m_bFirstAttacked = true;
                if (hurt.atker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    Singleton<EventRouter>.instance.BroadCastEvent<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, COM_PLAYERCAMP.COM_PLAYERCAMP_2);
                }
                else if (hurt.atker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                {
                    Singleton<EventRouter>.instance.BroadCastEvent<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, COM_PLAYERCAMP.COM_PLAYERCAMP_1);
                }
            }
            if (((this.cfgInfo.iBlockHeroAtkDamageMSec >= Singleton<FrameSynchr>.instance.LogicFrameTick) && (hurt.atker != 0)) && ((hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (hurt.atkSlot == SkillSlotType.SLOT_SKILL_0)))
            {
                hurt.iReduceDamage = this.cfgInfo.iBlockHeroAtkDamage;
            }
            else
            {
                hurt.iReduceDamage = 0;
            }
            return base.TakeDamage(ref hurt);
        }

        public override void UpdateLogic(int nDelta)
        {
            base.actor.ActorAgent.UpdateLogic(nDelta);
            base.UpdateLogic(nDelta);
            if (this.attackAreaCheck != null)
            {
                this.attackAreaCheck.UpdateLogic(base.actor.ObjID);
            }
            if (base.IsInBattle)
            {
                this.nOutCombatHpRecoveryTick = 0;
            }
            else
            {
                this.nOutCombatHpRecoveryTick += nDelta;
                if (this.nOutCombatHpRecoveryTick >= 0x3e8)
                {
                    int nAddHp = (this.cfgInfo.iOutBattleHPAdd * base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue) / 0x2710;
                    base.ReviveHp(nAddHp);
                    this.nOutCombatHpRecoveryTick -= 0x3e8;
                }
            }
        }

        public ResOrganCfgInfo cfgInfo { get; private set; }

        public override int CfgReviveCD
        {
            get
            {
                return 0x7fffffff;
            }
        }

        public bool isTower
        {
            get
            {
                return ((base.actor != null) && ((base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4)));
            }
        }
    }
}

