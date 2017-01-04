namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class CallMonsterTick : TickEvent
    {
        public bool bCopyedHeroInfo;
        public ECampType CampType;
        public int ConfigID;
        public bool Invincible;
        public int LifeTime;
        private static readonly int MaxLevel = 6;
        public bool Moveable;
        [ObjectTemplate(new System.Type[] {  })]
        public int TargetId = -1;
        private GameObject wayPoint;
        public int WayPointId = -1;

        private void ApplyMonsterAdditive(ref PoolObjHandle<ActorRoot> OrignalHost, ref PoolObjHandle<ActorRoot> Monster, ref ResCallMonster CallMonsterCfg)
        {
            this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT, (int) CallMonsterCfg.dwAddAttack, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT));
            this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT, (int) CallMonsterCfg.dwAddMagic, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT));
            this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT, (int) CallMonsterCfg.dwAddArmor, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT));
            this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT, (int) CallMonsterCfg.dwAddResistant, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT));
            this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, (int) CallMonsterCfg.dwAddHp, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP));
            if ((CallMonsterCfg.bAddType & 4) != 0)
            {
                Monster.handle.ValueComponent.actorHp = (Monster.handle.ValueComponent.actorHpTotal * OrignalHost.handle.ValueComponent.actorHp) / OrignalHost.handle.ValueComponent.actorHpTotal;
                Monster.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].addValue = OrignalHost.handle.ValueComponent.actorEpTotal;
                Monster.handle.ValueComponent.actorEp = OrignalHost.handle.ValueComponent.actorEp;
            }
            else
            {
                Monster.handle.ValueComponent.actorHp = Monster.handle.ValueComponent.actorHpTotal;
            }
            this.ApplyProperty(ref Monster, RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, (int) CallMonsterCfg.dwAddSpeed, this.GetAddValue(ref OrignalHost, ref Monster, ref CallMonsterCfg, RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD));
        }

        private void ApplyProperty(ref PoolObjHandle<ActorRoot> Monster, RES_FUNCEFT_TYPE InType, int InValue, int InBase)
        {
            int num = (int) ((InBase * InValue) / 10000.0);
            Monster.handle.ValueComponent.mActorValue[InType].addValue += num;
        }

        public override BaseEvent Clone()
        {
            CallMonsterTick tick = ClassObjPool<CallMonsterTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            CallMonsterTick tick = src as CallMonsterTick;
            this.TargetId = tick.TargetId;
            this.WayPointId = tick.WayPointId;
            this.ConfigID = tick.ConfigID;
            this.LifeTime = tick.LifeTime;
            this.CampType = tick.CampType;
            this.Invincible = tick.Invincible;
            this.Moveable = tick.Moveable;
            this.bCopyedHeroInfo = tick.bCopyedHeroInfo;
        }

        private int GetAddValue(ref PoolObjHandle<ActorRoot> OrignalHost, ref PoolObjHandle<ActorRoot> Monster, ref ResCallMonster CallMonsterCfg, RES_FUNCEFT_TYPE type)
        {
            int num = 0;
            byte bAddType = CallMonsterCfg.bAddType;
            if ((CallMonsterCfg.bAddType & 1) != 0)
            {
                num += OrignalHost.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
            }
            if ((CallMonsterCfg.bAddType & 2) != 0)
            {
                num += OrignalHost.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
            }
            if ((CallMonsterCfg.bAddType & 4) != 0)
            {
                num += OrignalHost.handle.ValueComponent.mActorValue[type].totalValue;
            }
            return num;
        }

        public override void OnUse()
        {
            base.OnUse();
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.TargetId);
            if (actorHandle == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                this.wayPoint = _action.GetGameObject(this.WayPointId);
                this.SpawnMonster(_action, ref actorHandle);
            }
        }

        private COM_PLAYERCAMP SelectCamp(ref PoolObjHandle<ActorRoot> InActor)
        {
            if (this.CampType == ECampType.ECampType_Self)
            {
                return InActor.handle.TheActorMeta.ActorCamp;
            }
            if (this.CampType == ECampType.ECampType_Hostility)
            {
                switch (InActor.handle.TheActorMeta.ActorCamp)
                {
                    case COM_PLAYERCAMP.COM_PLAYERCAMP_1:
                        return COM_PLAYERCAMP.COM_PLAYERCAMP_2;

                    case COM_PLAYERCAMP.COM_PLAYERCAMP_2:
                        return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                }
            }
            return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
        }

        private int SelectLevel(ref PoolObjHandle<ActorRoot> HostActor, ref ResCallMonster CallMonsterCfg, ref SkillUseContext SkillContext)
        {
            if (CallMonsterCfg.bDependencyType == 1)
            {
                return HostActor.handle.ValueComponent.actorSoulLevel;
            }
            if (CallMonsterCfg.bDependencyType == 2)
            {
                return HostActor.handle.SkillControl.SkillSlotArray[(int) SkillContext.SlotType].GetSkillLevel();
            }
            return 0;
        }

        private void SpawnMonster(AGE.Action _action, ref PoolObjHandle<ActorRoot> tarActor)
        {
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            if (((refParamObject == null) || (refParamObject.Originator == 0)) || (refParamObject.Originator.handle.ActorControl == null))
            {
                object[] inParameters = new object[] { _action.name };
                DebugHelper.Assert(false, "Failed find orignal actor of this skill. action:{0}", inParameters);
            }
            else if (!refParamObject.Originator.handle.ActorControl.IsDeadState)
            {
                DebugHelper.Assert(refParamObject.Originator.handle.ValueComponent != null, "ValueComponent is null");
                ResCallMonster dataByKey = GameDataMgr.callMonsterDatabin.GetDataByKey((long) this.ConfigID);
                object[] objArray2 = new object[] { this.ConfigID, _action.name };
                DebugHelper.Assert(dataByKey != null, "Failed find call monster config id:{0} action:{1}", objArray2);
                if (dataByKey != null)
                {
                    int diffLevel = Math.Min(MaxLevel, this.SelectLevel(ref refParamObject.Originator, ref dataByKey, ref refParamObject));
                    ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo((int) dataByKey.dwMonsterID, diffLevel);
                    object[] objArray3 = new object[] { dataByKey.dwMonsterID, diffLevel, _action.name };
                    DebugHelper.Assert(dataCfgInfo != null, "Failed find monster id={0} diff={1} action:{2}", objArray3);
                    if (dataCfgInfo != null)
                    {
                        string fullPathInResources = StringHelper.UTF8BytesToString(ref dataCfgInfo.szCharacterInfo) + ".asset";
                        CActorInfo content = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(CActorInfo), enResourceType.BattleScene, false, false).m_content as CActorInfo;
                        if (content != null)
                        {
                            ActorMeta actorMeta = new ActorMeta {
                                ConfigId = (int) dataByKey.dwMonsterID,
                                ActorType = ActorTypeDef.Actor_Type_Monster,
                                ActorCamp = this.SelectCamp(ref refParamObject.Originator),
                                EnCId = (CrypticInt32) dataByKey.dwMonsterID,
                                Difficuty = (byte) diffLevel,
                                SkinID = refParamObject.Originator.handle.TheActorMeta.SkinID
                            };
                            VInt3 location = tarActor.handle.location;
                            VInt3 forward = tarActor.handle.forward;
                            if (!PathfindingUtility.IsValidTarget(refParamObject.Originator.handle, location))
                            {
                                location = refParamObject.Originator.handle.location;
                                forward = refParamObject.Originator.handle.forward;
                            }
                            PoolObjHandle<ActorRoot> monster = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref actorMeta, location, forward, false, true);
                            if (monster != 0)
                            {
                                monster.handle.InitActor();
                                this.ApplyMonsterAdditive(ref refParamObject.Originator, ref monster, ref dataByKey);
                                MonsterWrapper actorControl = monster.handle.ActorControl as MonsterWrapper;
                                if (actorControl != null)
                                {
                                    actorControl.SetHostActorInfo(ref refParamObject.Originator, refParamObject.SlotType, this.bCopyedHeroInfo);
                                    if (this.wayPoint != null)
                                    {
                                        actorControl.AttackAlongRoute(this.wayPoint.GetComponent<WaypointsHolder>());
                                    }
                                    if (this.LifeTime > 0)
                                    {
                                        actorControl.LifeTime = this.LifeTime;
                                    }
                                }
                                monster.handle.PrepareFight();
                                Singleton<GameObjMgr>.instance.AddActor(monster);
                                monster.handle.StartFight();
                                monster.handle.ObjLinker.Invincible = this.Invincible;
                                monster.handle.ObjLinker.CanMovable = this.Moveable;
                                monster.handle.Visible = refParamObject.Originator.handle.Visible;
                                monster.handle.ValueComponent.actorSoulLevel = refParamObject.Originator.handle.ValueComponent.actorSoulLevel;
                                refParamObject.Originator.handle.ValueComponent.AddSoulExp(0, false, AddSoulType.Other);
                            }
                        }
                    }
                }
            }
        }
    }
}

