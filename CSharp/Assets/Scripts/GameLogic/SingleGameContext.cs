namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class SingleGameContext : GameContextBase
    {
        public SingleGameContext(SCPKG_STARTSINGLEGAMERSP InMessage)
        {
            DebugHelper.Assert(InMessage != null, "输入不应该为null");
            base.RewardCount = (InMessage == null) ? 0 : ((int) InMessage.dwRewardNum);
            Singleton<ActorDataCenter>.instance.ClearHeroServerData();
            if (Singleton<GamePlayerCenter>.instance.GetAllPlayers().Count > 0)
            {
            }
            Singleton<GamePlayerCenter>.instance.ClearAllPlayers();
            if (((InMessage != null) && (InMessage.stDetail.stSingleGameSucc != null)) && (InMessage.stDetail.stSingleGameSucc.bNum >= 1))
            {
                this.DoNew9SlotCalc(InMessage);
                int num = Mathf.Min(InMessage.stDetail.stSingleGameSucc.bNum, InMessage.stDetail.stSingleGameSucc.astFighter.Length);
                for (int i = 0; i < num; i++)
                {
                    COMDT_PLAYERINFO comdt_playerinfo = InMessage.stDetail.stSingleGameSucc.astFighter[i];
                    if (CheatCommandReplayEntry.heroPerformanceTest)
                    {
                        comdt_playerinfo.astChoiceHero = InMessage.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero;
                    }
                    if (comdt_playerinfo.bObjType != 0)
                    {
                        ulong uid = 0L;
                        uint dwFakeLogicWorldID = 0;
                        uint level = 1;
                        int honorId = 0;
                        int honorLevel = 0;
                        uint gradeOfRank = 0;
                        if (comdt_playerinfo.bObjType == 2)
                        {
                            if ((InMessage.bGameType == 1) && Convert.ToBoolean(InMessage.stGameParam.stSingleGameRspOfCombat.bIsWarmBattle))
                            {
                                uid = comdt_playerinfo.stDetail.stPlayerOfNpc.ullFakeUid;
                                dwFakeLogicWorldID = comdt_playerinfo.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
                                level = comdt_playerinfo.stDetail.stPlayerOfNpc.dwFakePvpLevel;
                            }
                            else
                            {
                                uid = 0L;
                                dwFakeLogicWorldID = 0;
                                level = comdt_playerinfo.dwLevel;
                            }
                        }
                        else
                        {
                            uid = (comdt_playerinfo.bObjType != 1) ? ((ulong) 0L) : comdt_playerinfo.stDetail.stPlayerOfAcnt.ullUid;
                            dwFakeLogicWorldID = (comdt_playerinfo.bObjType != 1) ? 0 : ((uint) comdt_playerinfo.stDetail.stPlayerOfAcnt.iLogicWorldID);
                            level = comdt_playerinfo.dwLevel;
                            honorId = comdt_playerinfo.stDetail.stPlayerOfAcnt.iHonorID;
                            honorLevel = comdt_playerinfo.stDetail.stPlayerOfAcnt.iHonorLevel;
                            gradeOfRank = (InMessage.stGameParam.stSingleGameRspOfCombat != null) ? InMessage.stGameParam.stSingleGameRspOfCombat.bGradeOfRank : 0;
                        }
                        uint vipLv = 0;
                        if (comdt_playerinfo.stDetail.stPlayerOfAcnt != null)
                        {
                            vipLv = comdt_playerinfo.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
                        }
                        Player player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(comdt_playerinfo.dwObjId, (COM_PLAYERCAMP) comdt_playerinfo.bObjCamp, comdt_playerinfo.bPosOfCamp, level, comdt_playerinfo.bObjType != 1, StringHelper.UTF8BytesToString(ref comdt_playerinfo.szName), 0, (int) dwFakeLogicWorldID, uid, vipLv, null, gradeOfRank, 0, honorId, honorLevel);
                        if (player != null)
                        {
                            for (int j = 0; j < comdt_playerinfo.astChoiceHero.Length; j++)
                            {
                                uint dwHeroID = comdt_playerinfo.astChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID;
                                player.AddHero(dwHeroID);
                            }
                            player.isGM = LobbyMsgHandler.isHostGMAcnt;
                        }
                        Singleton<ActorDataCenter>.instance.AddHeroesServerData(comdt_playerinfo.dwObjId, comdt_playerinfo.astChoiceHero);
                    }
                    if (comdt_playerinfo.bObjType == 1)
                    {
                        Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(comdt_playerinfo.dwObjId);
                    }
                }
                base.LevelContext = CLevelCfgLogicManager.CreatePveLevelContext(InMessage);
            }
        }

        private void Calc9SlotHeroStandingPosition(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
        {
            Calc9SlotHeroData[] heroes = new Calc9SlotHeroData[3];
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
            ActorStaticData actorData = new ActorStaticData();
            ActorMeta actorMeta = new ActorMeta();
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                for (int i = 0; (i < stBattlePlayer.astFighter[0].astChoiceHero.Length) && (i < 3); i++)
                {
                    uint dwHeroID = stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID;
                    if (dwHeroID != 0)
                    {
                        actorMeta.ConfigId = (int) dwHeroID;
                        actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
                        heroes[i].Level = 1;
                        heroes[i].Quality = 1;
                        heroes[i].RecommendPos = actorData.TheHeroOnlyInfo.RecommendStandPos;
                        heroes[i].Ability = (uint) CHeroDataFactory.CreateHeroData(dwHeroID).combatEft;
                        heroes[i].ConfigId = dwHeroID;
                        heroes[i].selected = false;
                        heroes[i].BornIndex = -1;
                    }
                }
                this.ImpCalc9SlotHeroStandingPosition(ref heroes);
                for (int j = 0; (j < stBattlePlayer.astFighter[0].astChoiceHero.Length) && (j < 3); j++)
                {
                    stBattlePlayer.astFighter[0].astChoiceHero[j].stHeroExtral.iHeroPos = heroes[j].BornIndex;
                }
                for (int k = 0; (k < stBattlePlayer.astFighter[1].astChoiceHero.Length) && (k < 3); k++)
                {
                    uint id = stBattlePlayer.astFighter[1].astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID;
                    if (id != 0)
                    {
                        actorMeta.ConfigId = (int) id;
                        actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
                        heroes[k].Level = stBattlePlayer.astFighter[1].astChoiceHero[k].stBaseInfo.stCommonInfo.wLevel;
                        heroes[k].Quality = stBattlePlayer.astFighter[1].astChoiceHero[k].stBaseInfo.stCommonInfo.stQuality.wQuality;
                        heroes[k].RecommendPos = actorData.TheHeroOnlyInfo.RecommendStandPos;
                        heroes[k].Ability = (uint) CHeroDataFactory.CreateHeroData(id).combatEft;
                        heroes[k].ConfigId = id;
                        heroes[k].selected = false;
                        heroes[k].BornIndex = -1;
                    }
                }
                this.ImpCalc9SlotHeroStandingPosition(ref heroes);
                for (int m = 0; (m < stBattlePlayer.astFighter[1].astChoiceHero.Length) && (m < 3); m++)
                {
                    stBattlePlayer.astFighter[1].astChoiceHero[m].stHeroExtral.iHeroPos = heroes[m].BornIndex;
                }
            }
        }

        public override GameInfoBase CreateGameInfo()
        {
            SingleGameInfo info = new SingleGameInfo();
            info.Initialize(this);
            return info;
        }

        protected void DoNew9SlotCalc(SCPKG_STARTSINGLEGAMERSP inMessage)
        {
            if ((inMessage.bGameType == 8) || (inMessage.bGameType == 7))
            {
                this.Calc9SlotHeroStandingPosition(inMessage.stDetail.stSingleGameSucc);
            }
        }

        private List<int> HasPositionHero(ref Calc9SlotHeroData[] heroes, RES_HERO_RECOMMEND_POSITION pos)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (heroes[i].RecommendPos == pos)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        private void ImpCalc9SlotHeroStandingPosition(ref Calc9SlotHeroData[] heroes)
        {
            List<int> list = this.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_FRONT);
            int index = 0;
            switch (list.Count)
            {
                case 1:
                    for (int i = 0; i < 3; i++)
                    {
                        if (heroes[i].RecommendPos == 0)
                        {
                            heroes[i].selected = true;
                            heroes[i].BornIndex = 1;
                            break;
                        }
                    }
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    if (heroes[index].RecommendPos == 1)
                    {
                        heroes[index].BornIndex = 3;
                        index = this.WhoIsBestHero(ref heroes);
                        heroes[index].selected = true;
                        heroes[index].BornIndex = (heroes[index].RecommendPos != 1) ? 8 : 5;
                    }
                    else
                    {
                        heroes[index].BornIndex = 8;
                        index = this.WhoIsBestHero(ref heroes);
                        heroes[index].selected = true;
                        heroes[index].BornIndex = (heroes[index].RecommendPos != 1) ? 6 : 3;
                    }
                    return;

                case 2:
                    for (int j = 0; j < 3; j++)
                    {
                        if (heroes[j].RecommendPos == 1)
                        {
                            heroes[j].selected = true;
                            heroes[j].BornIndex = 3;
                            break;
                        }
                        if (heroes[j].RecommendPos == 2)
                        {
                            heroes[j].selected = true;
                            heroes[j].BornIndex = 6;
                            break;
                        }
                    }
                    break;

                case 3:
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 1;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 0;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 2;
                    return;

                default:
                    switch (this.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_CENTER).Count)
                    {
                        case 1:
                            for (int k = 0; k < 3; k++)
                            {
                                if (heroes[k].RecommendPos == 1)
                                {
                                    heroes[k].selected = true;
                                    heroes[k].BornIndex = 1;
                                    break;
                                }
                            }
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 8;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 6;
                            return;

                        case 2:
                            for (int m = 0; m < 3; m++)
                            {
                                if (heroes[m].RecommendPos == 2)
                                {
                                    heroes[m].selected = true;
                                    heroes[m].BornIndex = 3;
                                    break;
                                }
                            }
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 1;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 0;
                            return;

                        case 3:
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 1;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 0;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 2;
                            return;
                    }
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 4;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 3;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 5;
                    return;
            }
            index = this.WhoIsBestHero(ref heroes);
            heroes[index].selected = true;
            heroes[index].BornIndex = 1;
            index = this.WhoIsBestHero(ref heroes);
            heroes[index].selected = true;
            heroes[index].BornIndex = 0;
        }

        private bool IsBetterHero(ref Calc9SlotHeroData heroe1, ref Calc9SlotHeroData heroe2)
        {
            return (((heroe1.ConfigId > 0) && !heroe1.selected) && (((((heroe2.ConfigId == 0) || heroe2.selected) || (heroe1.Ability > heroe2.Ability)) || ((heroe1.Ability == heroe2.Ability) && (heroe1.Level > heroe2.Level))) || (((heroe1.Ability == heroe2.Ability) && (heroe1.Level == heroe2.Level)) && (heroe1.Quality >= heroe2.Quality))));
        }

        private int WhoIsBestHero(ref Calc9SlotHeroData[] heroes)
        {
            if (this.IsBetterHero(ref heroes[0], ref heroes[1]) && this.IsBetterHero(ref heroes[0], ref heroes[2]))
            {
                return 0;
            }
            if (this.IsBetterHero(ref heroes[1], ref heroes[0]) && this.IsBetterHero(ref heroes[1], ref heroes[2]))
            {
                return 1;
            }
            return 2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Calc9SlotHeroData
        {
            public uint ConfigId;
            public int RecommendPos;
            public uint Ability;
            public uint Level;
            public int Quality;
            public int BornIndex;
            public bool selected;
        }
    }
}

