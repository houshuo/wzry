namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    public class CBattleDeadStat
    {
        private int m_baojunEnterCombatTime;
        private int m_baronEnterCombatTime;
        private int m_bigDragonEnterCombatTime;
        private int m_deadMonsterNum;
        private List<DeadRecord> m_deadRecordList = new List<DeadRecord>(0x20);
        public uint m_uiFBTime;

        public void AddEventHandler()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnDeadRecord));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.OnEnterCombat));
        }

        public void Clear()
        {
            this.m_deadMonsterNum = 0;
            this.m_deadRecordList.Clear();
            this.RemoveEventHandler();
        }

        public int GetAllMonsterDeadNum()
        {
            return this.m_deadMonsterNum;
        }

        public int GetBaronDeadCount()
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.actorType == ActorTypeDef.Actor_Type_Monster) && (record.actorSubType == 2)) && (record.actorSubSoliderType == 8))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetBaronDeadCount(COM_PLAYERCAMP killerCamp)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((killerCamp == record.killerCamp) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 2) && (record.actorSubSoliderType == 8)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetBaronDeadTime(int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.actorType == ActorTypeDef.Actor_Type_Monster) && (record.actorSubType == 2)) && (record.actorSubSoliderType == 8))
                {
                    if (num == index)
                    {
                        DeadRecord record2 = this.m_deadRecordList[i];
                        return record2.deadTime;
                    }
                    num++;
                }
            }
            return 0;
        }

        public int GetDeadNum(COM_PLAYERCAMP camp, ActorTypeDef actorType, int subType, int cfgId)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (camp == record.camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (actorType == record2.actorType)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (subType == record3.actorSubType)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (cfgId == record4.cfgId)
                            {
                                num++;
                            }
                        }
                    }
                }
            }
            return num;
        }

        public int GetDeadNumAtTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int subType, int deadTime)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((camp == record.camp) && (record.actorType == actorType)) && ((record.actorSubType == subType) && (record.deadTime < deadTime)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetDeadTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (actorType == record.actorType)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (camp == record2.camp)
                    {
                        if (num == index)
                        {
                            DeadRecord record3 = this.m_deadRecordList[i];
                            return record3.deadTime;
                        }
                        num++;
                    }
                }
            }
            return 0;
        }

        public byte GetDestroyTowerCount(COM_PLAYERCAMP killerCamp, int TowerType)
        {
            byte num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((killerCamp == record.killerCamp) && (record.actorType == ActorTypeDef.Actor_Type_Organ)) && (record.actorSubType == TowerType))
                {
                    num = (byte) (num + 1);
                }
            }
            return num;
        }

        public int GetDragonDeadTime(int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.actorType == ActorTypeDef.Actor_Type_Monster)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorSubType == 2)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubSoliderType != 9)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType != 7)
                            {
                                continue;
                            }
                        }
                        if (num == index)
                        {
                            DeadRecord record5 = this.m_deadRecordList[i];
                            return record5.deadTime;
                        }
                        num++;
                    }
                }
            }
            return 0;
        }

        public int GetHeroDeadAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Hero)) && (record.deadTime < deadTime))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetKillBlueBaNumAtTime(uint playerID, int deadTime)
        {
            return this.GetKillSpecialMonsterNumAtTime(playerID, deadTime, 10);
        }

        public int GetKillDragonNum()
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.actorType == ActorTypeDef.Actor_Type_Monster)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorSubType == 2)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubSoliderType != 9)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType != 7)
                            {
                                continue;
                            }
                        }
                        num++;
                    }
                }
            }
            return num;
        }

        public int GetKillDragonNum(COM_PLAYERCAMP killerCamp)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (killerCamp == record.killerCamp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorType == ActorTypeDef.Actor_Type_Monster)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubType == 2)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType != 9)
                            {
                                DeadRecord record5 = this.m_deadRecordList[i];
                                if (record5.actorSubSoliderType != 7)
                                {
                                    continue;
                                }
                            }
                            num++;
                        }
                    }
                }
            }
            return num;
        }

        public int GetKillDragonNumAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && (record.actorSubType == 2)) && ((record.actorSubSoliderType == 9) || (record.actorSubSoliderType == 7))) && (record.deadTime < deadTime))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetKillRedBaNumAtTime(uint playerID, int deadTime)
        {
            return this.GetKillSpecialMonsterNumAtTime(playerID, deadTime, 11);
        }

        public int GetKillSpecialMonsterNumAtTime(uint playerID, int deadTime, byte bySoldierType)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if ((((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 2) && (record.actorSubSoliderType == bySoldierType))) && (record.deadTime < deadTime))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetMonsterDeadAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 2) && (record.deadTime < deadTime)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetOrganTimeByOrder(int iOrder)
        {
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.actorType == ActorTypeDef.Actor_Type_Organ)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.iOrder == iOrder)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        return record3.deadTime;
                    }
                }
            }
            return 0;
        }

        public DeadRecord GetRecordAtIndex(COM_PLAYERCAMP camp, ActorTypeDef actorType, byte actorSubType, byte actorSubSoliderType, int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.camp == camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorType == actorType)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubType == actorSubType)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType == actorSubSoliderType)
                            {
                                if (num == index)
                                {
                                    return this.m_deadRecordList[i];
                                }
                                num++;
                            }
                        }
                    }
                }
            }
            return new DeadRecord();
        }

        public int GetSoldierDeadAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 1) && (record.deadTime < deadTime)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetTotalNum(COM_PLAYERCAMP camp, ActorTypeDef actorType, byte actorSubType, byte actorSubSoliderType)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.camp == camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorType == actorType)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubType == actorSubType)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType == actorSubSoliderType)
                            {
                                num++;
                            }
                        }
                    }
                }
            }
            return num;
        }

        private void OnDeadRecord(ref GameDeadEventParam prm)
        {
            if (!prm.bImmediateRevive)
            {
                PoolObjHandle<ActorRoot> src = prm.src;
                PoolObjHandle<ActorRoot> orignalAtker = prm.orignalAtker;
                if ((src != 0) && (orignalAtker != 0))
                {
                    if (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        this.m_deadRecordList.Add(new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int) Singleton<FrameSynchr>.instance.LogicFrameTick, orignalAtker.handle.TheActorMeta.ActorCamp, orignalAtker.handle.TheActorMeta.PlayerId));
                        if (this.m_uiFBTime == 0)
                        {
                            this.m_uiFBTime = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
                        }
                    }
                    else if (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                    {
                        DeadRecord item = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int) Singleton<FrameSynchr>.instance.LogicFrameTick, orignalAtker.handle.TheActorMeta.ActorCamp, orignalAtker.handle.TheActorMeta.PlayerId);
                        if (src.handle.ActorControl != null)
                        {
                            item.actorSubType = src.handle.ActorControl.GetActorSubType();
                            item.actorSubSoliderType = src.handle.ActorControl.GetActorSubSoliderType();
                            if (item.actorSubType == 2)
                            {
                                if (item.actorSubType == 7)
                                {
                                    item.fightTime = ((int) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_baojunEnterCombatTime;
                                }
                                else if (item.actorSubType == 8)
                                {
                                    item.fightTime = ((int) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_baronEnterCombatTime;
                                }
                                else if (item.actorSubType == 9)
                                {
                                    item.fightTime = ((int) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_bigDragonEnterCombatTime;
                                }
                            }
                        }
                        this.m_deadRecordList.Add(item);
                        this.m_deadMonsterNum++;
                    }
                    else if ((src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ) && (((src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4)) || (src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)))
                    {
                        DeadRecord record2 = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int) Singleton<FrameSynchr>.instance.LogicFrameTick, orignalAtker.handle.TheActorMeta.ActorCamp, orignalAtker.handle.TheActorMeta.PlayerId);
                        if (src.handle.ObjLinker != null)
                        {
                            record2.iOrder = src.handle.ObjLinker.BattleOrder;
                            record2.actorSubType = (byte) src.handle.TheStaticData.TheOrganOnlyInfo.OrganType;
                        }
                        this.m_deadRecordList.Add(record2);
                    }
                }
            }
        }

        private void OnEnterCombat(ref DefaultGameEventParam prm)
        {
            if (((prm.src != 0) && (prm.src.handle.ActorControl != null)) && ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (prm.src.handle.ActorControl.GetActorSubType() == 2)))
            {
                if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 7)
                {
                    this.m_baojunEnterCombatTime = (int) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
                else if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 8)
                {
                    this.m_baronEnterCombatTime = (int) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
                else if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 9)
                {
                    this.m_bigDragonEnterCombatTime = (int) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
            }
        }

        public void RemoveEventHandler()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnDeadRecord));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.OnEnterCombat));
        }

        public void StartRecord()
        {
            this.Clear();
            this.AddEventHandler();
        }
    }
}

