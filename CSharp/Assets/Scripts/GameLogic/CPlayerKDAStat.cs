namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CPlayerKDAStat
    {
        public CCampKDAStat m_CampKdaStat;
        private DictionaryView<uint, PlayerKDA> m_PlayerKDA = new DictionaryView<uint, PlayerKDA>();

        public void DumpDebugInfo()
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                Debug.Log(string.Format("PlayerKDA Id {0}", current.Key));
            }
        }

        public void GenerateStatData()
        {
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xbc).dwConfValue;
            int hurtToEnemy = 0;
            int hurtTakenByEnemy = 0;
            int numAssist = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xbd).dwConfValue;
            int totalCoin = 0;
            int numKillOrgan = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 190).dwConfValue;
            int hurtToHero = 0;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (dwConfValue < enumerator2.Current.numKill)
                    {
                        dwConfValue = enumerator2.Current.numKill;
                    }
                    if (hurtToEnemy < enumerator2.Current.hurtToEnemy)
                    {
                        hurtToEnemy = enumerator2.Current.hurtToEnemy;
                    }
                    if (hurtTakenByEnemy < enumerator2.Current.hurtTakenByEnemy)
                    {
                        hurtTakenByEnemy = enumerator2.Current.hurtTakenByEnemy;
                    }
                    if (numAssist < enumerator2.Current.numAssist)
                    {
                        numAssist = enumerator2.Current.numAssist;
                    }
                    if (totalCoin < enumerator2.Current.TotalCoin)
                    {
                        totalCoin = enumerator2.Current.TotalCoin;
                    }
                    if (numKillOrgan < enumerator2.Current.numKillOrgan)
                    {
                        numKillOrgan = enumerator2.Current.numKillOrgan;
                    }
                    if (hurtToHero < enumerator2.Current.hurtToHero)
                    {
                        hurtToHero = enumerator2.Current.hurtToHero;
                    }
                }
            }
            enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator3 = pair2.Value.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    if (dwConfValue == enumerator3.Current.numKill)
                    {
                        enumerator3.Current.bKillMost = true;
                    }
                    if (hurtToEnemy == enumerator3.Current.hurtToEnemy)
                    {
                        enumerator3.Current.bHurtMost = true;
                    }
                    if (hurtTakenByEnemy == enumerator3.Current.hurtTakenByEnemy)
                    {
                        enumerator3.Current.bHurtTakenMost = true;
                    }
                    if (numAssist == enumerator3.Current.numAssist)
                    {
                        enumerator3.Current.bAsssistMost = true;
                    }
                    if (totalCoin == enumerator3.Current.TotalCoin)
                    {
                        enumerator3.Current.bGetCoinMost = true;
                    }
                    if (numKillOrgan == enumerator3.Current.numKillOrgan)
                    {
                        enumerator3.Current.bKillOrganMost = true;
                    }
                    if (hurtToHero == enumerator3.Current.hurtToHero)
                    {
                        enumerator3.Current.bHurtToHeroMost = true;
                    }
                }
            }
            if (this.m_CampKdaStat == null)
            {
                this.m_CampKdaStat = new CCampKDAStat();
                if (this.m_CampKdaStat != null)
                {
                    this.m_CampKdaStat.Initialize(this.m_PlayerKDA);
                }
            }
        }

        public DictionaryView<uint, PlayerKDA>.Enumerator GetEnumerator()
        {
            return this.m_PlayerKDA.GetEnumerator();
        }

        public PlayerKDA GetHostKDA()
        {
            PlayerKDA rkda;
            this.m_PlayerKDA.TryGetValue(Singleton<GamePlayerCenter>.instance.HostPlayerId, out rkda);
            return rkda;
        }

        public PlayerKDA GetPlayerKDA(uint playerId)
        {
            PlayerKDA rkda;
            this.m_PlayerKDA.TryGetValue(playerId, out rkda);
            return rkda;
        }

        public int GetTeamAssistNum(COM_PLAYERCAMP camp)
        {
            int num = 0;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                if (current.Value.PlayerCamp == camp)
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    num += pair2.Value.numAssist;
                }
            }
            return num;
        }

        public int GetTeamDeadNum(COM_PLAYERCAMP camp)
        {
            int num = 0;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                if (current.Value.PlayerCamp == camp)
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    num += pair2.Value.numDead;
                }
            }
            return num;
        }

        public float GetTeamKDA(COM_PLAYERCAMP camp)
        {
            float num = 0f;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                if (current.Value.PlayerCamp == camp)
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    num += pair2.Value.KDAValue;
                }
            }
            return num;
        }

        public int GetTeamKillNum(COM_PLAYERCAMP camp)
        {
            int num = 0;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                if (current.Value.PlayerCamp == camp)
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    num += pair2.Value.numKill;
                }
            }
            return num;
        }

        private void initialize()
        {
            List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.instance.GetAllPlayers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Player current = enumerator.Current;
                if (current != null)
                {
                    PlayerKDA rkda = new PlayerKDA();
                    rkda.initialize(current);
                    this.m_PlayerKDA.Add(current.PlayerId, rkda);
                }
            }
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, sbyte, uint>("HeroLearnTalent", new Action<PoolObjHandle<ActorRoot>, sbyte, uint>(this.OnHeroLearnTalent));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this.OnHeroBattleEquipChange));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnActorOdyssey));
            Singleton<GameEventSys>.instance.AddEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, new RefAction<SkillChooseTargetEventParam>(this.OnActorBeChosen));
            Singleton<GameEventSys>.instance.AddEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, new RefAction<SkillChooseTargetEventParam>(this.OnHitTrigger));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorBattleCoinChanged));
            Singleton<GameEventSys>.instance.AddEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
        }

        public void OnActorBattleCoinChanged(PoolObjHandle<ActorRoot> actor, int changeValue, int currentValue, bool isIncome)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (actor == enumerator2.Current.actorHero)
                    {
                        enumerator2.Current.OnActorBattleCoinChanged(actor, changeValue, currentValue, isIncome);
                        Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
                        return;
                    }
                }
            }
        }

        public void OnActorBeChosen(ref SkillChooseTargetEventParam prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.OnActorBeChosen(ref prm);
                }
            }
        }

        public void OnActorDamage(ref HurtEventResultInfo prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.OnActorDamage(ref prm);
                }
            }
        }

        public void OnActorDead(ref GameDeadEventParam prm)
        {
            if (!prm.bImmediateRevive)
            {
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        enumerator2.Current.OnActorDead(ref prm);
                    }
                }
                if ((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                {
                    Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
                    Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD);
                }
            }
        }

        public void OnActorDoubleKill(ref DefaultGameEventParam prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if ((prm.atker != 0) && (prm.atker == enumerator2.Current.actorHero))
                    {
                        enumerator2.Current.OnActorDoubleKill(ref prm);
                        return;
                    }
                }
            }
        }

        private void OnActorHemophagia(ref HemophagiaEventResultInfo hri)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.OnActorHemophagia(ref hri);
                }
            }
        }

        public void OnActorOdyssey(ref DefaultGameEventParam prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if ((prm.atker != 0) && (prm.atker == enumerator2.Current.actorHero))
                    {
                        enumerator2.Current.OnActorOdyssey(ref prm);
                        return;
                    }
                }
            }
        }

        public void OnActorPentaKill(ref DefaultGameEventParam prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if ((prm.atker != 0) && (prm.atker == enumerator2.Current.actorHero))
                    {
                        enumerator2.Current.OnActorPentaKill(ref prm);
                        return;
                    }
                }
            }
        }

        public void OnActorQuataryKill(ref DefaultGameEventParam prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if ((prm.atker != 0) && (prm.atker == enumerator2.Current.actorHero))
                    {
                        enumerator2.Current.OnActorQuataryKill(ref prm);
                        return;
                    }
                }
            }
        }

        public void OnActorTripleKill(ref DefaultGameEventParam prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if ((prm.atker != 0) && (prm.atker == enumerator2.Current.actorHero))
                    {
                        enumerator2.Current.OnActorTripleKill(ref prm);
                        return;
                    }
                }
            }
        }

        private void OnGameEnd(ref DefaultGameEventParam prm)
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnActorOdyssey));
            Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, new RefAction<SkillChooseTargetEventParam>(this.OnActorBeChosen));
            Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, new RefAction<SkillChooseTargetEventParam>(this.OnHitTrigger));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorBattleCoinChanged));
            Singleton<GameEventSys>.instance.RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
        }

        public void OnHeroBattleEquipChange(uint actorId, stEquipInfo[] equips)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if ((enumerator2.Current != null) && (enumerator2.Current.actorHero.handle.ObjID == actorId))
                    {
                        equips.CopyTo(enumerator2.Current.Equips, 0);
                        Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
                        Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_HERO_PROPERTY_CHANGED);
                        return;
                    }
                }
            }
        }

        public void OnHeroLearnTalent(PoolObjHandle<ActorRoot> hero, sbyte talentLevel, uint talentID)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (hero == enumerator2.Current.actorHero)
                    {
                        enumerator2.Current.TalentArr[(int) talentLevel].dwTalentID = talentID;
                        enumerator2.Current.TalentArr[(int) talentLevel].dwLearnLevel = (uint) hero.handle.ValueComponent.actorSoulLevel;
                        return;
                    }
                }
            }
        }

        public void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (hero == enumerator2.Current.actorHero)
                    {
                        enumerator2.Current.SoulLevel = Math.Max(level, 1);
                        Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
                        Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_HERO_PROPERTY_CHANGED);
                        return;
                    }
                }
            }
        }

        public void OnHitTrigger(ref SkillChooseTargetEventParam prm)
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.OnHitTrigger(ref prm);
                }
            }
        }

        public void reset()
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                current.Value.clear();
            }
            this.m_PlayerKDA.Clear();
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, sbyte, uint>("HeroLearnTalent", new Action<PoolObjHandle<ActorRoot>, sbyte, uint>(this.OnHeroLearnTalent));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this.OnHeroBattleEquipChange));
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnActorOdyssey));
            Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, new RefAction<SkillChooseTargetEventParam>(this.OnActorBeChosen));
            Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, new RefAction<SkillChooseTargetEventParam>(this.OnHitTrigger));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorBattleCoinChanged));
            Singleton<GameEventSys>.instance.RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
        }

        public void StartKDARecord()
        {
            this.reset();
            this.initialize();
        }
    }
}

