namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class BurnExpeditionUT
    {
        public static void ApplyBuff(List<PoolObjHandle<ActorRoot>> actorList, int buffid)
        {
            if (buffid != 0)
            {
                List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = actorList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PoolObjHandle<ActorRoot> current = enumerator.Current;
                    if (current.handle.IsHostCamp())
                    {
                        new BufConsumer(buffid, enumerator.Current, enumerator.Current).Use();
                    }
                }
            }
        }

        public static void ApplyHP2Game(List<PoolObjHandle<ActorRoot>> actorList)
        {
            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = actorList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                current.handle.ActorControl.bForceNotRevive = true;
                IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
                ActorServerData actorData = new ActorServerData();
                PoolObjHandle<ActorRoot> handle2 = enumerator.Current;
                actorDataProvider.GetActorServerData(ref handle2.handle.TheActorMeta, ref actorData);
                PoolObjHandle<ActorRoot> handle3 = enumerator.Current;
                int actorHpTotal = handle3.handle.ValueComponent.actorHpTotal;
                int num2 = (int) Convert_FactHP(actorData.TheBurnInfo.HeroRemainingHp, (uint) actorHpTotal);
                PoolObjHandle<ActorRoot> handle4 = enumerator.Current;
                handle4.handle.ValueComponent.actorHp = num2;
            }
        }

        public static void Build_Burn_BattleParam(COMDT_SINGLE_GAME_PARAM param, bool bClickGameOver)
        {
            param.bGameType = 7;
            PlayerKDA hostKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetHostKDA();
            if (hostKDA == null)
            {
                DebugHelper.Assert(hostKDA != null, "Failed find player kda");
                Singleton<BattleStatistic>.instance.m_playerKDAStat.DumpDebugInfo();
            }
            else
            {
                ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                if (!bClickGameOver)
                {
                    while (enumerator.MoveNext())
                    {
                        int configId = enumerator.Current.actorHero.handle.TheActorMeta.ConfigId;
                        Singleton<BurnExpeditionController>.GetInstance().model.SetHero_Hp((uint) configId, (int) Get_BloodTH(enumerator.Current.actorHero.handle));
                    }
                }
                param.stGameDetail.stBurning = new COMDT_BURNING_ENEMY_HERO_DETAIL();
                COMDT_BURNING_HERO_INFO[] astHeroList = param.stGameDetail.stBurning.astHeroList;
                List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
                int index = 0;
                for (int i = 0; i < heroActors.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = heroActors[i];
                    ActorRoot actor = handle.handle;
                    if (!actor.IsHostCamp())
                    {
                        astHeroList[index].dwHeroID = (uint) actor.TheActorMeta.ConfigId;
                        bool flag = actor.ValueComponent.actorHp == 0;
                        uint blood = 0;
                        byte num5 = 0;
                        if (flag)
                        {
                            blood = 0;
                            num5 = 1;
                        }
                        else
                        {
                            blood = Get_BloodTH(actor);
                            num5 = 0;
                        }
                        astHeroList[index].dwBloodTTH = blood;
                        astHeroList[index].bIsDead = num5;
                        if (!bClickGameOver)
                        {
                            Record_EnemyHero_HPInfo(actor.TheActorMeta.ConfigId, blood);
                        }
                        index++;
                    }
                }
                param.stGameDetail.stBurning.bHeroNum = (byte) index;
            }
        }

        public static uint Convert_BloodTH(uint curHp, uint max)
        {
            return ((curHp * 0x2710) / max);
        }

        public static uint Convert_FactHP(uint bloodTH, uint max)
        {
            return Math.Max((uint) ((bloodTH * max) / 0x2710), (uint) 1);
        }

        public static CSDT_SINGLE_GAME_OF_BURNING Create_CSDT_SINGLE_GAME_OF_BURNING(int levelIndex)
        {
            return new CSDT_SINGLE_GAME_OF_BURNING { bLevelNo = Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelNo(levelIndex), iLevelID = Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(levelIndex) };
        }

        public static void Finish_Level(int level_index)
        {
            BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
            model.FinishLevel(level_index);
            model.UnLockBox(level_index);
            model.CalcProgress();
        }

        public static uint Get_BloodTH(ActorRoot actor)
        {
            if ((actor != null) && (actor.ValueComponent != null))
            {
                return Get_BloodTH((uint) actor.ValueComponent.actorHp, (uint) actor.ValueComponent.actorHpTotal);
            }
            return 0;
        }

        public static uint Get_BloodTH(uint curHP, uint max)
        {
            return Convert_BloodTH(curHP, max);
        }

        public static int Get_Buff_CDTime(int buffid)
        {
            ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long) buffid);
            if (dataByKey == null)
            {
                return -1;
            }
            return dataByKey.iDuration;
        }

        public static ResLevelCfgInfo Get_LevelConfigInfo(int levelIndex)
        {
            return GameDataMgr.burnMap.GetDataByKey((long) Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(levelIndex));
        }

        public static int GetIndex(string s)
        {
            return int.Parse(s.Substring(s.IndexOf("_") + 1));
        }

        public static int GetLevelCFGID(int level_index)
        {
            return (0x7725 + level_index);
        }

        public static void Handle_Burn_Settle(ref SCPKG_SINGLEGAMEFINRSP rsp)
        {
            if (rsp.iErrCode == 0)
            {
                if (rsp.stDetail.stGameInfo.bGameResult == 1)
                {
                    Finish_Level(Singleton<BurnExpeditionController>.GetInstance().model.curSelect_LevelIndex);
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(UT.GetText("Burn_Error_Settle_Failed"), rsp.iErrCode), false);
            }
        }

        public static void Record_EnemyHero_HPInfo(int heroID, uint blood)
        {
            COMDT_PLAYERINFO comdt_playerinfo = Singleton<BurnExpeditionController>.GetInstance().model.Get_Current_Enemy_PlayerInfo();
            for (int i = 0; i < comdt_playerinfo.astChoiceHero.Length; i++)
            {
                COMDT_CHOICEHERO comdt_choicehero = comdt_playerinfo.astChoiceHero[i];
                if ((comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID != 0) && (comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID == heroID))
                {
                    comdt_choicehero.stBurningInfo.dwBloodTTH = blood;
                    if (blood == 0)
                    {
                        comdt_choicehero.stBurningInfo.bIsDead = 1;
                    }
                    else
                    {
                        comdt_choicehero.stBurningInfo.bIsDead = 0;
                    }
                }
            }
        }
    }
}

