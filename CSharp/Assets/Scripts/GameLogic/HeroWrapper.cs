namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using behaviac;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class HeroWrapper : ObjWrapper
    {
        private bool autoRevived;
        private bool bDeadLevelUp;
        public bool bGodMode;
        private int contiDeadNum;
        private int contiKillNum;
        private HeroProficiency m_heroProficiency;
        private uint m_skinCfgId;
        private uint m_skinId;
        private uint[] m_talentArr = new uint[5];
        private int multiKillNum;
        private string skillEffectPath = string.Empty;

        public override void AddDisableSkillFlag(SkillSlotType _type)
        {
            base.AddDisableSkillFlag(_type);
            if (_type == SkillSlotType.SLOT_SKILL_COUNT)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (base.actor.SkillControl.DisableSkill[i] == 1)
                    {
                        DefaultSkillEventParam param = new DefaultSkillEventParam((SkillSlotType) i, 0, base.actorPtr);
                        Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                    }
                }
            }
            else if (base.actor.SkillControl.DisableSkill[(int) _type] == 1)
            {
                DefaultSkillEventParam param2 = new DefaultSkillEventParam(_type, 0, base.actorPtr);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, base.GetActor(), ref param2, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            base.actor.MovementComponent = base.actor.CreateLogicComponent<PlayerMovement>(base.actor);
            base.actor.MatHurtEffect = base.actor.CreateActorComponent<MaterialHurtEffect>(base.actor);
            base.actor.EffectControl = base.actor.CreateLogicComponent<EffectPlayComponent>(base.actor);
            base.actor.EquipComponent = base.actor.CreateLogicComponent<EquipComponent>(base.actor);
            base.actor.ShadowEffect = base.actor.CreateActorComponent<UpdateShadowPlane>(base.actor);
            VCollisionShape.InitActorCollision(base.actor);
            base.actor.DefaultAttackModeControl = base.actor.CreateLogicComponent<DefaultAttackMode>(base.actor);
            base.actor.LockTargetAttackModeControl = base.actor.CreateLogicComponent<LockTargetAttackMode>(base.actor);
            this.m_heroProficiency = new HeroProficiency();
            this.m_heroProficiency.Init(this);
        }

        public override void CmdCommonLearnSkill(IFrameCommand cmd)
        {
            FrameCommand<LearnSkillCommand> command = (FrameCommand<LearnSkillCommand>) cmd;
            if (Singleton<BattleLogic>.instance.IsMatchLearnSkillRule(base.actorPtr, (SkillSlotType) command.cmdData.bSlotType) && (base.actor.SkillControl.m_iSkillPoint > 0))
            {
                base.actor.SkillControl.m_iSkillPoint--;
            }
            else
            {
                return;
            }
            PoolObjHandle<ActorRoot> actorPtr = base.actorPtr;
            if (base.actor.SkillControl.m_iSkillPoint >= 0)
            {
                SkillSlot slot;
                actorPtr.handle.SkillControl.TryGetSkillSlot((SkillSlotType) command.cmdData.bSlotType, out slot);
                if (slot != null)
                {
                    int skillLevel = slot.GetSkillLevel();
                    if (skillLevel == command.cmdData.bSkillLevel)
                    {
                        slot.SetSkillLevel(skillLevel + 1);
                        Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", actorPtr, command.cmdData.bSlotType, (byte) (skillLevel + 1));
                    }
                }
            }
        }

        public override void Deactive()
        {
            if (this.m_heroProficiency != null)
            {
                this.m_heroProficiency.UnInit();
            }
            this.m_heroProficiency = null;
            base.Deactive();
        }

        protected override bool DoesApplyExposingRule()
        {
            return true;
        }

        protected override bool DoesIgnoreAlreadyLit()
        {
            return false;
        }

        public override void Fight()
        {
            base.Fight();
            if (ActorHelper.IsCaptainActor(ref this.actorPtr))
            {
                base.m_isControledByMan = true;
                base.m_isAutoAI = false;
            }
            else
            {
                base.m_isControledByMan = false;
                base.m_isAutoAI = true;
            }
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
            ActorServerData actorData = new ActorServerData();
            if ((actorDataProvider != null) && actorDataProvider.GetActorServerData(ref base.actor.TheActorMeta, ref actorData))
            {
                this.m_skinId = actorData.SkinId;
                this.m_skinCfgId = CSkinInfo.GetSkinCfgId((uint) base.actor.TheActorMeta.ConfigId, this.m_skinId);
                if (this.m_skinId != 0)
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin((uint) base.actor.TheActorMeta.ConfigId, this.m_skinId);
                    if ((heroSkin != null) && !string.IsNullOrEmpty(heroSkin.szSoundSwitchEvent))
                    {
                        Singleton<CSoundManager>.instance.PostEvent(heroSkin.szSoundSwitchEvent, base.actor.gameObject);
                    }
                }
            }
            this.SetSkillEffectPath();
            base.EnableRVO(false);
        }

        public int GetAdvanceSkinIndex()
        {
            if (((this.m_skinId > 0) && (base.actor.ValueComponent.actorSoulLevel > 1)) && (base.actor.CharInfo != null))
            {
                return base.actor.CharInfo.GetAdvanceSkinIndexByLevel(this.m_skinId, base.actor.ValueComponent.actorSoulLevel);
            }
            return 0;
        }

        public string GetLevelUpEftPath(int level)
        {
            if ((((this.m_skinId > 0) && (base.actor.ValueComponent.actorSoulLevel > 1)) && ((base.actor.CharInfo != null) && (base.actor.CharInfo.SkinPrefab != null))) && ((this.m_skinId >= 1) && (this.m_skinId <= base.actor.CharInfo.SkinPrefab.Length)))
            {
                SkinElement element = base.actor.CharInfo.SkinPrefab[(int) ((IntPtr) (this.m_skinId - 1))];
                if ((element != null) && (element.AdvanceSkin != null))
                {
                    for (int i = 0; i < element.AdvanceSkin.Length; i++)
                    {
                        if ((element.AdvanceSkin[i] != null) && (level == element.AdvanceSkin[i].Level))
                        {
                            StringBuilder builder = new StringBuilder(this.GetSkinEffectPath());
                            builder.AppendFormat("/levelUpEftPath{0}", i + 1);
                            return builder.ToString();
                        }
                    }
                }
            }
            return string.Empty;
        }

        public bool GetSkinCfgID(out uint skinCfgID)
        {
            skinCfgID = this.m_skinCfgId;
            return (this.m_skinId != 0);
        }

        public string GetSkinEffectPath()
        {
            return this.skillEffectPath;
        }

        public override string GetTypeName()
        {
            return "HeroWrapper";
        }

        public override bool IsBossOrHeroAutoAI()
        {
            return (base.myBehavior == ObjBehaviMode.State_AutoAI);
        }

        private void OnAdvanceSkin()
        {
            if ((this.m_skinId > 0) && (base.actor.ValueComponent.actorSoulLevel > 1))
            {
                string prefabPath = string.Empty;
                if ((base.actor.CharInfo != null) && base.actor.CharInfo.GetAdvanceSkinPrefabName(out prefabPath, this.m_skinId, base.actor.ValueComponent.actorSoulLevel, -1))
                {
                    bool isInit = false;
                    GameObject inActorMesh = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(prefabPath, false, SceneObjType.ActionRes, Vector3.zero, Quaternion.identity, out isInit);
                    if (inActorMesh != null)
                    {
                        inActorMesh.transform.SetParent(base.actor.gameObject.transform);
                        inActorMesh.transform.localPosition = Vector3.zero;
                        inActorMesh.transform.localRotation = Quaternion.identity;
                        base.actor.SetActorMesh(inActorMesh);
                    }
                }
            }
        }

        public void OnApChangeByMgcEffect()
        {
            int totalEftRatioByMgc = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalEftRatioByMgc;
            int totalOldEftRatioByMgc = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalOldEftRatioByMgc;
            base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalEftRatioByMgc = totalOldEftRatioByMgc;
            VFactor factor = new VFactor((long) base.actor.ValueComponent.actorHp, (long) base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
            base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalEftRatioByMgc = totalEftRatioByMgc;
            if ((totalOldEftRatioByMgc < totalEftRatioByMgc) && !base.actor.ActorControl.IsDeadState)
            {
                VFactor factor2 = factor * base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                base.actor.ValueComponent.actorHp = factor2.roundInt;
            }
        }

        protected override void OnDead()
        {
            base.OnDead();
            this.contiKillNum = 0;
            this.contiDeadNum++;
            base.actor.SkillControl.SkillUseCache.Clear();
            BaseAttackMode currentAttackMode = base.GetCurrentAttackMode();
            if (currentAttackMode != null)
            {
                currentAttackMode.OnDead();
            }
        }

        protected override void OnRevive()
        {
            VInt num3;
            VInt3 zero = VInt3.zero;
            VInt3 forward = VInt3.forward;
            if (this.autoRevived && this.m_reviveContext.bBaseRevive)
            {
                Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref base.actor.TheActorMeta, false, out zero, out forward);
                base.actor.EquipComponent.ResetHasLeftEquipBoughtArea();
            }
            else
            {
                Player player = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
                zero = player.Captain.handle.location;
                forward = player.Captain.handle.forward;
            }
            if (PathfindingUtility.GetGroundY(zero, out num3))
            {
                base.actor.groundY = num3;
                zero.y = num3.i;
            }
            base.actor.forward = forward;
            base.actor.location = zero;
            base.actor.ObjLinker.SetForward(forward, -1);
            base.OnRevive();
            if (!base.actor.ActorAgent.IsAutoAI())
            {
                base.SetObjBehaviMode(ObjBehaviMode.State_Revive);
            }
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            if (ownerPlayer != null)
            {
                Singleton<EventRouter>.instance.BroadCastEvent<Player>(EventID.PlayerReviveTime, ownerPlayer);
            }
            if (this.bDeadLevelUp)
            {
                this.OnAdvanceSkin();
                this.bDeadLevelUp = false;
            }
        }

        protected override void OnSoulLvlChange()
        {
            if ((this.m_skinId > 0) && (base.actor.ValueComponent.actorSoulLevel > 1))
            {
                if (base.IsDeadState)
                {
                    this.bDeadLevelUp = true;
                }
                else
                {
                    this.OnAdvanceSkin();
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            if (this.m_heroProficiency != null)
            {
                this.m_heroProficiency.UnInit();
            }
            this.m_heroProficiency = null;
            this.multiKillNum = 0;
            this.contiKillNum = 0;
            this.contiDeadNum = 0;
            this.bGodMode = false;
            this.autoRevived = false;
            for (byte i = 0; i < this.m_talentArr.Length; i = (byte) (i + 1))
            {
                this.m_talentArr[i] = 0;
            }
            this.m_skinCfgId = 0;
            this.m_skinId = 0;
            this.bDeadLevelUp = false;
            this.skillEffectPath = string.Empty;
        }

        public override void Prepare()
        {
            if (base.actor.ActorAgent.IsNeedToPlayBornAge() == EBTStatus.BT_SUCCESS)
            {
                base.actor.Visible = false;
            }
        }

        protected override int QueryExposeDuration()
        {
            return Horizon.QueryExposeDurationHero();
        }

        public override void Revive(bool auto)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            bool flag = (curLvelContext != null) && curLvelContext.IsMobaMode();
            if (flag == auto)
            {
                this.autoRevived = auto;
                base.Revive(auto);
            }
        }

        public override void RmvDisableSkillFlag(SkillSlotType _type)
        {
            base.RmvDisableSkillFlag(_type);
            if (_type == SkillSlotType.SLOT_SKILL_COUNT)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (base.actor.SkillControl.DisableSkill[i] == 0)
                    {
                        DefaultSkillEventParam param = new DefaultSkillEventParam((SkillSlotType) i, 0, base.actorPtr);
                        Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                    }
                }
            }
            else if (base.actor.SkillControl.DisableSkill[(int) _type] == 0)
            {
                DefaultSkillEventParam param2 = new DefaultSkillEventParam(_type, 0, base.actorPtr);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, base.GetActor(), ref param2, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        private void SetSkillEffectPath()
        {
            int configId = base.actor.TheStaticData.TheActorMeta.ConfigId;
            string heroNamePinYin = base.actor.TheStaticData.TheHeroOnlyInfo.HeroNamePinYin;
            string str2 = "prefab_skill_effects/hero_skill_effects/";
            StringBuilder builder = new StringBuilder(str2);
            builder.AppendFormat("{0}_{1}/{2}", configId, heroNamePinYin, this.m_skinCfgId);
            this.skillEffectPath = builder.ToString();
        }

        public override int TakeDamage(ref HurtDataInfo hurt)
        {
            if (this.bGodMode)
            {
                return 0;
            }
            if (((Singleton<BattleLogic>.instance.GetCurLvelContext() != null) && (hurt.atker != 0)) && (hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
            {
                OrganWrapper wrapper = hurt.atker.handle.AsOrgan();
                if (wrapper != null)
                {
                    int attackCounter = wrapper.GetAttackCounter(base.actorPtr);
                    if (attackCounter > 1)
                    {
                        int iContiAttakMax = (attackCounter - 1) * wrapper.cfgInfo.iContiAttakAdd;
                        if (iContiAttakMax > wrapper.cfgInfo.iContiAttakMax)
                        {
                            iContiAttakMax = wrapper.cfgInfo.iContiAttakMax;
                        }
                        hurt.adValue += iContiAttakMax;
                    }
                }
            }
            return base.TakeDamage(ref hurt);
        }

        public override void UpdateLogic(int nDelta)
        {
            base.actor.ActorAgent.UpdateLogic(nDelta);
            base.UpdateLogic(nDelta);
            this.m_heroProficiency.UpdateLogic(nDelta);
        }

        public override int CfgReviveCD
        {
            get
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                return Singleton<BattleLogic>.instance.dynamicProperty.GetDynamicReviveTime(curLvelContext.m_dynamicPropertyConfig, curLvelContext.m_baseReviveTime);
            }
        }

        public int ContiDeadNum
        {
            get
            {
                return this.contiDeadNum;
            }
            set
            {
                this.contiDeadNum = value;
            }
        }

        public int ContiKillNum
        {
            get
            {
                return this.contiKillNum;
            }
            set
            {
                this.contiKillNum = value;
            }
        }

        public uint[] GetTalentArr
        {
            get
            {
                return this.m_talentArr;
            }
        }

        public int MultiKillNum
        {
            get
            {
                return this.multiKillNum;
            }
            set
            {
                this.multiKillNum = value;
            }
        }
    }
}

