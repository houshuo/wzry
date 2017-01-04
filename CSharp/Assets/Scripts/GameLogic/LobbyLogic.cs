namespace Assets.Scripts.GameLogic
{
    using Apollo;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.Sound;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class LobbyLogic : Singleton<LobbyLogic>, IUpdateLogic
    {
        private int _gameEndTimer;
        private int _settleMsgTimer;
        private int _settlePanelTimer;
        public bool inMultiGame;
        public bool inMultiRoom;
        public bool isLogin;
        private bool m_bShouldNewbieEnterHall;
        private bool m_bShowNewbieEnterExploreForm;
        private bool m_bShowNewbieEnterSymbolForm;
        private bool m_bShowNewbieEnterTaskForm;
        public bool NeedUpdateClient;
        public ulong ulAccountUid;
        public uint uPlayerID;

        private int CheckWifi()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                return 1;
            }
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return 2;
            }
            return 0;
        }

        public bool ConnectServer()
        {
            if (Singleton<NetworkModule>.GetInstance().isOnlineMode && !this.isLogin)
            {
                Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent -= new NetConnectedEvent(this.onLobbyConnected);
                Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent -= new NetDisconnectEvent(this.onLobbyDisconnected);
                Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent += new NetConnectedEvent(this.onLobbyConnected);
                Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent += new NetDisconnectEvent(this.onLobbyDisconnected);
                ConnectorParam para = new ConnectorParam {
                    url = ApolloConfig.loginUrl,
                    ip = ApolloConfig.loginOnlyIpOrUrl,
                    vPort = ApolloConfig.loginOnlyVPort
                };
                return Singleton<NetworkModule>.GetInstance().InitLobbyServerConnect(para);
            }
            return false;
        }

        private void CreateAttrReportData(ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
        {
            ListView<IStarEvaluation>.Enumerator enumerator = Singleton<StarSystem>.GetInstance().GetEnumerator();
            int index = 0;
            CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum].bReportType = 2;
            COMDT_STATISTIC_BASE_STRUCT[] astDetail = CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum].astDetail;
            while (enumerator.MoveNext())
            {
                ListView<IStarCondition>.Enumerator enumerator2 = enumerator.Current.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.type == 2)
                    {
                        int[] keys = enumerator2.Current.keys;
                        int[] values = enumerator2.Current.values;
                        DebugHelper.Assert(keys.Length == 4);
                        astDetail[index].stKeyInfo.bKeyNum = (byte) keys.Length;
                        for (int i = 0; i < keys.Length; i++)
                        {
                            astDetail[index].stKeyInfo.KeyDetail[i] = keys[i];
                        }
                        DebugHelper.Assert((values != null) && (values.Length >= 0));
                        astDetail[index].stValueInfo.bValueNum = (byte) values.Length;
                        for (int j = 0; j < values.Length; j++)
                        {
                            if (!bGMWin)
                            {
                                astDetail[index].stValueInfo.ValueDetail[j] = values[j];
                            }
                            else
                            {
                                astDetail[index].stValueInfo.ValueDetail[j] = this.FakeValue(enumerator2.Current.configInfo.ComparetorDetail[j], enumerator2.Current.configInfo.ValueDetail[j]);
                            }
                        }
                        index++;
                    }
                }
            }
            CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum].wNum = (ushort) index;
            CommonData.stStatisticData.bNum = (byte) (CommonData.stStatisticData.bNum + 1);
        }

        private void CreateBattleNonHeroData(ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
        {
        }

        private void CreateBattleStatisticInfoData(HeroKDA HeroInfo, ref COMDT_HERO_BATTLE_STATISTIC_INFO stBattleStatisticInfo)
        {
            stBattleStatisticInfo.dwKillCnt = (uint) HeroInfo.numKill;
            stBattleStatisticInfo.dwDeadCnt = (uint) HeroInfo.numDead;
            stBattleStatisticInfo.dwAssistCnt = (uint) HeroInfo.numAssist;
            stBattleStatisticInfo.bDestroyBaseCnt = (byte) HeroInfo.numDestroyBase;
            stBattleStatisticInfo.bDestroyTowerCnt = (byte) HeroInfo.numKillOrgan;
            stBattleStatisticInfo.wKillMonsterCnt = (ushort) HeroInfo.numKillMonster;
            stBattleStatisticInfo.wKillFakeMonsterCnt = (ushort) HeroInfo.numKillFakeMonster;
            stBattleStatisticInfo.wKillSoldierCnt = (ushort) HeroInfo.numKillSoldier;
            stBattleStatisticInfo.dwTotalHurtCnt = (uint) HeroInfo.hurtToEnemy;
            stBattleStatisticInfo.dwTotalHurtNum = HeroInfo.HurtToEnemyNum;
            stBattleStatisticInfo.dwTotalHurtHeroCnt = (uint) HeroInfo.hurtToHero;
            stBattleStatisticInfo.dwTotalHurtHeroNum = HeroInfo.HurtToHeroNum;
            stBattleStatisticInfo.dwTotalBeHurtCnt = (uint) HeroInfo.hurtTakenByEnemy;
            stBattleStatisticInfo.dwTotalBeHurtNum = HeroInfo.TotalBeAttackNum;
            stBattleStatisticInfo.dwTotalBeChosenAsAttackTargetNum = HeroInfo.TotalBeChosenAsAttackTargetNum;
            stBattleStatisticInfo.dwTotalBeHeroHurtCnt = (uint) HeroInfo.hurtTakenByHero;
            stBattleStatisticInfo.dwTotalHealCnt = (uint) HeroInfo.beHeal;
            stBattleStatisticInfo.dwTotalBeHealNum = HeroInfo.TotalBeHealNum;
            stBattleStatisticInfo.dwTotalBeChosenAsHealTargetNum = HeroInfo.TotalBeChosenAsHealTargetNum;
            stBattleStatisticInfo.bKillDragonCnt = (byte) HeroInfo.numKillDragon;
            stBattleStatisticInfo.bKillBigDragonCnt = (byte) HeroInfo.numKillBaron;
            stBattleStatisticInfo.dwDoubleKillCnt = (uint) HeroInfo.DoubleKillNum;
            stBattleStatisticInfo.dwTripleKillCnt = (uint) HeroInfo.TripleKillNum;
            stBattleStatisticInfo.dwUltraKillCnt = (uint) HeroInfo.QuataryKillNum;
            stBattleStatisticInfo.dwRampageCnt = (uint) HeroInfo.PentaKillNum;
            stBattleStatisticInfo.iPhysicalHurt = HeroInfo.physHurtToHero;
            stBattleStatisticInfo.iSpellHurt = HeroInfo.magicHurtToHero;
            stBattleStatisticInfo.iRealHurt = HeroInfo.realHurtToHero;
            stBattleStatisticInfo.iHealCnt = HeroInfo.heal;
            stBattleStatisticInfo.dwTotalHurtOrganCnt = (uint) HeroInfo.hurtToOrgan;
            float num = ((float) Singleton<FrameSynchr>.instance.LogicFrameTick) / 60000f;
            if (((num > 0f) && (HeroInfo.actorHero != 0)) && (HeroInfo.actorHero.handle.ValueComponent != null))
            {
                stBattleStatisticInfo.iGPM = (int) (((float) HeroInfo.actorHero.handle.ValueComponent.GetGoldCoinIncomeInBattle()) / num);
                stBattleStatisticInfo.iXPM = (int) (((float) (HeroInfo.actorHero.handle.ValueComponent.actorSoulExp + HeroInfo.actorHero.handle.ValueComponent.actorSoulMaxExp)) / num);
            }
            if (Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.m_CampKdaStat != null)
            {
                uint teamTotalDamage = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.m_CampKdaStat.GetTeamTotalDamage(HeroInfo.actorHero.handle.TheActorMeta.ActorCamp);
                if (teamTotalDamage != 0)
                {
                    stBattleStatisticInfo.bTotalHurtPercent = (byte) (((long) (100 * HeroInfo.hurtToEnemy)) / ((ulong) teamTotalDamage));
                }
                uint teamTotalToHeroDamage = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.m_CampKdaStat.GetTeamTotalToHeroDamage(HeroInfo.actorHero.handle.TheActorMeta.ActorCamp);
                if (teamTotalToHeroDamage != 0)
                {
                    stBattleStatisticInfo.bTotalHurtHeroPercent = (byte) (((long) (100 * HeroInfo.hurtToHero)) / ((ulong) teamTotalToHeroDamage));
                }
                uint teamTotalTakenDamage = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.m_CampKdaStat.GetTeamTotalTakenDamage(HeroInfo.actorHero.handle.TheActorMeta.ActorCamp);
                if (teamTotalTakenDamage != 0)
                {
                    stBattleStatisticInfo.bTotalBeHurtPercent = (byte) (((long) (100 * HeroInfo.hurtTakenByEnemy)) / ((ulong) teamTotalTakenDamage));
                }
            }
            stBattleStatisticInfo.dwLevelThreeTime = Singleton<BattleStatistic>.instance.m_playerSoulLevelStat.GetPlayerLevelChangedTime(HeroInfo.actorHero.handle.TheActorMeta.PlayerId, 3);
            stBattleStatisticInfo.dwLevelFourTime = Singleton<BattleStatistic>.instance.m_playerSoulLevelStat.GetPlayerLevelChangedTime(HeroInfo.actorHero.handle.TheActorMeta.PlayerId, 4);
            stBattleStatisticInfo.iHeroHurtCount = (int) HeroInfo.TotalBeAttackNum;
            stBattleStatisticInfo.iHeroHurtMax = HeroInfo.CountBeHurtMax;
            if (HeroInfo.CountBeHurtMin != -1)
            {
                stBattleStatisticInfo.iHeroHurtMin = HeroInfo.CountBeHurtMin;
            }
            stBattleStatisticInfo.iHeroHurtTotal = HeroInfo.hurtTakenByEnemy;
            stBattleStatisticInfo.iHeroHealCount = (int) HeroInfo.TotalBeHealNum;
            stBattleStatisticInfo.iHeroHealMax = HeroInfo.CountBeHealMax;
            if (HeroInfo.CountBeHealMin != -1)
            {
                stBattleStatisticInfo.iHeroHealMin = HeroInfo.CountBeHealMin;
            }
            stBattleStatisticInfo.iHeroHealTotal = HeroInfo.beHeal;
            stBattleStatisticInfo.iHeroCureMax = HeroInfo.CountSelfHealMax;
            if (HeroInfo.CountSelfHealMin != -1)
            {
                stBattleStatisticInfo.iHeroCureMin = HeroInfo.CountSelfHealMin;
            }
            stBattleStatisticInfo.iHeroCureTotal = HeroInfo.TotalCountSelfHeal;
            if (((HeroInfo.actorHero != 0) && (HeroInfo.actorHero.handle.SkillControl != null)) && (HeroInfo.actorHero.handle.SkillControl.stSkillStat != null))
            {
                stBattleStatisticInfo.iHeroCtrlSkillNum = HeroInfo.actorHero.handle.SkillControl.stSkillStat.GetStunSkillNum();
                stBattleStatisticInfo.iHeroBeCtrledTime = (int) HeroInfo.actorHero.handle.SkillControl.stSkillStat.BeStunTime;
                stBattleStatisticInfo.iHeroCtrlOtherTime = (int) HeroInfo.actorHero.handle.SkillControl.stSkillStat.StunTime;
            }
        }

        private void CreateDestroyReportData(ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
        {
            int index = 0;
            DictionaryView<uint, Dictionary<int, DestroyStat>> destroyStat = Singleton<BattleStatistic>.GetInstance().GetDestroyStat();
            COMDT_STATISTIC_STRUCT_PILE comdt_statistic_struct_pile = CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum];
            COMDT_STATISTIC_BASE_STRUCT[] astDetail = comdt_statistic_struct_pile.astDetail;
            if (destroyStat.Count != 0)
            {
                comdt_statistic_struct_pile.bReportType = 1;
                DictionaryView<uint, Dictionary<int, DestroyStat>>.Enumerator enumerator = destroyStat.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Dictionary<int, DestroyStat>> current = enumerator.Current;
                    Dictionary<int, DestroyStat>.Enumerator enumerator2 = current.Value.GetEnumerator();
                    int num2 = 0;
                    int num3 = 0;
                    while (enumerator2.MoveNext())
                    {
                        astDetail[index] = new COMDT_STATISTIC_BASE_STRUCT();
                        astDetail[index].stKeyInfo.bKeyNum = 3;
                        KeyValuePair<uint, Dictionary<int, DestroyStat>> pair2 = enumerator.Current;
                        astDetail[index].stKeyInfo.KeyDetail[0] = pair2.Key;
                        KeyValuePair<int, DestroyStat> pair3 = enumerator2.Current;
                        astDetail[index].stKeyInfo.KeyDetail[1] = pair3.Key;
                        astDetail[index].stKeyInfo.KeyDetail[2] = 1;
                        astDetail[index].stValueInfo.bValueNum = 1;
                        KeyValuePair<int, DestroyStat> pair4 = enumerator2.Current;
                        astDetail[index].stValueInfo.ValueDetail[0] = pair4.Value.CampEnemyNum;
                        index++;
                        KeyValuePair<int, DestroyStat> pair5 = enumerator2.Current;
                        num2 += pair5.Value.CampEnemyNum;
                        astDetail[index].stKeyInfo.bKeyNum = 3;
                        KeyValuePair<uint, Dictionary<int, DestroyStat>> pair6 = enumerator.Current;
                        astDetail[index].stKeyInfo.KeyDetail[0] = pair6.Key;
                        KeyValuePair<int, DestroyStat> pair7 = enumerator2.Current;
                        astDetail[index].stKeyInfo.KeyDetail[1] = pair7.Key;
                        astDetail[index].stKeyInfo.KeyDetail[2] = 0;
                        astDetail[index].stValueInfo.bValueNum = 1;
                        KeyValuePair<int, DestroyStat> pair8 = enumerator2.Current;
                        astDetail[index].stValueInfo.ValueDetail[0] = pair8.Value.CampSelfNum;
                        index++;
                        KeyValuePair<int, DestroyStat> pair9 = enumerator2.Current;
                        num3 += pair9.Value.CampSelfNum;
                    }
                    astDetail[index].stKeyInfo.bKeyNum = 3;
                    KeyValuePair<uint, Dictionary<int, DestroyStat>> pair10 = enumerator.Current;
                    astDetail[index].stKeyInfo.KeyDetail[0] = pair10.Key;
                    astDetail[index].stKeyInfo.KeyDetail[1] = 0;
                    astDetail[index].stKeyInfo.KeyDetail[2] = 1;
                    astDetail[index].stValueInfo.bValueNum = 1;
                    astDetail[index].stValueInfo.ValueDetail[0] = num2;
                    index++;
                    astDetail[index].stKeyInfo.bKeyNum = 3;
                    KeyValuePair<uint, Dictionary<int, DestroyStat>> pair11 = enumerator.Current;
                    astDetail[index].stKeyInfo.KeyDetail[0] = pair11.Key;
                    astDetail[index].stKeyInfo.KeyDetail[1] = 0;
                    astDetail[index].stKeyInfo.KeyDetail[2] = 0;
                    astDetail[index].stValueInfo.bValueNum = 1;
                    astDetail[index].stValueInfo.ValueDetail[0] = num3;
                    index++;
                }
                CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum].wNum = (ushort) index;
                CommonData.stStatisticData.bNum = (byte) (CommonData.stStatisticData.bNum + 1);
            }
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext == null) || !curLvelContext.IsMobaMode())
            {
                if (bGMWin)
                {
                    this.FakeStarInfoForGMWin(ref CommonData);
                }
                else
                {
                    this.VerifyReportData(ref CommonData);
                }
            }
        }

        private void CreateGeneralData(uint playerId, ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
        {
            DictionaryView<uint, NONHERO_STATISTIC_INFO> view2;
            NONHERO_STATISTIC_INFO nonhero_statistic_info;
            CBattleDeadStat battleDeadStat = Singleton<BattleStatistic>.instance.m_battleDeadStat;
            COMDT_SETTLE_GAME_GENERAL_INFO stGeneralData = CommonData.stGeneralData;
            stGeneralData.dwFBTime = battleDeadStat.m_uiFBTime;
            stGeneralData.bKillDragonNum = (byte) battleDeadStat.GetKillDragonNum();
            uint data = CPlayerBehaviorStat.GetData(CPlayerBehaviorStat.BehaviorType.SortBYCoinBtnClick);
            stGeneralData.dwBoardSortNum = data;
            for (int i = 0; i < stGeneralData.bKillDragonNum; i++)
            {
                if (i >= 10)
                {
                    stGeneralData.bKillDragonNum = (byte) i;
                    break;
                }
                stGeneralData.KillDragonTime[i] = (uint) battleDeadStat.GetDragonDeadTime(i);
            }
            int baronDeadCount = battleDeadStat.GetBaronDeadCount();
            stGeneralData.bKillBigDragonNum = (byte) baronDeadCount;
            for (int j = 0; j < baronDeadCount; j++)
            {
                if (j >= 10)
                {
                    stGeneralData.bKillBigDragonNum = (byte) j;
                    break;
                }
                stGeneralData.KillBigDragonTime[j] = (uint) battleDeadStat.GetBaronDeadTime(j);
            }
            List<CShenFuStat.ShenFuRecord> shenFuRecord = Singleton<BattleStatistic>.instance.m_shenFuStat.GetShenFuRecord(playerId);
            Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
            for (int k = 0; k < shenFuRecord.Count; k++)
            {
                CShenFuStat.ShenFuRecord record = shenFuRecord[k];
                if (!dictionary.ContainsKey((int) record.shenFuId))
                {
                    List<int> list2 = new List<int> {
                        record.shenFuId
                    };
                    dictionary.Add((int) record.shenFuId, list2);
                }
                else
                {
                    dictionary[(int) record.shenFuId].Add((int) record.shenFuId);
                }
            }
            int index = 0;
            foreach (int num7 in dictionary.Keys)
            {
                if (index >= 10)
                {
                    break;
                }
                stGeneralData.astRuneTypePickUpNum[index].dwRuneID = (uint) num7;
                stGeneralData.astRuneTypePickUpNum[index].dwPickUpNum = (uint) dictionary[num7].Count;
                index++;
            }
            stGeneralData.bRuneTypeNum = (byte) index;
            PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(playerId);
            int num8 = (int) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick * 0.001f);
            num8 -= 60;
            if (num8 > 0)
            {
                int num9 = num8 / 30;
                float num10 = (num8 * 1f) / 30f;
                if ((num10 - num9) > 0f)
                {
                    num9++;
                }
                if (num9 > 0x3b)
                {
                    num9 = 0x3b;
                }
                stGeneralData.bGame30SecNum = (byte) num9;
                for (int num11 = 0; num11 < num9; num11++)
                {
                    if (num11 >= 60)
                    {
                        stGeneralData.bGame30SecNum = (byte) num11;
                        break;
                    }
                    int num12 = 60 + (num11 * 30);
                    stGeneralData.astGame30SecDetail[num11].dwGhostLevel = (uint) Singleton<BattleStatistic>.instance.m_playerSoulLevelStat.GetPlayerSoulLevelAtTime(playerId, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwKillCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetHeroDeadAtTime(playerId, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwKillDragonCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillDragonNumAtTime(playerId, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwKillBigDragonCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillSpecialMonsterNumAtTime(playerId, num12 * 0x3e8, 8);
                    stGeneralData.astGame30SecDetail[num11].dwDestroyTowerCnt = (uint) (Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 1, num12 * 0x3e8) + Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 4, num12 * 0x3e8));
                    stGeneralData.astGame30SecDetail[num11].dwDestroyOuterTowerCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 1, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwDestroyBaseTowerCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 4, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwBigDragonBuffCnt = (uint) Singleton<BattleStatistic>.instance.m_battleBuffStat.GetDataByIndex(playerKDA.PlayerCamp, num11);
                    stGeneralData.astGame30SecDetail[num11].dwKillMonsterCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetMonsterDeadAtTime(playerId, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwKillSoldierCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetSoldierDeadAtTime(playerId, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwCoinCnt = playerKDA.GetPlayerCoinAtTime(num11 + 1);
                    stGeneralData.astGame30SecDetail[num11].dwRedBuffCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillRedBaNumAtTime(playerId, num12 * 0x3e8);
                    stGeneralData.astGame30SecDetail[num11].dwBlueBuffCnt = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillBlueBaNumAtTime(playerId, num12 * 0x3e8);
                    VInt2 timeLocation = Singleton<BattleStatistic>.instance.m_locStat.GetTimeLocation(playerId, num11);
                    stGeneralData.astGame30SecDetail[num11].iXPos = timeLocation.x;
                    stGeneralData.astGame30SecDetail[num11].iZPos = timeLocation.y;
                }
            }
            stGeneralData.dwCamp1TowerFirstAttackTime = playerKDA.m_Camp1TowerFirstAttackTime;
            stGeneralData.dwCamp2TowerFirstAttackTime = playerKDA.m_Camp2TowerFirstAttackTime;
            PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(playerId).Captain;
            if ((captain != 0) && (captain.handle.EquipComponent != null))
            {
                CExistEquipInfoSet existEquipInfoSet = captain.handle.EquipComponent.GetExistEquipInfoSet();
                int num14 = (existEquipInfoSet.m_existEquipInfoCount >= 6) ? 6 : existEquipInfoSet.m_existEquipInfoCount;
                stGeneralData.bEquipNum = (byte) num14;
                for (int num15 = 0; num15 < num14; num15++)
                {
                    stGeneralData.astEquipDetail[num15].bCnt = (byte) existEquipInfoSet.m_existEquipInfos[num15].m_amount;
                    stGeneralData.astEquipDetail[num15].dwEquipID = existEquipInfoSet.m_existEquipInfos[num15].m_equipID;
                }
            }
            if ((captain != 0) && (captain.handle.ValueComponent != null))
            {
                stGeneralData.dwTotalInGameCoin = (uint) captain.handle.ValueComponent.GetGoldCoinIncomeInBattle();
                stGeneralData.dwMaxInGameCoin = (uint) captain.handle.ValueComponent.GetMaxGoldCoinIncomeInBattle();
            }
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext.IsMobaModeWithOutGuide())
            {
                int num16 = 10;
                if ((curLvelContext.m_mapType == 4) || (curLvelContext.m_pvpPlayerNum < 10))
                {
                    num16 = 3;
                }
                num16 *= captain.handle.TheActorMeta.ActorCamp - 1;
                stGeneralData.dwUpRoadTower1DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 1);
                stGeneralData.dwUpRoadTower2DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 2);
                stGeneralData.dwUpRoadTower3DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 3);
                if (!curLvelContext.IsGameTypeEntertainment() && (curLvelContext.m_pvpPlayerNum == 10))
                {
                    stGeneralData.dwMidRoadTower1DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 4);
                    stGeneralData.dwMidRoadTower2DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 5);
                    stGeneralData.dwMidRoadTower3DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 6);
                    stGeneralData.dwDownRoadTower1DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 7);
                    stGeneralData.dwDownRoadTower2DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 8);
                    stGeneralData.dwDownRoadTower3DesTime = (uint) Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num16 + 9);
                }
            }
            uint num17 = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetTotalNum(playerKDA.PlayerCamp, ActorTypeDef.Actor_Type_Monster, 2, 8);
            if (num17 > 10)
            {
                num17 = 10;
            }
            stGeneralData.dwBigDragonBattleToDeadTimeNum = num17;
            for (uint m = 0; m < stGeneralData.dwBigDragonBattleToDeadTimeNum; m++)
            {
                stGeneralData.BigDragonBattleToDeadTimeDtail[m] = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetRecordAtIndex(playerKDA.PlayerCamp, ActorTypeDef.Actor_Type_Monster, 2, 8, (int) m).fightTime;
            }
            if ((curLvelContext != null) && curLvelContext.IsMobaModeWithOutGuide())
            {
                if (curLvelContext.m_pvpPlayerNum == 10)
                {
                    uint num19 = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetTotalNum(playerKDA.PlayerCamp, ActorTypeDef.Actor_Type_Monster, 2, 9);
                    if (num19 > 10)
                    {
                        num19 = 10;
                    }
                    stGeneralData.dwDragonBattleToDeadTimeNum = num19;
                    for (uint num20 = 0; num20 < stGeneralData.dwDragonBattleToDeadTimeNum; num20++)
                    {
                        stGeneralData.DragonBattleToDeadTimeDtail[num20] = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetRecordAtIndex(playerKDA.PlayerCamp, ActorTypeDef.Actor_Type_Monster, 2, 9, (int) num20).fightTime;
                    }
                }
                else
                {
                    uint num21 = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetTotalNum(playerKDA.PlayerCamp, ActorTypeDef.Actor_Type_Monster, 2, 7);
                    if (num21 > 10)
                    {
                        num21 = 10;
                    }
                    stGeneralData.dwDragonBattleToDeadTimeNum = num21;
                    for (uint num22 = 0; num22 < stGeneralData.dwDragonBattleToDeadTimeNum; num22++)
                    {
                        stGeneralData.DragonBattleToDeadTimeDtail[num22] = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetRecordAtIndex(playerKDA.PlayerCamp, ActorTypeDef.Actor_Type_Monster, 2, 7, (int) num22).fightTime;
                    }
                }
                if (curLvelContext.m_pvpPlayerNum == 10)
                {
                    int[] numArray = new int[] { 0x1776, 0x1778, 0x1777, 0x177d, 0x177b, 0x177a };
                    uint length = (uint) numArray.Length;
                    if (length > 20)
                    {
                        length = 20;
                    }
                    stGeneralData.dwMonsterDeadNum = length;
                    for (uint num24 = 0; num24 < stGeneralData.dwMonsterDeadNum; num24++)
                    {
                        stGeneralData.MonsterDeadDetail[num24] = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNum(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, numArray[num24]);
                    }
                }
                else
                {
                    int[] numArray2 = new int[] { 30, 0x1f, 0x20, 0x21, 0x2a, 0x2b, 0x2c, 0x2d, 0x31, 0x3b };
                    uint num25 = (uint) numArray2.Length;
                    if (num25 > 20)
                    {
                        num25 = 20;
                    }
                    stGeneralData.dwMonsterDeadNum = num25;
                    for (uint num26 = 0; num26 < stGeneralData.dwMonsterDeadNum; num26++)
                    {
                        stGeneralData.MonsterDeadDetail[num26] = (uint) Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNum(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, numArray2[num26]);
                    }
                }
            }
            stGeneralData.iTimeUse = (int) (Singleton<FrameSynchr>.instance.LogicFrameTick * 0.001f);
            stGeneralData.iPauseTimeTotal = (int) ((Time.timeSinceLevelLoad - stGeneralData.iTimeUse) - 15f);
            if (stGeneralData.iPauseTimeTotal < 0)
            {
                stGeneralData.iPauseTimeTotal = 0;
            }
            COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerCamp;
            DictionaryView<uint, DictionaryView<uint, NONHERO_STATISTIC_INFO>> nonHeroInfo = Singleton<BattleStatistic>.instance.m_NonHeroInfo;
            if (nonHeroInfo.TryGetValue(2, out view2))
            {
                if (view2.TryGetValue((uint) playerCamp, out nonhero_statistic_info))
                {
                    stGeneralData.iBuildingAttackRange = (int) nonhero_statistic_info.uiAttackDistanceMax;
                    stGeneralData.iBuildingAttackDamageMax = (int) nonhero_statistic_info.uiHurtMax;
                    if (nonhero_statistic_info.uiHurtMin != uint.MaxValue)
                    {
                        stGeneralData.iBuildingAttackDamageMin = (int) nonhero_statistic_info.uiHurtMin;
                    }
                    stGeneralData.iBuildingHPMax = (int) nonhero_statistic_info.uiHpMax;
                    if (nonhero_statistic_info.uiHpMin != uint.MaxValue)
                    {
                        stGeneralData.iBuildingHPMin = (int) nonhero_statistic_info.uiHpMin;
                    }
                    stGeneralData.iBuildingHurtCount = (int) nonhero_statistic_info.uiTotalBeAttackedNum;
                    stGeneralData.iBuildingHurtMax = (int) nonhero_statistic_info.uiBeHurtMax;
                    if (nonhero_statistic_info.uiBeHurtMin != uint.MaxValue)
                    {
                        stGeneralData.iBuildingHurtMin = (int) nonhero_statistic_info.uiBeHurtMin;
                    }
                    stGeneralData.iBuildingHurtTotal = (int) nonhero_statistic_info.uiTotalBeHurtCount;
                }
                else
                {
                    stGeneralData.iBuildingHurtCount = 0;
                    stGeneralData.iBuildingHurtMax = 0;
                    stGeneralData.iBuildingHurtMin = 0;
                    stGeneralData.iBuildingHurtTotal = 0;
                    stGeneralData.iBuildingAttackDamageMax = 0;
                    stGeneralData.iBuildingAttackDamageMin = 0;
                    stGeneralData.iBuildingAttackRange = 0;
                    List<PoolObjHandle<ActorRoot>> list4 = Singleton<GameObjMgr>.instance.OrganActors;
                    for (int num27 = 0; num27 < list4.Count; num27++)
                    {
                        PoolObjHandle<ActorRoot> actor = list4[num27];
                        if (ActorHelper.IsHostEnemyActor(ref actor))
                        {
                            int actorHp = actor.handle.ValueComponent.actorHp;
                            stGeneralData.iBuildingHPMax = Math.Max(actorHp, stGeneralData.iBuildingHPMax);
                            if (stGeneralData.iBuildingHPMin == 0)
                            {
                                stGeneralData.iBuildingHPMin = actorHp;
                            }
                            else
                            {
                                stGeneralData.iBuildingHPMin = Math.Min(actorHp, stGeneralData.iBuildingHPMin);
                            }
                        }
                    }
                }
            }
            if (nonHeroInfo.TryGetValue(2, out view2))
            {
                COM_PLAYERCAMP com_playercamp2 = (playerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_1 : COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                if (view2.TryGetValue((uint) com_playercamp2, out nonhero_statistic_info))
                {
                    stGeneralData.iEnemyBuildingAttackRange = (int) nonhero_statistic_info.uiAttackDistanceMax;
                    stGeneralData.iEnemyBuildingDamageMax = (int) nonhero_statistic_info.uiHurtMax;
                    if (nonhero_statistic_info.uiHurtMin != uint.MaxValue)
                    {
                        stGeneralData.iEnemyBuildingDamageMin = (int) nonhero_statistic_info.uiHurtMin;
                    }
                    stGeneralData.iEnemyBuildingHPMax = (int) nonhero_statistic_info.uiHpMax;
                    if (nonhero_statistic_info.uiHpMin != uint.MaxValue)
                    {
                        stGeneralData.iEnemyBuildingHPMin = (int) nonhero_statistic_info.uiHpMin;
                    }
                    stGeneralData.iEnemyBuildingHurtMax = (int) nonhero_statistic_info.uiBeHurtMax;
                    if (nonhero_statistic_info.uiBeHurtMin != uint.MaxValue)
                    {
                        stGeneralData.iEnemyBuildingHurtMin = (int) nonhero_statistic_info.uiBeHurtMin;
                    }
                    stGeneralData.iEnemyBuildingHurtTotal = (int) nonhero_statistic_info.uiTotalBeHurtCount;
                }
                else
                {
                    stGeneralData.iEnemyBuildingHurtMax = 0;
                    stGeneralData.iEnemyBuildingHurtMin = 0;
                    stGeneralData.iEnemyBuildingHurtTotal = 0;
                    stGeneralData.iEnemyBuildingDamageMax = 0;
                    stGeneralData.iEnemyBuildingDamageMin = 0;
                    stGeneralData.iEnemyBuildingAttackRange = 0;
                    List<PoolObjHandle<ActorRoot>> list5 = Singleton<GameObjMgr>.instance.OrganActors;
                    for (int num29 = 0; num29 < list5.Count; num29++)
                    {
                        PoolObjHandle<ActorRoot> handle3 = list5[num29];
                        if (!ActorHelper.IsHostEnemyActor(ref handle3))
                        {
                            int num30 = handle3.handle.ValueComponent.actorHp;
                            stGeneralData.iEnemyBuildingHPMax = Math.Max(num30, stGeneralData.iEnemyBuildingHPMax);
                            if (stGeneralData.iEnemyBuildingHPMin == 0)
                            {
                                stGeneralData.iEnemyBuildingHPMin = num30;
                            }
                            else
                            {
                                stGeneralData.iEnemyBuildingHPMin = Math.Min(num30, stGeneralData.iEnemyBuildingHPMin);
                            }
                        }
                    }
                }
            }
            if (Singleton<BattleStatistic>.instance.m_stSoulStatisticInfo != null)
            {
                GET_SOUL_EXP_STATISTIC_INFO stSoulStatisticInfo = Singleton<BattleStatistic>.instance.m_stSoulStatisticInfo;
                stGeneralData.iExperienceHPAdd1 = stSoulStatisticInfo.iKillSoldierExpMax;
                stGeneralData.iExperienceHPAdd2 = stSoulStatisticInfo.iKillHeroExpMax;
                stGeneralData.iExperienceHPAdd3 = stSoulStatisticInfo.iKillOrganExpMax;
                stGeneralData.iExperienceHPAdd4 = stSoulStatisticInfo.iKillMonsterExpMax;
                stGeneralData.iExperienceHPAddTotal = stSoulStatisticInfo.iAddExpTotal;
            }
            if (nonHeroInfo.TryGetValue(1, out view2))
            {
                COM_PLAYERCAMP com_playercamp3 = (playerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_1 : COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                if (view2.TryGetValue((uint) com_playercamp3, out nonhero_statistic_info))
                {
                    stGeneralData.iEnemyAttackMax = (int) nonhero_statistic_info.uiHurtMax;
                    stGeneralData.iEnemyAttackMin = (int) nonhero_statistic_info.uiHurtMin;
                    stGeneralData.iEnemyHPMax = (int) nonhero_statistic_info.uiHpMax;
                    stGeneralData.iEnemyHPMin = (int) nonhero_statistic_info.uiHpMin;
                    if (stGeneralData.iEnemyHPMin == -1)
                    {
                        stGeneralData.iEnemyHPMin = 0;
                    }
                }
            }
            List<PoolObjHandle<ActorRoot>> list6 = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(Singleton<BattleLogic>.instance.FilterEnemyActor));
            stGeneralData.iBossCount = list6.Count;
            for (int n = 0; n < list6.Count; n++)
            {
                PoolObjHandle<ActorRoot> handle5 = list6[n];
                stGeneralData.iBossHPMax = Math.Max(stGeneralData.iBossHPMax, handle5.handle.ValueComponent.ObjValueStatistic.iActorMaxHp);
                PoolObjHandle<ActorRoot> handle6 = list6[n];
                if (handle6.handle.ValueComponent.ObjValueStatistic.iActorMinHp != -1)
                {
                    if (n == 0)
                    {
                        PoolObjHandle<ActorRoot> handle7 = list6[n];
                        stGeneralData.iBossHPMin = handle7.handle.ValueComponent.ObjValueStatistic.iActorMinHp;
                    }
                    else
                    {
                        PoolObjHandle<ActorRoot> handle8 = list6[n];
                        stGeneralData.iBossHPMin = Math.Min(stGeneralData.iBossHPMin, handle8.handle.ValueComponent.ObjValueStatistic.iActorMinHp);
                    }
                }
                for (int num32 = 0; num32 < 8; num32++)
                {
                    if (list6[n] != 0)
                    {
                        PoolObjHandle<ActorRoot> handle9 = list6[n];
                        if (handle9.handle.SkillControl != null)
                        {
                            PoolObjHandle<ActorRoot> handle10 = list6[n];
                            if (handle10.handle.SkillControl.stSkillStat != null)
                            {
                                PoolObjHandle<ActorRoot> handle11 = list6[n];
                                if (handle11.handle.SkillControl.stSkillStat.SkillStatistictInfo != null)
                                {
                                    PoolObjHandle<ActorRoot> handle12 = list6[n];
                                    SKILLSTATISTICTINFO[] skillStatistictInfo = handle12.handle.SkillControl.stSkillStat.SkillStatistictInfo;
                                    stGeneralData.iBossHurtMax = Math.Max(stGeneralData.iBossHurtMax, skillStatistictInfo[num32].iHurtMax);
                                    if (((n == 0) && (num32 == 0)) || (stGeneralData.iBossHurtMin == -1))
                                    {
                                        stGeneralData.iBossHurtMin = skillStatistictInfo[num32].iHurtMin;
                                    }
                                    else if (skillStatistictInfo[num32].iHurtMin != -1)
                                    {
                                        stGeneralData.iBossHurtMin = Math.Min(stGeneralData.iBossHurtMin, skillStatistictInfo[num32].iHurtMin);
                                    }
                                    stGeneralData.iBossHurtTotal += skillStatistictInfo[num32].iHurtTotal;
                                    if (num32 == 0)
                                    {
                                        stGeneralData.iBossAttackCount += (int) skillStatistictInfo[num32].uiUsedTimes;
                                        stGeneralData.iBossAttackMax = Math.Max(stGeneralData.iBossAttackMax, skillStatistictInfo[num32].iHurtMax);
                                        if (skillStatistictInfo[num32].iHurtMin != -1)
                                        {
                                            if (n == 0)
                                            {
                                                stGeneralData.iBossAttackMin = skillStatistictInfo[num32].iHurtMin;
                                            }
                                            else
                                            {
                                                stGeneralData.iBossAttackMin = Math.Min(stGeneralData.iBossAttackMin, skillStatistictInfo[num32].iHurtMin);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        stGeneralData.iBossUseSkillCount += (int) skillStatistictInfo[num32].uiUsedTimes;
                                        stGeneralData.iBossSkillDamageMax = Math.Max(stGeneralData.iBossSkillDamageMax, skillStatistictInfo[num32].iHurtMax);
                                        if (n == 0)
                                        {
                                            if (skillStatistictInfo[num32].iHurtMin != -1)
                                            {
                                                stGeneralData.iBossSkillDamageMin = skillStatistictInfo[num32].iHurtMin;
                                            }
                                        }
                                        else if (skillStatistictInfo[num32].iHurtMin != -1)
                                        {
                                            stGeneralData.iBossSkillDamageMin = Math.Min(stGeneralData.iBossSkillDamageMin, skillStatistictInfo[num32].iHurtMin);
                                        }
                                    }
                                    stGeneralData.iBossAttackTotal += skillStatistictInfo[num32].ihurtCount;
                                }
                            }
                        }
                    }
                }
            }
            stGeneralData.iCommunicationCount2 = (int) Singleton<NetworkModule>.instance.RecvGameMsgCount;
            stGeneralData.iCommunicationCount1 = (int) Singleton<NetworkModule>.instance.RecvGameMsgCount;
            stGeneralData.bSelfCampKillTowerCnt = Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDestroyTowerCount(playerKDA.PlayerCamp, 1);
            stGeneralData.bSelfCampKillBaseCnt = Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDestroyTowerCount(playerKDA.PlayerCamp, 2);
            this.FillVDStat(ref stGeneralData, playerId);
            List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.instance.OrganActors;
            for (int num33 = 0; num33 < organActors.Count; num33++)
            {
                PoolObjHandle<ActorRoot> handle4 = organActors[num33];
                if (((handle4.handle.TheActorMeta.ActorCamp == captain.handle.TheActorMeta.ActorCamp) && (handle4.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)) && (handle4.handle.ValueComponent != null))
                {
                    stGeneralData.dwSelfCampBaseBlood = (uint) handle4.handle.ValueComponent.actorHp;
                }
            }
            if ((captain != 0) && (captain.handle.EquipComponent != null))
            {
                stGeneralData.bQuickBuyItemCnt = (byte) captain.handle.EquipComponent.m_iFastBuyEquipCount;
                stGeneralData.bPanelBuyItemCnt = (byte) captain.handle.EquipComponent.m_iBuyEquipCount;
            }
            int num34 = 0;
            stGeneralData.dwClickNum = 0x17;
            for (int num35 = 1; num35 <= 0x17; num35++)
            {
                if (num34 >= 30)
                {
                    stGeneralData.dwClickNum = (uint) num34;
                    break;
                }
                stGeneralData.ClickDetail[num34] = CPlayerBehaviorStat.GetData((CPlayerBehaviorStat.BehaviorType) num35);
                num34++;
            }
            stGeneralData.dwOperateNum = 6;
            stGeneralData.OperateType[0] = (uint) GameSettings.TheCommonAttackType;
            stGeneralData.OperateType[1] = (uint) GameSettings.TheCastType;
            stGeneralData.OperateType[2] = (uint) GameSettings.TheSelectType;
            stGeneralData.OperateType[3] = (uint) GameSettings.LunPanSensitivity;
            stGeneralData.OperateType[4] = (uint) GameSettings.JoyStickShowType;
            stGeneralData.OperateType[5] = (uint) GameSettings.TheSkillCancleType;
        }

        private void CreateHeroBaseInfoData(HeroKDA HeroInfo, ref COMDT_HERO_BASE_INFO stHeroDetailInfo)
        {
            if ((HeroInfo.actorHero.handle != null) && (HeroInfo.actorHero.handle.ValueComponent != null))
            {
                ActorValueStatistic objValueStatistic = HeroInfo.actorHero.handle.ValueComponent.ObjValueStatistic;
                if (objValueStatistic != null)
                {
                    stHeroDetailInfo.iActorLvl = objValueStatistic.iActorLvl;
                    stHeroDetailInfo.iActorATT = objValueStatistic.iActorATT;
                    stHeroDetailInfo.iActorINT = objValueStatistic.iActorINT;
                    stHeroDetailInfo.iActorMaxHp = objValueStatistic.iActorMaxHp;
                    stHeroDetailInfo.iDEFStrike = objValueStatistic.iDEFStrike;
                    stHeroDetailInfo.iRESStrike = objValueStatistic.iRESStrike;
                    stHeroDetailInfo.iFinalHurt = objValueStatistic.iFinalHurt;
                    stHeroDetailInfo.iCritStrikeRate = objValueStatistic.iCritStrikeRate;
                    stHeroDetailInfo.iCritStrikeValue = objValueStatistic.iCritStrikeValue;
                    stHeroDetailInfo.iReduceCritStrikeRate = objValueStatistic.iReduceCritStrikeRate;
                    stHeroDetailInfo.iReduceCritStrikeValue = objValueStatistic.iReduceCritStrikeValue;
                    stHeroDetailInfo.iCritStrikeEff = objValueStatistic.iCritStrikeEff;
                    stHeroDetailInfo.iPhysicsHemophagiaRate = objValueStatistic.iPhysicsHemophagiaRate;
                    stHeroDetailInfo.iMagicHemophagiaRate = objValueStatistic.iMagicHemophagiaRate;
                    stHeroDetailInfo.iPhysicsHemophagia = objValueStatistic.iPhysicsHemophagia;
                    stHeroDetailInfo.iMagicHemophagia = objValueStatistic.iMagicHemophagia;
                    stHeroDetailInfo.iHurtOutputRate = objValueStatistic.iHurtOutputRate;
                    stHeroDetailInfo.iMaxMoveSpeed = objValueStatistic.iMoveSpeedMax;
                    stHeroDetailInfo.iMaxSoulExp = objValueStatistic.iSoulExpMax;
                    stHeroDetailInfo.iTotalSoulExp = HeroInfo.actorHero.handle.ValueComponent.actorSoulExp + HeroInfo.actorHero.handle.ValueComponent.actorSoulMaxExp;
                }
                ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
                if (HeroInfo.actorHero.handle.MovementComponent != null)
                {
                    PlayerMovement movementComponent = HeroInfo.actorHero.handle.MovementComponent as PlayerMovement;
                    uint num2 = (uint) (logicFrameTick - movementComponent.m_ulLastMoveEndTime);
                    movementComponent.m_uiMoveIntervalMax = (movementComponent.m_uiMoveIntervalMax <= num2) ? num2 : movementComponent.m_uiMoveIntervalMax;
                    movementComponent.m_uiNonMoveTotalTime += num2;
                }
            }
        }

        private void CreateHeroData(uint playerId, ref COMDT_SETTLE_COMMON_DATA CommonData)
        {
            PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(playerId);
            if (playerKDA == null)
            {
                object[] inParameters = new object[] { playerId };
                DebugHelper.Assert(playerKDA != null, "Failed find player kda, id = {0}", inParameters);
                Singleton<BattleStatistic>.instance.m_playerKDAStat.DumpDebugInfo();
            }
            else
            {
                ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
                byte index = 0;
                while (enumerator.MoveNext())
                {
                    if ((enumerator.Current.actorHero == 0) || (enumerator.Current.actorHero.handle.ValueComponent == null))
                    {
                        continue;
                    }
                    bool inCondition = index < CommonData.stHeroData.astHeroList.Length;
                    DebugHelper.Assert(inCondition, "Invalid member count!");
                    if (!inCondition)
                    {
                        break;
                    }
                    uint num2 = BurnExpeditionUT.Get_BloodTH(enumerator.Current.actorHero.handle);
                    CommonData.stHeroData.astHeroList[index].dwHeroConfID = (uint) enumerator.Current.actorHero.handle.TheActorMeta.ConfigId;
                    CommonData.stHeroData.astHeroList[index].dwBloodTTH = num2;
                    CommonData.stHeroData.astHeroList[index].astTalentDetail = enumerator.Current.TalentArr;
                    CommonData.stHeroData.astHeroList[index].dwGhostLevel = (uint) enumerator.Current.actorHero.handle.ValueComponent.actorSoulLevel;
                    this.CreateHeroBaseInfoData(enumerator.Current, ref CommonData.stHeroData.astHeroList[index].stHeroDetailInfo);
                    this.CreateBattleStatisticInfoData(enumerator.Current, ref CommonData.stHeroData.astHeroList[index].stHeroBattleInfo);
                    this.CreateSkillDetailInfoData(enumerator.Current, ref CommonData.stHeroData.astHeroList[index].astSkillStatisticInfo, ref CommonData.stHeroData.astHeroList[index].stHeroBattleInfo);
                    PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(playerId).Captain;
                    if (captain.handle.EquipComponent != null)
                    {
                        Dictionary<ushort, uint> equipBoughtHistory = captain.handle.EquipComponent.GetEquipBoughtHistory();
                        byte count = (byte) equipBoughtHistory.Count;
                        count = (count >= 30) ? ((byte) 30) : count;
                        CommonData.stHeroData.astHeroList[index].bInBattleEquipNum = count;
                        Dictionary<ushort, uint>.Enumerator enumerator2 = equipBoughtHistory.GetEnumerator();
                        int num4 = 0;
                        while (enumerator2.MoveNext())
                        {
                            KeyValuePair<ushort, uint> current = enumerator2.Current;
                            CommonData.stHeroData.astHeroList[index].astInBattleEquipInfo[num4].dwEquipID = current.Key;
                            KeyValuePair<ushort, uint> pair2 = enumerator2.Current;
                            CommonData.stHeroData.astHeroList[index].astInBattleEquipInfo[num4].dwFirstBuyTime = pair2.Value;
                            if (++num4 >= count)
                            {
                                break;
                            }
                        }
                    }
                    CommonData.stHeroData.astHeroList[index].stHeroBattleInfo.dwFirstMoveTime = playerKDA.m_firstMoveTime;
                    int nReviveCount = playerKDA.m_nReviveCount;
                    if (nReviveCount > 20)
                    {
                        nReviveCount = 20;
                    }
                    for (int i = 0; ((i < nReviveCount) && (i < playerKDA.m_reviveMoveTime.Length)) && (i < CommonData.stHeroData.astHeroList[index].stHeroBattleInfo.ReviveDetail.Length); i++)
                    {
                        CommonData.stHeroData.astHeroList[index].stHeroBattleInfo.ReviveDetail[i] = playerKDA.m_reviveMoveTime[i];
                    }
                    CommonData.stHeroData.astHeroList[index].stHeroBattleInfo.bReviveNum = (byte) nReviveCount;
                    CommonData.stHeroData.astHeroList[index].stHeroBattleInfo.iHeroEndLive = (enumerator.Current.actorHero.handle.ActorControl.myBehavior != ObjBehaviMode.State_Dead) ? 1 : 0;
                    CommonData.stHeroData.astHeroList[index].stHeroBattleInfo.iHeroInitHp = enumerator.Current.actorHero.handle.ValueComponent.actorHpTotal;
                    CommonData.stHeroData.astHeroList[index].stHeroBattleInfo.iHeroEndHp = enumerator.Current.actorHero.handle.ValueComponent.actorHp;
                    index = (byte) (index + 1);
                }
                CommonData.stHeroData.bNum = index;
            }
        }

        public void CreateLocalPlayer(uint playerID, ulong ullUplayerID)
        {
            Singleton<CRoleInfoManager>.GetInstance().SetMaterUUID(ullUplayerID);
            Singleton<CRoleInfoManager>.GetInstance().CreateRoleInfo(enROLEINFO_TYPE.PLAYER, ullUplayerID);
        }

        private COMDT_SETTLE_COMMON_DATA CreateReportData(uint playerId, bool bGMWin = false, bool bWin = false)
        {
            COMDT_SETTLE_COMMON_DATA commonData = new COMDT_SETTLE_COMMON_DATA();
            this.GenerateStatData();
            this.CreateHeroData(playerId, ref commonData);
            this.CreateDestroyReportData(ref commonData, bGMWin);
            this.CreateSettleReportData(playerId, ref commonData, bGMWin, bWin);
            this.CreateAttrReportData(ref commonData, bGMWin);
            this.CreateBattleNonHeroData(ref commonData, bGMWin);
            this.CreateGeneralData(playerId, ref commonData, bGMWin);
            return commonData;
        }

        private void CreateSettleReportData(uint playerId, ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin, bool bWin)
        {
            PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(playerId);
            CBattleDeadStat battleDeadStat = Singleton<BattleStatistic>.instance.m_battleDeadStat;
            int num = 0;
            int pentaKillNum = 0;
            int quataryKillNum = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            while (enumerator.MoveNext())
            {
                num += enumerator.Current.LegendaryNum;
                pentaKillNum = enumerator.Current.PentaKillNum;
                quataryKillNum = enumerator.Current.QuataryKillNum;
                num4 += enumerator.Current.TripleKillNum;
                num5 += enumerator.Current.DoubleKillNum;
                if (enumerator.Current.bHurtTakenMost)
                {
                    flag3 = true;
                }
                if (enumerator.Current.bGetCoinMost)
                {
                    flag2 = true;
                }
                if (enumerator.Current.bHurtToHeroMost)
                {
                    flag = true;
                }
                if (enumerator.Current.bAsssistMost)
                {
                    flag5 = true;
                }
                if (enumerator.Current.bKillMost)
                {
                    flag4 = true;
                }
                if (enumerator.Current.bKillOrganMost)
                {
                    flag6 = true;
                }
            }
            if (bWin)
            {
                uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
                if (mvpPlayer != 0)
                {
                    num6 = (mvpPlayer != playerKDA.PlayerId) ? 0 : 1;
                }
            }
            else
            {
                uint num9 = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
                if (num9 != 0)
                {
                    num7 = (num9 != playerKDA.PlayerId) ? 0 : 1;
                }
            }
            int index = 0;
            CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum].bReportType = 4;
            COMDT_STATISTIC_BASE_STRUCT[] astDetail = CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum].astDetail;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 1;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = playerKDA.numKill;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 2;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = playerKDA.numDead;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 4;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = playerKDA.numKillMonster;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 3;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = playerKDA.numAssist;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 5;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = Singleton<StarSystem>.GetInstance().GetStarBits();
            if (bGMWin)
            {
                astDetail[index].stValueInfo.ValueDetail[0] = 7;
            }
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 6;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ActorTypeDef.Actor_Type_Organ, 0);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 7;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ActorTypeDef.Actor_Type_Organ, 1);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 8;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ActorTypeDef.Actor_Type_Organ, 0);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 9;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ActorTypeDef.Actor_Type_Organ, 1);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 10;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetKillDragonNum(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 11;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetKillDragonNum(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 12;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetAllMonsterDeadNum();
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 13;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = num6;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 14;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = num7;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 15;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = num;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x10;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = num5;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x11;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = num4;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x1b;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = quataryKillNum;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x1c;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = pentaKillNum;
            index++;
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                astDetail[index].stKeyInfo.bKeyNum = 1;
                astDetail[index].stKeyInfo.KeyDetail[0] = 0x13;
                astDetail[index].stValueInfo.bValueNum = 1;
                astDetail[index].stValueInfo.ValueDetail[0] = (int) Singleton<CBattleSystem>.GetInstance().m_MaxBattleFPS;
                index++;
                astDetail[index].stKeyInfo.bKeyNum = 1;
                astDetail[index].stKeyInfo.KeyDetail[0] = 20;
                astDetail[index].stValueInfo.bValueNum = 1;
                if (Singleton<CBattleSystem>.GetInstance().m_BattleFPSCount == 0f)
                {
                    Singleton<CBattleSystem>.GetInstance().m_BattleFPSCount = 1f;
                }
                astDetail[index].stValueInfo.ValueDetail[0] = (int) (Singleton<CBattleSystem>.GetInstance().m_AveBattleFPS / Singleton<CBattleSystem>.GetInstance().m_BattleFPSCount);
                index++;
                astDetail[index].stKeyInfo.bKeyNum = 1;
                astDetail[index].stKeyInfo.KeyDetail[0] = 0x15;
                astDetail[index].stValueInfo.bValueNum = 1;
                astDetail[index].stValueInfo.ValueDetail[0] = (int) Singleton<CBattleSystem>.GetInstance().m_MinBattleFPS;
                index++;
            }
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x16;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = (int) Singleton<FrameSynchr>.instance.m_MaxPing;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x17;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = (int) Singleton<FrameSynchr>.instance.m_AvePing;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x18;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = (int) Singleton<FrameSynchr>.instance.m_MinPing;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x19;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetBaronDeadCount(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x1a;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = battleDeadStat.GetBaronDeadCount(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x1d;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = !flag ? 0 : 1;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 30;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = !flag2 ? 0 : 1;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x1f;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = !flag3 ? 0 : 1;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x20;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = !flag5 ? 0 : 1;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x21;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = !flag4 ? 0 : 1;
            index++;
            astDetail[index].stKeyInfo.bKeyNum = 1;
            astDetail[index].stKeyInfo.KeyDetail[0] = 0x22;
            astDetail[index].stValueInfo.bValueNum = 1;
            astDetail[index].stValueInfo.ValueDetail[0] = !flag6 ? 0 : 1;
            index++;
            CommonData.stStatisticData.astDetail[CommonData.stStatisticData.bNum].wNum = (ushort) index;
            CommonData.stStatisticData.bNum = (byte) (CommonData.stStatisticData.bNum + 1);
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            DebugHelper.Assert(accountInfo != null, "info is null in report codes.");
            events.Add(new KeyValuePair<string, string>("OpenID", (accountInfo == null) ? "0" : accountInfo.OpenId));
            events.Add(new KeyValuePair<string, string>("BattleResult", bWin.ToString()));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("CreateSettleReportData", events, true);
        }

        private void CreateSkillDetailInfoData(HeroKDA HeroInfo, ref COMDT_SKILL_STATISTIC_INFO[] stSkillDetailInfo, ref COMDT_HERO_BATTLE_STATISTIC_INFO stBattleStatisticInfo)
        {
            if (((HeroInfo.actorHero != 0) && (HeroInfo.actorHero.handle.SkillControl != null)) && ((HeroInfo.actorHero.handle.SkillControl.stSkillStat != null) && (HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo != null)))
            {
                SKILLSTATISTICTINFO[] skillStatistictInfo = HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo;
                SkillSlot[] skillSlotArray = HeroInfo.actorHero.handle.SkillControl.SkillSlotArray;
                for (int i = 0; i < 5; i++)
                {
                    if (skillSlotArray[i] != null)
                    {
                        stSkillDetailInfo[i].iSkillCfgID = skillStatistictInfo[i].iSkillCfgID;
                        stSkillDetailInfo[i].dwUsedTimes = skillStatistictInfo[i].uiUsedTimes;
                        stSkillDetailInfo[i].dwCDIntervalMin = skillStatistictInfo[i].uiCDIntervalMin;
                        stSkillDetailInfo[i].dwAttackDistanceMax = (uint) skillStatistictInfo[i].iAttackDistanceMax;
                        stSkillDetailInfo[i].dwHurtMax = (uint) skillStatistictInfo[i].iHurtMax;
                        stSkillDetailInfo[i].iHurtValue = skillStatistictInfo[i].ihurtValue;
                        stSkillDetailInfo[i].iAdValue = skillStatistictInfo[i].iadValue;
                        stSkillDetailInfo[i].iApValue = skillStatistictInfo[i].iapValue;
                        stSkillDetailInfo[i].iHpValue = skillStatistictInfo[i].ihpValue;
                        stSkillDetailInfo[i].iLoseHpValue = skillStatistictInfo[i].iloseHpValue;
                        stSkillDetailInfo[i].iHurtCount = skillStatistictInfo[i].ihurtCount;
                        stSkillDetailInfo[i].iHemoFadeRate = skillStatistictInfo[i].ihemoFadeRate;
                        stSkillDetailInfo[i].bSkillLevel = (byte) skillSlotArray[i].GetSkillLevel();
                        stSkillDetailInfo[i].iTotalHurtValue = skillStatistictInfo[i].iHurtToHeroTotal;
                        stSkillDetailInfo[i].iCDMax = (int) skillStatistictInfo[i].uiCDIntervalMax;
                        if (skillStatistictInfo[i].uiCDIntervalMin != uint.MaxValue)
                        {
                            stSkillDetailInfo[i].iCDMin = (int) skillStatistictInfo[i].uiCDIntervalMin;
                        }
                        uint num3 = (uint) (Singleton<FrameSynchr>.instance.LogicFrameTick - ((ulong) skillSlotArray[i].lLastUseTime));
                        skillStatistictInfo[i].uiCDIntervalMax = (skillStatistictInfo[i].uiCDIntervalMax <= num3) ? num3 : skillStatistictInfo[i].uiCDIntervalMax;
                        if (i > 0)
                        {
                            stBattleStatisticInfo.iHeroSkillDamageTotal += skillStatistictInfo[i].iHurtTotal;
                            stBattleStatisticInfo.iHeroSkillCount += (int) stSkillDetailInfo[i].dwUsedTimes;
                            stBattleStatisticInfo.iHeroCDMax = Math.Max(stBattleStatisticInfo.iHeroCDMax, (int) skillStatistictInfo[i].uiCDIntervalMax);
                            if ((skillStatistictInfo[i].uiUsedTimes > 1) && (skillStatistictInfo[i].uiCDIntervalMin != uint.MaxValue))
                            {
                                if (stBattleStatisticInfo.iHeroCDMin <= 0)
                                {
                                    stBattleStatisticInfo.iHeroCDMin = (int) skillStatistictInfo[i].uiCDIntervalMin;
                                }
                                else
                                {
                                    stBattleStatisticInfo.iHeroCDMin = Math.Min(stBattleStatisticInfo.iHeroCDMin, (int) skillStatistictInfo[i].uiCDIntervalMin);
                                }
                            }
                            stBattleStatisticInfo.iHeroSkillDamageMax1 = Math.Max(stBattleStatisticInfo.iHeroSkillDamageMax1, skillStatistictInfo[i].iHitAllHurtTotalMax);
                            if (skillStatistictInfo[i].iHitAllHurtTotalMin != -1)
                            {
                                if (stBattleStatisticInfo.iHeroSkillDamageMin1 <= 0)
                                {
                                    stBattleStatisticInfo.iHeroSkillDamageMin1 = skillStatistictInfo[i].iHitAllHurtTotalMin;
                                }
                                else
                                {
                                    stBattleStatisticInfo.iHeroSkillDamageMin1 = Math.Min(stBattleStatisticInfo.iHeroSkillDamageMin1, skillStatistictInfo[i].iHitAllHurtTotalMin);
                                }
                            }
                            stBattleStatisticInfo.iHeroSkillDamageMax2 = Math.Max(stBattleStatisticInfo.iHeroSkillDamageMax2, skillStatistictInfo[i].iHurtMax);
                            if (skillStatistictInfo[i].iHurtMin >= 0)
                            {
                                if (stBattleStatisticInfo.iHeroSkillDamageMin2 == 0)
                                {
                                    stBattleStatisticInfo.iHeroSkillDamageMin2 = skillStatistictInfo[i].iHurtMin;
                                }
                                else
                                {
                                    stBattleStatisticInfo.iHeroSkillDamageMin2 = Math.Min(stBattleStatisticInfo.iHeroSkillDamageMin2, skillStatistictInfo[i].iHurtMin);
                                }
                            }
                            stBattleStatisticInfo.iHeroSkillDamageCountMax = Math.Max(stBattleStatisticInfo.iHeroSkillDamageCountMax, skillStatistictInfo[i].iHitCountMax);
                            if (skillStatistictInfo[i].iHitCountMin >= 0)
                            {
                                if (stBattleStatisticInfo.iHeroSkillDamageCountMin == 0)
                                {
                                    stBattleStatisticInfo.iHeroSkillDamageCountMin = skillStatistictInfo[i].iHitCountMin;
                                }
                                else
                                {
                                    stBattleStatisticInfo.iHeroSkillDamageCountMin = Math.Min(stBattleStatisticInfo.iHeroSkillDamageCountMin, skillStatistictInfo[i].iHitCountMin);
                                }
                            }
                            if (i == 4)
                            {
                                stBattleStatisticInfo.iHeroCureCount = (int) stSkillDetailInfo[i].dwUsedTimes;
                            }
                        }
                        else
                        {
                            stBattleStatisticInfo.iHeroAttackCount = (int) stSkillDetailInfo[i].dwUsedTimes;
                            stBattleStatisticInfo.iHeroDamageMax = (int) stSkillDetailInfo[i].dwHurtMax;
                            if (skillStatistictInfo[i].iHurtMin != -1)
                            {
                                stBattleStatisticInfo.iHeroDamageMin = skillStatistictInfo[i].iHurtMin;
                            }
                            stBattleStatisticInfo.iHeroAttackCountTotal += skillStatistictInfo[i].iHurtTotal;
                        }
                    }
                }
            }
        }

        private void FakeStarInfoForGMWin(ref COMDT_SETTLE_COMMON_DATA CommonData)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            DebugHelper.Assert(curLvelContext != null);
            if (curLvelContext != null)
            {
                for (int i = 0; i < curLvelContext.m_starDetail.Length; i++)
                {
                    int iParam = curLvelContext.m_starDetail[i].iParam;
                    if (iParam == 0)
                    {
                        return;
                    }
                    ResEvaluateStarInfo dataByKey = GameDataMgr.evaluateCondInfoDatabin.GetDataByKey((uint) iParam);
                    if (dataByKey != null)
                    {
                        COMDT_STATISTIC_STRUCT_PILE statRef = null;
                        for (int j = 0; j < CommonData.stStatisticData.bNum; j++)
                        {
                            if (CommonData.stStatisticData.astDetail[j].bReportType == 1)
                            {
                                statRef = CommonData.stStatisticData.astDetail[j];
                                break;
                            }
                        }
                        if (statRef == null)
                        {
                            int bNum = CommonData.stStatisticData.bNum;
                            statRef = CommonData.stStatisticData.astDetail[bNum];
                            statRef.bReportType = 1;
                            CommonData.stStatisticData.bNum = (byte) (CommonData.stStatisticData.bNum + 1);
                        }
                        COMDT_STATISTIC_BASE_STRUCT[] astDetail = statRef.astDetail;
                        for (int k = 0; k < dataByKey.astConditions.Length; k++)
                        {
                            if (dataByKey.astConditions[k].dwType == 1)
                            {
                                this.FakeWithCondition(ref dataByKey.astConditions[k], ref statRef, ref astDetail);
                            }
                        }
                    }
                }
            }
        }

        private int FakeValue(int Comparation, int CfgValue)
        {
            switch (Comparation)
            {
                case 1:
                case 2:
                    return (CfgValue - 1);

                case 3:
                    return CfgValue;

                case 4:
                case 5:
                    return (CfgValue + 1);
            }
            DebugHelper.Assert(false, "what the fuck?");
            return 0;
        }

        private void FakeWithCondition(ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_STRUCT_PILE StatRef, ref COMDT_STATISTIC_BASE_STRUCT[] DetailData)
        {
            int num = ConditionInfo.KeyDetail[0];
            int num2 = ConditionInfo.KeyDetail[1];
            int num3 = ConditionInfo.KeyDetail[2];
            for (int i = 0; i < DetailData.Length; i++)
            {
                COMDT_STATISTIC_BASE_STRUCT data = DetailData[i];
                if (data.stKeyInfo.bKeyNum <= 0)
                {
                    break;
                }
                if (((data.stKeyInfo.KeyDetail[0] == num) && (data.stKeyInfo.KeyDetail[1] == num2)) && (data.stKeyInfo.KeyDetail[2] == num3))
                {
                    this.FakeWithValues(ref ConditionInfo, ref data);
                    return;
                }
            }
            DetailData[StatRef.wNum].stKeyInfo.bKeyNum = 3;
            DetailData[StatRef.wNum].stKeyInfo.KeyDetail[0] = num;
            DetailData[StatRef.wNum].stKeyInfo.KeyDetail[1] = num2;
            DetailData[StatRef.wNum].stKeyInfo.KeyDetail[2] = num3;
            this.FakeWithValues(ref ConditionInfo, ref DetailData[StatRef.wNum]);
            StatRef.wNum = (ushort) (StatRef.wNum + 1);
        }

        private void FakeWithValues(ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_BASE_STRUCT Data)
        {
            for (int i = 0; i < ConditionInfo.ValueDetail.Length; i++)
            {
                if (ConditionInfo.ValueDetail[i] == 0)
                {
                    break;
                }
                Data.stValueInfo.bValueNum = (byte) (i + 1);
                Data.stValueInfo.ValueDetail[i] = this.FakeValue(ConditionInfo.ComparetorDetail[i], ConditionInfo.ValueDetail[i]);
            }
        }

        private void FillVDStat(ref COMDT_SETTLE_GAME_GENERAL_INFO stGeneralData, uint playerID)
        {
            if (Singleton<BattleStatistic>.instance.m_vdStat != null)
            {
                Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(playerID);
                if (player != null)
                {
                    COM_PLAYERCAMP inTo = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
                    if (player.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                    {
                        inTo = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                    }
                    else if (player.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        inTo = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                    }
                    stGeneralData.bAdvantageNum = (byte) Singleton<BattleStatistic>.instance.m_vdStat.count;
                    for (int i = 0; i < stGeneralData.bAdvantageNum; i++)
                    {
                        Singleton<BattleStatistic>.instance.m_vdStat.GetMaxCampStat(i, player.PlayerCamp, inTo, out stGeneralData.astAdvantageDetail[i].iMaxAdvantageValue, out stGeneralData.astAdvantageDetail[i].iMaxDisadvantageValue);
                    }
                    stGeneralData.iCurrentDisparity = Singleton<BattleStatistic>.instance.m_vdStat.CalcCampStat(player.PlayerCamp, inTo);
                    stGeneralData.bSelfCampHaveWinningFlag = !Singleton<BattleStatistic>.instance.bSelfCampHaveWinningFlag ? ((byte) 0) : ((byte) 1);
                }
            }
        }

        private void GenerateStatData()
        {
            Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GenerateStatData();
        }

        public void GotoAccLoginPage()
        {
            Singleton<GameBuilder>.GetInstance().EndGame();
            Singleton<LobbySvrMgr>.GetInstance().isLogin = false;
            Singleton<LobbyLogic>.GetInstance().isLogin = false;
            Singleton<LobbySvrMgr>.GetInstance().isFirstLogin = false;
            if (!Singleton<ApolloHelper>.GetInstance().Logout())
            {
                Singleton<ApolloHelper>.GetInstance().Logout();
            }
            Singleton<GameStateCtrl>.GetInstance().GotoState("LoginState");
            Singleton<GameLogic>.GetInstance().OnPlayerLogout();
        }

        public override void Init()
        {
            this.isLogin = false;
            this.inMultiRoom = false;
            this._settleMsgTimer = 0;
            this._gameEndTimer = 0;
            this._settlePanelTimer = 0;
            CUIManager instance = Singleton<CUIManager>.GetInstance();
            instance.onFormSorted = (CUIManager.OnFormSorted) Delegate.Combine(instance.onFormSorted, new CUIManager.OnFormSorted(this.OnUiFormUpdate));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_SingleGameFinishError, new CUIEventManager.OnUIEventHandler(this.OnClickSingleGameFinishError));
        }

        public static bool IsLobbyFormPure()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
            return ((null != form) && !form.IsHided());
        }

        public void LateUpdate()
        {
            if (this.m_bShouldNewbieEnterHall)
            {
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.enterHall, new uint[0]);
                CLobbySystem.AutoPopAllow = !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding;
                this.m_bShouldNewbieEnterHall = false;
            }
            if (this.m_bShowNewbieEnterTaskForm)
            {
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterTaskForm, new uint[0]);
                this.m_bShowNewbieEnterTaskForm = false;
            }
            if (this.m_bShowNewbieEnterExploreForm)
            {
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterExploreForm, new uint[0]);
                this.m_bShowNewbieEnterExploreForm = false;
            }
            if (this.m_bShowNewbieEnterSymbolForm)
            {
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterSymbolForm, new uint[0]);
                this.m_bShowNewbieEnterSymbolForm = false;
            }
            Singleton<CLobbySystem>.GetInstance().CheckMianLiu();
        }

        public void LoginGame()
        {
            if (!this.isLogin)
            {
                Singleton<GameLogic>.GetInstance().ClearLogicData();
                string[] exceptFormNames = new string[] { CLoginSystem.s_splashFormPath, CLoginSystem.sLoginFormPath, CLobbySystem.LOBBY_FORM_PATH, CLobbySystem.SYSENTRY_FORM_PATH, CChatController.ChatFormPath, CLobbySystem.RANKING_BTN_FORM_PATH };
                Singleton<CUIManager>.GetInstance().CloseAllForm(exceptFormNames, true, true);
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3ea);
                uint versionNumber = CVersion.GetVersionNumber(GameFramework.AppVersion);
                msg.stPkgData.stGameLoginReq.iCltAppVersion = (int) versionNumber;
                uint num2 = CVersion.GetVersionNumber(CVersion.GetUsedResourceVersion());
                msg.stPkgData.stGameLoginReq.iCltResVersion = (int) num2;
                Utility.StringToByteArray(CVersion.GetBuildNumber(), ref msg.stPkgData.stGameLoginReq.stClientInfo.szCltBuildNumber);
                Utility.StringToByteArray(CVersion.GetRevisonNumber(), ref msg.stPkgData.stGameLoginReq.stClientInfo.szCltSvnVersion);
                Utility.StringToByteArray(SystemInfo.deviceName + " " + SystemInfo.deviceModel, ref msg.stPkgData.stGameLoginReq.stClientInfo.szSystemHardware);
                Utility.StringToByteArray(SystemInfo.operatingSystem, ref msg.stPkgData.stGameLoginReq.stClientInfo.szSystemSoftware);
                switch (Application.internetReachability)
                {
                    case NetworkReachability.ReachableViaCarrierDataNetwork:
                        Utility.StringToByteArray("2G/3G", ref msg.stPkgData.stGameLoginReq.stClientInfo.szNetwork);
                        break;

                    case NetworkReachability.ReachableViaLocalAreaNetwork:
                        Utility.StringToByteArray("Wi-Fi/LAN", ref msg.stPkgData.stGameLoginReq.stClientInfo.szNetwork);
                        break;
                }
                Utility.StringToByteArray(GameFramework.AppVersion, ref msg.stPkgData.stGameLoginReq.stClientInfo.szClientVersion);
                ulong num3 = 0L;
                if (PlayerPrefs.HasKey("visitorUid"))
                {
                    string s = PlayerPrefs.GetString("visitorUid", "NotFound");
                    if (s != "NotFound")
                    {
                        num3 = ulong.Parse(s);
                    }
                }
                msg.stPkgData.stGameLoginReq.ullVisitorUid = num3;
                msg.stPkgData.stGameLoginReq.stClientInfo.iLoginChannel = MonoSingleton<IDIPSys>.GetInstance().m_ChannelID;
                msg.stPkgData.stGameLoginReq.stClientInfo.iMemorySize = SystemInfo.systemMemorySize;
                msg.stPkgData.stGameLoginReq.iLogicWorldID = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
                msg.stPkgData.stGameLoginReq.dwTaccZoneID = (uint) MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.nodeID;
                msg.stPkgData.stGameLoginReq.bPrivilege = (byte) Singleton<ApolloHelper>.GetInstance().GetCurrentLoginPrivilege();
                byte[] bytes = Encoding.ASCII.GetBytes(SystemInfo.deviceUniqueIdentifier);
                if (bytes.Length > 0)
                {
                    Buffer.BlockCopy(bytes, 0, msg.stPkgData.stGameLoginReq.szCltIMEI, 0, (bytes.Length <= 0x40) ? bytes.Length : 0x40);
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        protected void OnClickSingleGameFinishError(CUIEvent e)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<GameBuilder>.instance.EndGame();
        }

        private void onGameEndTimer(int seq)
        {
            DefaultGameEventParam prm = new DefaultGameEventParam();
            Singleton<BattleLogic>.GetInstance().onGameEnd(ref prm);
        }

        private void onLobbyConnected(object sender)
        {
            Debug.Log("onLobbyConnected");
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        private void onLobbyDisconnected(object sender)
        {
            Debug.Log("onLobbyDisconnected");
        }

        public void OnSendSingleGameFinishFail()
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBox("网络异常，点击确定返回大厅。", enUIEventID.Net_SingleGameFinishError, false);
        }

        private void onSettleMsgTimer(int seq)
        {
            Singleton<GameBuilder>.GetInstance().EndGame();
            Singleton<CUIManager>.GetInstance().OpenTips("disconWithServer", true, 2f, null, new object[0]);
        }

        private void onSettlePanelTimer(int seq)
        {
            Singleton<GameBuilder>.GetInstance().EndGame();
            Singleton<CUIManager>.GetInstance().OpenTips("cannotShowSettle", true, 2f, null, new object[0]);
            BuglyAgent.ReportException(new SettlementError("SettlementTimeout"), "SettlementError: mask= " + Convert.ToString((long) Singleton<BattleLogic>.GetInstance().BattleStepMask, 0x10));
        }

        private void OnUiFormUpdate(ListView<CUIFormScript> inForms)
        {
            if (inForms != null)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
                CUIFormScript item = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
                CUIFormScript script3 = Singleton<CUIManager>.GetInstance().GetForm(CChatController.ChatFormPath);
                CUIFormScript script4 = Singleton<CUIManager>.GetInstance().GetForm(CChatController.ChatEntryPath);
                CUIFormScript script5 = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FPS_FORM_PATH);
                CUIFormScript script6 = Singleton<CUIManager>.GetInstance().GetForm(CUIParticleSystem.s_qualifyingFormPath);
                CUIFormScript script7 = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Dialog/Form_WeakGuide");
                CUIFormScript script8 = Singleton<CUIManager>.GetInstance().GetForm(CFunctionUnlockSys.FUC_UNLOCK_FORM_PATH);
                CUIFormScript script9 = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Dialog/Form_GuideMask");
                CUIFormScript script10 = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FORM_MESSAGE_BOX);
                CUIFormScript script11 = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FORM_SENDING_ALERT);
                CUIFormScript inTesterForm = Singleton<CUIManager>.instance.GetForm(CTaskSys.TASK_FORM_PATH);
                CUIFormScript script13 = Singleton<CUIManager>.instance.GetForm(CAdventureSys.EXLPORE_FORM_PATH);
                CUIFormScript script14 = Singleton<CUIManager>.instance.GetForm(CSymbolSystem.s_symbolFormPath);
                CUIFormScript script15 = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.RANKING_BTN_FORM_PATH);
                ListView<CUIFormScript> view = new ListView<CUIFormScript>();
                if (form != null)
                {
                    view.Add(form);
                }
                if (item != null)
                {
                    view.Add(item);
                }
                if (script3 != null)
                {
                    view.Add(script3);
                }
                if (script4 != null)
                {
                    view.Add(script4);
                }
                if (script5 != null)
                {
                    view.Add(script5);
                }
                if (script6 != null)
                {
                    view.Add(script6);
                }
                if (script7 != null)
                {
                    view.Add(script7);
                }
                if (script15 != null)
                {
                    view.Add(script15);
                }
                bool flag = true;
                ListView<CUIFormScript>.Enumerator enumerator = inForms.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!view.Contains(enumerator.Current))
                    {
                        flag = false;
                        break;
                    }
                }
                if (((flag && inForms.Contains(form)) && (inForms.Contains(item) && !inForms.Contains(script8))) && Singleton<GameStateCtrl>.GetInstance().isLobbyState)
                {
                    this.m_bShouldNewbieEnterHall = true;
                }
                else
                {
                    this.m_bShouldNewbieEnterHall = false;
                }
                if (this.m_bShouldNewbieEnterHall)
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                    if (((masterRoleInfo != null) && masterRoleInfo.IsOldPlayer()) && !masterRoleInfo.IsOldPlayerGuided())
                    {
                        Singleton<CBattleGuideManager>.instance.OpenOldPlayerFirstForm();
                    }
                }
                if (script9 != null)
                {
                    view.Add(script9);
                }
                flag = true;
                enumerator = inForms.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!view.Contains(enumerator.Current))
                    {
                        flag = false;
                        break;
                    }
                }
                if (((flag && inForms.Contains(form)) && (inForms.Contains(item) && !inForms.Contains(script8))) && (Singleton<GameStateCtrl>.GetInstance().isLobbyState && Singleton<LobbySvrMgr>.GetInstance().isLogin))
                {
                    Singleton<EventRouter>.instance.BroadCastEvent("FucUnlockConditionChanged");
                }
                if (script10 != null)
                {
                    view.Add(script10);
                }
                if (script11 != null)
                {
                    view.Add(script11);
                }
                flag = true;
                enumerator = inForms.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!view.Contains(enumerator.Current))
                    {
                        flag = false;
                        break;
                    }
                }
                if ((flag && inForms.Contains(form)) && (inForms.Contains(item) && Singleton<GameStateCtrl>.GetInstance().isLobbyState))
                {
                    if (!this.CanShowRolling)
                    {
                        this.CanShowRolling = true;
                        Singleton<CRollingSystem>.GetInstance().StartRolling();
                    }
                }
                else if (this.CanShowRolling)
                {
                    this.CanShowRolling = false;
                    Singleton<CRollingSystem>.GetInstance().StopRolling();
                }
                ListView<CUIFormScript> inExcludeForms = new ListView<CUIFormScript>();
                inExcludeForms.Add(script6);
                inExcludeForms.Add(script5);
                inExcludeForms.Add(script7);
                inExcludeForms.Add(item);
                this.m_bShowNewbieEnterTaskForm = TestTopFormCustom(inTesterForm, inExcludeForms);
                this.m_bShowNewbieEnterExploreForm = TestTopFormCustom(script13, inExcludeForms);
                this.m_bShowNewbieEnterSymbolForm = TestTopFormCustom(script14, inExcludeForms);
            }
        }

        public void OpenLobby()
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_OpenLobbyForm);
            Singleton<GameInput>.GetInstance().ChangeLobbyMode();
            Singleton<CChatController>.GetInstance();
            Singleton<CChatController>.GetInstance().CreateForm();
            Singleton<CChatController>.GetInstance().ShowPanel(true, false);
            Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0L, 0);
            Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
            Singleton<CChatController>.GetInstance().bSendChat = true;
            Singleton<CLoudSpeakerSys>.instance.StartReqTimer();
            MonoSingleton<ShareSys>.GetInstance().SendShareDataMsg();
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_PrepareFight_Sub);
        }

        public void ReqMultiGameOver(bool ClickGameOver = false)
        {
            BattleLogic instance = Singleton<BattleLogic>.GetInstance();
            instance.BattleStepMask |= 8;
            BattleStatistic battleStat = Singleton<BattleLogic>.GetInstance().battleStat;
            if (this.isLogin && (battleStat != null))
            {
                if (ClickGameOver)
                {
                    battleStat.iBattleResult = 2;
                }
                List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
                COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
                int num = 0;
                int num2 = 0;
                switch (battleStat.iBattleResult)
                {
                    case 1:
                        if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            num = 1;
                            num2 = 2;
                        }
                        else
                        {
                            num = 2;
                            num2 = 1;
                        }
                        break;

                    case 2:
                        if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            num = 2;
                            num2 = 1;
                        }
                        else
                        {
                            num = 1;
                            num2 = 2;
                        }
                        break;

                    default:
                        num = 3;
                        num2 = 3;
                        break;
                }
                int index = 0;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x438);
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    if (!allPlayers[i].Computer)
                    {
                        if (allPlayers[i].PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            msg.stPkgData.stMultGameOverReq.astAcntInfo[index].stBattleParam.bGameResult = (byte) num;
                        }
                        else
                        {
                            msg.stPkgData.stMultGameOverReq.astAcntInfo[index].stBattleParam.bGameResult = (byte) num2;
                        }
                        if (allPlayers[i] == Singleton<GamePlayerCenter>.instance.GetHostPlayer())
                        {
                            msg.stPkgData.stMultGameOverReq.astAcntInfo[index].stBattleParam.wNetworkMode = (ushort) this.CheckWifi();
                        }
                        else
                        {
                            msg.stPkgData.stMultGameOverReq.astAcntInfo[index].stBattleParam.wNetworkMode = 0;
                        }
                        bool bGMWin = msg.stPkgData.stMultGameOverReq.astAcntInfo[index].stBattleParam.bGameResult == 1;
                        msg.stPkgData.stMultGameOverReq.astAcntInfo[index].dwAcntObjID = allPlayers[i].PlayerId;
                        msg.stPkgData.stMultGameOverReq.astAcntInfo[index].stCommonData = this.CreateReportData(allPlayers[i].PlayerId, bGMWin, bGMWin);
                        index++;
                        if (index > 4)
                        {
                            msg.stPkgData.stMultGameOverReq.bAcntNum = (byte) index;
                            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0x439);
                            index = 0;
                            msg = NetworkModule.CreateDefaultCSPKG(0x438);
                        }
                    }
                }
                if (index > 0)
                {
                    msg.stPkgData.stMultGameOverReq.bAcntNum = (byte) index;
                    Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0x439);
                    index = 0;
                }
                BattleLogic local2 = Singleton<BattleLogic>.GetInstance();
                local2.BattleStepMask |= 0x10;
            }
        }

        public void ReqMultiGameRunaway()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x43e);
            msg.stPkgData.stRunAwayReq = new CSPKG_MULTGAME_RUNAWAY_REQ();
            Singleton<NetworkModule>.instance.SendGameMsg(ref msg, 0);
        }

        public void ReqQuitMultiGame()
        {
            if (this.isLogin && this.inMultiRoom)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3ff);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public void ReqSelectHero(uint id)
        {
            if (this.isLogin)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x708);
                msg.stPkgData.stAchieveHeroReq.dwHeroId = id;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
        }

        public void ReqSingleGameFinish(bool ClickGameOver = false, bool bGmWin = false)
        {
            if (this.isLogin)
            {
                BattleStatistic battleStat = Singleton<BattleLogic>.GetInstance().battleStat;
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext != null)
                {
                    byte num = 0;
                    if (bGmWin)
                    {
                        num = 1;
                    }
                    else if (curLvelContext.IsMobaMode() && !curLvelContext.IsFireHolePlayMode())
                    {
                        num = !Singleton<WinLose>.instance.LastSingleGameWin ? ((byte) 2) : ((byte) 1);
                    }
                    else if (Singleton<StarSystem>.instance.isFailure)
                    {
                        num = 2;
                    }
                    else
                    {
                        num = !Singleton<WinLose>.instance.LastSingleGameWin ? ((byte) 2) : ((byte) 1);
                    }
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x41c);
                    msg.stPkgData.stFinSingleGameReq.stBattleParam.bGameType = (byte) curLvelContext.GetGameType();
                    msg.stPkgData.stFinSingleGameReq.stBattleParam.iLevelID = curLvelContext.m_mapID;
                    msg.stPkgData.stFinSingleGameReq.stBattleParam.dwUsedTime = ((uint) Singleton<FrameSynchr>.GetInstance().LogicFrameTick) / 0x3e8;
                    msg.stPkgData.stFinSingleGameReq.stBattleParam.bGameResult = num;
                    msg.stPkgData.stFinSingleGameReq.stCommonData = this.CreateReportData(Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerId, bGmWin, num == 1);
                    if (curLvelContext.IsGameTypeAdventure())
                    {
                        msg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure = new COMDT_SINGLE_GAME_PARAM_ADVENTURE();
                        msg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure.bChapterNo = (byte) curLvelContext.m_chapterNo;
                        msg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure.bLevelNo = curLvelContext.m_levelNo;
                        msg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure.bDifficultType = (byte) curLvelContext.m_levelDifficulty;
                    }
                    else if (curLvelContext.IsGameTypeBurning())
                    {
                        BurnExpeditionUT.Build_Burn_BattleParam(msg.stPkgData.stFinSingleGameReq.stBattleParam, ClickGameOver);
                        Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Burning Game Client Requset Ending Package", null, true);
                    }
                    if (!bGmWin)
                    {
                        if (ClickGameOver)
                        {
                            msg.stPkgData.stFinSingleGameReq.bPressExit = 1;
                            battleStat.iBattleResult = 2;
                        }
                        else
                        {
                            msg.stPkgData.stFinSingleGameReq.bPressExit = 0;
                        }
                    }
                    Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SyncNewbieAchieveToSvr(false);
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                }
            }
        }

        public void ReqStartGuideLevel(int inGuideLevelId, uint heroID)
        {
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x41a);
                if (GameDataMgr.levelDatabin.GetDataByKey((long) inGuideLevelId) == null)
                {
                    Debug.LogError("ResLevelCfgInfo can not find id = " + inGuideLevelId);
                }
                msg.stPkgData.stStartSingleGameReq.stBattleParam.bGameType = 2;
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.construct(2L);
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfGuide.iLevelID = inGuideLevelId;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = 1;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList[0] = heroID;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                masterRoleInfo.battleHeroList.Clear();
                masterRoleInfo.battleHeroList.Add(heroID);
                ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) inGuideLevelId);
                object[] inParameters = new object[] { inGuideLevelId };
                DebugHelper.Assert(dataByKey != null, "Can't find level config with ID -- {0}", inParameters);
                CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer = msg.stPkgData.stStartSingleGameReq.stBattlePlayer;
                stBattlePlayer.astFighter[0].bPosOfCamp = 0;
                stBattlePlayer.astFighter[0].bObjType = 1;
                stBattlePlayer.astFighter[0].bObjCamp = 1;
                stBattlePlayer.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = heroID;
                int index = 1;
                for (int i = 0; i < dataByKey.SelfCampAIHeroID.Length; i++)
                {
                    if (dataByKey.SelfCampAIHeroID[i] != 0)
                    {
                        stBattlePlayer.astFighter[index].bPosOfCamp = (byte) (i + 1);
                        stBattlePlayer.astFighter[index].bObjType = 2;
                        stBattlePlayer.astFighter[index].bObjCamp = 1;
                        stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.SelfCampAIHeroID[i];
                        index++;
                    }
                }
                for (int j = 0; j < dataByKey.AIHeroID.Length; j++)
                {
                    if (dataByKey.AIHeroID[j] != 0)
                    {
                        stBattlePlayer.astFighter[index].bPosOfCamp = (byte) j;
                        stBattlePlayer.astFighter[index].bObjType = 2;
                        stBattlePlayer.astFighter[index].bObjCamp = 2;
                        stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.AIHeroID[j];
                        index++;
                    }
                }
                stBattlePlayer.bNum = (byte) index;
                if (inGuideLevelId == CBattleGuideManager.GuideLevelID1v1)
                {
                    Singleton<CSoundManager>.instance.LoadBank("Newguide_Voice", CSoundManager.BankType.Battle);
                }
                else if (inGuideLevelId == CBattleGuideManager.GuideLevelID3v3)
                {
                    Singleton<CSoundManager>.instance.LoadBank("Newguide_3V3_Voice", CSoundManager.BankType.Battle);
                }
                else if (inGuideLevelId == CBattleGuideManager.GuideLevelID5v5)
                {
                    Singleton<CSoundManager>.instance.LoadBank("Newguide_5V5_Voice", CSoundManager.BankType.Battle);
                }
                else if (inGuideLevelId == CBattleGuideManager.GuideLevelIDCasting)
                {
                    Singleton<CSoundManager>.instance.LoadBank("Newguide_Voice_Rotate", CSoundManager.BankType.Battle);
                }
                else if (inGuideLevelId == CBattleGuideManager.GuideLevelIDJungle)
                {
                    Singleton<CSoundManager>.instance.LoadBank("Newguide_Voice_WildMonster", CSoundManager.BankType.Battle);
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void ReqStartGuideLevel11(bool bReEntry)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x72).dwConfValue;
            int inGuideLevelId = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x74).dwConfValue;
            Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(inGuideLevelId, dwConfValue);
            CLobbySystem.AutoPopAllow = false;
            Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
        }

        public static void ReqStartGuideLevel33(bool bReEntry)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x90).dwConfValue;
            int inGuideLevelId = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x75).dwConfValue;
            Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(inGuideLevelId, dwConfValue);
            CLobbySystem.AutoPopAllow = false;
            Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
            Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
        }

        public static void ReqStartGuideLevel55(bool bReEntry)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x91).dwConfValue;
            int inGuideLevelId = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x76).dwConfValue;
            Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(inGuideLevelId, dwConfValue);
            CLobbySystem.AutoPopAllow = false;
            Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
            Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
        }

        public static void ReqStartGuideLevelCasting(bool bReEntry)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x92).dwConfValue;
            int inGuideLevelId = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x77).dwConfValue;
            Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(inGuideLevelId, dwConfValue);
            CLobbySystem.AutoPopAllow = false;
            Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
            Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
        }

        public static void ReqStartGuideLevelJungle(bool bReEntry)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x73).dwConfValue;
            int inGuideLevelId = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x79).dwConfValue;
            Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(inGuideLevelId, dwConfValue);
            CLobbySystem.AutoPopAllow = false;
            Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
            Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
        }

        public static void ReqStartGuideLevelSelHero(bool bReEntry, int levelId)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) levelId);
            object[] inParameters = new object[] { levelId };
            DebugHelper.Assert(dataByKey != null, "Can't find level config -- ID: {0}", inParameters);
            DebugHelper.Assert(Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null, "Master role info is NULL!");
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
                Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
                Singleton<CAdventureSys>.instance.currentLevelId = levelId;
                Singleton<CAdventureSys>.instance.currentDifficulty = 1;
                CSDT_SINGLE_GAME_OF_ADVENTURE reportInfo = new CSDT_SINGLE_GAME_OF_ADVENTURE {
                    iLevelID = dataByKey.iCfgID,
                    bChapterNo = (byte) dataByKey.iChapterId,
                    bLevelNo = dataByKey.bLevelNo,
                    bDifficultType = 1
                };
                Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithAdventure(dataByKey.dwBattleListID, reportInfo, StringHelper.UTF8BytesToString(ref dataByKey.szName));
                Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enGuide, (byte) dataByKey.iHeroNum, 0, 0, 0);
            }
        }

        public void ReqStartMultiGame()
        {
            if (this.isLogin && this.inMultiRoom)
            {
                if (!Singleton<BattleLogic>.instance.isWaitMultiStart)
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x434);
                    Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
                }
                else
                {
                    Singleton<BattleLogic>.instance.StartFightMultiGame();
                }
            }
        }

        public void StartSettlePanelTimer()
        {
            this.StopSettlePanelTimer();
            this._settlePanelTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0x2710, 1, new CTimer.OnTimeUpHandler(this.onSettlePanelTimer));
        }

        public void StartSettleTimers()
        {
            this.StopSettleMsgTimer();
            this._settleMsgTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0x4e20, 1, new CTimer.OnTimeUpHandler(this.onSettleMsgTimer));
            this.StopGameEndTimer();
            this._gameEndTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0x3a98, 1, new CTimer.OnTimeUpHandler(this.onGameEndTimer));
            this.StopSettlePanelTimer();
        }

        public void StopGameEndTimer()
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._gameEndTimer);
        }

        public void StopSettleMsgTimer()
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._settleMsgTimer);
        }

        public void StopSettlePanelTimer()
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._settlePanelTimer);
        }

        public void SvrNtfUpdateClient()
        {
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                this.NeedUpdateClient = true;
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionIsLow"), enUIEventID.None, false);
            }
        }

        private static bool TestTopFormCustom(CUIFormScript inTesterForm, ListView<CUIFormScript> inExcludeForms)
        {
            if (inTesterForm == null)
            {
                return false;
            }
            bool flag = false;
            ListView<CUIFormScript> view = new ListView<CUIFormScript>();
            ListView<CUIFormScript> forms = Singleton<CUIManager>.instance.GetForms();
            if (forms != null)
            {
                view.AddRange(forms);
            }
            if (view.Count > 0)
            {
                if ((inExcludeForms != null) && (inExcludeForms.Count > 0))
                {
                    ListView<CUIFormScript>.Enumerator enumerator = inExcludeForms.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        view.Remove(enumerator.Current);
                    }
                }
                if (!view.Contains(inTesterForm))
                {
                    return flag;
                }
                CUIFormScript script = null;
                int count = view.Count;
                for (int i = 0; i < count; i++)
                {
                    if (view[i] != null)
                    {
                        if (script == null)
                        {
                            script = view[i];
                        }
                        else if (view[i].GetSortingOrder() > script.GetSortingOrder())
                        {
                            script = view[i];
                        }
                    }
                }
                if (script == inTesterForm)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_SingleGameFinishError, new CUIEventManager.OnUIEventHandler(this.OnClickSingleGameFinishError));
        }

        public void UpdateLogic(int delta)
        {
        }

        private void VerifyCondition(IStarCondition InCondition, ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_STRUCT_PILE StatRef, ref COMDT_STATISTIC_BASE_STRUCT[] DetailData)
        {
            int num = ConditionInfo.KeyDetail[0];
            int num2 = ConditionInfo.KeyDetail[1];
            int num3 = ConditionInfo.KeyDetail[2];
            int[] keys = InCondition.keys;
            object[] inParameters = new object[] { num, num2, num3 };
            DebugHelper.Assert((((keys.Length >= 3) && (keys[0] == num)) && (keys[1] == num2)) && (keys[2] == num3), "VerifyCondition Error: {0}-{1}-{2}", inParameters);
            for (int i = 0; i < DetailData.Length; i++)
            {
                COMDT_STATISTIC_BASE_STRUCT data = DetailData[i];
                if (data.stKeyInfo.bKeyNum <= 0)
                {
                    break;
                }
                if (((data.stKeyInfo.KeyDetail[0] == num) && (data.stKeyInfo.KeyDetail[1] == num2)) && (data.stKeyInfo.KeyDetail[2] == num3))
                {
                    this.VerifyValues(InCondition, ref ConditionInfo, ref data);
                    return;
                }
            }
            DebugHelper.Assert(false, "can not visit this code!");
        }

        private void VerifyReportData(ref COMDT_SETTLE_COMMON_DATA CommonData)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            DebugHelper.Assert(curLvelContext != null);
            if (curLvelContext != null)
            {
                for (int i = 0; i < curLvelContext.m_starDetail.Length; i++)
                {
                    int iParam = curLvelContext.m_starDetail[i].iParam;
                    if (iParam == 0)
                    {
                        return;
                    }
                    ResEvaluateStarInfo dataByKey = GameDataMgr.evaluateCondInfoDatabin.GetDataByKey((uint) iParam);
                    if (dataByKey != null)
                    {
                        COMDT_STATISTIC_STRUCT_PILE statRef = null;
                        for (int j = 0; j < CommonData.stStatisticData.bNum; j++)
                        {
                            if (CommonData.stStatisticData.astDetail[j].bReportType == 1)
                            {
                                statRef = CommonData.stStatisticData.astDetail[j];
                                break;
                            }
                        }
                        if (statRef == null)
                        {
                            int bNum = CommonData.stStatisticData.bNum;
                            statRef = CommonData.stStatisticData.astDetail[bNum];
                            statRef.bReportType = 1;
                            CommonData.stStatisticData.bNum = (byte) (CommonData.stStatisticData.bNum + 1);
                        }
                        COMDT_STATISTIC_BASE_STRUCT[] astDetail = statRef.astDetail;
                        IStarEvaluation evaluationAt = Singleton<StarSystem>.instance.GetEvaluationAt(i);
                        if (evaluationAt != null)
                        {
                            for (int k = 0; k < dataByKey.astConditions.Length; k++)
                            {
                                if (dataByKey.astConditions[k].dwType == 1)
                                {
                                    IStarCondition conditionAt = evaluationAt.GetConditionAt(k);
                                    DebugHelper.Assert(conditionAt != null, "StarCondition==null");
                                    if (conditionAt != null)
                                    {
                                        this.VerifyCondition(conditionAt, ref dataByKey.astConditions[k], ref statRef, ref astDetail);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void VerifyValues(IStarCondition InCondition, ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_BASE_STRUCT Data)
        {
            Singleton<StarSystem>.instance.GetEnumerator();
            int[] values = InCondition.values;
            DebugHelper.Assert((values != null) && (values.Length == Data.stValueInfo.bValueNum), "Values!=null&&Values.Length == Data.stValueInfo.bValueNum");
            for (int i = 0; (i < ConditionInfo.ValueDetail.Length) && (i < Data.stValueInfo.bValueNum); i++)
            {
                if (ConditionInfo.ValueDetail[i] == 0)
                {
                    break;
                }
                int num2 = Data.stValueInfo.ValueDetail[i];
                int num3 = values[i];
                int num4 = ConditionInfo.KeyDetail[0];
                int num5 = ConditionInfo.KeyDetail[1];
                int num6 = ConditionInfo.KeyDetail[2];
                if (num2 != num3)
                {
                    object[] inParameters = new object[] { num4, num5, num6 };
                    DebugHelper.Assert(false, "({0})({1})({2}) Report Value Missmatch", inParameters);
                    object[] objArray2 = new object[] { num2, num3 };
                    DebugHelper.Assert(false, "Missvalue {0} {1}", objArray2);
                    Data.stValueInfo.ValueDetail[i] = num3;
                }
            }
        }

        public bool CanShowRolling { get; private set; }

        protected delegate void SendMessageDeletageType(ref CSPkg msg);

        public delegate void SendMsgFailNetErrorCallback();

        private class SettlementError : Exception
        {
            public SettlementError(string msg) : base(msg)
            {
            }
        }
    }
}

