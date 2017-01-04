namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class VoiceSys : MonoSingleton<VoiceSys>
    {
        private CApolloVoiceSys m_ApolloVoiceMgr;
        private bool m_bClickSound;
        private bool m_bGetIsBattleSupportVoice;
        private bool m_bGobalUseVoice;
        private bool m_bInHeroSelectUI;
        private bool m_bInRoom;
        private bool m_bSoundInBattle;
        private bool m_bUpdateEnterRoomState;
        private bool m_bUseMicOnUser;
        private bool m_bUseVoiceSysSetting;
        private float m_fCurTimeHeroSelect;
        private float m_fStartTimeHeroSelect;
        private bool m_IsBattleSupportVoice;
        private bool m_isOpenMic;
        private bool m_isOpenSpeaker;
        private ROOMINFO m_MyRoomInfo = new ROOMINFO();
        private int m_nSoundBattleTimerID;
        private List<ROOMMate> m_RoomMateList = new List<ROOMMate>();
        private Transform m_SoundLevel_HeroSelect;
        private int m_timer = -1;
        private int m_TotalVoiceTime = 10;
        private float m_UpdateTime;
        public string m_Voice_Battle_CloseMic = string.Empty;
        public string m_Voice_Battle_CloseSpeaker = string.Empty;
        public string m_Voice_Battle_FIrstOPenSpeak = string.Empty;
        public string m_Voice_Battle_FirstTips = string.Empty;
        public string m_Voice_Battle_OpenMic = string.Empty;
        public string m_Voice_Battle_OpenSpeaker = string.Empty;
        public string m_Voice_Cannot_JoinVoiceRoom = string.Empty;
        public string m_Voice_Cannot_OpenSetting = string.Empty;
        public string m_Voice_Server_Not_Open_Tips = string.Empty;
        private Transform m_VoiceBtn;
        private float m_VoiceLevel = 100f;
        private List<VoiceState> m_voiceStateList = new List<VoiceState>();
        private float m_VoiceUpdateDelta = 0.1f;
        public static int maxDeltaTime = 0xbb8;

        public void ClearVoiceStateData()
        {
            this.m_voiceStateList.Clear();
            this.lastSendVoiceState = CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE;
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_timer);
            this.m_timer = -1;
        }

        private void CloseHeroSelctVoice()
        {
            if (this.m_bClickSound)
            {
                this.m_bClickSound = false;
                this.CloseMic();
                if (this.m_SoundLevel_HeroSelect != null)
                {
                    this.m_SoundLevel_HeroSelect.gameObject.CustomSetActive(false);
                }
            }
        }

        public void CloseMic()
        {
            if ((this.m_bInRoom && (this.ApolloVoiceMgr != null)) && (this.ApolloVoiceMgr.CallApolloVoiceSDK != null))
            {
                ApolloVoiceErr err = (ApolloVoiceErr) this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseMic();
                if (err == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                {
                    this.UpdateMyVoiceIcon(0);
                    this.m_isOpenMic = false;
                    this.PrintLog("CloseMic Succ", null, false);
                }
                else
                {
                    string message = string.Format("CloseMic Err is {0}", err);
                    this.PrintLog(message, null, false);
                }
            }
            else
            {
                this.m_isOpenMic = false;
            }
            this.PrintLog("onCloseMicButtonClick", null, false);
        }

        public void ClosenSpeakers()
        {
            if ((this.m_bInRoom && (this.ApolloVoiceMgr != null)) && (this.ApolloVoiceMgr.CallApolloVoiceSDK != null))
            {
                if (this.m_isOpenSpeaker)
                {
                    ApolloVoiceErr err = (ApolloVoiceErr) this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseSpeaker();
                    if (err == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                    {
                        this.m_isOpenSpeaker = false;
                        this.PrintLog("CloseSpeaker Succ", null, false);
                    }
                    else
                    {
                        string message = string.Format("CloseSpeaker Err is {0}", err);
                        this.PrintLog(message, null, false);
                    }
                }
            }
            else
            {
                this.m_isOpenSpeaker = false;
            }
            this.PrintLog("onClosenSpeakersButtonClick", null, false);
        }

        public void CloseSoundInBattle()
        {
            if (this.m_bSoundInBattle)
            {
                this.m_bSoundInBattle = false;
                this.CloseMic();
                this.m_nSoundBattleTimerID = 0;
            }
        }

        private void CreateEngine()
        {
            if (this.ApolloVoiceMgr != null)
            {
                ApolloVoiceErr err = (ApolloVoiceErr) this.ApolloVoiceMgr.CallApolloVoiceSDK._CreateApolloVoiceEngine(ApolloConfig.appID);
                if (err == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                {
                    this.PrintLog("CreateApolloVoiceEngine Succ", null, false);
                }
                else
                {
                    string message = string.Format("CreateApolloVoiceEngine Err is {0}", err);
                    this.PrintLog(message, null, false);
                }
            }
            this.PrintLog("CreateEngine", null, false);
        }

        private void DestoyEngine()
        {
            if (this.ApolloVoiceMgr != null)
            {
                ApolloVoiceErr err = (ApolloVoiceErr) this.ApolloVoiceMgr.CallApolloVoiceSDK._DestoryApolloVoiceEngine();
                if (err == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                {
                    this.PrintLog("_DestoryApolloVoiceEngine Succ", null, false);
                }
                else
                {
                    string message = string.Format("_DestoryApolloVoiceEngine Err is {0}", err);
                    this.PrintLog(message, null, false);
                }
            }
            this.PrintLog("_DestoryApolloVoiceEngine", null, false);
        }

        private void EnterRoom(SCPKG_CREATE_TVOIP_ROOM_NTF roomInfoSvr)
        {
            this.PrintLog("***recv enterroom msg***", null, false);
            this.m_bGobalUseVoice = true;
            this.EnterRoomReal(roomInfoSvr);
        }

        private void EnterRoomReal(SCPKG_CREATE_TVOIP_ROOM_NTF roomInfoSvr)
        {
            if (this.m_bInRoom)
            {
                this.PrintLog("***reconnect enter room***", null, false);
            }
            this.PrintLog("EnterRoom", null, false);
            this.m_bInRoom = false;
            this.LeaveRoom();
            this.UpdateHeroSelectVoiceBtnState(false);
            this.m_RoomMateList.Clear();
            string openId = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false).OpenId;
            this.m_MyRoomInfo = new ROOMINFO();
            if (this.m_ApolloVoiceMgr == null)
            {
                this.m_ApolloVoiceMgr = new CApolloVoiceSys();
                this.m_ApolloVoiceMgr.SysInitial();
                if (this.m_ApolloVoiceMgr != null)
                {
                    this.PrintLog("ApolloVoiceMgr Created", null, false);
                }
                this.CreateEngine();
            }
            if (this.ApolloVoiceMgr != null)
            {
                string[] strArray = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    strArray[i] = string.Empty;
                }
                if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai() && MonoSingleton<CTongCaiSys>.instance.IsLianTongIp())
                {
                    string str2 = "16285";
                    for (int k = 0; k < roomInfoSvr.wAccessIPCount; k++)
                    {
                        strArray[k] = "udp://" + MonoSingleton<CTongCaiSys>.instance.TongcaiIps[2] + ":" + str2;
                    }
                }
                else
                {
                    for (int m = 0; m < roomInfoSvr.wAccessIPCount; m++)
                    {
                        uint num5 = (uint) IPAddress.NetworkToHostOrder((int) roomInfoSvr.astAccessIPList[m].dwIP);
                        strArray[m] = "udp://" + IPAddress.Parse(num5.ToString()).ToString() + ":" + roomInfoSvr.astAccessIPList[m].wPort.ToString();
                        this.PrintLog("host_ip : " + strArray[m], null, false);
                    }
                }
                this.m_MyRoomInfo.roomid = roomInfoSvr.ullRoomID;
                this.m_MyRoomInfo.ullRoomKey = roomInfoSvr.ullRoomKey;
                this.m_MyRoomInfo.openid = openId;
                bool flag = false;
                for (int j = 0; j < roomInfoSvr.dwRoomUserCnt; j++)
                {
                    if (Utility.UTF8Convert(roomInfoSvr.astRoomUserList[j].szOpenID) == openId)
                    {
                        flag = true;
                        this.m_MyRoomInfo.memberid = roomInfoSvr.astRoomUserList[j].dwMemberID;
                        this.m_MyRoomInfo.uuid = roomInfoSvr.astRoomUserList[j].ullUid;
                    }
                    else
                    {
                        ROOMMate item = new ROOMMate {
                            openID = Utility.UTF8Convert(roomInfoSvr.astRoomUserList[j].szOpenID),
                            memID = roomInfoSvr.astRoomUserList[j].dwMemberID,
                            uuid = roomInfoSvr.astRoomUserList[j].ullUid
                        };
                        this.m_RoomMateList.Add(item);
                        this.PrintLog(string.Format("memList idx = {0}, id = {1} , openid = {2}", j.ToString(), item.memID, item.openID), null, false);
                    }
                }
                if (!flag)
                {
                    this.PrintLog("matelist error", null, false);
                }
                object[] args = new object[] { this.m_MyRoomInfo.roomid, this.m_MyRoomInfo.ullRoomKey, this.m_MyRoomInfo.openid, this.m_MyRoomInfo.memberid, strArray[0], strArray[1], strArray[2] };
                string message = string.Format("roomid is {0}, roomkey is {1}, openid is {2}, memberid is {3}, accIP is {4}, {5}, {6}\n", args);
                this.PrintLog(message, null, false);
                if ((this.JoinVoiceRoom(strArray[0], strArray[1], strArray[2], (long) this.m_MyRoomInfo.roomid, (long) this.m_MyRoomInfo.ullRoomKey, (short) this.m_MyRoomInfo.memberid, this.m_MyRoomInfo.openid, 0x1770) == 0) && !this.m_bUpdateEnterRoomState)
                {
                    this.m_bUpdateEnterRoomState = true;
                    this.PrintLog("UpdateEnterRoomState", null, false);
                    base.StartCoroutine(this.UpdateEnterRoomState());
                }
            }
        }

        private ulong FindUUIDByMemId(int memID)
        {
            if (this.m_MyRoomInfo.memberid == memID)
            {
                return this.m_MyRoomInfo.uuid;
            }
            for (int i = 0; i < this.m_RoomMateList.Count; i++)
            {
                ROOMMate mate = this.m_RoomMateList[i];
                if (mate.memID == memID)
                {
                    return mate.uuid;
                }
            }
            return 0L;
        }

        public void ForbidMemberVoice(int nMemberId)
        {
            if ((this.m_bInRoom && (this.ApolloVoiceMgr != null)) && ((this.ApolloVoiceMgr.CallApolloVoiceSDK != null) && (this.ApolloVoiceMgr.CallApolloVoiceSDK._ForbidMemberVoice(nMemberId) == 0)))
            {
                this.PrintLog(string.Format("ForbidMemberVoice id = {0}", nMemberId), null, false);
            }
        }

        public void HeroSelectTobattle()
        {
            try
            {
                this.m_bGetIsBattleSupportVoice = false;
                this.m_IsBattleSupportVoice = false;
                this.m_bInHeroSelectUI = false;
                this.m_SoundLevel_HeroSelect = null;
                this.m_VoiceBtn = null;
                MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
                this.CloseHeroSelctVoice();
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "Exception in HeroSelectTobattle {0} {1}", inParameters);
            }
        }

        protected override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_HoldStart_VOCEBtn, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_VOCEBtn_HeroSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_Hold_VOCEBtn, new CUIEventManager.OnUIEventHandler(this.OnHold_VOCEBtn_HeroSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_HoldEnd_VOCEBtn, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_VOCEBtn_HeroSelect));
            this.m_Voice_Server_Not_Open_Tips = Singleton<CTextManager>.GetInstance().GetText("Voice_Server_Not_Open_Tips");
            this.m_Voice_Cannot_JoinVoiceRoom = Singleton<CTextManager>.GetInstance().GetText("Voice_Cannot_JoinVoiceRoom");
            this.m_Voice_Cannot_OpenSetting = Singleton<CTextManager>.GetInstance().GetText("Voice_Cannot_OpenSetting");
            this.m_Voice_Battle_FirstTips = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_FirstTips");
            this.m_Voice_Battle_OpenSpeaker = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_OpenSpeaker");
            this.m_Voice_Battle_CloseSpeaker = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_CloseSpeaker");
            this.m_Voice_Battle_OpenMic = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_OpenMic");
            this.m_Voice_Battle_CloseMic = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_CloseMic");
            this.m_Voice_Battle_FIrstOPenSpeak = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_FIrstOPenSpeak");
        }

        public bool IsBattleSupportVoice()
        {
            if (this.m_bGetIsBattleSupportVoice)
            {
                return this.m_IsBattleSupportVoice;
            }
            this.m_bGetIsBattleSupportVoice = true;
            this.m_IsBattleSupportVoice = false;
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (hostPlayer != null)
            {
                int num = 0;
                COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
                List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(playerCamp);
                for (int i = 0; i < allCampPlayers.Count; i++)
                {
                    Player player2 = allCampPlayers[i];
                    if ((player2 != null) && !player2.Computer)
                    {
                        num++;
                    }
                }
                if (num >= 2)
                {
                    this.m_IsBattleSupportVoice = true;
                    return true;
                }
                this.m_IsBattleSupportVoice = false;
            }
            return false;
        }

        public bool IsInVoiceRoom()
        {
            return this.m_bInRoom;
        }

        public bool IsOpenMic()
        {
            return this.m_isOpenMic;
        }

        public bool IsOpenSpeak()
        {
            return this.m_isOpenSpeaker;
        }

        private int JoinVoiceRoom(string url1, string url2, string url3, long roomId, long roomKey, short memberId, string OpenId, int nTimeOut)
        {
            if ((this.ApolloVoiceMgr != null) && (this.ApolloVoiceMgr.CallApolloVoiceSDK != null))
            {
                return this.ApolloVoiceMgr.CallApolloVoiceSDK._JoinRoom(url1, url2, url3, roomId, roomKey, memberId, OpenId, nTimeOut);
            }
            this.PrintLog("JoinVoiceRoom fatal error", null, false);
            return 3;
        }

        public void LeaveRoom()
        {
            try
            {
                this.m_bInRoom = false;
                this.m_isOpenMic = false;
                this.m_isOpenSpeaker = false;
                this.QuitRoom();
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "Exception In VoiceSys.LeaveRoom, {0}\n {1}", inParameters);
            }
        }

        [MessageHandler(0xc1c)]
        public static void On_CreateVoiceRoom(CSPkg msg)
        {
            try
            {
                MonoSingleton<VoiceSys>.GetInstance().EnterRoom(msg.stPkgData.stCreateTvoipRoomNtf);
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "Exception in CreateVoiceRoom, {0}\n {1}", inParameters);
            }
        }

        [MessageHandler(0xc1d)]
        public static void On_UpdateRoomMateInfo(CSPkg msg)
        {
            try
            {
                MonoSingleton<VoiceSys>.GetInstance().UpdateRoomMateInfo(msg.stPkgData.stJoinTvoipRoomNtf);
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "Exception in On_UpdateRoomMateInfo, {0}\n {1}", inParameters);
            }
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            if (this.ApolloVoiceMgr != null)
            {
                this.PrintLog("Voice OnApplicationPause: " + pauseStatus, null, false);
                if (pauseStatus)
                {
                    if (this.ApolloVoiceMgr.CallApolloVoiceSDK._Pause() == 0)
                    {
                        this.PrintLog("_Pause succ", null, false);
                    }
                    if (this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseMic() == 0)
                    {
                        this.PrintLog("onPause_CloseMic succ", null, false);
                    }
                    if (this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseSpeaker() == 0)
                    {
                        this.PrintLog("onPause_CloseSpeaker succ", null, false);
                    }
                }
                else
                {
                    if (this.ApolloVoiceMgr.CallApolloVoiceSDK._Resume() == 0)
                    {
                        this.PrintLog("_Resume succ", null, false);
                    }
                    if (this.m_isOpenMic && (this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenMic() == 0))
                    {
                        this.PrintLog("onResume_OpenMic succ", null, false);
                    }
                    if (this.m_isOpenSpeaker && (this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenSpeaker() == 0))
                    {
                        this.PrintLog("onResume_OpenSpeaker succ", null, false);
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.PrintLog("Ondestoy LeaveRoom", null, false);
            this.LeaveRoom();
        }

        private void onGetMemberState()
        {
            if ((this.m_bInRoom && (this.ApolloVoiceMgr != null)) && (this.ApolloVoiceMgr.CallApolloVoiceSDK != null))
            {
                int[] memberState = new int[12];
                int num = this.ApolloVoiceMgr.CallApolloVoiceSDK._GetMemberState(memberState);
                if (num >= 0)
                {
                    for (int i = 0; i < num; i++)
                    {
                        int memID = memberState[2 * i];
                        int iMemberState = memberState[(2 * i) + 1];
                        this.UpdateVoiceIcon(memID, iMemberState);
                    }
                }
            }
        }

        private void OnHold_VOCEBtn_HeroSelect(CUIEvent uiEvent)
        {
            this.PrintLog("holding ", null, false);
            if ((Time.time - this.m_fStartTimeHeroSelect) >= this.m_TotalVoiceTime)
            {
                this.CloseHeroSelctVoice();
            }
            else if ((Time.time - this.m_fCurTimeHeroSelect) >= this.m_VoiceUpdateDelta)
            {
                this.PrintLog("holding " + this.m_fCurTimeHeroSelect, null, false);
                this.m_fCurTimeHeroSelect = Time.time;
                this.UpdateSoundLevel_HeroSelect(UnityEngine.Random.Range((float) 0f, (float) 1f), this.m_TotalVoiceTime - (Time.time - this.m_fStartTimeHeroSelect));
            }
        }

        private void OnHoldEnd_VOCEBtn_HeroSelect(CUIEvent uiEvent)
        {
            this.PrintLog("holdend ", null, false);
            this.CloseHeroSelctVoice();
        }

        private void OnHoldStart_VOCEBtn_HeroSelect(CUIEvent uiEvent)
        {
            if (CFakePvPHelper.bInFakeSelect)
            {
                if (!this.m_bUseVoiceSysSetting)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(this.m_Voice_Cannot_OpenSetting, false, 1.5f, null, new object[0]);
                    return;
                }
            }
            else
            {
                if (!this.m_bGobalUseVoice)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(this.m_Voice_Server_Not_Open_Tips, false, 1.5f, null, new object[0]);
                    return;
                }
                if (!this.m_bInRoom)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(this.m_Voice_Cannot_JoinVoiceRoom, false, 1.5f, null, new object[0]);
                    return;
                }
            }
            if (!this.m_bClickSound)
            {
                this.m_bClickSound = true;
                this.PrintLog("holdStart ", null, false);
                if (this.m_SoundLevel_HeroSelect != null)
                {
                    this.m_SoundLevel_HeroSelect.gameObject.CustomSetActive(true);
                }
                this.m_fStartTimeHeroSelect = Time.time;
                this.m_fCurTimeHeroSelect = Time.time;
                this.UpdateSoundLevel_HeroSelect(UnityEngine.Random.Range((float) 0f, (float) 1f), 10f);
                this.OpenMic();
            }
        }

        private void OnTimerBattle(int timeSeq)
        {
            this.CloseSoundInBattle();
        }

        public void OpenMic()
        {
            if ((this.m_bInRoom && (this.ApolloVoiceMgr != null)) && (this.ApolloVoiceMgr.CallApolloVoiceSDK != null))
            {
                ApolloVoiceErr err = (ApolloVoiceErr) this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenMic();
                if (err == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                {
                    this.UpdateMyVoiceIcon(1);
                    this.m_isOpenMic = true;
                    this.PrintLog("OpenMic Succ", null, false);
                }
                else
                {
                    string message = string.Format("OpenMic Err is {0}", err);
                    this.PrintLog(message, null, false);
                }
            }
            else
            {
                this.m_isOpenMic = true;
            }
            this.PrintLog("onOpenMicButtonClick", null, false);
        }

        public void OpenSoundInBattle()
        {
            if (this.m_bInRoom && !this.m_bSoundInBattle)
            {
                this.m_bSoundInBattle = true;
                this.OpenMic();
            }
        }

        public void OpenSpeakers()
        {
            if ((this.m_bInRoom && (this.ApolloVoiceMgr != null)) && (this.ApolloVoiceMgr.CallApolloVoiceSDK != null))
            {
                if (!this.m_isOpenSpeaker)
                {
                    ApolloVoiceErr err = (ApolloVoiceErr) this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenSpeaker();
                    if (err == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                    {
                        this.m_isOpenSpeaker = true;
                        this.PrintLog("OpenSpeaker Succ", null, false);
                    }
                    else
                    {
                        string message = string.Format("OpenSpeaker Err is {0}", err);
                        this.PrintLog(message, null, false);
                    }
                }
            }
            else
            {
                this.m_isOpenSpeaker = true;
            }
            this.PrintLog("onOpenSpeakersButtonClick", null, false);
        }

        protected void PrintLog(string message, string filename = null, bool append = false)
        {
        }

        private void QuitRoom()
        {
            if (this.ApolloVoiceMgr != null)
            {
                int num = this.ApolloVoiceMgr.CallApolloVoiceSDK._QuitRoom((long) this.m_MyRoomInfo.roomid, (short) this.m_MyRoomInfo.memberid, this.m_MyRoomInfo.openid);
                if (num == 0)
                {
                    this.m_bInRoom = false;
                    this.PrintLog("QuitRoom Succ", null, false);
                }
                else
                {
                    string message = string.Format("QuitRoom Err is {0}", num);
                    this.PrintLog(message, null, false);
                }
            }
        }

        private void SetSpeakerVolume(int level)
        {
            if ((this.m_bInRoom && (this.ApolloVoiceMgr != null)) && (this.ApolloVoiceMgr.CallApolloVoiceSDK != null))
            {
                if (level <= 2)
                {
                    level = 2;
                }
                this.ApolloVoiceMgr.CallApolloVoiceSDK._SetSpeakerVolume(level);
            }
        }

        public void SetVoiceState(ulong uid, CS_VOICESTATE_TYPE state)
        {
            for (int i = 0; i < this.m_voiceStateList.Count; i++)
            {
                VoiceState state2 = this.m_voiceStateList[i];
                if ((state2 != null) && (state2.uid == uid))
                {
                    state2.state = state;
                    return;
                }
            }
            this.m_voiceStateList.Add(new VoiceState(uid, state));
        }

        public void ShowVoiceBtn_HeroSelect(CUIFormScript formScript)
        {
            this.PrintLog("ShowVoiceBtn_HeroSelect", null, false);
            this.m_bInHeroSelectUI = true;
            if (formScript != null)
            {
                this.m_SoundLevel_HeroSelect = formScript.transform.Find("chatTools/volume_PL");
            }
        }

        public void StartSyncVoiceStateTimer(int ms = 0xfa0)
        {
            if (this.m_timer == -1)
            {
                this.m_timer = Singleton<CTimerManager>.instance.AddTimer(ms, 0, new CTimer.OnTimeUpHandler(this.UpdateSyncVoiceState));
            }
        }

        public CS_VOICESTATE_TYPE TryGetVoiceState(ulong uid)
        {
            for (int i = 0; i < this.m_voiceStateList.Count; i++)
            {
                VoiceState state = this.m_voiceStateList[i];
                if ((state != null) && (state.uid == uid))
                {
                    return state.state;
                }
            }
            return CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE;
        }

        private void Update()
        {
            if (this.m_bInRoom && ((Time.time - this.m_UpdateTime) >= 1f))
            {
                this.m_UpdateTime = Time.time;
                this.onGetMemberState();
            }
        }

        [DebuggerHidden]
        private IEnumerator UpdateEnterRoomState()
        {
            return new <UpdateEnterRoomState>c__Iterator2A { <>f__this = this };
        }

        private void UpdateHeroSelectVoiceBtnState(bool bEnable)
        {
            if (this.m_VoiceBtn != null)
            {
                if (!bEnable)
                {
                    this.m_VoiceBtn.GetComponent<CUIEventScript>().enabled = false;
                    this.m_VoiceBtn.GetComponent<Image>().color = new Color(this.m_VoiceBtn.GetComponent<Image>().color.r, this.m_VoiceBtn.GetComponent<Image>().color.g, this.m_VoiceBtn.GetComponent<Image>().color.b, 0.37f);
                    Text componentInChildren = this.m_VoiceBtn.GetComponentInChildren<Text>();
                    componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
                }
                else
                {
                    this.m_VoiceBtn.GetComponent<CUIEventScript>().enabled = true;
                    this.m_VoiceBtn.GetComponent<Image>().color = new Color(this.m_VoiceBtn.GetComponent<Image>().color.r, this.m_VoiceBtn.GetComponent<Image>().color.g, this.m_VoiceBtn.GetComponent<Image>().color.b, 1f);
                    Text text2 = this.m_VoiceBtn.GetComponentInChildren<Text>();
                    text2.color = new Color(text2.color.r, text2.color.g, text2.color.b, 1f);
                }
            }
        }

        public void UpdateMyVoiceIcon(int iMemberState)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo != null)
            {
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    ulong playerUID = 0L;
                    if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                    {
                        playerUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
                    }
                    if (this.m_bInHeroSelectUI)
                    {
                        Transform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(playerUID, masterMemberInfo.camp);
                        if (teamPlayerElement != null)
                        {
                            Transform transform2 = teamPlayerElement.FindChild("heroItemCell/VoiceIcon");
                            if (this.m_bInRoom && (transform2 != null))
                            {
                                transform2.gameObject.CustomSetActive(iMemberState >= 1);
                            }
                        }
                    }
                    else if ((playerUID > 0L) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
                    {
                        Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                        if ((((hostPlayer != null) && this.m_bInRoom) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain.handle != null))) && (hostPlayer.Captain.handle.HudControl != null))
                        {
                            hostPlayer.Captain.handle.HudControl.ShowVoiceIcon(iMemberState >= 1);
                        }
                    }
                }
            }
        }

        private void UpdateRoomMateInfo(SCPKG_JOIN_TVOIP_ROOM_NTF updateInfo)
        {
            uint dwMemberID = updateInfo.stUserInfo.dwMemberID;
            ulong ullUid = updateInfo.stUserInfo.ullUid;
            string str = Utility.UTF8Convert(updateInfo.stUserInfo.szOpenID);
            this.PrintLog(string.Concat(new object[] { "***recv update msg***memid ", dwMemberID, " uid ", ullUid, " id ", str }), null, false);
            bool flag = false;
            for (int i = 0; i < this.m_RoomMateList.Count; i++)
            {
                ROOMMate mate = this.m_RoomMateList[i];
                if ((mate.openID == str) && (mate.uuid == ullUid))
                {
                    mate.memID = dwMemberID;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                ROOMMate item = new ROOMMate {
                    memID = dwMemberID,
                    openID = str,
                    uuid = ullUid
                };
                this.m_RoomMateList.Add(item);
            }
        }

        private void UpdateSoundLevel_HeroSelect(float soudLevel, float leftSecond)
        {
            if (this.m_SoundLevel_HeroSelect != null)
            {
                this.m_SoundLevel_HeroSelect.Find("Volume").GetComponent<Image>().CustomFillAmount(soudLevel);
                this.m_SoundLevel_HeroSelect.Find("CountDown").GetComponent<Text>().text = string.Format("{0:0.00}", leftSecond);
            }
        }

        public void UpdateSyncVoiceState(int index)
        {
            if (this.lastSendVoiceState != this.curVoiceState)
            {
                VoiceStateNetCore.Send_Acnt_VoiceState(this.curVoiceState);
                this.lastSendVoiceState = this.curVoiceState;
            }
        }

        private void UpdateVoiceIcon(int memID, int iMemberState)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo != null)
            {
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    ulong playerUID = this.FindUUIDByMemId(memID);
                    if (this.m_bInHeroSelectUI)
                    {
                        Transform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(playerUID, masterMemberInfo.camp);
                        if (teamPlayerElement != null)
                        {
                            Transform transform2 = teamPlayerElement.FindChild("heroItemCell/VoiceIcon");
                            if (this.m_bInRoom && (transform2 != null))
                            {
                                transform2.gameObject.CustomSetActive(iMemberState >= 1);
                            }
                        }
                    }
                    else if ((playerUID > 0L) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
                    {
                        Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                        if ((hostPlayer != null) && this.m_bInRoom)
                        {
                            COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
                            List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(playerCamp);
                            for (int i = 0; i < allCampPlayers.Count; i++)
                            {
                                Player player2 = allCampPlayers[i];
                                if ((this.m_bInRoom && (player2 != null)) && (player2.PlayerUId == playerUID))
                                {
                                    if (((player2.Captain != 0) && (player2.Captain.handle != null)) && (player2.Captain.handle.HudControl != null))
                                    {
                                        player2.Captain.handle.HudControl.ShowVoiceIcon(iMemberState >= 1);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void WriteLogtoFile(string filename, string strinfo, bool append = false)
        {
            UnityEngine.Debug.Log("commonForTest.WriteLogtoFile");
            string str = filename;
            using (StreamWriter writer = new StreamWriter("sdcard/" + str + ".txt", append))
            {
                writer.WriteLine(strinfo);
            }
        }

        private CApolloVoiceSys ApolloVoiceMgr
        {
            get
            {
                if (this.m_ApolloVoiceMgr == null)
                {
                    return null;
                }
                if (this.m_ApolloVoiceMgr.CallApolloVoiceSDK == null)
                {
                    return null;
                }
                return this.m_ApolloVoiceMgr;
            }
        }

        public CS_VOICESTATE_TYPE curVoiceState
        {
            get
            {
                if (this.IsOpenMic())
                {
                    return CS_VOICESTATE_TYPE.CS_VOICESTATE_FULL;
                }
                if (this.IsOpenSpeak())
                {
                    return CS_VOICESTATE_TYPE.CS_VOICESTATE_PART;
                }
                return CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE;
            }
        }

        public bool GlobalVoiceSetting
        {
            get
            {
                return this.m_bGobalUseVoice;
            }
        }

        public bool IsUseVoiceSysSetting
        {
            get
            {
                return this.m_bUseVoiceSysSetting;
            }
            set
            {
                this.m_bUseVoiceSysSetting = value;
                if (this.m_bUseVoiceSysSetting)
                {
                    this.OpenSpeakers();
                }
                else
                {
                    this.ClosenSpeakers();
                }
            }
        }

        public CS_VOICESTATE_TYPE lastSendVoiceState { get; set; }

        public int TotalVoiceTime
        {
            get
            {
                return this.m_TotalVoiceTime;
            }
        }

        public bool UseMic
        {
            get
            {
                return this.m_isOpenMic;
            }
        }

        public bool UseMicOnUser
        {
            get
            {
                return this.m_bUseMicOnUser;
            }
            set
            {
                this.m_bUseMicOnUser = value;
            }
        }

        public bool UseSpeak
        {
            get
            {
                return this.m_isOpenSpeaker;
            }
        }

        public float VoiceLevel
        {
            get
            {
                return this.m_VoiceLevel;
            }
            set
            {
                this.m_VoiceLevel = value;
                this.SetSpeakerVolume((int) this.m_VoiceLevel);
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateEnterRoomState>c__Iterator2A : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal VoiceSys <>f__this;
            internal int <idx>__0;
            internal ApolloVoiceErr <result>__1;
            internal string <text>__2;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<idx>__0 = 0;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_01B5;
                }
                if (!this.<>f__this.m_bInRoom && (this.<>f__this.ApolloVoiceMgr != null))
                {
                    this.<idx>__0++;
                    if (this.<idx>__0 < 200)
                    {
                        this.<result>__1 = (ApolloVoiceErr) this.<>f__this.ApolloVoiceMgr.CallApolloVoiceSDK._GetJoinRoomResult();
                        if (this.<result>__1 != ApolloVoiceErr.APOLLO_VOICE_JOIN_SUCC)
                        {
                            this.<text>__2 = string.Format("JoinRoom failed :  Result : {0}, seq : {1}", this.<result>__1, this.<idx>__0);
                            this.<>f__this.PrintLog(this.<text>__2, null, false);
                            this.$current = new WaitForSeconds(0.1f);
                            this.$PC = 1;
                            return true;
                        }
                        this.<>f__this.m_bInRoom = true;
                        this.<>f__this.m_UpdateTime = Time.time;
                        if (this.<>f__this.IsUseVoiceSysSetting)
                        {
                            this.<>f__this.OpenSpeakers();
                        }
                        else
                        {
                            this.<>f__this.ClosenSpeakers();
                        }
                        this.<>f__this.ApolloVoiceMgr.CallApolloVoiceSDK._SetSpeakerVolume((int) this.<>f__this.VoiceLevel);
                        this.<>f__this.ApolloVoiceMgr.CallApolloVoiceSDK._SetMemberCount(2);
                        this.<>f__this.UpdateHeroSelectVoiceBtnState(true);
                        this.<>f__this.PrintLog("JoinRoom Succ", null, false);
                    }
                    else
                    {
                        this.<>f__this.PrintLog("_GetJoinRoomResult time Out", null, false);
                    }
                }
                this.<>f__this.m_bUpdateEnterRoomState = false;
                this.$PC = -1;
            Label_01B5:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ROOMINFO
        {
            public ulong roomid;
            public ulong ullRoomKey;
            public uint memberid;
            public string openid;
            public ulong uuid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ROOMMate
        {
            public uint memID;
            public string openID;
            public ulong uuid;
        }

        public class VoiceState
        {
            public CS_VOICESTATE_TYPE state;
            public ulong uid;

            public VoiceState(ulong uid, CS_VOICESTATE_TYPE state)
            {
                this.uid = uid;
                this.state = state;
            }
        }
    }
}

