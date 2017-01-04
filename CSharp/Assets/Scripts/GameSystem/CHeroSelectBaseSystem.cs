namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class CHeroSelectBaseSystem : Singleton<CHeroSelectBaseSystem>
    {
        private List<uint>[] campBanList;
        public int m_banHeroTeamMaxCount;
        public enBanPickStep m_banPickStep;
        public uint m_battleListID;
        public ListView<IHeroData> m_canUseHeroList = new ListView<IHeroData>();
        public uint m_chatID;
        public SCPKG_NTF_CUR_BAN_PICK_INFO m_curBanPickInfo = new SCPKG_NTF_CUR_BAN_PICK_INFO();
        public float m_fOpenHeroSelectForm;
        private enSelectGameType m_gameType = enSelectGameType.enNull;
        private int m_isAllowDupHero;
        public bool m_isAllowDupHeroAllCamp;
        public bool m_isInHeroSelectState;
        private bool m_isMobaMode;
        private bool m_isMultiMode;
        public bool m_isSelectConfirm;
        public uint m_mapId;
        private byte m_mapMaxHeroCount;
        public byte m_mapType;
        public ResDT_UnUseSkill m_mapUnUseSkill;
        public List<uint> m_notUseHeroList = new List<uint>();
        public byte m_selectHeroCount;
        public List<uint> m_selectHeroIDList = new List<uint>();
        private enSelectType m_selectType = enSelectType.enNull;
        public CSDT_SINGLE_GAME_OF_ADVENTURE m_stGameOfAdventure;
        public CSDT_SINGLE_GAME_OF_ARENA m_stGameOfArena;
        public CSDT_SINGLE_GAME_OF_BURNING m_stGameOfBurnning;
        public CSDT_SINGLE_GAME_OF_COMBAT m_stGameOfCombat;
        public SCPKG_NTF_SWAP_HERO m_swapInfo = new SCPKG_NTF_SWAP_HERO();
        public enSwapHeroState m_swapState;
        private enUIType m_uiType = enUIType.enNull;
        public int m_UseRandSelCount;
        public static COMDT_BATTLELIST_LIST s_defaultBattleListInfo = null;
        public static int[] s_propArr = new int[0x24];
        public static int[] s_propPctArr = new int[0x24];

        public void AddBanHero(COM_PLAYERCAMP camp, uint heroId)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.campBanList.Length))
            {
                this.campBanList[index].Add(heroId);
            }
        }

        public void AddBanHero(COM_PLAYERCAMP camp, uint[] heroIdArr)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.campBanList.Length))
            {
                this.campBanList[index].AddRange(heroIdArr);
            }
        }

        public void Clear()
        {
            this.m_isInHeroSelectState = false;
            this.m_gameType = enSelectGameType.enNull;
            this.m_selectType = enSelectType.enNull;
            this.m_uiType = enUIType.enNull;
            this.m_mapId = 0;
            this.m_mapType = 0;
            this.m_isAllowDupHero = 0;
            this.m_mapMaxHeroCount = 0;
            this.m_mapUnUseSkill = null;
            this.m_chatID = 0;
            this.m_notUseHeroList.Clear();
            this.m_canUseHeroList.Clear();
            this.m_selectHeroIDList.Clear();
            this.m_selectHeroCount = 0;
            this.m_isSelectConfirm = false;
            this.m_isMobaMode = false;
            this.m_isMultiMode = false;
            this.m_stGameOfAdventure = null;
            this.m_stGameOfCombat = null;
            this.m_stGameOfBurnning = null;
            this.m_stGameOfArena = null;
            this.m_battleListID = 0;
            s_propArr = new int[0x24];
            s_propPctArr = new int[0x24];
            this.m_UseRandSelCount = 0;
            this.m_banPickStep = enBanPickStep.enBan;
            this.m_banHeroTeamMaxCount = 0;
            this.ClearBanHero();
            this.m_swapState = enSwapHeroState.enIdle;
            this.m_swapInfo = new SCPKG_NTF_SWAP_HERO();
            this.m_curBanPickInfo = new SCPKG_NTF_CUR_BAN_PICK_INFO();
            this.m_isAllowDupHeroAllCamp = false;
            MonoSingleton<VoiceSys>.GetInstance().HeroSelectTobattle();
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_Hero_Select_CloseForm);
            Singleton<CUIManager>.instance.CloseForm(CHeroSkinBuyManager.s_buyHeroSkin3DFormPath);
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharSkillIcon");
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
        }

        public void ClearBanHero()
        {
            for (int i = 0; i < this.campBanList.Length; i++)
            {
                this.campBanList[i].Clear();
            }
        }

        public void CloseForm()
        {
            Singleton<CHeroSelectNormalSystem>.instance.CloseForm();
            Singleton<CHeroSelectBanPickSystem>.instance.CloseForm();
        }

        public List<uint> GetBanHeroList(COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.campBanList.Length))
            {
                return this.campBanList[index];
            }
            return null;
        }

        public int GetCanUseHeroIndex(uint heroID)
        {
            for (int i = 0; i < this.m_canUseHeroList.Count; i++)
            {
                if (this.m_canUseHeroList[i].cfgID == heroID)
                {
                    return i;
                }
            }
            return 0;
        }

        public List<uint> GetHeroListForBattleListID(uint battleListID)
        {
            List<uint> list = new List<uint>();
            if (s_defaultBattleListInfo != null)
            {
                for (int i = 0; i < s_defaultBattleListInfo.dwListNum; i++)
                {
                    if (s_defaultBattleListInfo.astBattleList[i].dwBattleListID == battleListID)
                    {
                        COMDT_BATTLEHERO stBattleList = s_defaultBattleListInfo.astBattleList[i].stBattleList;
                        for (int j = 0; j < stBattleList.BattleHeroList.Length; j++)
                        {
                            uint heroID = stBattleList.BattleHeroList[j];
                            if (!CHeroDataFactory.IsHeroCanUse(heroID))
                            {
                                heroID = 0;
                            }
                            if (heroID != 0)
                            {
                                list.Add(heroID);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public List<uint> GetPveTeamHeroIDList()
        {
            List<uint> selectHeroIDList = this.m_selectHeroIDList;
            if (this.gameType == enSelectGameType.enPVE_Computer)
            {
                selectHeroIDList = new List<uint>();
                MemberInfo masterMemberInfo = this.roomInfo.GetMasterMemberInfo();
                selectHeroIDList.Add(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
            }
            return selectHeroIDList;
        }

        public List<uint> GetTeamHeroList(COM_PLAYERCAMP tarCamp)
        {
            List<uint> list = new List<uint>();
            if (this.IsMobaMode())
            {
                if (this.roomInfo != null)
                {
                    ListView<MemberInfo> view = this.roomInfo[tarCamp];
                    for (int i = 0; i < view.Count; i++)
                    {
                        list.Add(view[i].ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                    }
                }
                return list;
            }
            return this.m_selectHeroIDList;
        }

        public Transform GetTeamPlayerElement(ulong playerUID, COM_PLAYERCAMP tarCamp)
        {
            Transform transform = null;
            if (this.uiType == enUIType.enNormal)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
                if (form == null)
                {
                    return null;
                }
                Transform transform2 = form.transform.Find("PanelRight/ListTeamHeroInfo");
                if (transform2 == null)
                {
                    return null;
                }
                CUIListScript component = transform2.gameObject.GetComponent<CUIListScript>();
                if (component == null)
                {
                    return null;
                }
                int teamPlayerIndex = this.GetTeamPlayerIndex(playerUID, tarCamp);
                if ((teamPlayerIndex >= 0) && (teamPlayerIndex < component.GetElementAmount()))
                {
                    transform = component.GetElemenet(teamPlayerIndex).transform;
                }
                return transform;
            }
            if (this.uiType == enUIType.enBanPick)
            {
                CUIFormScript script3 = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
                if (script3 == null)
                {
                    return null;
                }
                Transform transform3 = null;
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (((masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID) && (tarCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)) || ((masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID) && (tarCamp == masterMemberInfo.camp)))
                {
                    transform3 = script3.transform.Find("PanelLeft/TeamHeroInfo");
                }
                else
                {
                    transform3 = script3.transform.Find("PanelRight/TeamHeroInfo");
                }
                if (transform3 == null)
                {
                    return null;
                }
                CUIListScript script4 = transform3.gameObject.GetComponent<CUIListScript>();
                if (script4 == null)
                {
                    return null;
                }
                int index = this.GetTeamPlayerIndex(playerUID, tarCamp);
                if ((index >= 0) && (index < script4.GetElementAmount()))
                {
                    transform = script4.GetElemenet(index).transform;
                }
            }
            return transform;
        }

        public int GetTeamPlayerIndex(ulong playerUID, COM_PLAYERCAMP tarCamp)
        {
            if (this.IsMobaMode() && (this.roomInfo != null))
            {
                ListView<MemberInfo> view = this.roomInfo[tarCamp];
                for (int i = 0; i < view.Count; i++)
                {
                    if (view[i].ullUid == playerUID)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        public static List<int> HasPositionHero(ref Assets.Scripts.GameSystem.Calc9SlotHeroData[] heroes, RES_HERO_RECOMMEND_POSITION pos)
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

        public static void ImpCalc9SlotHeroStandingPosition(ref Assets.Scripts.GameSystem.Calc9SlotHeroData[] heroes)
        {
            List<int> list = HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_FRONT);
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
                    index = WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    if (heroes[index].RecommendPos == 1)
                    {
                        heroes[index].BornIndex = 3;
                        index = WhoIsBestHero(ref heroes);
                        heroes[index].selected = true;
                        heroes[index].BornIndex = (heroes[index].RecommendPos != 1) ? 8 : 5;
                    }
                    else
                    {
                        heroes[index].BornIndex = 8;
                        index = WhoIsBestHero(ref heroes);
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
                    index = WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 1;
                    index = WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 0;
                    index = WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 2;
                    return;

                default:
                    switch (HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_CENTER).Count)
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
                            index = WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 8;
                            index = WhoIsBestHero(ref heroes);
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
                            index = WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 1;
                            index = WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 0;
                            return;

                        case 3:
                            index = WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 1;
                            index = WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 0;
                            index = WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 2;
                            return;
                    }
                    index = WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 4;
                    index = WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 3;
                    index = WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 5;
                    return;
            }
            index = WhoIsBestHero(ref heroes);
            heroes[index].selected = true;
            heroes[index].BornIndex = 1;
            index = WhoIsBestHero(ref heroes);
            heroes[index].selected = true;
            heroes[index].BornIndex = 0;
        }

        public override void Init()
        {
            this.campBanList = new List<uint>[3];
            for (int i = 0; i < this.campBanList.Length; i++)
            {
                this.campBanList[i] = new List<uint>();
            }
        }

        private void InitCanUseHeroData()
        {
            Singleton<CHeroSelectNormalSystem>.instance.m_showHeroID = 0;
            if (this.m_gameType == enSelectGameType.enArenaDefTeamConfig)
            {
                this.LoadArenaDefHeroList(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList);
            }
            else if (!this.IsMobaMode())
            {
                this.LoadPveDefaultHeroList();
            }
            if (this.m_gameType == enSelectGameType.enLadder)
            {
                if (this.selectType == enSelectType.enBanPick)
                {
                    this.LoadServerCanUseHeroData();
                }
                else
                {
                    this.m_canUseHeroList = CHeroDataFactory.GetHostHeroList(false);
                }
            }
            else if (this.IsSpecTraingMode())
            {
                this.m_canUseHeroList = CHeroDataFactory.GetTrainingHeroList();
            }
            else
            {
                this.m_canUseHeroList = CHeroDataFactory.GetPvPHeroList();
                if (this.selectType != enSelectType.enBanPick)
                {
                    this.m_canUseHeroList.AddRange(CBagSystem.GetExpCardHeroList());
                }
            }
            if (this.IsMobaMode())
            {
                this.m_notUseHeroList.Clear();
                ResBanHeroConf dataByKey = GameDataMgr.banHeroBin.GetDataByKey(GameDataMgr.GetDoubleKey(this.m_mapType, this.m_mapId));
                if (dataByKey != null)
                {
                    for (int i = 0; i < dataByKey.BanHero.Length; i++)
                    {
                        if (dataByKey.BanHero[i] != 0)
                        {
                            this.m_notUseHeroList.Add(dataByKey.BanHero[i]);
                        }
                    }
                }
            }
        }

        private void InitMobaMode()
        {
            if ((((this.m_gameType != enSelectGameType.enNull) && (this.m_gameType != enSelectGameType.enPVE_Adventure)) && ((this.m_gameType != enSelectGameType.enBurning) && (this.m_gameType != enSelectGameType.enArena))) && ((this.m_gameType != enSelectGameType.enGuide) && (this.m_gameType != enSelectGameType.enArenaDefTeamConfig)))
            {
                this.m_isMobaMode = true;
            }
            else
            {
                this.m_isMobaMode = false;
            }
        }

        private void InitMultiMode()
        {
            if ((((this.m_gameType != enSelectGameType.enNull) && (this.m_gameType != enSelectGameType.enPVE_Adventure)) && ((this.m_gameType != enSelectGameType.enBurning) && (this.m_gameType != enSelectGameType.enArena))) && (((this.m_gameType != enSelectGameType.enGuide) && (this.m_gameType != enSelectGameType.enPVE_Computer)) && (this.m_gameType != enSelectGameType.enArenaDefTeamConfig)))
            {
                this.m_isMultiMode = true;
            }
            else
            {
                this.m_isMultiMode = false;
            }
        }

        public static void InitRoomData(COMDT_DESKINFO deskInfo, CSDT_CAMPINFO[] campInfo, COMDT_FREEHERO freeHero, COMDT_FREEHERO_INACNT freeHeroSymbol)
        {
            if (deskInfo.bMapType != 3)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                masterRoleInfo.SetFreeHeroInfo(freeHero);
                masterRoleInfo.SetFreeHeroSymbol(freeHeroSymbol);
            }
            Singleton<CRoomSystem>.GetInstance().UpdateRoomInfo(deskInfo, campInfo);
            Singleton<LobbyLogic>.GetInstance().inMultiRoom = true;
        }

        public void InitSelectHeroIDList(byte heroMaxCount)
        {
            this.m_selectHeroIDList.Clear();
            for (int i = 0; i < heroMaxCount; i++)
            {
                this.m_selectHeroIDList.Add(0);
            }
            this.m_selectHeroCount = 0;
            this.m_mapMaxHeroCount = heroMaxCount;
        }

        private void InitSelectType()
        {
            enSelectType enNormal = enSelectType.enNormal;
            if (((this.gameType == enSelectGameType.enPVE_Adventure) || (this.gameType == enSelectGameType.enBurning)) || (((this.gameType == enSelectGameType.enArena) || (this.gameType == enSelectGameType.enGuide)) || (this.gameType == enSelectGameType.enArenaDefTeamConfig)))
            {
                enNormal = enSelectType.enMutile;
            }
            else
            {
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.m_mapType, this.m_mapId);
                if (pvpMapCommonInfo != null)
                {
                    enNormal = (enSelectType) pvpMapCommonInfo.stPickRuleInfo.bPickType;
                    this.m_isAllowDupHeroAllCamp = pvpMapCommonInfo.stPickRuleInfo.Param[1] == 0;
                    this.m_isAllowDupHero = pvpMapCommonInfo.bIsAllowHeroDup;
                    this.m_mapUnUseSkill = pvpMapCommonInfo.stUnUseSkillInfo;
                    this.m_chatID = pvpMapCommonInfo.dwChatID;
                }
            }
            this.m_selectType = enNormal;
        }

        private void InitSystem()
        {
            this.InitMobaMode();
            this.InitMultiMode();
            this.InitSelectType();
            this.InitUIType();
        }

        private void InitUIType()
        {
            if (((this.m_selectType == enSelectType.enMutile) || (this.m_selectType == enSelectType.enNormal)) || ((this.m_selectType == enSelectType.enRandom) || (this.m_selectType == enSelectType.enClone)))
            {
                this.m_uiType = enUIType.enNormal;
            }
            else if (this.m_selectType == enSelectType.enBanPick)
            {
                this.m_uiType = enUIType.enBanPick;
            }
        }

        public bool IsBanByHeroID(uint heroID)
        {
            for (int i = 0; i < this.campBanList.Length; i++)
            {
                if (this.campBanList[i].Contains(heroID))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsBetterHero(ref Assets.Scripts.GameSystem.Calc9SlotHeroData heroe1, ref Assets.Scripts.GameSystem.Calc9SlotHeroData heroe2)
        {
            return (((heroe1.ConfigId > 0) && !heroe1.selected) && (((((heroe2.ConfigId == 0) || heroe2.selected) || (heroe1.Ability > heroe2.Ability)) || ((heroe1.Ability == heroe2.Ability) && (heroe1.Level > heroe2.Level))) || (((heroe1.Ability == heroe2.Ability) && (heroe1.Level == heroe2.Level)) && (heroe1.Quality >= heroe2.Quality))));
        }

        public bool IsHeroExist(uint heroID)
        {
            bool flag = true;
            if (this.IsMobaMode())
            {
                if (this.m_notUseHeroList.Contains(heroID))
                {
                    return flag;
                }
                if (this.selectType != enSelectType.enBanPick)
                {
                    if (this.m_isAllowDupHero == 0)
                    {
                        MemberInfo masterMemberInfo = this.roomInfo.GetMasterMemberInfo();
                        if (masterMemberInfo == null)
                        {
                            string str = this.roomInfo.selfObjID.ToString();
                        }
                        if (masterMemberInfo == null)
                        {
                            return flag;
                        }
                        return this.roomInfo.IsHeroExistWithCamp(masterMemberInfo.camp, heroID);
                    }
                    return false;
                }
                return (!this.m_isAllowDupHeroAllCamp && (this.roomInfo.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1, heroID) || this.roomInfo.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2, heroID)));
            }
            if ((this.m_selectHeroIDList != null) && ((this.m_selectHeroCount < this.m_selectHeroIDList.Count) || (this.m_selectHeroIDList.Count == 1)))
            {
                return this.m_selectHeroIDList.Contains(heroID);
            }
            return true;
        }

        public bool IsMobaMode()
        {
            return this.m_isMobaMode;
        }

        public bool IsMultilMode()
        {
            return this.m_isMultiMode;
        }

        public bool IsSingleWarmBattle()
        {
            return (((this.gameType == enSelectGameType.enPVE_Computer) && (this.roomInfo != null)) && this.roomInfo.roomAttrib.bWarmBattle);
        }

        public bool IsSpecTraingMode()
        {
            return (((this.m_gameType == enSelectGameType.enGuide) && (this.m_stGameOfAdventure != null)) && (this.m_stGameOfAdventure.iLevelID == GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 120).dwConfValue));
        }

        public void LoadArenaDefHeroList(List<uint> defaultHeroList)
        {
            for (int i = 0; i < defaultHeroList.Count; i++)
            {
                uint heroID = defaultHeroList[i];
                if (!CHeroDataFactory.IsHeroCanUse(heroID))
                {
                    heroID = 0;
                }
                if (heroID != 0)
                {
                    this.m_selectHeroIDList[this.m_selectHeroCount] = heroID;
                    this.m_selectHeroCount = (byte) (this.m_selectHeroCount + 1);
                    Singleton<CHeroSelectNormalSystem>.instance.m_showHeroID = heroID;
                }
            }
        }

        public void LoadPveDefaultHeroList()
        {
            if (s_defaultBattleListInfo != null)
            {
                for (int i = 0; i < s_defaultBattleListInfo.dwListNum; i++)
                {
                    if (s_defaultBattleListInfo.astBattleList[i].dwBattleListID == this.m_battleListID)
                    {
                        COMDT_BATTLEHERO stBattleList = s_defaultBattleListInfo.astBattleList[i].stBattleList;
                        for (int j = 0; j < stBattleList.BattleHeroList.Length; j++)
                        {
                            if (j < this.m_selectHeroIDList.Count)
                            {
                                uint heroCfgID = stBattleList.BattleHeroList[j];
                                if (((this.gameType == enSelectGameType.enBurning) && Singleton<BurnExpeditionController>.instance.model.IsHeroInRecord(heroCfgID)) && (Singleton<BurnExpeditionController>.instance.model.Get_HeroHP(heroCfgID) <= 0))
                                {
                                    heroCfgID = 0;
                                }
                                if (!CHeroDataFactory.IsHeroCanUse(heroCfgID))
                                {
                                    heroCfgID = 0;
                                }
                                if (heroCfgID != 0)
                                {
                                    this.m_selectHeroIDList[this.m_selectHeroCount] = heroCfgID;
                                    this.m_selectHeroCount = (byte) (this.m_selectHeroCount + 1);
                                    Singleton<CHeroSelectNormalSystem>.instance.m_showHeroID = heroCfgID;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                s_defaultBattleListInfo = new COMDT_BATTLELIST_LIST();
                s_defaultBattleListInfo.dwListNum = 0;
            }
        }

        private void LoadServerCanUseHeroData()
        {
            MemberInfo masterMemberInfo = this.roomInfo.GetMasterMemberInfo();
            if (masterMemberInfo != null)
            {
                uint num = 0;
                IHeroData item = null;
                for (int i = 0; i < masterMemberInfo.canUseHero.Length; i++)
                {
                    num = masterMemberInfo.canUseHero[i];
                    item = CHeroDataFactory.CreateHeroData(masterMemberInfo.canUseHero[i]);
                    if (item != null)
                    {
                        this.m_canUseHeroList.Add(item);
                    }
                }
            }
        }

        public void OnHeroSkinWearSuc(uint heroId, uint skinId)
        {
            if (this.uiType == enUIType.enNormal)
            {
                Singleton<CHeroSelectNormalSystem>.instance.OnHeroSkinWearSuc(heroId, skinId);
            }
            else if (this.uiType == enUIType.enBanPick)
            {
                Singleton<CHeroSelectBanPickSystem>.instance.OnHeroSkinWearSuc(heroId, skinId);
            }
        }

        public void OpenForm(enSelectGameType gameType, byte mapHeroMaxCount, uint mapID = 0, byte mapType = 0, int isAllowDupHero = 0)
        {
            this.m_fOpenHeroSelectForm = Time.time;
            Singleton<CRoomSystem>.GetInstance().CloseRoom();
            Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
            Singleton<CMatchingSystem>.GetInstance().CloseMatchingConfirm();
            this.CloseForm();
            this.m_isInHeroSelectState = true;
            this.m_gameType = gameType;
            this.m_selectType = this.selectType;
            this.m_mapId = mapID;
            this.m_mapType = mapType;
            this.m_isAllowDupHero = isAllowDupHero;
            this.InitSystem();
            this.InitSelectHeroIDList(mapHeroMaxCount);
            this.InitCanUseHeroData();
            if (this.m_uiType == enUIType.enNormal)
            {
                Singleton<CHeroSelectNormalSystem>.instance.OpenForm();
            }
            else if (this.m_uiType == enUIType.enBanPick)
            {
                Singleton<CHeroSelectBanPickSystem>.instance.OpenForm();
            }
            if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || this.IsSingleWarmBattle())
            {
                Singleton<CChatController>.instance.model.SetCurGroupTemplateInfo(this.m_chatID);
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_Hero_Select_OpenForm);
            }
        }

        public static void PostAdventureSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) Singleton<CAdventureSys>.instance.currentLevelId);
            if ((dataByKey != null) && ((dataByKey.iLevelType == 0) || ((dataByKey.iLevelType == 4) && Singleton<CBattleGuideManager>.instance.bTrainingAdv)))
            {
                uint dwAIPlayerLevel = dataByKey.dwAIPlayerLevel;
                uint[] aIHeroID = dataByKey.AIHeroID;
                stBattlePlayer.astFighter[0].bObjType = 1;
                stBattlePlayer.astFighter[0].bPosOfCamp = 0;
                stBattlePlayer.astFighter[0].bObjCamp = 1;
                for (int i = 0; i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count; i++)
                {
                    stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[i];
                }
                int index = 1;
                for (int j = 0; j < dataByKey.SelfCampAIHeroID.Length; j++)
                {
                    if (dataByKey.SelfCampAIHeroID[j] != 0)
                    {
                        stBattlePlayer.astFighter[index].bPosOfCamp = (byte) (j + 1);
                        stBattlePlayer.astFighter[index].bObjType = 2;
                        stBattlePlayer.astFighter[index].bObjCamp = 1;
                        stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.SelfCampAIHeroID[j];
                        index++;
                    }
                }
                for (int k = 0; k < dataByKey.AIHeroID.Length; k++)
                {
                    if (dataByKey.AIHeroID[k] != 0)
                    {
                        stBattlePlayer.astFighter[index].bPosOfCamp = (byte) k;
                        stBattlePlayer.astFighter[index].bObjType = 2;
                        stBattlePlayer.astFighter[index].bObjCamp = 2;
                        stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.AIHeroID[k];
                        index++;
                    }
                }
                stBattlePlayer.bNum = (byte) index;
            }
        }

        public static void PostArenaSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
        {
            stBattlePlayer.bNum = 2;
            stBattlePlayer.astFighter[0].bObjType = 1;
            stBattlePlayer.astFighter[0].bPosOfCamp = 0;
            stBattlePlayer.astFighter[0].bObjCamp = 1;
            for (int i = 0; i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count; i++)
            {
                stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[i];
            }
            COMDT_ARENA_MEMBER_OF_ACNT tarInfo = Singleton<CArenaSystem>.GetInstance().m_tarInfo;
            stBattlePlayer.astFighter[1].bObjType = 2;
            stBattlePlayer.astFighter[1].bPosOfCamp = 0;
            stBattlePlayer.astFighter[1].dwLevel = tarInfo.dwPVPLevel;
            stBattlePlayer.astFighter[1].bObjCamp = 2;
            for (int j = 0; j < tarInfo.stBattleHero.astHero.Length; j++)
            {
                stBattlePlayer.astFighter[1].astChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID = tarInfo.stBattleHero.astHero[j].dwHeroId;
            }
        }

        public static void PostBurningSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
        {
            BurnExpeditionModel model = Singleton<BurnExpeditionController>.instance.model;
            List<uint> list = model.Get_Enemy_HeroIDS();
            COMDT_PLAYERINFO comdt_playerinfo = model.Get_Current_Enemy_PlayerInfo();
            stBattlePlayer.bNum = 2;
            stBattlePlayer.astFighter[0].bObjType = 1;
            stBattlePlayer.astFighter[0].bPosOfCamp = 0;
            stBattlePlayer.astFighter[0].bObjCamp = 1;
            for (int i = 0; i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count; i++)
            {
                stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[i];
            }
            stBattlePlayer.astFighter[1].bObjType = 2;
            stBattlePlayer.astFighter[1].bPosOfCamp = 0;
            stBattlePlayer.astFighter[1].dwLevel = comdt_playerinfo.dwLevel;
            stBattlePlayer.astFighter[1].bObjCamp = 2;
            comdt_playerinfo.szName.CopyTo(stBattlePlayer.astFighter[1].szName, 0);
            for (int j = 0; j < list.Count; j++)
            {
                for (int k = 0; k < comdt_playerinfo.astChoiceHero.Length; k++)
                {
                    if (comdt_playerinfo.astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID == list[j])
                    {
                        if (comdt_playerinfo.astChoiceHero[k].stBurningInfo.bIsDead == 0)
                        {
                            stBattlePlayer.astFighter[1].astChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID = list[j];
                        }
                        break;
                    }
                }
            }
        }

        public static void PostCombatSingleGame(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
        {
            <PostCombatSingleGame>c__AnonStorey74 storey = new <PostCombatSingleGame>c__AnonStorey74();
            RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
            COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            int dwHeroID = 0;
            for (COM_PLAYERCAMP com_playercamp2 = COM_PLAYERCAMP.COM_PLAYERCAMP_1; com_playercamp2 < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; com_playercamp2 += 1)
            {
                ListView<MemberInfo> view = roomInfo[com_playercamp2];
                for (int i = 0; i < view.Count; i++)
                {
                    if (view[i].ullUid == roomInfo.selfInfo.ullUid)
                    {
                        camp = view[i].camp;
                        dwHeroID = (int) view[i].ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
                        break;
                    }
                }
            }
            List<uint>[] listArray = new List<uint>[2];
            if (Singleton<CHeroSelectBaseSystem>.instance.m_isAllowDupHero == 0)
            {
                listArray[0] = new List<uint>();
                listArray[1] = new List<uint>();
                listArray[(camp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? 1 : 0] = Singleton<CHeroSelectBaseSystem>.instance.GetPveTeamHeroIDList();
            }
            int index = 0;
            storey.canPickHeroNum = 0;
            GameDataMgr.heroDatabin.Accept(new Action<ResHeroCfgInfo>(storey.<>m__5D));
            DebugHelper.Assert(storey.canPickHeroNum >= 3, "Not Enough Hero To Pick!!!");
            for (COM_PLAYERCAMP com_playercamp3 = COM_PLAYERCAMP.COM_PLAYERCAMP_1; com_playercamp3 < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; com_playercamp3 += 1)
            {
                ListView<MemberInfo> view2 = roomInfo[com_playercamp3];
                for (int j = 0; j < view2.Count; j++)
                {
                    MemberInfo info2 = view2[j];
                    if (info2.RoomMemberType == 2)
                    {
                        stBattlePlayer.astFighter[index].bObjType = 2;
                        stBattlePlayer.astFighter[index].bPosOfCamp = (byte) j;
                        stBattlePlayer.astFighter[index].bObjCamp = (byte) com_playercamp3;
                        stBattlePlayer.astFighter[index].dwLevel = 1;
                        for (int k = 0; k < Singleton<CHeroSelectBaseSystem>.instance.m_mapMaxHeroCount; k++)
                        {
                            <PostCombatSingleGame>c__AnonStorey75 storey2 = new <PostCombatSingleGame>c__AnonStorey75();
                            int id = UnityEngine.Random.Range(0, GameDataMgr.heroDatabin.Count());
                            storey2.heroCfg = GameDataMgr.heroDatabin.GetDataByIndex(id);
                            bool flag = GameDataMgr.IsHeroCanBePickByComputer(storey2.heroCfg.dwCfgID);
                            while (!flag)
                            {
                                id = UnityEngine.Random.Range(0, GameDataMgr.heroDatabin.Count());
                                storey2.heroCfg = GameDataMgr.heroDatabin.GetDataByIndex(id);
                                flag = GameDataMgr.IsHeroCanBePickByComputer(storey2.heroCfg.dwCfgID);
                            }
                            if (Singleton<CHeroSelectBaseSystem>.instance.m_isAllowDupHero == 0)
                            {
                                while (((listArray[((int) com_playercamp3) - 1].FindIndex(new Predicate<uint>(storey2.<>m__5E)) != -1) || !flag) || (CSysDynamicBlock.bLobbyEntryBlocked && (storey2.heroCfg.bIOSHide == 1)))
                                {
                                    id = UnityEngine.Random.Range(0, GameDataMgr.heroDatabin.Count());
                                    storey2.heroCfg = GameDataMgr.heroDatabin.GetDataByIndex(id);
                                    flag = GameDataMgr.IsHeroCanBePickByComputer(storey2.heroCfg.dwCfgID);
                                }
                                listArray[((int) com_playercamp3) - 1].Add(storey2.heroCfg.dwCfgID);
                            }
                            stBattlePlayer.astFighter[index].astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID = storey2.heroCfg.dwCfgID;
                        }
                    }
                    else if (info2.RoomMemberType == 1)
                    {
                        stBattlePlayer.astFighter[index].bObjType = 1;
                        stBattlePlayer.astFighter[index].bPosOfCamp = (byte) j;
                        stBattlePlayer.astFighter[index].bObjCamp = (byte) camp;
                        for (int m = 0; m < Singleton<CHeroSelectBaseSystem>.instance.m_mapMaxHeroCount; m++)
                        {
                            stBattlePlayer.astFighter[index].astChoiceHero[m].stBaseInfo.stCommonInfo.dwHeroID = (uint) dwHeroID;
                        }
                    }
                    index++;
                }
            }
            stBattlePlayer.bNum = (byte) index;
        }

        [MessageHandler(0x48f)]
        public static void ReciveAddedSkillSel(CSPkg msg)
        {
            if (msg.stPkgData.stUnlockSkillSelRsp.iResult == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                bool flag = false;
                if ((masterRoleInfo != null) && (masterRoleInfo.playerUllUID == msg.stPkgData.stUnlockSkillSelRsp.ullAcntUid))
                {
                    masterRoleInfo.SetHeroSelSkillID(msg.stPkgData.stUnlockSkillSelRsp.dwHeroID, msg.stPkgData.stUnlockSkillSelRsp.dwSkillID);
                    flag = true;
                }
                RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
                if (roomInfo != null)
                {
                    MemberInfo memberInfo = roomInfo.GetMemberInfo(msg.stPkgData.stUnlockSkillSelRsp.ullAcntUid);
                    if ((memberInfo != null) && (memberInfo.ChoiceHero[0] != null))
                    {
                        memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID = msg.stPkgData.stUnlockSkillSelRsp.dwSkillID;
                    }
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
                {
                    Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(false, true);
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
                {
                    Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
                    Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
                    if (flag)
                    {
                        Singleton<CHeroSelectBanPickSystem>.instance.RefreshAddedSkillItem();
                    }
                }
                Singleton<CUIManager>.instance.CloseSendMsgAlert();
            }
        }

        [MessageHandler(0x70f)]
        public static void ReciveBATTLELIST_NTY(CSPkg msg)
        {
        }

        [MessageHandler(0x70e)]
        public static void ReciveBATTLELIST_RSP(CSPkg msg)
        {
            SendSingleGameStartMsg(Singleton<CHeroSelectBaseSystem>.instance.roomInfo, Singleton<CHeroSelectBaseSystem>.instance.m_battleListID, Singleton<CHeroSelectBaseSystem>.instance.gameType, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList);
        }

        [MessageHandler(0x4dc)]
        public static void ReciveDefaultSelectHeroes(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (Singleton<GameDataValidator>.instance.ValidateGameData())
            {
                SCPKG_DEFAULT_HERO_NTF stDefaultHeroNtf = msg.stPkgData.stDefaultHeroNtf;
                RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
                bool flag = false;
                for (int i = 0; i < stDefaultHeroNtf.bAcntNum; i++)
                {
                    COMDT_PLAYERINFO comdt_playerinfo = stDefaultHeroNtf.astDefaultHeroGrp[i];
                    MemberInfo memberInfo = roomInfo.GetMemberInfo((COM_PLAYERCAMP) comdt_playerinfo.bObjCamp, comdt_playerinfo.bPosOfCamp);
                    if (memberInfo != null)
                    {
                        memberInfo.ChoiceHero = comdt_playerinfo.astChoiceHero;
                        if (memberInfo.dwObjId == roomInfo.selfObjID)
                        {
                            Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            flag = true;
                        }
                    }
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
                {
                    if (flag)
                    {
                        Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                    }
                    Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(false, true);
                }
            }
        }

        [MessageHandler(0x4d9)]
        public static void ReciveHeroSelect(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stOperHeroRsp.bErrCode != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("HeroIsSelectByOther", true, 1.5f, null, new object[0]);
            }
            else if (Singleton<GameDataValidator>.instance.ValidateGameData())
            {
                RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
                if (roomInfo != null)
                {
                    MemberInfo memberInfo = roomInfo.GetMemberInfo(msg.stPkgData.stOperHeroRsp.stChoiceHero.dwObjId);
                    bool bForceRefreshAddSkillPanel = false;
                    if (memberInfo != null)
                    {
                        memberInfo.ChoiceHero = msg.stPkgData.stOperHeroRsp.stChoiceHero.astChoiceHero;
                        if (memberInfo.dwObjId == Singleton<CHeroSelectBaseSystem>.instance.roomInfo.selfObjID)
                        {
                            Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            bForceRefreshAddSkillPanel = true;
                        }
                        if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
                        {
                            if (bForceRefreshAddSkillPanel)
                            {
                                Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
                                if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
                                {
                                    Singleton<CHeroSelectNormalSystem>.instance.RefreshSkinPanel(null);
                                }
                            }
                            Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(bForceRefreshAddSkillPanel, bForceRefreshAddSkillPanel);
                        }
                        else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
                        {
                            Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
                            Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
                            Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
                            if (bForceRefreshAddSkillPanel)
                            {
                                Singleton<CHeroSelectBanPickSystem>.instance.RefreshBottom();
                            }
                        }
                    }
                }
            }
        }

        [MessageHandler(0x472)]
        public static void ReciveHeroSymbolPageSel(CSPkg msg)
        {
            uint dwHeroID = msg.stPkgData.stSymbolPageChgRsp.dwHeroID;
            int bPageIdx = msg.stPkgData.stSymbolPageChgRsp.bPageIdx;
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SetHeroSymbolPageIdx(dwHeroID, bPageIdx);
            if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
            {
                Singleton<CHeroSelectNormalSystem>.GetInstance().OnSymbolPageChange();
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
            {
                Singleton<CHeroSelectBanPickSystem>.GetInstance().OnSymbolPageChange();
            }
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
        }

        [MessageHandler(0x42f)]
        public static void ReciveMultiAjustHeroBegin(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enSwap;
            int dwTimeout = (int) msg.stPkgData.stMultGameBeginAdjust.dwTimeout;
            if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
            {
                if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enClone)
                {
                    Singleton<CHeroSelectNormalSystem>.instance.StartEndTimer(dwTimeout);
                    Singleton<CUIManager>.instance.OpenTips("Clone_Swap_Tips", true, 1.5f, null, new object[0]);
                }
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
            {
                Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
                Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
                Singleton<CHeroSelectBanPickSystem>.instance.StartEndTimer(dwTimeout);
                Singleton<CSoundManager>.GetInstance().PostEvent("Set_BanPickEnd", null);
            }
        }

        [MessageHandler(0x42d)]
        public static void ReciveMultiBanBegin(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
            {
                StartPvpHeroSelectSystem(msg.stPkgData.stMultGameBeginBan.stDeskInfo, msg.stPkgData.stMultGameBeginBan.astCampInfo, msg.stPkgData.stMultGameBeginBan.stFreeHero, msg.stPkgData.stMultGameBeginBan.stFreeHeroSymbol);
            }
            Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = msg.stPkgData.stMultGameBeginBan.bBanPosNum;
            Singleton<CHeroSelectBanPickSystem>.instance.RefreshTop();
            Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
            Singleton<CSoundManager>.GetInstance().PostEvent("Play_Music_BanPick", null);
        }

        [MessageHandler(0x42e)]
        public static void ReciveMultiChooseHeroBegin(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
            {
                StartPvpHeroSelectSystem(msg.stPkgData.stMultGameBeginPick.stDeskInfo, msg.stPkgData.stMultGameBeginPick.astCampInfo, msg.stPkgData.stMultGameBeginPick.stFreeHero, msg.stPkgData.stMultGameBeginPick.stFreeHeroSymbol);
            }
            if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enBanPick)
            {
                Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enPick;
                Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
                Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(true);
                Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
                Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment1", null);
            }
        }

        [MessageHandler(0x145b)]
        public static void RecivePlayerBanOrPick(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_NTF_CUR_BAN_PICK_INFO stNtfCurBanPickInfo = msg.stPkgData.stNtfCurBanPickInfo;
            Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo = stNtfCurBanPickInfo;
            Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
            Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
            Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
            Singleton<CHeroSelectBanPickSystem>.instance.PlayCurrentBgAnimation();
            RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
            if (roomInfo != null)
            {
                MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                    if ((memberInfo != null) && (masterMemberInfo == memberInfo))
                    {
                        Utility.VibrateHelper();
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_MyTurn", null);
                        if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment2", null);
                        }
                    }
                }
            }
        }

        [MessageHandler(0x145d)]
        public static void RecivePlayerBanRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_BAN_HERO_RSP stBanHeroRsp = msg.stPkgData.stBanHeroRsp;
            RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
            if ((roomInfo != null) && (roomInfo.GetMasterMemberInfo() != null))
            {
                Singleton<CHeroSelectBaseSystem>.instance.AddBanHero((COM_PLAYERCAMP) stBanHeroRsp.bCamp, stBanHeroRsp.dwHeroID);
                Singleton<CHeroSelectBanPickSystem>.instance.RefreshTop();
                Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_Ban_Button", null);
            }
        }

        [MessageHandler(0x4db)]
        public static void RecivePlayerConfirm(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (Singleton<GameDataValidator>.instance.ValidateGameData())
            {
                SCPKG_CONFIRM_HERO_NTF stConfirmHeroNtf = msg.stPkgData.stConfirmHeroNtf;
                bool flag = false;
                RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
                if (roomInfo != null)
                {
                    MemberInfo memberInfo = roomInfo.GetMemberInfo(stConfirmHeroNtf.dwObjId);
                    if (memberInfo != null)
                    {
                        memberInfo.isPrepare = true;
                        if (memberInfo.dwObjId == roomInfo.selfObjID)
                        {
                            Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = true;
                            flag = true;
                        }
                        if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
                        {
                            if ((memberInfo.dwObjId == roomInfo.selfObjID) && (Singleton<CHeroSelectBaseSystem>.instance.selectType != enSelectType.enRandom))
                            {
                                Singleton<CHeroSelectNormalSystem>.instance.SwitchSkinMenuSelect();
                            }
                            Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(false, memberInfo.dwObjId == roomInfo.selfObjID);
                            if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enClone)
                            {
                                MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
                                if (masterMemberInfo == null)
                                {
                                    return;
                                }
                                if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsAllConfirmHeroByTeam(masterMemberInfo.camp))
                                {
                                    Singleton<CUIManager>.instance.OpenTips("Clone_Confirm_Tips", true, 1.5f, null, new object[0]);
                                }
                            }
                        }
                        else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
                        {
                            Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
                            Singleton<CHeroSelectBanPickSystem>.instance.RefreshCenter();
                            Singleton<CHeroSelectBanPickSystem>.instance.RefreshRight();
                            if (flag)
                            {
                                Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
                            }
                        }
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_Select_Hero", null);
                    }
                }
            }
        }

        [MessageHandler(0x1461)]
        public static void RecivePlayerSwapConfirmRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_NTF_CONFIRM_SWAP_HERO stNtfConfirmSwapHero = msg.stPkgData.stNtfConfirmSwapHero;
            Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enIdle;
            Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
            Singleton<CHeroSelectBanPickSystem>.instance.RefreshSwapPanel();
            if (stNtfConfirmSwapHero.bAccept == 1)
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_Select_Hero", null);
            }
        }

        [MessageHandler(0x146b)]
        public static void RecivePlayerSwapReqCancelRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stCancelSwapHeroRsp.iErrCode == 0)
            {
                Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enIdle;
                Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
                Singleton<CHeroSelectBanPickSystem>.instance.RefreshSwapPanel();
            }
        }

        [MessageHandler(0x145f)]
        public static void RecivePlayerSwapReqRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_NTF_SWAP_HERO stNtfSwapHero = msg.stPkgData.stNtfSwapHero;
            RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
            if (roomInfo != null)
            {
                MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    if (stNtfSwapHero.iErrCode == 0)
                    {
                        if (stNtfSwapHero.dwActiveObjID == masterMemberInfo.dwObjId)
                        {
                            Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enReqing;
                            Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo = stNtfSwapHero;
                        }
                        else if (stNtfSwapHero.dwPassiveObjID == masterMemberInfo.dwObjId)
                        {
                            Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enSwapAllow;
                            Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo = stNtfSwapHero;
                        }
                        Singleton<CHeroSelectBanPickSystem>.instance.RefreshLeft();
                        Singleton<CHeroSelectBanPickSystem>.instance.RefreshSwapPanel();
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_Exchange_Hero", null);
                    }
                    else if (stNtfSwapHero.iErrCode == 0x9c)
                    {
                        Singleton<CUIManager>.instance.OpenTips("BP_ChangeHero_Busy", true, 1.5f, null, new object[0]);
                    }
                    else if (stNtfSwapHero.iErrCode == 0x9d)
                    {
                        Singleton<CUIManager>.instance.OpenTips("BP_ChangeHero_Busy", true, 1.5f, null, new object[0]);
                    }
                }
            }
        }

        [MessageHandler(0x423)]
        public static void ReciveQuitSingleGame(CSPkg msg)
        {
            if (msg.stPkgData.stQuitSingleGameRsp.bErrCode == 0)
            {
                Singleton<CHeroSelectNormalSystem>.instance.CloseForm();
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x423, msg.stPkgData.stQuitSingleGameRsp.bErrCode), false, 1.5f, null, new object[0]);
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CUIEvent uiEvent = new CUIEvent {
                m_eventID = enUIEventID.ReplayKit_Pause_Recording
            };
            uiEvent.m_eventParams.tag2 = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        [MessageHandler(0x7dc)]
        public static void ReciveSingleChooseHeroBegin(CSPkg msg)
        {
            CSDT_SINGLE_GAME_OF_COMBAT reportInfo = new CSDT_SINGLE_GAME_OF_COMBAT();
            if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo != null)
            {
                reportInfo.bRoomType = (byte) Singleton<CRoomSystem>.instance.RoomType;
                reportInfo.dwMapId = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.dwMapId;
                reportInfo.bMapType = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.bMapType;
                reportInfo.bAILevel = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.npcAILevel;
            }
            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(reportInfo.bMapType, reportInfo.dwMapId);
            Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithCombat(pvpMapCommonInfo.dwHeroFormId, reportInfo, "Room Type");
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            DebugHelper.Assert(roomInfo != null);
            if ((roomInfo != null) && roomInfo.roomAttrib.bWarmBattle)
            {
                CUIEvent uiEvent = new CUIEvent {
                    m_eventID = enUIEventID.Matching_OpenConfirmBox
                };
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
                CFakePvPHelper.SetConfirmFakeData();
                CFakePvPHelper.StartFakeConfirm();
            }
            else
            {
                Singleton<LobbyLogic>.GetInstance().inMultiRoom = false;
                Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enPVE_Computer, 1, reportInfo.dwMapId, reportInfo.bMapType, 0);
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        public void ResetRandHeroLeftCount(int inLeftCount)
        {
            this.m_UseRandSelCount = inLeftCount;
        }

        public void SavePveDefaultHeroList()
        {
            if (s_defaultBattleListInfo != null)
            {
                bool flag = false;
                for (int i = 0; i < s_defaultBattleListInfo.dwListNum; i++)
                {
                    if (s_defaultBattleListInfo.astBattleList[i].dwBattleListID == this.m_battleListID)
                    {
                        COMDT_BATTLEHERO stBattleList = s_defaultBattleListInfo.astBattleList[i].stBattleList;
                        stBattleList.wHeroCnt = (ushort) this.m_selectHeroIDList.Count;
                        stBattleList.BattleHeroList = this.m_selectHeroIDList.ToArray();
                        flag = true;
                    }
                }
                if (!flag)
                {
                    ListLinqView<COMDT_BATTLELIST> view = new ListLinqView<COMDT_BATTLELIST>();
                    for (int j = 0; j < s_defaultBattleListInfo.dwListNum; j++)
                    {
                        view.Add(s_defaultBattleListInfo.astBattleList[j]);
                    }
                    COMDT_BATTLELIST item = new COMDT_BATTLELIST {
                        dwBattleListID = this.m_battleListID,
                        stBattleList = new COMDT_BATTLEHERO()
                    };
                    item.stBattleList.wHeroCnt = (ushort) this.m_selectHeroIDList.Count;
                    item.stBattleList.BattleHeroList = this.m_selectHeroIDList.ToArray();
                    view.Add(item);
                    s_defaultBattleListInfo.dwListNum = (uint) view.Count;
                    s_defaultBattleListInfo.astBattleList = view.ToArray();
                }
            }
        }

        public static void SendBanHeroMsg(uint heroID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x145c);
            msg.stPkgData.stBanHeroReq.dwHeroID = heroID;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
        }

        public static void SendCanelSwapHeroMsg()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x146a);
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
        }

        public static void SendHeroSelectMsg(byte operType, byte operPos, uint heroID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4d8);
            msg.stPkgData.stOperHeroReq.bOperType = operType;
            msg.stPkgData.stOperHeroReq.stOperDetail = new CSDT_OPER_HERO();
            if (operType == 0)
            {
                msg.stPkgData.stOperHeroReq.stOperDetail.stSetHero = new CSDT_SETHERO();
                msg.stPkgData.stOperHeroReq.stOperDetail.stSetHero.bHeroPos = operPos;
                msg.stPkgData.stOperHeroReq.stOperDetail.stSetHero.dwHeroId = heroID;
            }
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
        }

        public static void SendHeroSelectSymbolPage(uint heroId, int selIndex, bool bSendGame = false)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x471);
            msg.stPkgData.stSymbolPageChgReq = new CSPKG_CMD_SYMBOLPAGESEL();
            msg.stPkgData.stSymbolPageChgReq.dwHeroID = heroId;
            msg.stPkgData.stSymbolPageChgReq.bPageIdx = (byte) selIndex;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendMuliPrepareToBattleMsg()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4da);
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
        }

        public static void SendQuitSingleGameReq()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x422);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendSingleGameStartMsg(RoomInfo roomInfo, uint battleListID, enSelectGameType selectGameType, byte selectHeroCount, List<uint> selectHeroIDList)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x41a);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            msg.stPkgData.stStartSingleGameReq.stBattleParam.bGameType = (byte) selectGameType;
            msg.stPkgData.stStartSingleGameReq.stBattleList.dwBattleListID = battleListID;
            if (selectGameType == enSelectGameType.enPVE_Adventure)
            {
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfAdventure = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfAdventure;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = selectHeroCount;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
                masterRoleInfo.battleHeroList = selectHeroIDList;
                Singleton<CHeroSelectBaseSystem>.instance.SavePveDefaultHeroList();
                PostAdventureSingleGame(msg.stPkgData.stStartSingleGameReq.stBattlePlayer);
            }
            else if (selectGameType == enSelectGameType.enPVE_Computer)
            {
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfCombat = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfCombat;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = 1;
                MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList[0] = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
                PostCombatSingleGame(msg.stPkgData.stStartSingleGameReq.stBattlePlayer);
            }
            else if (selectGameType == enSelectGameType.enBurning)
            {
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfBurning = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfBurnning;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = selectHeroCount;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
                Singleton<CHeroSelectBaseSystem>.instance.SavePveDefaultHeroList();
                PostBurningSingleGame(msg.stPkgData.stStartSingleGameReq.stBattlePlayer);
            }
            else if (selectGameType == enSelectGameType.enArena)
            {
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfArena = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfArena;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = selectHeroCount;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
                Singleton<CHeroSelectBaseSystem>.instance.SavePveDefaultHeroList();
                PostArenaSingleGame(msg.stPkgData.stStartSingleGameReq.stBattlePlayer);
            }
            else if (selectGameType == enSelectGameType.enGuide)
            {
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = selectHeroCount;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList = selectHeroIDList.ToArray();
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.construct(2L);
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfGuide.iLevelID = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfAdventure.iLevelID;
                masterRoleInfo.battleHeroList = selectHeroIDList;
                PostAdventureSingleGame(msg.stPkgData.stStartSingleGameReq.stBattlePlayer);
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendSingleGameStartMsgSkipHeroSelect(int iLevelID, int iDifficult)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) iLevelID);
            DebugHelper.Assert(dataByKey != null);
            CSDT_SINGLE_GAME_OF_ADVENTURE reportInfo = new CSDT_SINGLE_GAME_OF_ADVENTURE {
                iLevelID = iLevelID,
                bChapterNo = (byte) dataByKey.iChapterId,
                bLevelNo = dataByKey.bLevelNo,
                bDifficultType = (byte) iDifficult
            };
            byte iHeroNum = (byte) dataByKey.iHeroNum;
            uint dwBattleListID = dataByKey.dwBattleListID;
            Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithAdventure(dwBattleListID, reportInfo, string.Empty);
            Singleton<CHeroSelectBaseSystem>.instance.InitSelectHeroIDList(iHeroNum);
            Singleton<CHeroSelectBaseSystem>.instance.LoadPveDefaultHeroList();
            SendSinglePrepareToBattleMsg(Singleton<CHeroSelectBaseSystem>.instance.roomInfo, Singleton<CHeroSelectBaseSystem>.instance.m_battleListID, Singleton<CHeroSelectBaseSystem>.instance.gameType, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList);
        }

        public static void SendSinglePrepareToBattleMsg(RoomInfo roomInfo, uint battleListID, enSelectGameType selectGameType, byte selectHeroCount, List<uint> selectHeroIDList)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x70d);
            msg.stPkgData.stBattleListReq.stBattleList.dwBattleListID = battleListID;
            if (selectGameType == enSelectGameType.enPVE_Computer)
            {
                msg.stPkgData.stBattleListReq.stBattleList.stBattleList.wHeroCnt = 1;
                MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
                msg.stPkgData.stBattleListReq.stBattleList.stBattleList.BattleHeroList[0] = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
            }
            else
            {
                msg.stPkgData.stBattleListReq.stBattleList.stBattleList.wHeroCnt = selectHeroCount;
                for (int i = 0; i < msg.stPkgData.stBattleListReq.stBattleList.stBattleList.wHeroCnt; i++)
                {
                    msg.stPkgData.stBattleListReq.stBattleList.stBattleList.BattleHeroList[i] = selectHeroIDList[i];
                }
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendSwapAcceptHeroMsg(byte bAccept)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1460);
            msg.stPkgData.stConfirmSwapHeroReq.bAccept = bAccept;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
        }

        public static void SendSwapHeroMsg(uint passiveObjID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x145e);
            msg.stPkgData.stSwapHeroReq.dwPassiveObjID = passiveObjID;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
        }

        public void SetPVEDataWithAdventure(uint battleListID, CSDT_SINGLE_GAME_OF_ADVENTURE reportInfo, string levleName = "")
        {
            this.m_battleListID = battleListID;
            this.m_stGameOfAdventure = reportInfo;
            this.m_gameType = enSelectGameType.enPVE_Adventure;
        }

        public void SetPveDataWithArena(uint battleListID, CSDT_SINGLE_GAME_OF_ARENA reportInfo = null, string levleName = "")
        {
            this.m_battleListID = battleListID;
            this.m_stGameOfArena = reportInfo;
        }

        public void SetPVEDataWithBurnExpedition(uint battleListID, CSDT_SINGLE_GAME_OF_BURNING reportInfo, string levleName = "")
        {
            this.m_battleListID = battleListID;
            this.m_stGameOfBurnning = reportInfo;
        }

        public void SetPVEDataWithCombat(uint battleListID, CSDT_SINGLE_GAME_OF_COMBAT reportInfo, string levleName = "")
        {
            this.m_battleListID = battleListID;
            this.m_stGameOfCombat = reportInfo;
        }

        public void SetPvpHeroSelect(uint heroID)
        {
            Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0] = heroID;
            Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount = 1;
        }

        public static void StartPvpHeroSelectSystem(COMDT_DESKINFO deskInfo, CSDT_CAMPINFO[] campInfo, COMDT_FREEHERO freeHero, COMDT_FREEHERO_INACNT freeHeroSymbol)
        {
            InitRoomData(deskInfo, campInfo, freeHero, freeHeroSymbol);
            enSelectGameType enNull = enSelectGameType.enNull;
            if (deskInfo.bMapType == 1)
            {
                enNull = enSelectGameType.enPVP;
            }
            else if (deskInfo.bMapType == 3)
            {
                enNull = enSelectGameType.enLadder;
            }
            else if (deskInfo.bMapType == 4)
            {
                enNull = enSelectGameType.enLuanDou;
            }
            else if (deskInfo.bMapType == 5)
            {
                enNull = enSelectGameType.enUnion;
            }
            uint dwMapId = deskInfo.dwMapId;
            byte bMapType = deskInfo.bMapType;
            Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enNull, 1, dwMapId, bMapType, 0);
        }

        public static int WhoIsBestHero(ref Assets.Scripts.GameSystem.Calc9SlotHeroData[] heroes)
        {
            if (IsBetterHero(ref heroes[0], ref heroes[1]) && IsBetterHero(ref heroes[0], ref heroes[2]))
            {
                return 0;
            }
            if (IsBetterHero(ref heroes[1], ref heroes[0]) && IsBetterHero(ref heroes[1], ref heroes[2]))
            {
                return 1;
            }
            return 2;
        }

        public enSelectGameType gameType
        {
            get
            {
                return this.m_gameType;
            }
        }

        public RoomInfo roomInfo
        {
            get
            {
                return Singleton<CRoomSystem>.GetInstance().roomInfo;
            }
        }

        public enSelectType selectType
        {
            get
            {
                return this.m_selectType;
            }
        }

        public enUIType uiType
        {
            get
            {
                return this.m_uiType;
            }
        }

        [CompilerGenerated]
        private sealed class <PostCombatSingleGame>c__AnonStorey74
        {
            internal int canPickHeroNum;

            internal void <>m__5D(ResHeroCfgInfo heroCfg)
            {
                if ((heroCfg != null) && GameDataMgr.IsHeroCanBePickByComputer(heroCfg.dwCfgID))
                {
                    this.canPickHeroNum++;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <PostCombatSingleGame>c__AnonStorey75
        {
            internal ResHeroCfgInfo heroCfg;

            internal bool <>m__5E(uint x)
            {
                return (x == this.heroCfg.dwCfgID);
            }
        }
    }
}

