namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CFakePvPHelper
    {
        private static Dictionary<ulong, uint> ChosenHeroes;
        public const int CONFIRM_TIMEOUT = 30;
        private static int ConfirmedFakePlayerNum;
        private static int CurrentSelectTime;
        private static List<uint> EnemyChosenHeroes;
        public static readonly int[] FAKE_CONFIRM_MAP_3V3;
        public static readonly int[] FAKE_CONFIRM_MAP_5V5;
        private static ListView<FakeHeroSelect> FakeHeroSelectList;
        private static int FakePlayerConfirmNum;
        private static ListView<FakePlayerConfirm> FakePlayerList;
        private static int HeroConfirmedPlayerNum;
        private static int MapPlayerNum;
        public const int MAX_CHANGE_HERO_TIME = 3;
        public const int MAX_CONFIRM_TIME = 20;
        public const int MAX_LUANDOU_CHANGE_HERO_TIME = 2;
        public const int MAX_ROOKIE_LEVEL = 6;
        private static int RealPlayerConfirmNum;
        public const int SELECT_HERO_TIMER_COUNT = 60;

        static CFakePvPHelper()
        {
            int[] numArray1 = new int[3];
            numArray1[0] = 2;
            numArray1[2] = 1;
            FAKE_CONFIRM_MAP_3V3 = numArray1;
            FAKE_CONFIRM_MAP_5V5 = new int[] { 3, 1, 0, 4, 3 };
            FakePlayerList = new ListView<FakePlayerConfirm>();
            ChosenHeroes = new Dictionary<ulong, uint>();
            EnemyChosenHeroes = new List<uint>();
            FakeHeroSelectList = new ListView<FakeHeroSelect>();
        }

        public static void BeginFakeSelectHero()
        {
            HeroConfirmedPlayerNum = 0;
            FakeHeroSelectList.Clear();
            ChosenHeroes.Clear();
            EnemyChosenHeroes.Clear();
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            DebugHelper.Assert(roomInfo != null);
            DebugHelper.Assert(roomInfo.roomAttrib.bWarmBattle);
            COM_PLAYERCAMP selfCamp = roomInfo.GetSelfCamp();
            ListView<MemberInfo> view = roomInfo[selfCamp];
            for (int i = 0; i < view.Count; i++)
            {
                MemberInfo info2 = view[i];
                if (info2.RoomMemberType == 2)
                {
                    FakeHeroSelect fakeSelect = new FakeHeroSelect {
                        FakePlayer = info2.WarmNpc
                    };
                    if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
                    {
                        fakeSelect.maxChangeHeroCount = UnityEngine.Random.Range(0, 2);
                        DoSelectAction(ref fakeSelect);
                    }
                    else
                    {
                        fakeSelect.maxChangeHeroCount = UnityEngine.Random.Range(1, 4);
                    }
                    fakeSelect.nextActionSec = UnityEngine.Random.Range(3, 6);
                    FakeHeroSelectList.Add(fakeSelect);
                }
            }
            if (FakeHeroSelectList.Count > 0)
            {
                Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 60, new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeSelectHero));
                bInFakeSelect = true;
            }
            Singleton<CTimerManager>.GetInstance().AddTimer(0xea60, 1, new CTimer.OnTimeUpHandler(CFakePvPHelper.OnSelectHeroTimeout));
        }

        private static void DatabinCheck()
        {
            DebugHelper.Assert(GameDataMgr.robotRookieHeroSkinDatabin.Count() > 5, "Not Enough Hero");
            DebugHelper.Assert(GameDataMgr.robotVeteranHeroSkinDatabin.Count() > 5, "Not Enough Hero");
            int num = GameDataMgr.robotPlayerSkillDatabin.Count();
            for (int i = 0; i < num; i++)
            {
                ResFakeAcntSkill dataByIndex = GameDataMgr.robotPlayerSkillDatabin.GetDataByIndex(i);
                uint num3 = 0;
                for (int j = 0; j < dataByIndex.SkillId.Length; j++)
                {
                    num3 += dataByIndex.SkillId[j];
                }
                DebugHelper.Assert(num3 > 0, "Invalid PlayerSkill Databin");
            }
        }

        private static void DoSelectAction(ref FakeHeroSelect fakeSelect)
        {
            if (IsInSelectHero())
            {
                if (fakeSelect.changeHeroCount < fakeSelect.maxChangeHeroCount)
                {
                    int id = 0;
                    uint dwHeroID = 0;
                    ResFakeAcntHero dataByIndex = null;
                    if (fakeSelect.FakePlayer.dwAcntPvpLevel <= 6)
                    {
                        id = UnityEngine.Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
                        dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
                        if (dataByIndex == null)
                        {
                            return;
                        }
                        for (dwHeroID = dataByIndex.dwHeroID; ChosenHeroes.ContainsValue(dataByIndex.dwHeroID); dwHeroID = dataByIndex.dwHeroID)
                        {
                            id = UnityEngine.Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
                            dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
                            if (dataByIndex == null)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        int num3 = GameDataMgr.robotVeteranHeroSkinDatabin.Count();
                        int max = num3 + ((masterRoleInfo == null) ? 0 : masterRoleInfo.freeHeroList.Count);
                        id = UnityEngine.Random.Range(0, max);
                        if (id < num3)
                        {
                            dataByIndex = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(id);
                            if (dataByIndex == null)
                            {
                                return;
                            }
                            dwHeroID = dataByIndex.dwHeroID;
                        }
                        else
                        {
                            dwHeroID = masterRoleInfo.freeHeroList[id - num3].dwFreeHeroID;
                        }
                        while (ChosenHeroes.ContainsValue(dwHeroID))
                        {
                            id = UnityEngine.Random.Range(0, max);
                            if (id < num3)
                            {
                                dataByIndex = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(id);
                                if (dataByIndex == null)
                                {
                                    return;
                                }
                                dwHeroID = dataByIndex.dwHeroID;
                            }
                            else
                            {
                                dwHeroID = masterRoleInfo.freeHeroList[id - num3].dwFreeHeroID;
                            }
                        }
                    }
                    ChosenHeroes[fakeSelect.FakePlayer.ullUid] = dwHeroID;
                    fakeSelect.selectedHero = dwHeroID;
                    fakeSelect.changeHeroCount++;
                    RoomInfo roomInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo;
                    if (roomInfo != null)
                    {
                        MemberInfo memberInfo = roomInfo.GetMemberInfo(fakeSelect.FakePlayer.ullUid);
                        if ((memberInfo != null) && (memberInfo.ChoiceHero[0] != null))
                        {
                            memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dwHeroID;
                            Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(false, true);
                        }
                    }
                }
                else
                {
                    if (fakeSelect.FakePlayer.dwAcntPvpLevel <= 6)
                    {
                        ResFakeAcntHero hero2 = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByKey(fakeSelect.selectedHero);
                        if (((hero2 != null) && (hero2.dwSkinID != 0)) && (UnityEngine.Random.Range(0, 0x2710) < hero2.dwSkinRate))
                        {
                            fakeSelect.selectedSkin = (int) hero2.dwSkinID;
                        }
                    }
                    else
                    {
                        ResFakeAcntHero hero3 = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByKey(fakeSelect.selectedHero);
                        if (((hero3 != null) && (hero3.dwSkinID != 0)) && (UnityEngine.Random.Range(0, 0x2710) < hero3.dwSkinRate))
                        {
                            fakeSelect.selectedSkin = (int) hero3.dwSkinID;
                        }
                    }
                    ResFakeAcntSkill dataByKey = GameDataMgr.robotPlayerSkillDatabin.GetDataByKey(fakeSelect.FakePlayer.dwAcntPvpLevel);
                    if (dataByKey != null)
                    {
                        int index = UnityEngine.Random.Range(0, dataByKey.SkillId.Length);
                        while (dataByKey.SkillId[index] == 0)
                        {
                            index = UnityEngine.Random.Range(0, dataByKey.SkillId.Length);
                        }
                        uint num8 = dataByKey.SkillId[index];
                        fakeSelect.selectedPlayerSkill = num8;
                    }
                    fakeSelect.bConfirmed = true;
                    HeroConfirmedPlayerNum++;
                    MemberInfo info5 = Singleton<CRoomSystem>.GetInstance().roomInfo.GetMemberInfo(fakeSelect.FakePlayer.ullUid);
                    if ((info5 != null) && (info5.ChoiceHero[0] != null))
                    {
                        info5.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID = fakeSelect.selectedPlayerSkill;
                        info5.isPrepare = true;
                        Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(false, true);
                    }
                    if (HeroConfirmedPlayerNum == (FakeHeroSelectList.Count + 1))
                    {
                        ReqStartSingleWarmBattle();
                    }
                }
            }
        }

        private static void FakeConfirmLater(int seq)
        {
            if (FakePlayerConfirmNum < (MapPlayerNum / 2))
            {
                int[] numArray = (MapPlayerNum != 6) ? FAKE_CONFIRM_MAP_5V5 : FAKE_CONFIRM_MAP_3V3;
                FakePlayerConfirm confirm = FakePlayerList[numArray[FakePlayerConfirmNum]];
                confirm.bConfirmed = true;
                FakePlayerConfirmNum++;
                CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
                instance.confirmPlayerNum++;
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
                if (form != null)
                {
                    CMatchingView.UpdateConfirmBox(form.gameObject, confirm.FakePlayer.ullUid);
                }
            }
        }

        public static void FakeLoadProcess(float progress)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUILoadingSystem.PVP_PATH_LOADING);
            if (form != null)
            {
                List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Computer)
                    {
                        int num = enumerator.Current.CampPos + 1;
                        GameObject obj2 = null;
                        obj2 = (enumerator.Current.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? form.gameObject.transform.FindChild("DownPanel").FindChild(string.Format("Down_Player{0}", num)).gameObject : form.gameObject.transform.FindChild("UpPanel").FindChild(string.Format("Up_Player{0}", num)).gameObject;
                        if (obj2 != null)
                        {
                            GameObject gameObject = obj2.transform.Find("Txt_LoadingPct").gameObject;
                            if (gameObject != null)
                            {
                                Text component = gameObject.GetComponent<Text>();
                                string text = component.text;
                                if (text != "100%")
                                {
                                    int num4;
                                    int max = UnityEngine.Random.Range(1, 30);
                                    int num3 = Math.Min(100, Convert.ToInt32((float) ((progress * 100f) + UnityEngine.Random.Range(0, max))));
                                    int.TryParse(text.Substring(0, 2), out num4);
                                    if (num3 > num4)
                                    {
                                        component.text = string.Format("{0}%", num3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void FakeSendChat(string content)
        {
            try
            {
                RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
                DebugHelper.Assert(roomInfo != null);
                CChatEntity chatEnt = new CChatEntity {
                    ullUid = roomInfo.selfInfo.ullUid,
                    iLogicWorldID = (uint) roomInfo.selfInfo.iLogicWorldId,
                    type = EChaterType.Self,
                    text = content
                };
                CChatUT.GetUser(chatEnt.type, chatEnt.ullUid, chatEnt.iLogicWorldID, out chatEnt.name, out chatEnt.level, out chatEnt.head_url, out chatEnt.stGameVip);
                Singleton<CChatController>.instance.model.channelMgr.Add_ChatEntity(chatEnt, EChatChannel.Select_Hero, 0L, 0);
                Singleton<EventRouter>.instance.BroadCastEvent("Chat_HeorSelectChatData_Change");
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "Fake Send Chat, {0}", inParameters);
            }
        }

        public static void FakeSendChatTemplate(int index)
        {
            FakeSendChat(Singleton<CChatController>.instance.model.Get_HeroSelect_ChatTemplate(index).GetTemplateString());
        }

        private static FakeHeroSelect GetFakeHeroSelect(ulong playerUid)
        {
            for (int i = 0; i < FakeHeroSelectList.Count; i++)
            {
                if (FakeHeroSelectList[i].FakePlayer.ullUid == playerUid)
                {
                    return FakeHeroSelectList[i];
                }
            }
            return null;
        }

        private static void GotoHeroSelectPage()
        {
            bInFakeConfirm = false;
            RealPlayerConfirmNum = 0;
            FakePlayerConfirmNum = 0;
            Singleton<LobbyLogic>.GetInstance().inMultiRoom = false;
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            if (roomInfo != null)
            {
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(roomInfo.roomAttrib.bMapType, roomInfo.roomAttrib.dwMapId);
                Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enPVE_Computer, pvpMapCommonInfo.bHeroNum, roomInfo.roomAttrib.dwMapId, roomInfo.roomAttrib.bMapType, pvpMapCommonInfo.bIsAllowHeroDup);
                Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnConfirmTimout));
            }
        }

        private static bool IsInSelectHero()
        {
            return (Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath) != null);
        }

        private static void OnConfirmTimout(int seq)
        {
            bInFakeConfirm = false;
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            if (roomInfo != null)
            {
                roomInfo.roomAttrib.bWarmBattle = false;
            }
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
            if (form != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(form);
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Err_NM_Cancel"), false, 1.5f, null, new object[0]);
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x422);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
        }

        private static void OnSelectHeroTimeout(int seq)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath) != null)
            {
                RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
                if ((roomInfo != null) && (roomInfo.GetMasterMemberInfo() != null))
                {
                    ListView<IHeroData> pvPHeroList = CHeroDataFactory.GetPvPHeroList();
                    DebugHelper.Assert(pvPHeroList.Count > ChosenHeroes.Count, "May have not enough Candidate Heroes!!!");
                    int num = UnityEngine.Random.Range(0, pvPHeroList.Count);
                    IHeroData data = pvPHeroList[num];
                    while (ChosenHeroes.ContainsValue(data.cfgID))
                    {
                        num = UnityEngine.Random.Range(0, pvPHeroList.Count);
                        data = pvPHeroList[num];
                    }
                    roomInfo.GetMasterMemberInfo().ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = data.cfgID;
                    ReqStartSingleWarmBattle();
                }
            }
        }

        public static void OnSelfConfirmed(GameObject root, int PlayerNum)
        {
            CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
            instance.confirmPlayerNum++;
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            DebugHelper.Assert(roomInfo != null);
            if (roomInfo != null)
            {
                CMatchingView.UpdateConfirmBox(root, roomInfo.selfInfo.ullUid);
                if (Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum == Singleton<CMatchingSystem>.GetInstance().currentMapPlayerNum)
                {
                    GotoHeroSelectPage();
                }
            }
        }

        public static void OnSelfHeroConfirmed()
        {
            HeroConfirmedPlayerNum++;
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            roomInfo.GetMemberInfo(roomInfo.selfInfo.ullUid).isPrepare = true;
            if (HeroConfirmedPlayerNum == (FakeHeroSelectList.Count + 1))
            {
                ReqStartSingleWarmBattle();
            }
        }

        public static void OnSelfSelectHero(ulong selfUid, uint heroID)
        {
            ChosenHeroes[selfUid] = heroID;
        }

        private static void RemoveAllFakeTimer()
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.FakeConfirmLater));
            Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeConfirm));
            Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnConfirmTimout));
            Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeSelectHero));
            Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnSelectHeroTimeout));
        }

        private static void ReqStartSingleWarmBattle()
        {
            bInFakeSelect = false;
            Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.OnSelectHeroTimeout));
            RemoveAllFakeTimer();
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x41a);
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            if (roomInfo == null)
            {
                DebugHelper.Assert(roomInfo != null, "RoomInfo Should not be NULL!!!");
            }
            else if (roomInfo != null)
            {
                msg.stPkgData.stStartSingleGameReq.stBattleParam.bGameType = 1;
                msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfCombat = Singleton<CHeroSelectBaseSystem>.instance.m_stGameOfCombat;
                msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = 1;
                if (msg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfCombat != null)
                {
                    MemberInfo masterMemberInfo = roomInfo.GetMasterMemberInfo();
                    if (masterMemberInfo != null)
                    {
                        msg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList[0] = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
                        ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(roomInfo.roomAttrib.bMapType, roomInfo.roomAttrib.dwMapId);
                        if (pvpMapCommonInfo != null)
                        {
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
                            CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer = msg.stPkgData.stStartSingleGameReq.stBattlePlayer;
                            int index = 0;
                            for (COM_PLAYERCAMP com_playercamp3 = COM_PLAYERCAMP.COM_PLAYERCAMP_1; com_playercamp3 < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; com_playercamp3 += 1)
                            {
                                ListView<MemberInfo> view2 = roomInfo[com_playercamp3];
                                for (int j = 0; j < view2.Count; j++)
                                {
                                    MemberInfo info4 = view2[j];
                                    if (info4 != null)
                                    {
                                        if (info4.RoomMemberType == 2)
                                        {
                                            stBattlePlayer.astFighter[index].bObjType = 2;
                                            stBattlePlayer.astFighter[index].bPosOfCamp = (byte) j;
                                            stBattlePlayer.astFighter[index].bObjCamp = (byte) com_playercamp3;
                                            stBattlePlayer.astFighter[index].dwLevel = info4.dwMemberPvpLevel;
                                            FakeHeroSelect fakeHeroSelect = GetFakeHeroSelect(info4.ullUid);
                                            if (fakeHeroSelect != null)
                                            {
                                                stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = fakeHeroSelect.selectedHero;
                                                stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID = fakeHeroSelect.selectedPlayerSkill;
                                                stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo.stCommonInfo.wSkinID = (ushort) fakeHeroSelect.selectedSkin;
                                                stBattlePlayer.astFighter[index].szName = fakeHeroSelect.FakePlayer.szUserName;
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc = new COMDT_PLAYERINFO_OF_NPC();
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc.ullFakeUid = fakeHeroSelect.FakePlayer.ullUid;
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc.dwFakePvpLevel = fakeHeroSelect.FakePlayer.dwAcntPvpLevel;
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc.dwFakeLogicWorldID = fakeHeroSelect.FakePlayer.dwLogicWorldId;
                                            }
                                            else
                                            {
                                                SelectHeroForEnemyPlayer(ref stBattlePlayer.astFighter[index].astChoiceHero[0].stBaseInfo, stBattlePlayer.astFighter[index].dwLevel);
                                                stBattlePlayer.astFighter[index].szName = info4.WarmNpc.szUserName;
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc = new COMDT_PLAYERINFO_OF_NPC();
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc.ullFakeUid = info4.WarmNpc.ullUid;
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc.dwFakePvpLevel = info4.WarmNpc.dwAcntPvpLevel;
                                                stBattlePlayer.astFighter[index].stDetail.stPlayerOfNpc.dwFakeLogicWorldID = info4.WarmNpc.dwLogicWorldId;
                                            }
                                        }
                                        else if (info4.RoomMemberType == 1)
                                        {
                                            stBattlePlayer.astFighter[index].bObjType = 1;
                                            stBattlePlayer.astFighter[index].bPosOfCamp = (byte) j;
                                            stBattlePlayer.astFighter[index].bObjCamp = (byte) camp;
                                            for (int k = 0; k < pvpMapCommonInfo.bHeroNum; k++)
                                            {
                                                stBattlePlayer.astFighter[index].astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID = (uint) dwHeroID;
                                            }
                                        }
                                        index++;
                                    }
                                }
                            }
                            stBattlePlayer.bNum = (byte) index;
                            roomInfo.roomAttrib.bWarmBattle = false;
                            roomInfo.roomAttrib.npcAILevel = 2;
                            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                        }
                    }
                    else
                    {
                        DebugHelper.Assert(roomInfo != null, "selfInfo Should not be NULL!!!");
                    }
                }
            }
        }

        private static List<FakePlayerConfirm> SelectConfirmPlayer(int time)
        {
            List<FakePlayerConfirm> list = new List<FakePlayerConfirm>();
            for (int i = 0; i < FakePlayerList.Count; i++)
            {
                if (!FakePlayerList[i].bConfirmed && (FakePlayerList[i].confirmWaitTime == time))
                {
                    list.Add(FakePlayerList[i]);
                }
            }
            return list;
        }

        private static void SelectHeroForEnemyPlayer(ref COMDT_HEROINFO heroInfo, uint playerLevel)
        {
            <SelectHeroForEnemyPlayer>c__AnonStorey83 storey = new <SelectHeroForEnemyPlayer>c__AnonStorey83();
            int id = 0;
            storey.heroID = 0;
            ushort dwSkinID = 0;
            uint num3 = 0;
            ResFakeAcntHero dataByIndex = null;
            if (playerLevel <= 6)
            {
                id = UnityEngine.Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
                dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
                if (dataByIndex == null)
                {
                    return;
                }
                storey.heroID = dataByIndex.dwHeroID;
                while (EnemyChosenHeroes.FindIndex(new Predicate<uint>(storey.<>m__80)) != -1)
                {
                    id = UnityEngine.Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
                    dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
                    if (dataByIndex == null)
                    {
                        return;
                    }
                    storey.heroID = dataByIndex.dwHeroID;
                }
                if (((dataByIndex != null) && (dataByIndex.dwSkinID != 0)) && (UnityEngine.Random.Range(0, 0x2710) < dataByIndex.dwSkinRate))
                {
                    dwSkinID = (ushort) dataByIndex.dwSkinID;
                }
            }
            else
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    return;
                }
                int num5 = GameDataMgr.robotVeteranHeroSkinDatabin.Count();
                int max = num5 + ((masterRoleInfo == null) ? 0 : masterRoleInfo.freeHeroList.Count);
                id = UnityEngine.Random.Range(0, max);
                if (id < num5)
                {
                    dataByIndex = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(id);
                    if (dataByIndex == null)
                    {
                        return;
                    }
                    storey.heroID = dataByIndex.dwHeroID;
                }
                else
                {
                    storey.heroID = masterRoleInfo.freeHeroList[id - num5].dwFreeHeroID;
                }
                while (EnemyChosenHeroes.FindIndex(new Predicate<uint>(storey.<>m__81)) != -1)
                {
                    id = UnityEngine.Random.Range(0, max);
                    if (id < num5)
                    {
                        dataByIndex = GameDataMgr.robotVeteranHeroSkinDatabin.GetDataByIndex(id);
                        if (dataByIndex == null)
                        {
                            return;
                        }
                        storey.heroID = dataByIndex.dwHeroID;
                    }
                    else
                    {
                        storey.heroID = masterRoleInfo.freeHeroList[id - num5].dwFreeHeroID;
                    }
                }
            }
            ResFakeAcntSkill dataByKey = GameDataMgr.robotPlayerSkillDatabin.GetDataByKey(playerLevel);
            if (dataByKey != null)
            {
                int index = UnityEngine.Random.Range(0, dataByKey.SkillId.Length);
                while (dataByKey.SkillId[index] == 0)
                {
                    index = UnityEngine.Random.Range(0, dataByKey.SkillId.Length);
                }
                num3 = dataByKey.SkillId[index];
            }
            EnemyChosenHeroes.Add(storey.heroID);
            heroInfo.stCommonInfo.dwHeroID = storey.heroID;
            heroInfo.stCommonInfo.stSkill.dwSelSkillID = num3;
            heroInfo.stCommonInfo.wSkinID = dwSkinID;
        }

        public static void SetConfirmFakeData()
        {
            RemoveAllFakeTimer();
            DatabinCheck();
            RealPlayerConfirmNum = 0;
            FakePlayerConfirmNum = 0;
            FakePlayerList.Clear();
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            DebugHelper.Assert(roomInfo != null);
            DebugHelper.Assert(roomInfo.roomAttrib.bWarmBattle);
            for (COM_PLAYERCAMP com_playercamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1; com_playercamp < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; com_playercamp += 1)
            {
                ListView<MemberInfo> view = roomInfo[com_playercamp];
                for (int i = 0; i < view.Count; i++)
                {
                    MemberInfo info2 = view[i];
                    if (info2.RoomMemberType == 2)
                    {
                        FakePlayerConfirm item = new FakePlayerConfirm {
                            FakePlayer = info2.WarmNpc,
                            confirmWaitTime = UnityEngine.Random.Range(2, 11)
                        };
                        FakePlayerList.Add(item);
                    }
                }
            }
        }

        public static void StartFakeConfirm()
        {
            bInFakeConfirm = true;
            CurrentSelectTime = 0;
            ConfirmedFakePlayerNum = 0;
            Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 20, new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeConfirm));
            Singleton<CTimerManager>.GetInstance().AddTimer(0x7530, 1, new CTimer.OnTimeUpHandler(CFakePvPHelper.OnConfirmTimout));
        }

        public static void UpdateConfirmBox(GameObject root, int PlayerNum)
        {
            DebugHelper.Assert(FakePlayerList.Count <= 5, string.Format("FakePlayerList Count Error!! Count: {0}", FakePlayerList.Count));
            MapPlayerNum = PlayerNum;
            RealPlayerConfirmNum++;
            if (RealPlayerConfirmNum == (PlayerNum / 2))
            {
                for (int i = 0; i < FakePlayerList.Count; i++)
                {
                    FakePlayerConfirm confirm = FakePlayerList[i];
                    if (!confirm.bConfirmed)
                    {
                        confirm.bConfirmed = true;
                        FakePlayerConfirmNum++;
                        CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
                        instance.confirmPlayerNum++;
                        CMatchingView.UpdateConfirmBox(root, confirm.FakePlayer.ullUid);
                    }
                }
                Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.FakeConfirmLater));
            }
            else
            {
                Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 1, new CTimer.OnTimeUpHandler(CFakePvPHelper.FakeConfirmLater));
            }
        }

        private static void UpdateFakeConfirm(int seq)
        {
            List<FakePlayerConfirm> list = SelectConfirmPlayer(CurrentSelectTime++);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
            if (form != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    FakePlayerConfirm confirm = list[i];
                    confirm.bConfirmed = true;
                    ConfirmedFakePlayerNum++;
                    CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
                    instance.confirmPlayerNum++;
                    CMatchingView.UpdateConfirmBox(form.gameObject, confirm.FakePlayer.ullUid);
                    if (ConfirmedFakePlayerNum == FakePlayerList.Count)
                    {
                        Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeConfirm));
                    }
                    if (Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum == Singleton<CMatchingSystem>.GetInstance().currentMapPlayerNum)
                    {
                        GotoHeroSelectPage();
                    }
                }
            }
        }

        private static void UpdateFakeSelectHero(int seq)
        {
            int num = 0;
            for (int i = 0; i < FakeHeroSelectList.Count; i++)
            {
                FakeHeroSelect fakeSelect = FakeHeroSelectList[i];
                if (fakeSelect.bConfirmed)
                {
                    num++;
                }
                else if (fakeSelect.idleSec == fakeSelect.nextActionSec)
                {
                    DoSelectAction(ref fakeSelect);
                    fakeSelect.idleSec = 0;
                    fakeSelect.nextActionSec = UnityEngine.Random.Range(3, 6);
                }
                else
                {
                    fakeSelect.idleSec++;
                }
            }
            if (num == FakeHeroSelectList.Count)
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CFakePvPHelper.UpdateFakeSelectHero));
            }
        }

        public static bool bInFakeConfirm
        {
            [CompilerGenerated]
            get
            {
                return <bInFakeConfirm>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bInFakeConfirm>k__BackingField = value;
            }
        }

        public static bool bInFakeSelect
        {
            [CompilerGenerated]
            get
            {
                return <bInFakeSelect>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bInFakeSelect>k__BackingField = value;
            }
        }

        [CompilerGenerated]
        private sealed class <SelectHeroForEnemyPlayer>c__AnonStorey83
        {
            internal uint heroID;

            internal bool <>m__80(uint x)
            {
                return (x == this.heroID);
            }

            internal bool <>m__81(uint x)
            {
                return (x == this.heroID);
            }
        }

        private class FakeHeroSelect
        {
            public bool bConfirmed;
            public int changeHeroCount;
            public COMDT_FAKEACNT_DETAIL FakePlayer;
            public int idleSec;
            public int maxChangeHeroCount;
            public int nextActionSec;
            public uint selectedHero;
            public uint selectedPlayerSkill;
            public int selectedSkin;
        }

        private class FakePlayerConfirm
        {
            public bool bConfirmed;
            public int confirmWaitTime;
            public COMDT_FAKEACNT_DETAIL FakePlayer;
        }
    }
}

