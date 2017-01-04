namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CRecordUseSDK : Singleton<CRecordUseSDK>
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<float, float>, float> <>f__am$cache19;
        [CompilerGenerated]
        private static Func<KeyValuePair<float, RECORD_INFO>, float> <>f__am$cache1A;
        private const string ASSISTNUM = "ASSISTNUM";
        private const string DEADNUM = "DEADNUM";
        private const string GRADENAME = "GRADE";
        private const string HERONAME = "HERO";
        private const string KILLNUM = "KILLNUM";
        private bool m_bIsCallGameJoyGenerate;
        private bool m_bIsMvp;
        private bool m_bIsRecordMomentsEnable;
        private bool m_bIsStartRecordOk;
        private RECORD_EVENT_PRIORITY m_enLastEventPriority;
        private CHECK_PERMISSION_STUTAS m_enmCheckPermissionRes = CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT;
        private CHECK_WHITELIST_STATUS m_enmCheckWhiteListStatus;
        private float m_fGameEndTime;
        private float m_fGameStartTime;
        private float m_fLastEventEndTime;
        private float m_fLastEventStartTime;
        private PoolObjHandle<ActorRoot> m_hostActor;
        private int m_iContinuKillMaxNum = -1;
        private int m_iHostPlaterAssistNum;
        private int m_iHostPlaterDeadNum;
        private int m_iHostPlaterKillNum;
        private GameObject m_objKingBar;
        private Transform m_RecorderPanel;
        private Dictionary<RECORD_EVENT_PRIORITY, Dictionary<float, float>> m_stRecordInfo = new Dictionary<RECORD_EVENT_PRIORITY, Dictionary<float, float>>();
        private string m_strHostPlayerName;
        private uint m_uiEventNumMax = 5;
        private uint m_uiEventTimeInterval = 10;
        private uint m_uiEventTotalTime = 120;
        private uint m_uiMinSpaceLimit = 200;
        private uint m_uiWarningSpaceLimit = 500;
        private const string MODENAME = "MODE";
        private const int RECORD_SHOW_GRADENAME_MIN = 10;

        private void AddRecordEvent(RECORD_EVENT_PRIORITY eventPriority, float fStartTime, float fEndTime)
        {
            if (this.m_stRecordInfo != null)
            {
                Dictionary<float, float> dictionary = null;
                if (!this.m_stRecordInfo.TryGetValue(eventPriority, out dictionary))
                {
                    dictionary = new Dictionary<float, float>();
                    this.m_stRecordInfo.Add(eventPriority, dictionary);
                }
                if (dictionary != null)
                {
                    dictionary.Add(fStartTime, fEndTime);
                }
                this.m_enLastEventPriority = RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID;
            }
        }

        public void CallGameJoyGenerateWithNothing()
        {
            if ((this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk) && !this.m_bIsCallGameJoyGenerate)
            {
                this.m_bIsCallGameJoyGenerate = true;
                Singleton<GameJoy>.instance.GenerateMomentsVideo(null, null, null);
            }
        }

        private void CheckPermission()
        {
            GameJoy.checkSDKPermission();
        }

        private bool CheckStorage()
        {
            bool flag = true;
            long num = 0L;
            object[] args = new object[] { Application.persistentDataPath };
            using (AndroidJavaObject obj2 = new AndroidJavaObject("android.os.StatFs", args))
            {
                num = ((((long) obj2.Call<int>("getBlockSize", new object[0])) * ((long) obj2.Call<int>("getAvailableBlocks", new object[0]))) / 0x400L) / 0x400L;
            }
            if (num < this.m_uiMinSpaceLimit)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Disk_Space_Limit"), false, 1.5f, null, new object[0]);
                flag = false;
            }
            return flag;
        }

        private void CheckWhiteList()
        {
            GameJoy.CheckRecorderAvailability();
        }

        private void ChooseTopEvent()
        {
            int num = 0;
            int num2 = 0;
            bool flag = false;
            Dictionary<float, RECORD_INFO> source = new Dictionary<float, RECORD_INFO>();
            Dictionary<float, float> dictionary2 = null;
            for (RECORD_EVENT_PRIORITY record_event_priority = RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL; record_event_priority > RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID; record_event_priority -= 1)
            {
                if (((this.m_stRecordInfo != null) && this.m_stRecordInfo.TryGetValue(record_event_priority, out dictionary2)) && (dictionary2 != null))
                {
                    if (<>f__am$cache19 == null)
                    {
                        <>f__am$cache19 = s => s.Key;
                    }
                    IEnumerator<KeyValuePair<float, float>> enumerator = dictionary2.OrderBy<KeyValuePair<float, float>, float>(<>f__am$cache19).GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<float, float> current = enumerator.Current;
                            num2++;
                            if (num2 > this.m_uiEventNumMax)
                            {
                                flag = true;
                                goto Label_013E;
                            }
                            float key = (current.Key >= this.m_uiEventTimeInterval) ? (current.Key - this.m_uiEventTimeInterval) : 0f;
                            float num4 = current.Value + this.m_uiEventTimeInterval;
                            num4 = (num4 <= this.m_fGameEndTime) ? num4 : this.m_fGameEndTime;
                            num += (int) (num4 - key);
                            if (num > this.m_uiEventTotalTime)
                            {
                                flag = true;
                                goto Label_013E;
                            }
                            source.Add(key, new RECORD_INFO(record_event_priority, num4));
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                }
            Label_013E:
                if (flag)
                {
                    break;
                }
            }
            if (<>f__am$cache1A == null)
            {
                <>f__am$cache1A = s => s.Key;
            }
            IOrderedEnumerable<KeyValuePair<float, RECORD_INFO>> enumerable2 = source.OrderBy<KeyValuePair<float, RECORD_INFO>, float>(<>f__am$cache1A);
            List<TimeStamp> timeStampList = new List<TimeStamp>();
            float num5 = 0f;
            float num6 = 0f;
            IEnumerator<KeyValuePair<float, RECORD_INFO>> enumerator2 = enumerable2.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    KeyValuePair<float, RECORD_INFO> pair2 = enumerator2.Current;
                    float num7 = pair2.Key;
                    float fEndTime = pair2.Value.fEndTime;
                    if ((timeStampList.Count > 0) && (num5 > num7))
                    {
                        timeStampList.RemoveAt(timeStampList.Count - 1);
                        num -= (int) (num5 - num6);
                        num5 = (num5 + num7) / 2f;
                        num7 = num5;
                        num += (int) (num5 - num6);
                        timeStampList.Add(new TimeStamp(((ulong) num6) * 0x3e8L, ((ulong) num5) * 0x3e8L));
                    }
                    num6 = num7;
                    num5 = fEndTime;
                    timeStampList.Add(new TimeStamp(((ulong) num7) * 0x3e8L, ((ulong) fEndTime) * 0x3e8L));
                }
            }
            finally
            {
                if (enumerator2 == null)
                {
                }
                enumerator2.Dispose();
            }
            this.m_bIsCallGameJoyGenerate = true;
            Singleton<GameJoy>.instance.GenerateMomentsVideo(timeStampList, this.GetVideoName(), this.GetExtraInfos());
        }

        private void CloseRecorderPanel()
        {
            if (this.m_RecorderPanel != null)
            {
                Transform transform = this.m_RecorderPanel.FindChild("Extra");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }

        private int ConvertMaxMultiKillPriorityToResDef()
        {
            int num = -1;
            if (this.m_stRecordInfo.Count > 0)
            {
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL))
                {
                    return 6;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_QUATARYKILL))
                {
                    return 5;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_TRIPLEKILL))
                {
                    return 4;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL))
                {
                    return 3;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ONCEKILL))
                {
                    return 2;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST))
                {
                    num = 0;
                }
            }
            return num;
        }

        public void DoFightOver()
        {
            this.UpdateRecordEvent(new PoolObjHandle<ActorRoot>(), RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID);
            this.m_fGameEndTime = Time.realtimeSinceStartup - this.m_fGameStartTime;
            if (this.m_hostActor != 0)
            {
                this.m_strHostPlayerName = this.m_hostActor.handle.name;
                uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(this.m_hostActor.handle.TheActorMeta.ActorCamp, true);
                if ((mvpPlayer != 0) && (mvpPlayer == this.m_hostActor.handle.TheActorMeta.PlayerId))
                {
                    this.m_bIsMvp = true;
                }
            }
            PlayerKDA hostKDA = null;
            if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
            {
                hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                if (hostKDA != null)
                {
                    this.m_iHostPlaterKillNum = hostKDA.numKill;
                    this.m_iHostPlaterDeadNum = hostKDA.numDead;
                    this.m_iHostPlaterAssistNum = hostKDA.numAssist;
                }
            }
            this.StopMomentsRecording();
        }

        private Dictionary<string, string> GetExtraInfos()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                dictionary.Add("MODE", curLvelContext.m_gameMatchName);
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) masterRoleInfo.m_rankGrade);
                if (dataByKey != null)
                {
                    dictionary.Add("GRADE", dataByKey.szGradeDesc);
                }
            }
            if (!string.IsNullOrEmpty(this.m_strHostPlayerName))
            {
                int index = this.m_strHostPlayerName.IndexOf('(');
                string str = this.m_strHostPlayerName.Substring(index + 1, (this.m_strHostPlayerName.Length - index) - 2);
                dictionary.Add("HERO", str);
            }
            dictionary.Add("KILLNUM", this.m_iHostPlaterKillNum.ToString());
            dictionary.Add("DEADNUM", this.m_iHostPlaterDeadNum.ToString());
            dictionary.Add("ASSISTNUM", this.m_iHostPlaterAssistNum.ToString());
            return dictionary;
        }

        public bool GetRecorderGlobalCfgEnableFlag()
        {
            bool flag = false;
            if ((GameDataMgr.svr2CltCfgDict != null) && GameDataMgr.svr2CltCfgDict.ContainsKey(13))
            {
                ResGlobalInfo info = new ResGlobalInfo();
                if (GameDataMgr.svr2CltCfgDict.TryGetValue(13, out info))
                {
                    flag = info.dwConfValue > 0;
                }
            }
            return flag;
        }

        private Vector3 GetRecorderPosition()
        {
            Vector3 zero = Vector3.zero;
            if (this.m_RecorderPanel != null)
            {
                Transform transform = this.m_RecorderPanel.FindChild("Record");
                if (transform == null)
                {
                    return zero;
                }
                Camera current = Camera.current;
                if (current == null)
                {
                    current = Camera.allCameras[0];
                }
                if (current != null)
                {
                    zero = current.WorldToViewportPoint(transform.transform.position);
                }
            }
            return zero;
        }

        private string GetVideoName()
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("RecordMomentVideoNameHeader");
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                text = text + curLvelContext.m_levelName;
                if (curLvelContext.IsGameTypeLadder())
                {
                    text = text + curLvelContext.m_gameMatchName;
                }
            }
            if (this.m_bIsMvp)
            {
                text = text + "MVP";
            }
            if (!string.IsNullOrEmpty(this.m_strHostPlayerName))
            {
                int index = this.m_strHostPlayerName.IndexOf('(');
                string str2 = this.m_strHostPlayerName.Substring(index + 1, (this.m_strHostPlayerName.Length - index) - 2);
                text = text + str2;
            }
            int num2 = this.ConvertMaxMultiKillPriorityToResDef();
            if (num2 > 2)
            {
                ResMultiKill dataByKey = GameDataMgr.multiKillDatabin.GetDataByKey((long) num2);
                if (dataByKey != null)
                {
                    text = text + dataByKey.szAchievementName;
                }
            }
            return (text + Singleton<CTextManager>.GetInstance().GetText("RecordMomentVideoNameTail"));
        }

        public override void Init()
        {
            base.Init();
            this.Reset();
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Save_Moment_Video, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Save_Moment_Video_Cancel, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideoCancel));
            Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, new Action<bool>(this.OnGameJoyStartRecordResult));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Btn_VideoMgr_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnVideoMgrClick));
            Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckPermissionResult));
            Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_AVAILABILITY_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckAvailabilityResult));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Check_WhiteList_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnCheckWhiteListTimeUp));
            this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK;
            this.CheckWhiteList();
        }

        private void OnActorDead(ref GameDeadEventParam prm)
        {
            if ((this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk) && ((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)))
            {
                this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ONCEKILL);
                if (((Singleton<GamePlayerCenter>.instance != null) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && (prm.atker != Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain))
                {
                    if ((prm.src != 0) && (prm.src.handle.ActorControl != null))
                    {
                        List<KeyValuePair<uint, ulong>>.Enumerator enumerator = prm.src.handle.ActorControl.hurtSelfActorList.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<uint, ulong> current = enumerator.Current;
                            if (current.Key == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID)
                            {
                                this.UpdateRecordEvent(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST);
                                return;
                            }
                        }
                    }
                    if ((prm.atker != 0) && (prm.atker.handle.ActorControl != null))
                    {
                        List<KeyValuePair<uint, ulong>>.Enumerator enumerator2 = prm.atker.handle.ActorControl.helpSelfActorList.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            KeyValuePair<uint, ulong> pair2 = enumerator2.Current;
                            if (pair2.Key == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID)
                            {
                                this.UpdateRecordEvent(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST);
                                return;
                            }
                        }
                    }
                }
                else if (((Singleton<GamePlayerCenter>.instance != null) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && (prm.atker == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain))
                {
                    HeroWrapper actorControl = prm.orignalAtker.handle.ActorControl as HeroWrapper;
                    if ((actorControl != null) && (actorControl.ContiKillNum > this.m_iContinuKillMaxNum))
                    {
                        this.m_iContinuKillMaxNum = actorControl.ContiKillNum;
                    }
                }
            }
        }

        private void OnActorDoubleKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL);
        }

        private void OnActorPentaKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL);
        }

        private void OnActorQuataryKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_QUATARYKILL);
        }

        private void OnActorTripleKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_TRIPLEKILL);
        }

        private void OnBtnVideoMgrClick(CUIEvent cuiEvent)
        {
            Singleton<GameJoy>.instance.ShowVideoListDialog();
        }

        private void OnCheckWhiteListTimeUp(CUIEvent uiEvent)
        {
            this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_TIMEUP;
        }

        private void OnFightPrepare(ref DefaultGameEventParam prm)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext != null)
            {
                ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
                if (accountInfo != null)
                {
                    this.m_bIsRecordMomentsEnable = (((GameSettings.EnableKingTimeMode && this.GetRecorderGlobalCfgEnableFlag()) && (!Singleton<WatchController>.GetInstance().IsWatching && curLvelContext.IsMobaModeWithOutGuide())) && (accountInfo.Platform != ApolloPlatform.Guest)) && (this.m_enmCheckWhiteListStatus == CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTOK);
                    if (this.m_bIsRecordMomentsEnable)
                    {
                        this.Reset();
                        this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                        Singleton<GameJoy>.instance.StartMomentsRecording();
                        this.m_fGameStartTime = Time.realtimeSinceStartup;
                    }
                }
            }
        }

        private void OnGameJoyCheckAvailabilityResult(bool bRes)
        {
            if ((this.m_enmCheckWhiteListStatus != CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_TIMEUP) && (this.m_enmCheckWhiteListStatus != CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK))
            {
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                if (!bRes)
                {
                    Singleton<CUIManager>.instance.OpenTips("GamejoyCheckAvailabilityFailed", true, 1.5f, null, new object[0]);
                    this.SetKingBarSliderState(false);
                }
                this.m_enmCheckWhiteListStatus = !bRes ? CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED : CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTOK;
            }
            else if (this.m_enmCheckWhiteListStatus == CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK)
            {
                this.m_enmCheckWhiteListStatus = !bRes ? CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED : CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTOK;
            }
        }

        private void OnGameJoyCheckPermissionResult(bool bRes)
        {
            this.m_enmCheckPermissionRes = !bRes ? CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_NOPERMISSION : CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_PERMISSIONOK;
            if (this.m_objKingBar != null)
            {
                if (!bRes)
                {
                    Singleton<CUIManager>.instance.OpenTips("GameJoyCheckPermissionFailed", true, 1.5f, null, new object[0]);
                }
                else
                {
                    this.SetKingBarSliderState(true);
                }
            }
        }

        private void OnGameJoyStartRecordResult(bool bRes)
        {
            this.m_bIsStartRecordOk = bRes;
        }

        private void OnSaveMomentVideo(CUIEvent uiEvent)
        {
            Singleton<CTaskSys>.instance.Increse(6);
            Vector3 recorderPosition = this.GetRecorderPosition();
            Singleton<GameJoy>.instance.SetDefaultUploadShareDialogPosition(recorderPosition.x, recorderPosition.y);
            this.CloseRecorderPanel();
            this.ChooseTopEvent();
        }

        private void OnSaveMomentVideoCancel(CUIEvent uiEvent)
        {
            this.CloseRecorderPanel();
            this.CallGameJoyGenerateWithNothing();
        }

        public void OpenMsgBoxForMomentRecorder(Transform container)
        {
            if ((this.m_bIsRecordMomentsEnable && (container != null)) && ((this.m_stRecordInfo != null) && (this.m_stRecordInfo.Count != 0)))
            {
                this.m_RecorderPanel = container;
                if (GameSettings.EnableKingTimeMode && this.m_bIsStartRecordOk)
                {
                    Transform transform = container.FindChild("Extra/Image/Image/Text");
                    if ((transform != null) && (transform.gameObject != null))
                    {
                        Text component = transform.gameObject.GetComponent<Text>();
                        if (component != null)
                        {
                            component.text = Singleton<CTextManager>.GetInstance().GetText("RecordSaveMomentVideo");
                        }
                    }
                    container.gameObject.CustomSetActive(true);
                }
            }
        }

        public bool OpenRecorderCheck(GameObject KingBar)
        {
            this.m_objKingBar = KingBar;
            if (this.CheckStorage())
            {
                this.CheckPermission();
                if (this.m_enmCheckPermissionRes != CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT)
                {
                    bool flag = this.m_enmCheckPermissionRes == CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_PERMISSIONOK;
                    this.m_enmCheckPermissionRes = CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT;
                    if (!flag)
                    {
                        return false;
                    }
                    this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_INVALID;
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, 3, enUIEventID.Record_Check_WhiteList_TimeUp);
                    this.CheckWhiteList();
                    return true;
                }
            }
            return false;
        }

        private void Reset()
        {
            this.m_enLastEventPriority = RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID;
            this.m_fLastEventStartTime = 0f;
            this.m_fLastEventEndTime = 0f;
            this.m_fGameStartTime = 0f;
            this.m_fGameEndTime = 0f;
            this.m_iContinuKillMaxNum = -1;
            this.m_bIsStartRecordOk = false;
            this.m_bIsMvp = false;
            this.m_iHostPlaterKillNum = 0;
            this.m_iHostPlaterDeadNum = 0;
            this.m_iHostPlaterAssistNum = 0;
            this.m_bIsCallGameJoyGenerate = false;
            if (this.m_hostActor != 0)
            {
                this.m_hostActor.Release();
            }
            if (this.m_stRecordInfo != null)
            {
                this.m_stRecordInfo.Clear();
            }
            this.m_uiEventTimeInterval = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTTIMEINTERVAL);
            this.m_uiEventNumMax = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTNUMMAX);
            this.m_uiEventTotalTime = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_VIDEOTOTALTIME);
            this.m_uiMinSpaceLimit = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_ANDROIDMINSPACELIMIT);
        }

        private void SetKingBarSliderState(bool bIsOpen)
        {
            if (this.m_objKingBar != null)
            {
                Transform transform = this.m_objKingBar.transform.FindChild("Slider");
                if (transform != null)
                {
                    CUISliderEventScript component = transform.GetComponent<CUISliderEventScript>();
                    int num = !bIsOpen ? 0 : 1;
                    if (((int) component.value) != num)
                    {
                        component.value = num;
                    }
                }
            }
        }

        public void StopMomentsRecording()
        {
            if (this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk)
            {
                Singleton<GameJoy>.instance.EndMomentsRecording();
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_stRecordInfo.Clear();
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Save_Moment_Video, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Save_Moment_Video_Cancel, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideoCancel));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, new Action<bool>(this.OnGameJoyStartRecordResult));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Btn_VideoMgr_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnVideoMgrClick));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckPermissionResult));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_AVAILABILITY_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckAvailabilityResult));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Check_WhiteList_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnCheckWhiteListTimeUp));
        }

        private void UpdateRecordEvent(PoolObjHandle<ActorRoot> eventActor, RECORD_EVENT_PRIORITY eventPriority)
        {
            if ((this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk) && ((eventPriority == RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID) || (Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain == eventActor)))
            {
                float num = Time.realtimeSinceStartup - this.m_fGameStartTime;
                if (((num - this.m_fLastEventStartTime) >= this.m_uiEventTimeInterval) || (eventPriority == RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID))
                {
                    if (this.m_enLastEventPriority > RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID)
                    {
                        this.AddRecordEvent(this.m_enLastEventPriority, this.m_fLastEventStartTime, this.m_fLastEventEndTime);
                    }
                    this.m_enLastEventPriority = eventPriority;
                    this.m_fLastEventStartTime = num;
                    this.m_fLastEventEndTime = num;
                }
                else if ((this.m_enLastEventPriority == RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID) || (eventPriority > this.m_enLastEventPriority))
                {
                    if (this.m_enLastEventPriority <= RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST)
                    {
                        this.m_fLastEventStartTime = num;
                    }
                    this.m_enLastEventPriority = eventPriority;
                    this.m_fLastEventEndTime = num;
                }
            }
        }

        public enum CHECK_PERMISSION_STUTAS
        {
            CHECK_PERMISSION_STUTAS_TYPE_NOPERMISSION = 0,
            CHECK_PERMISSION_STUTAS_TYPE_PERMISSIONOK = 1,
            CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT = -1
        }

        public enum CHECK_WHITELIST_STATUS
        {
            CHECK_WHITELIST_STATUS_TYPE_INVALID,
            CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK,
            CHECK_WHITELIST_STATUS_TYPE_TIMEUP,
            CHECK_WHITELIST_STATUS_TYPE_RESULTOK,
            CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED
        }

        public enum RECORD_EVENT_PRIORITY
        {
            RECORD_EVENT_TYPE_INVALID,
            RECORD_EVENT_TYPE_ASSIST,
            RECORD_EVENT_TYPE_ONCEKILL,
            RECORD_EVENT_TYPE_DOUBLEKILL,
            RECORD_EVENT_TYPE_TRIPLEKILL,
            RECORD_EVENT_TYPE_QUATARYKILL,
            RECORD_EVENT_TYPE_PENTAKILL
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECORD_INFO
        {
            public CRecordUseSDK.RECORD_EVENT_PRIORITY eventPriority;
            public float fEndTime;
            public RECORD_INFO(CRecordUseSDK.RECORD_EVENT_PRIORITY _eventPriority, float _fEndTime)
            {
                this.eventPriority = _eventPriority;
                this.fEndTime = _fEndTime;
            }
        }
    }
}

