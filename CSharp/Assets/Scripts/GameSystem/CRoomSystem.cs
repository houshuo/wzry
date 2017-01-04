namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    internal class CRoomSystem : Singleton<CRoomSystem>
    {
        private bool bInRoom;
        public int m_roomType;
        private uint mapId;
        private ListView<RoomMapInfo> mapList;
        private byte mapType = 1;
        public const int MAX_NUM_PER_TEAM = 5;
        private uint MeleeMapId;
        private static ulong NpcUlId = 1L;
        public static string PATH_CREATE_ROOM = "UGUI/Form/System/PvP/Room/Form_CreateRoom.prefab";
        public static string PATH_ROOM = "UGUI/Form/System/PvP/Room/Form_Room.prefab";
        public static string PATH_ROOM_SWAP = "UGUI/Form/System/PvP/Room/Form_RoomSwapMessageBox.prefab";
        public RoomInfo roomInfo;

        public void BuildRoomInfo(COMDT_JOINMULTGAMERSP_SUCC roomData)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                this.roomInfo = new RoomInfo();
                this.roomInfo.iRoomEntity = roomData.iRoomEntity;
                this.roomInfo.dwRoomID = roomData.dwRoomID;
                this.roomInfo.dwRoomSeq = roomData.dwRoomSeq;
                this.roomInfo.roomAttrib.bGameMode = roomData.stRoomInfo.bGameMode;
                this.roomInfo.roomAttrib.bPkAI = 0;
                this.roomInfo.roomAttrib.bMapType = roomData.stRoomInfo.bMapType;
                this.roomInfo.roomAttrib.dwMapId = roomData.stRoomInfo.dwMapId;
                this.roomInfo.roomAttrib.ullFeature = roomData.stRoomInfo.ullFeature;
                this.roomInfo.roomAttrib.bWarmBattle = false;
                this.roomInfo.roomAttrib.npcAILevel = 2;
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
                this.roomInfo.roomAttrib.judgeNum = (pvpMapCommonInfo == null) ? 0 : ((int) pvpMapCommonInfo.dwJudgeNum);
                this.roomInfo.selfInfo.ullUid = roomData.ullSelfUid;
                this.roomInfo.selfInfo.iGameEntity = roomData.iSelfGameEntity;
                this.roomInfo.roomOwner.ullUid = roomData.stRoomMaster.ullMasterUid;
                this.roomInfo.roomOwner.iGameEntity = roomData.stRoomMaster.iMasterGameEntity;
                for (int i = 0; i < 3; i++)
                {
                    COM_PLAYERCAMP camp = (COM_PLAYERCAMP) i;
                    ListView<MemberInfo> view = this.roomInfo[camp];
                    view.Clear();
                    for (int j = 0; j < roomData.stMemInfo.astCampMem[i].dwMemNum; j++)
                    {
                        COMDT_ROOMMEMBER_DT memberDT = roomData.stMemInfo.astCampMem[i].astMemInfo[j];
                        MemberInfo item = this.CreateMemInfo(ref memberDT, camp, this.roomInfo.roomAttrib.bWarmBattle);
                        if (item.ullUid == masterRoleInfo.playerUllUID)
                        {
                            this.roomInfo.selfObjID = item.dwObjId;
                        }
                        view.Add(item);
                    }
                    this.roomInfo.SortCampMemList(camp);
                }
            }
        }

        public void BuildRoomInfo(COMDT_MATCH_SUCC_DETAIL roomData)
        {
            this.roomInfo = new RoomInfo();
            this.roomInfo.iRoomEntity = roomData.iRoomEntity;
            this.roomInfo.dwRoomID = roomData.dwRoomID;
            this.roomInfo.dwRoomSeq = roomData.dwRoomSeq;
            this.roomInfo.roomAttrib.bGameMode = roomData.stRoomInfo.bGameMode;
            this.roomInfo.roomAttrib.bPkAI = roomData.stRoomInfo.bPkAI;
            this.roomInfo.roomAttrib.bMapType = roomData.stRoomInfo.bMapType;
            this.roomInfo.roomAttrib.dwMapId = roomData.stRoomInfo.dwMapId;
            this.roomInfo.roomAttrib.bWarmBattle = Convert.ToBoolean(roomData.stRoomInfo.bIsWarmBattle);
            this.roomInfo.roomAttrib.npcAILevel = roomData.stRoomInfo.bAILevel;
            this.roomInfo.roomAttrib.ullFeature = roomData.stRoomInfo.ullFeature;
            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
            this.roomInfo.roomAttrib.judgeNum = (pvpMapCommonInfo == null) ? 0 : ((int) pvpMapCommonInfo.dwJudgeNum);
            this.roomInfo.selfInfo.ullUid = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
            for (int i = 0; i < 3; i++)
            {
                COM_PLAYERCAMP camp = (COM_PLAYERCAMP) i;
                ListView<MemberInfo> view = this.roomInfo[camp];
                view.Clear();
                for (int j = 0; j < roomData.stMemInfo.astCampMem[i].dwMemNum; j++)
                {
                    COMDT_ROOMMEMBER_DT memberDT = roomData.stMemInfo.astCampMem[i].astMemInfo[j];
                    MemberInfo item = this.CreateMemInfo(ref memberDT, camp, this.roomInfo.roomAttrib.bWarmBattle);
                    view.Add(item);
                }
                this.roomInfo.SortCampMemList(camp);
            }
        }

        public void Clear()
        {
            this.bInRoom = false;
            this.roomInfo = null;
            this.m_roomType = 0;
        }

        public void CloseRoom()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_CREATE_ROOM);
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_ROOM);
            Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
            Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
            CChatUT.LeaveRoom();
            this.bInRoom = false;
        }

        private MemberInfo CreateMemInfo(ref COMDT_ROOMMEMBER_DT memberDT, COM_PLAYERCAMP camp, bool bWarmBattle)
        {
            MemberInfo info = new MemberInfo {
                RoomMemberType = memberDT.dwRoomMemberType,
                dwPosOfCamp = memberDT.dwPosOfCamp,
                camp = camp
            };
            if (memberDT.dwRoomMemberType == 1)
            {
                info.ullUid = memberDT.stMemberDetail.stMemberOfAcnt.ullUid;
                info.iFromGameEntity = memberDT.stMemberDetail.stMemberOfAcnt.iFromGameEntity;
                info.iLogicWorldID = memberDT.stMemberDetail.stMemberOfAcnt.iLogicWorldID;
                info.MemberName = StringHelper.UTF8BytesToString(ref memberDT.stMemberDetail.stMemberOfAcnt.szMemberName);
                info.dwMemberLevel = memberDT.stMemberDetail.stMemberOfAcnt.dwMemberLevel;
                info.dwMemberPvpLevel = memberDT.stMemberDetail.stMemberOfAcnt.dwMemberPvpLevel;
                info.dwMemberHeadId = memberDT.stMemberDetail.stMemberOfAcnt.dwMemberHeadId;
                info.MemberHeadUrl = StringHelper.UTF8BytesToString(ref memberDT.stMemberDetail.stMemberOfAcnt.szMemberHeadUrl);
                info.ChoiceHero = new COMDT_CHOICEHERO[] { new COMDT_CHOICEHERO() };
                info.canUseHero = new uint[0];
                info.isPrepare = false;
                info.dwObjId = 0;
                return info;
            }
            if (memberDT.dwRoomMemberType == 2)
            {
                NpcUlId += (ulong) 1L;
                info.ullUid = NpcUlId;
                info.iFromGameEntity = 0;
                info.MemberName = Singleton<CTextManager>.GetInstance().GetText("PVP_NPC");
                info.dwMemberLevel = memberDT.stMemberDetail.stMemberOfNpc.bLevel;
                info.dwMemberHeadId = 1;
                info.ChoiceHero = new COMDT_CHOICEHERO[] { new COMDT_CHOICEHERO() };
                info.canUseHero = new uint[0];
                info.isPrepare = true;
                info.dwObjId = 0;
                info.WarmNpc = memberDT.stMemberDetail.stMemberOfNpc.stDetail;
                if (bWarmBattle)
                {
                    info.ullUid = info.WarmNpc.ullUid;
                    info.dwMemberPvpLevel = info.WarmNpc.dwAcntPvpLevel;
                    info.MemberName = StringHelper.UTF8BytesToString(ref info.WarmNpc.szUserName);
                    info.MemberHeadUrl = StringHelper.UTF8BytesToString(ref info.WarmNpc.szUserHeadUrl);
                    info.isPrepare = false;
                }
            }
            return info;
        }

        private void entertainmentAddLock(CUIFormScript form)
        {
            CUIListScript component = form.transform.FindChild("Panel_Main/List").GetComponent<CUIListScript>();
            for (int i = 0; i < this.mapList.Count; i++)
            {
                if (this.mapList[i] == null)
                {
                    break;
                }
                if (this.mapList[i].mapID == this.MeleeMapId)
                {
                    Transform transform = component.GetElemenet(i).transform;
                    if (transform != null)
                    {
                        if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ENTERTAINMENT))
                        {
                            transform.GetComponent<Image>().color = CUIUtility.s_Color_Button_Disable;
                            transform.FindChild("Lock").gameObject.CustomSetActive(true);
                            ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x19);
                            transform.FindChild("Lock/Text").GetComponent<Text>().text = Utility.UTF8Convert(dataByKey.szLockedTip);
                        }
                        else
                        {
                            transform.GetComponent<Image>().color = CUIUtility.s_Color_White;
                            transform.FindChild("Lock").gameObject.CustomSetActive(false);
                        }
                    }
                    break;
                }
            }
        }

        private void GetMemberPosInfo(GameObject go, out COM_PLAYERCAMP Camp, out int Pos)
        {
            Camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            if (go.name.StartsWith("Left"))
            {
                Camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            }
            else if (go.name.StartsWith("Right"))
            {
                Camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
            }
            Pos = 0;
            if (go.name.EndsWith("1"))
            {
                Pos = 0;
            }
            else if (go.name.EndsWith("2"))
            {
                Pos = 1;
            }
            else if (go.name.EndsWith("3"))
            {
                Pos = 2;
            }
            else if (go.name.EndsWith("4"))
            {
                Pos = 3;
            }
            else if (go.name.EndsWith("5"))
            {
                Pos = 4;
            }
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_OpenCreateForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenCreateForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_CreateRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_CreateRoom));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_SelectMap, new CUIEventManager.OnUIEventHandler(this.OnRoom_SelectMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_OpenInvite, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenInvite));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_StartGame, new CUIEventManager.OnUIEventHandler(this.OnRoom_StartGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_AddRobot, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddRobot));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos, new CUIEventManager.OnUIEventHandler(this.OnRoom_ChangePos));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_KickPlayer, new CUIEventManager.OnUIEventHandler(this.OnRoom_KickPlayer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_LeaveRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_LeaveRoom));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ShareRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_ShareFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosTimeUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_Refuse, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosRefuse));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_Box_TimerChange, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosBoxTimerChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_On_Close, new CUIEventManager.OnUIEventHandler(this.OnRoomClose));
            CRoomObserve.RegisterEvents();
        }

        private void InitMaps(CUIFormScript rootFormScript)
        {
            this.mapList = new ListView<RoomMapInfo>();
            uint[] numArray = new uint[10];
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_1"), out numArray[0]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_2"), out numArray[1]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_3"), out numArray[2]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_4"), out numArray[3]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_5"), out numArray[4]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_6"), out numArray[5]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_7"), out numArray[6]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_8"), out numArray[7]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_9"), out numArray[8]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_10"), out numArray[9]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_MELEE"), out this.MeleeMapId);
            uint[] numArray2 = new uint[10];
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_1"), out numArray2[0]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_2"), out numArray2[1]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_3"), out numArray2[2]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_4"), out numArray2[3]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_5"), out numArray2[4]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_6"), out numArray2[5]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_7"), out numArray2[6]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_8"), out numArray2[7]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_9"), out numArray2[8]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_10"), out numArray2[9]);
            for (int i = 0; i < numArray.Length; i++)
            {
                if ((numArray[i] != 0) && (CLevelCfgLogicManager.GetPvpMapCommonInfo((byte) numArray2[i], numArray[i]) != null))
                {
                    RoomMapInfo item = new RoomMapInfo {
                        mapType = (byte) numArray2[i],
                        mapID = numArray[i]
                    };
                    this.mapList.Add(item);
                }
            }
            CUIListScript component = rootFormScript.transform.Find("Panel_Main/List").gameObject.GetComponent<CUIListScript>();
            component.SetElementAmount(this.mapList.Count);
            for (int j = 0; j < component.m_elementAmount; j++)
            {
                Image image = component.GetElemenet(j).transform.GetComponent<Image>();
                string prefabPath = CUIUtility.s_Sprite_Dynamic_PvpEntry_Dir + this.mapList[j].mapID;
                image.SetSprite(prefabPath, rootFormScript, true, false, false);
            }
            component.SelectElement(-1, true);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform = rootFormScript.transform.Find("panelGroupBottom");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }

        [MessageHandler(0x400)]
        public static void OnLeaveRoom(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stQuitMultGameRsp.iErrCode == 0)
            {
                if (msg.stPkgData.stQuitMultGameRsp.bLevelFromType == 1)
                {
                    Singleton<CRoomSystem>.GetInstance().bInRoom = false;
                    Singleton<CUIManager>.GetInstance().CloseForm(PATH_ROOM);
                    Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
                    Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
                    CChatUT.LeaveRoom();
                }
                else if (msg.stPkgData.stQuitMultGameRsp.bLevelFromType == 2)
                {
                    CMatchingSystem.OnPlayerLeaveMatching();
                }
            }
            else
            {
                object[] replaceArr = new object[] { Utility.ProtErrCodeToStr(0x400, msg.stPkgData.stQuitMultGameRsp.iErrCode) };
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Exit_Room_Error", true, 1f, null, replaceArr);
            }
        }

        [MessageHandler(0x3fe)]
        public static void OnPlayerJoinRoom(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stJoinMultGameRsp.iErrCode == 0)
            {
                Singleton<GameBuilder>.instance.EndGame();
                CRoomSystem instance = Singleton<CRoomSystem>.GetInstance();
                instance.bInRoom = true;
                instance.BuildRoomInfo(msg.stPkgData.stJoinMultGameRsp.stInfo.stOfSucc);
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(PATH_ROOM, false, true);
                Singleton<CTopLobbyEntry>.GetInstance().OpenForm();
                Singleton<CInviteSystem>.GetInstance().OpenInviteForm(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM);
                CChatUT.EnterRoom();
                CRoomView.SetRoomData(script.gameObject, instance.roomInfo);
                CRoomObserve.SetObservers(Utility.FindChild(script.gameObject, "Panel_Main/Observers"), instance.roomInfo.roomAttrib.judgeNum, instance.roomInfo[COM_PLAYERCAMP.COM_PLAYERCAMP_MID], instance.roomInfo.GetMasterMemberInfo());
                Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
                Singleton<CMatchingSystem>.instance.cacheMathingInfo.CanGameAgain = instance.IsSelfRoomOwner;
                if (!instance.IsSelfRoomOwner)
                {
                    MonoSingleton<NewbieGuideManager>.instance.StopCurrentGuide();
                }
            }
            else if (msg.stPkgData.stJoinMultGameRsp.iErrCode == 0x1a)
            {
                DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
                object[] args = new object[] { banTime.Year, banTime.Month, banTime.Day, banTime.Hour, banTime.Minute };
                string strContent = string.Format("您被禁止竞技！截止时间为{0}年{1}月{2}日{3}时{4}分", args);
                Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
            }
            else
            {
                object[] replaceArr = new object[] { Utility.ProtErrCodeToStr(0x3fe, msg.stPkgData.stJoinMultGameRsp.iErrCode) };
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Enter_Room_Error", true, 1f, null, replaceArr);
            }
        }

        private void OnRoom_AddFriend(CUIEvent uiEvent)
        {
            GameObject gameObject = uiEvent.m_srcWidget.transform.parent.parent.gameObject;
            COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            int pos = 0;
            this.GetMemberPosInfo(gameObject, out camp, out pos);
            if (this.roomInfo != null)
            {
                MemberInfo memberInfo = this.roomInfo.GetMemberInfo(camp, pos);
                object[] inParameters = new object[] { camp, pos };
                DebugHelper.Assert(memberInfo != null, "Room member info is NULL!! Camp -- {0}, Pos -- {1}", inParameters);
                if (memberInfo != null)
                {
                    Singleton<CFriendContoller>.instance.Open_Friend_Verify(memberInfo.ullUid, (uint) memberInfo.iFromGameEntity, false);
                }
            }
        }

        private void OnRoom_AddRobot(CUIEvent uiEvent)
        {
            if (this.IsSelfRoomOwner)
            {
                COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                if (uiEvent.m_eventParams.tag == 1)
                {
                    camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                }
                else if (uiEvent.m_eventParams.tag == 2)
                {
                    camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                }
                ReqAddRobot(camp);
            }
            else
            {
                DebugHelper.Assert(false);
            }
        }

        private void OnRoom_ChangePos(CUIEvent uiEvent)
        {
            COM_PLAYERCAMP tag = (COM_PLAYERCAMP) uiEvent.m_eventParams.tag;
            int pos = uiEvent.m_eventParams.tag2;
            COM_CHGROOMPOS_TYPE type = (COM_CHGROOMPOS_TYPE) uiEvent.m_eventParams.tag3;
            ReqChangeCamp(tag, pos, type);
        }

        private void OnRoom_CloseForm(CUIEvent uiEvent)
        {
            ReqLeaveRoom();
        }

        private void OnRoom_CreateRoom(CUIEvent uiEvent)
        {
            if (this.mapId > 0)
            {
                Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
                Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapType = this.mapType;
                Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId = this.mapId;
                ReqCreateRoom(this.mapId, this.mapType);
            }
        }

        private void OnRoom_KickPlayer(CUIEvent uiEvent)
        {
            if (this.IsSelfRoomOwner)
            {
                GameObject gameObject = uiEvent.m_srcWidget.transform.parent.parent.gameObject;
                COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                int pos = 0;
                this.GetMemberPosInfo(gameObject, out camp, out pos);
                ReqKickPlayer(camp, pos);
            }
            else
            {
                DebugHelper.Assert(false, "Not Room Owner!");
            }
        }

        private void OnRoom_LeaveRoom(CUIEvent uiEvent)
        {
            ReqLeaveRoom();
        }

        private void OnRoom_OpenCreateForm(CUIEvent uiEvent)
        {
            CUIFormScript rootFormScript = Singleton<CUIManager>.GetInstance().OpenForm(PATH_CREATE_ROOM, false, true);
            this.InitMaps(rootFormScript);
            this.ShowBonusImage(rootFormScript);
            this.entertainmentAddLock(rootFormScript);
        }

        private void OnRoom_OpenInvite(CUIEvent uiEvent)
        {
        }

        private void OnRoom_SelectMap(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            if ((selectedIndex >= 0) && (selectedIndex < this.mapList.Count))
            {
                RoomMapInfo info = this.mapList[selectedIndex];
                this.mapId = info.mapID;
                this.mapType = info.mapType;
                if ((this.mapId == this.MeleeMapId) && !Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ENTERTAINMENT))
                {
                    ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x19);
                    Singleton<CUIManager>.GetInstance().OpenTips(dataByKey.szLockedTip, false, 1.5f, null, new object[0]);
                }
                else
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Room_CreateRoom);
                }
            }
        }

        private void OnRoom_ShareFriend(CUIEvent uiEvent)
        {
            if (Singleton<CRoomSystem>.GetInstance().IsInRoom)
            {
                this.OnRoom_ShareFriend_Room(uiEvent);
            }
            else if (Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam)
            {
                Singleton<CMatchingSystem>.GetInstance().OnTeam_ShareFriend_Team(uiEvent);
            }
        }

        private void OnRoom_ShareFriend_Room(CUIEvent uiEvent)
        {
            if ((this.roomInfo != null) && (this.roomInfo.roomAttrib != null))
            {
                uint dwMapId = this.roomInfo.roomAttrib.dwMapId;
                int bMapType = this.roomInfo.roomAttrib.bMapType;
                int bMaxAcntNum = 0;
                string szName = string.Empty;
                string str2 = string.Empty;
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte) bMapType, dwMapId);
                bMaxAcntNum = pvpMapCommonInfo.bMaxAcntNum;
                szName = pvpMapCommonInfo.szName;
                if (bMapType == 3)
                {
                    bMaxAcntNum = 4;
                    str2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_6");
                }
                else
                {
                    str2 = Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", bMaxAcntNum / 2));
                }
                string text = Singleton<CTextManager>.GetInstance().GetText("Share_Room_Info_Title");
                string[] args = new string[] { str2, szName };
                string desc = Singleton<CTextManager>.instance.GetText("Share_Room_Info_Desc", args);
                string roomInfo = MonoSingleton<ShareSys>.GetInstance().PackRoomData(this.roomInfo.iRoomEntity, this.roomInfo.dwRoomID, this.roomInfo.dwRoomSeq, this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId, this.roomInfo.roomAttrib.ullFeature);
                Singleton<ApolloHelper>.GetInstance().InviteFriendToRoom(text, desc, roomInfo);
            }
        }

        private void OnRoom_StartGame(CUIEvent uiEvent)
        {
            if (this.IsSelfRoomOwner)
            {
                if (uiEvent.m_srcWidget.GetComponent<Button>().interactable)
                {
                    ReqStartGame();
                }
            }
            else
            {
                DebugHelper.Assert(false);
            }
        }

        [MessageHandler(0x401)]
        public static void OnRoomChange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            if (roomInfo == null)
            {
                DebugHelper.Assert(false, "Room Info is NULL!!!");
            }
            else
            {
                bool flag = false;
                bool flag2 = false;
                if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 0)
                {
                    COM_PLAYERCAMP iCamp = (COM_PLAYERCAMP) msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerAdd.iCamp;
                    MemberInfo item = Singleton<CRoomSystem>.GetInstance().CreateMemInfo(ref msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerAdd.stMemInfo, iCamp, roomInfo.roomAttrib.bWarmBattle);
                    roomInfo[iCamp].Add(item);
                    flag = true;
                }
                else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType != 1)
                {
                    if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 2)
                    {
                        Singleton<CRoomSystem>.GetInstance().bInRoom = false;
                        Singleton<CUIManager>.GetInstance().CloseForm(PATH_CREATE_ROOM);
                        Singleton<CUIManager>.GetInstance().CloseForm(PATH_ROOM);
                        Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
                        Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
                        CChatUT.LeaveRoom();
                        Singleton<CChatController>.instance.ShowPanel(false, false);
                        Singleton<CUIManager>.GetInstance().OpenTips("PVP_Room_Kick_Tip", true, 1.5f, null, new object[0]);
                    }
                    else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 4)
                    {
                        roomInfo.roomOwner.ullUid = msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stMasterChg.stNewMaster.ullMasterUid;
                        roomInfo.roomOwner.iGameEntity = msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stMasterChg.stNewMaster.iMasterGameEntity;
                        flag = true;
                    }
                    else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 5)
                    {
                        COMDT_ROOMCHG_CHGMEMBERPOS stChgMemberPos = msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stChgMemberPos;
                        ListView<MemberInfo> view2 = roomInfo[(COM_PLAYERCAMP) stChgMemberPos.bOldCamp];
                        ListView<MemberInfo> view3 = roomInfo[(COM_PLAYERCAMP) stChgMemberPos.bNewCamp];
                        MemberInfo memberInfo = roomInfo.GetMemberInfo(stChgMemberPos.ullMemberUid);
                        if (memberInfo == null)
                        {
                            return;
                        }
                        if ((memberInfo.camp == ((COM_PLAYERCAMP) stChgMemberPos.bNewCamp)) && (memberInfo.dwPosOfCamp == stChgMemberPos.bNewPosOfCamp))
                        {
                            return;
                        }
                        view2.Remove(memberInfo);
                        MemberInfo info4 = roomInfo.GetMemberInfo((COM_PLAYERCAMP) stChgMemberPos.bNewCamp, stChgMemberPos.bNewPosOfCamp);
                        DebugHelper.Assert(memberInfo != null, "srcMemberInfo is NULL!!");
                        memberInfo.camp = (COM_PLAYERCAMP) stChgMemberPos.bNewCamp;
                        memberInfo.dwPosOfCamp = stChgMemberPos.bNewPosOfCamp;
                        view3.Add(memberInfo);
                        if (info4 != null)
                        {
                            view3.Remove(info4);
                            info4.camp = (COM_PLAYERCAMP) stChgMemberPos.bOldCamp;
                            info4.dwPosOfCamp = stChgMemberPos.bOldPosOfCamp;
                            view2.Add(info4);
                        }
                        if (roomInfo.GetMasterMemberInfo().ullUid == stChgMemberPos.ullMemberUid)
                        {
                            flag2 = true;
                        }
                        flag = true;
                    }
                    else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 3)
                    {
                        enRoomState bOldState = (enRoomState) msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stStateChg.bOldState;
                        enRoomState bNewState = (enRoomState) msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stStateChg.bNewState;
                        if ((bOldState == enRoomState.E_ROOM_PREPARE) && (bNewState == enRoomState.E_ROOM_WAIT))
                        {
                            Singleton<LobbyLogic>.GetInstance().inMultiRoom = false;
                            Singleton<CHeroSelectBaseSystem>.instance.CloseForm();
                            Singleton<CUIManager>.GetInstance().OpenForm(PATH_ROOM, false, true);
                            CChatUT.EnterRoom();
                        }
                        if ((bOldState == enRoomState.E_ROOM_WAIT) && (bNewState == enRoomState.E_ROOM_CONFIRM))
                        {
                            CUIEvent uiEvent = new CUIEvent {
                                m_eventID = enUIEventID.Matching_OpenConfirmBox
                            };
                            uiEvent.m_eventParams.tag = roomInfo.roomAttrib.bPkAI;
                            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
                            if (roomInfo.roomAttrib.bWarmBattle)
                            {
                                CFakePvPHelper.SetConfirmFakeData();
                            }
                        }
                    }
                }
                else
                {
                    COM_PLAYERCAMP com_playercamp2 = (COM_PLAYERCAMP) msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerLeave.iCamp;
                    int bPos = msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerLeave.bPos;
                    ListView<MemberInfo> view = roomInfo[com_playercamp2];
                    for (int j = 0; j < view.Count; j++)
                    {
                        if (view[j].dwPosOfCamp == bPos)
                        {
                            view.RemoveAt(j);
                            break;
                        }
                    }
                    flag = true;
                }
                for (int i = 0; i < 3; i++)
                {
                    roomInfo[(COM_PLAYERCAMP) i].Sort(new Comparison<MemberInfo>(CRoomSystem.SortMemeberFun));
                }
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_ROOM);
                if (form != null)
                {
                    if (flag)
                    {
                        CRoomView.SetRoomData(form.gameObject, roomInfo);
                        CRoomObserve.SetObservers(Utility.FindChild(form.gameObject, "Panel_Main/Observers"), roomInfo.roomAttrib.judgeNum, roomInfo[COM_PLAYERCAMP.COM_PLAYERCAMP_MID], roomInfo.GetMasterMemberInfo());
                    }
                    if (flag2)
                    {
                        Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
                    }
                }
            }
        }

        private void OnRoomChangePosBoxTimerChange(CUIEvent uiEvent)
        {
            CRoomView.UpdateSwapBox(uiEvent.m_eventParams.tag);
        }

        private void OnRoomChangePosConfirm(CUIEvent uiEvent)
        {
            if (Singleton<CRoomSystem>.instance.roomInfo != null)
            {
                MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f5);
                msg.stPkgData.stChgRoomPosConfirmReq.bIsAccept = 1;
                msg.stPkgData.stChgRoomPosConfirmReq.dwChgSeq = masterMemberInfo.swapSeq;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
            }
        }

        private void OnRoomChangePosRefuse(CUIEvent uiEvent)
        {
            if (Singleton<CRoomSystem>.instance.roomInfo != null)
            {
                MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f5);
                msg.stPkgData.stChgRoomPosConfirmReq.bIsAccept = 0;
                msg.stPkgData.stChgRoomPosConfirmReq.dwChgSeq = masterMemberInfo.swapSeq;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
            }
        }

        private void OnRoomChangePosTimeUp(CUIEvent uiEvent)
        {
            this.RestMasterSwapInfo();
        }

        [MessageHandler(0x7f6)]
        public static void OnRoomChgPosNtf(CSPkg msg)
        {
            COM_CHGROOMPOS_RESULT bResult = (COM_CHGROOMPOS_RESULT) msg.stPkgData.stChgRoomPosNtf.bResult;
            COMDT_ROOMCHGPOS_DATA stChgPosData = msg.stPkgData.stChgRoomPosNtf.stChgPosData;
            if (Singleton<CRoomSystem>.instance.roomInfo != null)
            {
                MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    switch (bResult)
                    {
                        case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_BEGIN:
                        {
                            if (!RoomInfo.IsSameMemeber(masterMemberInfo, (COM_PLAYERCAMP) stChgPosData.stMemberInfo.stSender.bCamp, stChgPosData.stMemberInfo.stSender.bPosOfCamp))
                            {
                                if (RoomInfo.IsSameMemeber(masterMemberInfo, (COM_PLAYERCAMP) stChgPosData.stMemberInfo.stReceiver.bCamp, stChgPosData.stMemberInfo.stReceiver.bPosOfCamp))
                                {
                                    MemberInfo info3 = Singleton<CRoomSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) stChgPosData.stMemberInfo.stSender.bCamp, stChgPosData.stMemberInfo.stSender.bPosOfCamp);
                                    if (info3 == null)
                                    {
                                        return;
                                    }
                                    CRoomView.SetChgEnable(false, (COM_PLAYERCAMP) stChgPosData.stMemberInfo.stSender.bCamp, stChgPosData.stMemberInfo.stSender.bPosOfCamp);
                                    CRoomView.SetSwapTimer(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
                                    CRoomView.ShowSwapMsg((int) stChgPosData.stMemberInfo.dwTimeOutSec, (COM_PLAYERCAMP) stChgPosData.stMemberInfo.stSender.bCamp, stChgPosData.stMemberInfo.stSender.bPosOfCamp);
                                    masterMemberInfo.swapSeq = stChgPosData.stMemberInfo.dwChgPosSeq;
                                    masterMemberInfo.swapStatus = 2;
                                    masterMemberInfo.swapUid = info3.ullUid;
                                }
                                break;
                            }
                            MemberInfo memberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) stChgPosData.stMemberInfo.stReceiver.bCamp, stChgPosData.stMemberInfo.stReceiver.bPosOfCamp);
                            if (memberInfo != null)
                            {
                                CRoomView.SetChgEnable(false);
                                CRoomView.SetSwapTimer((int) stChgPosData.stMemberInfo.dwTimeOutSec, (COM_PLAYERCAMP) stChgPosData.stMemberInfo.stReceiver.bCamp, stChgPosData.stMemberInfo.stReceiver.bPosOfCamp);
                                CRoomView.ShowSwapMsg(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
                                masterMemberInfo.swapSeq = stChgPosData.stMemberInfo.dwChgPosSeq;
                                masterMemberInfo.swapStatus = 1;
                                masterMemberInfo.swapUid = memberInfo.ullUid;
                                Singleton<CUIManager>.instance.CloseSendMsgAlert();
                                break;
                            }
                            return;
                        }
                        case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_BUSY:
                            Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_4", true, 1.5f, null, new object[0]);
                            Singleton<CUIManager>.instance.CloseSendMsgAlert();
                            break;

                        case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_NPC:
                            Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_5", true, 1.5f, null, new object[0]);
                            Singleton<CUIManager>.instance.CloseSendMsgAlert();
                            break;

                        case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_TIMEOUT:
                            Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_6", true, 1.5f, null, new object[0]);
                            Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
                            break;

                        case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_CANCEL:
                            Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_7", true, 1.5f, null, new object[0]);
                            Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
                            break;

                        case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_REFUSE:
                            Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_8", true, 1.5f, null, new object[0]);
                            Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
                            break;
                    }
                }
            }
        }

        private void OnRoomClose(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.instance.CloseForm(PATH_ROOM_SWAP);
        }

        [MessageHandler(0x7de)]
        public static void OnRoomStarted(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stStartMultiGameRsp.bErrcode == 0)
            {
                Singleton<CRoomSystem>.instance.SetRoomType(2);
            }
            else
            {
                object[] replaceArr = new object[] { Utility.ProtErrCodeToStr(0x7de, msg.stPkgData.stStartMultiGameRsp.bErrcode) };
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Start_Game_Error", true, 1f, null, replaceArr);
            }
        }

        public static void ReqAddRobot(COM_PLAYERCAMP Camp)
        {
            Debug.Log("ReqAddRobot");
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7df);
            msg.stPkgData.stAddNpcReq.stNpcInfo.iCamp = (int) Camp;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqChangeCamp(COM_PLAYERCAMP Camp, int Pos, COM_CHGROOMPOS_TYPE type)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f1);
            msg.stPkgData.stChgMemberPosReq.bCamp = (byte) Camp;
            msg.stPkgData.stChgMemberPosReq.bPosOfCamp = (byte) Pos;
            msg.stPkgData.stChgMemberPosReq.bChgType = (byte) type;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqCreateRoom(uint MapId, byte mapType)
        {
            Debug.Log("ReqCreateRoom");
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3fc);
            StringHelper.StringToUTF8Bytes("testRoom", ref msg.stPkgData.stCreateMultGameReq.szRoomName);
            msg.stPkgData.stCreateMultGameReq.bMapType = mapType;
            msg.stPkgData.stCreateMultGameReq.dwMapId = MapId;
            msg.stPkgData.stCreateMultGameReq.bGameMode = 1;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqKickPlayer(COM_PLAYERCAMP Camp, int Pos)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e3);
            msg.stPkgData.stKickoutRoomMemberReq.stKickMemberInfo.iCamp = (int) Camp;
            msg.stPkgData.stKickoutRoomMemberReq.stKickMemberInfo.bPosOfCamp = (byte) Pos;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqLeaveRoom()
        {
            Debug.Log("ReqLeaveRoom");
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3ff);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqStartGame()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7dd);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void RestMasterSwapInfo()
        {
            CRoomView.ResetSwapView();
            MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
            if (masterMemberInfo != null)
            {
                masterMemberInfo.swapSeq = 0;
                masterMemberInfo.swapStatus = 0;
                masterMemberInfo.swapUid = 0L;
            }
        }

        public void SetRoomType(int roomType)
        {
            this.m_roomType = roomType;
        }

        private void ShowBonusImage(CUIFormScript form)
        {
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                GameObject gameObject = form.transform.FindChild("panelGroupBottom/ButtonTrain/ImageBonus").gameObject;
                if ((masterRoleInfo != null) && masterRoleInfo.IsTrainingLevelFin())
                {
                    gameObject.CustomSetActive(false);
                }
                else
                {
                    gameObject.CustomSetActive(true);
                }
            }
        }

        private static int SortMemeberFun(MemberInfo left, MemberInfo right)
        {
            return (int) (left.dwPosOfCamp - right.dwPosOfCamp);
        }

        public override void UnInit()
        {
            this.roomInfo = null;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_OpenCreateForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenCreateForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_CreateRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_CreateRoom));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_CloseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_SelectMap, new CUIEventManager.OnUIEventHandler(this.OnRoom_SelectMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_OpenInvite, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenInvite));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_StartGame, new CUIEventManager.OnUIEventHandler(this.OnRoom_StartGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_AddRobot, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddRobot));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos, new CUIEventManager.OnUIEventHandler(this.OnRoom_ChangePos));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_KickPlayer, new CUIEventManager.OnUIEventHandler(this.OnRoom_KickPlayer));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_LeaveRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_LeaveRoom));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ShareRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_ShareFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosTimeUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_Refuse, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosRefuse));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_Box_TimerChange, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosBoxTimerChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_On_Close, new CUIEventManager.OnUIEventHandler(this.OnRoomClose));
            CRoomObserve.UnRegisterEvents();
            base.UnInit();
        }

        public void UpdateRoomInfo(COMDT_DESKINFO inDeskInfo, CSDT_CAMPINFO[] inCampInfo)
        {
            uint dwObjId = 0;
            if (this.roomInfo == null)
            {
                this.roomInfo = new RoomInfo();
                this.roomInfo.roomAttrib.bGameMode = inDeskInfo.bGameMode;
                this.roomInfo.roomAttrib.bPkAI = 0;
                this.roomInfo.roomAttrib.bMapType = inDeskInfo.bMapType;
                this.roomInfo.roomAttrib.dwMapId = inDeskInfo.dwMapId;
                this.roomInfo.roomAttrib.bWarmBattle = Convert.ToBoolean(inDeskInfo.bIsWarmBattle);
                this.roomInfo.roomAttrib.npcAILevel = inDeskInfo.bAILevel;
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
                this.roomInfo.roomAttrib.judgeNum = (pvpMapCommonInfo == null) ? 0 : ((int) pvpMapCommonInfo.dwJudgeNum);
                for (int i = 0; i < inCampInfo.Length; i++)
                {
                    COM_PLAYERCAMP camp = (COM_PLAYERCAMP) (i + 1);
                    CSDT_CAMPINFO csdt_campinfo = inCampInfo[i];
                    ListView<MemberInfo> view = this.roomInfo[camp];
                    view.Clear();
                    for (int j = 0; j < csdt_campinfo.dwPlayerNum; j++)
                    {
                        MemberInfo item = new MemberInfo();
                        COMDT_PLAYERINFO stPlayerInfo = csdt_campinfo.astCampPlayerInfo[j].stPlayerInfo;
                        COMDT_ACNT_USABLE_HERO stUsableHero = csdt_campinfo.astCampPlayerInfo[j].stUsableHero;
                        item.RoomMemberType = stPlayerInfo.bObjType;
                        item.dwPosOfCamp = stPlayerInfo.bPosOfCamp;
                        item.camp = camp;
                        item.dwMemberLevel = stPlayerInfo.dwLevel;
                        if (item.RoomMemberType == 1)
                        {
                            item.dwMemberPvpLevel = stPlayerInfo.stDetail.stPlayerOfAcnt.dwPvpLevel;
                        }
                        item.dwObjId = stPlayerInfo.dwObjId;
                        item.MemberName = StringHelper.UTF8BytesToString(ref stPlayerInfo.szName);
                        item.ChoiceHero = stPlayerInfo.astChoiceHero;
                        item.canUseHero = stUsableHero.HeroDetail;
                        if (stPlayerInfo.bObjType == 1)
                        {
                            item.dwMemberHeadId = stPlayerInfo.stDetail.stPlayerOfAcnt.dwHeadId;
                            if (stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid == Singleton<CRoleInfoManager>.instance.masterUUID)
                            {
                                dwObjId = stPlayerInfo.dwObjId;
                                Singleton<CHeroSelectBaseSystem>.instance.ResetRandHeroLeftCount((int) csdt_campinfo.astCampPlayerInfo[j].dwRandomHeroCnt);
                            }
                            item.ullUid = stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                        }
                        else if (stPlayerInfo.bObjType == 2)
                        {
                            item.dwMemberHeadId = 1;
                            item.ullUid = stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
                        }
                        view.Add(item);
                    }
                    this.roomInfo.SortCampMemList(camp);
                }
            }
            else
            {
                this.roomInfo.roomAttrib.bGameMode = inDeskInfo.bGameMode;
                this.roomInfo.roomAttrib.bPkAI = 0;
                this.roomInfo.roomAttrib.bMapType = inDeskInfo.bMapType;
                this.roomInfo.roomAttrib.dwMapId = inDeskInfo.dwMapId;
                for (int k = 0; k < inCampInfo.Length; k++)
                {
                    COM_PLAYERCAMP com_playercamp2 = (COM_PLAYERCAMP) (k + 1);
                    CSDT_CAMPINFO csdt_campinfo2 = inCampInfo[k];
                    ListView<MemberInfo> view2 = this.roomInfo[com_playercamp2];
                    for (int m = 0; m < csdt_campinfo2.dwPlayerNum; m++)
                    {
                        COMDT_PLAYERINFO comdt_playerinfo2 = csdt_campinfo2.astCampPlayerInfo[m].stPlayerInfo;
                        COMDT_ACNT_USABLE_HERO comdt_acnt_usable_hero2 = csdt_campinfo2.astCampPlayerInfo[m].stUsableHero;
                        MemberInfo memberInfo = this.roomInfo.GetMemberInfo(com_playercamp2, comdt_playerinfo2.bPosOfCamp);
                        if (memberInfo != null)
                        {
                            memberInfo.dwObjId = comdt_playerinfo2.dwObjId;
                            memberInfo.camp = com_playercamp2;
                            memberInfo.ChoiceHero = comdt_playerinfo2.astChoiceHero;
                            memberInfo.canUseHero = comdt_acnt_usable_hero2.HeroDetail;
                            if (comdt_playerinfo2.bObjType == 1)
                            {
                                memberInfo.dwMemberHeadId = comdt_playerinfo2.stDetail.stPlayerOfAcnt.dwHeadId;
                                if (comdt_playerinfo2.stDetail.stPlayerOfAcnt.ullUid == Singleton<CRoleInfoManager>.instance.masterUUID)
                                {
                                    dwObjId = comdt_playerinfo2.dwObjId;
                                    Singleton<CHeroSelectBaseSystem>.instance.ResetRandHeroLeftCount((int) csdt_campinfo2.astCampPlayerInfo[m].dwRandomHeroCnt);
                                }
                                memberInfo.ullUid = comdt_playerinfo2.stDetail.stPlayerOfAcnt.ullUid;
                            }
                            else if (comdt_playerinfo2.bObjType == 2)
                            {
                                memberInfo.dwMemberHeadId = 1;
                                memberInfo.ullUid = comdt_playerinfo2.stDetail.stPlayerOfNpc.ullFakeUid;
                            }
                        }
                    }
                    this.roomInfo.SortCampMemList(com_playercamp2);
                }
            }
            this.roomInfo.selfObjID = dwObjId;
        }

        public void UpdateRoomInfoReconnectPick(COMDT_DESKINFO inDeskInfo, CSDT_RECONN_CAMPPICKINFO[] inCampInfo)
        {
            this.roomInfo = new RoomInfo();
            this.roomInfo.roomAttrib.bGameMode = inDeskInfo.bGameMode;
            this.roomInfo.roomAttrib.bPkAI = 0;
            this.roomInfo.roomAttrib.bMapType = inDeskInfo.bMapType;
            this.roomInfo.roomAttrib.dwMapId = inDeskInfo.dwMapId;
            this.roomInfo.roomAttrib.bWarmBattle = Convert.ToBoolean(inDeskInfo.bIsWarmBattle);
            this.roomInfo.roomAttrib.npcAILevel = inDeskInfo.bAILevel;
            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
            this.roomInfo.roomAttrib.judgeNum = (pvpMapCommonInfo == null) ? 0 : ((int) pvpMapCommonInfo.dwJudgeNum);
            for (int i = 0; i < inCampInfo.Length; i++)
            {
                COM_PLAYERCAMP camp = (COM_PLAYERCAMP) (i + 1);
                CSDT_RECONN_CAMPPICKINFO csdt_reconn_camppickinfo = inCampInfo[i];
                ListView<MemberInfo> view = this.roomInfo[camp];
                view.Clear();
                for (int j = 0; j < csdt_reconn_camppickinfo.dwPlayerNum; j++)
                {
                    MemberInfo item = new MemberInfo();
                    COMDT_PLAYERINFO stPlayerInfo = csdt_reconn_camppickinfo.astPlayerInfo[j].stPickHeroInfo.stPlayerInfo;
                    COMDT_ACNT_USABLE_HERO stUsableHero = csdt_reconn_camppickinfo.astPlayerInfo[j].stPickHeroInfo.stUsableHero;
                    item.isPrepare = csdt_reconn_camppickinfo.astPlayerInfo[j].bIsPickOK > 0;
                    item.RoomMemberType = stPlayerInfo.bObjType;
                    item.dwPosOfCamp = stPlayerInfo.bPosOfCamp;
                    item.camp = camp;
                    item.dwMemberLevel = stPlayerInfo.dwLevel;
                    if (item.RoomMemberType == 1)
                    {
                        item.ullUid = stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                        item.dwMemberPvpLevel = stPlayerInfo.stDetail.stPlayerOfAcnt.dwPvpLevel;
                    }
                    item.dwObjId = stPlayerInfo.dwObjId;
                    item.MemberName = StringHelper.UTF8BytesToString(ref stPlayerInfo.szName);
                    item.ChoiceHero = stPlayerInfo.astChoiceHero;
                    item.canUseHero = stUsableHero.HeroDetail;
                    item.isGM = csdt_reconn_camppickinfo.astPlayerInfo[j].stPickHeroInfo.bIsGM > 0;
                    if (stPlayerInfo.bObjType == 1)
                    {
                        item.dwMemberHeadId = stPlayerInfo.stDetail.stPlayerOfAcnt.dwHeadId;
                        if (stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid == Singleton<CRoleInfoManager>.instance.masterUUID)
                        {
                            this.roomInfo.selfObjID = stPlayerInfo.dwObjId;
                            Singleton<CHeroSelectBaseSystem>.instance.ResetRandHeroLeftCount((int) csdt_reconn_camppickinfo.astPlayerInfo[j].stPickHeroInfo.dwRandomHeroCnt);
                        }
                        item.ullUid = stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                    }
                    else if (stPlayerInfo.bObjType == 2)
                    {
                        item.dwMemberHeadId = 1;
                        item.ullUid = stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
                    }
                    view.Add(item);
                }
                this.roomInfo.SortCampMemList(camp);
            }
        }

        public bool IsInRoom
        {
            get
            {
                return this.bInRoom;
            }
        }

        public bool IsSelfRoomOwner
        {
            get
            {
                return ((this.roomInfo.selfInfo.ullUid == this.roomInfo.roomOwner.ullUid) && (this.roomInfo.selfInfo.iGameEntity == this.roomInfo.roomOwner.iGameEntity));
            }
        }

        public int RoomType
        {
            get
            {
                return this.m_roomType;
            }
        }
    }
}

