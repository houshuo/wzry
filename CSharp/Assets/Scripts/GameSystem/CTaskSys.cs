namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;

    public class CTaskSys : Singleton<CTaskSys>
    {
        private uint _monthCardExpireTime;
        private uint _monthCardTaskID;
        private uint _weekCardExpireTime;
        private uint _weekCardTaskID;
        public const string finish_task_form = "UGUI/Form/System/Task/Form_Task_Finish.prefab";
        public const string finish_task_form_new = "UGUI/Form/System/Task/Form_Task_Finish_New.prefab";
        public CTaskView m_taskView = new CTaskView();
        public CTaskModel model;
        private uint[] reportInts = new uint[7];
        private int sendReportTimer = -1;
        public static readonly string TASK_FORM_PATH = "UGUI/Form/System/Task/Form_MainTask.prefab";
        public static readonly string TASK_LevelRewardFORM_PATH = "UGUI/Form/System/Task/Form_LevelRewardTask.prefab";

        private void _check(CUIEvent uiEvent, bool bDay)
        {
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            DebugHelper.Assert(tagUInt > 0, "id must > 0");
            if (tagUInt != 0)
            {
                ResHuoYueDuReward rewardCfg = this.model.huoyue_data.GetRewardCfg((ushort) tagUInt);
                RES_HUOYUEDU_TYPE type = !bDay ? RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_WEEK : RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY;
                if (this.model.huoyue_data.BAlready_Reward(type, rewardCfg.wID))
                {
                    Singleton<CUIManager>.instance.OpenTips(UT.GetText("CS_HUOYUEDUREWARD_GETED"), false, 1.5f, null, new object[0]);
                }
                else
                {
                    uint num2 = !bDay ? this.model.huoyue_data.week_curNum : this.model.huoyue_data.day_curNum;
                    if (num2 >= rewardCfg.dwHuoYueDu)
                    {
                        TaskNetUT.Send_GetHuoyue_Reward(rewardCfg.wID);
                    }
                    else
                    {
                        Singleton<CUICommonSystem>.instance.OpenUseableTips(this.model.huoyue_data.GetUsable(rewardCfg.wID), uiEvent.m_pointerEventData.pressPosition.x, uiEvent.m_pointerEventData.pressPosition.y, enUseableTipsPos.enTop);
                    }
                }
            }
        }

        private void _show_task_award(uint taskid)
        {
            CTask task = this.model.task_Data.GetTask(taskid);
            if (task != null)
            {
                ResTaskReward resAward = task.resAward;
                if (resAward != null)
                {
                    int num = 0;
                    for (int i = 0; i < resAward.astRewardInfo.Length; i++)
                    {
                        ResTaskRewardDetail detail = resAward.astRewardInfo[i];
                        if (detail.iCnt > 0)
                        {
                            num++;
                        }
                    }
                    CUseable[] items = new CUseable[num];
                    for (int j = 0; j < resAward.astRewardInfo.Length; j++)
                    {
                        ResTaskRewardDetail detail2 = resAward.astRewardInfo[j];
                        if (detail2.iCnt > 0)
                        {
                            items[j] = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) detail2.dwRewardType, detail2.iCnt, detail2.dwRewardID);
                        }
                    }
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(items, null, false, enUIEventID.None, false, false, "Form_Award");
                }
            }
        }

        public void Clear()
        {
            this.model.Clear();
        }

        public void ClearReport()
        {
            for (int i = 0; i < this.reportInts.Length; i++)
            {
                this.reportInts[i] = 0;
            }
        }

        public CTaskView GetTaskView()
        {
            return this.m_taskView;
        }

        public int GetTotalTaskOfState(RES_TASK_TYPE type, COM_TASK_STATE inState)
        {
            return this.model.GetTotalTaskOfState(type, inState);
        }

        public void Increse(int index)
        {
            this.reportInts[index]++;
        }

        public override void Init()
        {
            if (this.model == null)
            {
                this.model = new CTaskModel();
            }
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTaskForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseTaskForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_EngineCloseForm, new CUIEventManager.OnUIEventHandler(this.OnTask_EngineCloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_LinkPve, new CUIEventManager.OnUIEventHandler(this.OnLinkPveClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_TabChanged, new CUIEventManager.OnUIEventHandler(this.OnTaskChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_AwardClose, new CUIEventManager.OnUIEventHandler(this.OnTaskAwardClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_List_ElementEnable, new CUIEventManager.OnUIEventHandler(this.On_Task_List_ElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Submit, new CUIEventManager.OnUIEventHandler(this.On_Task_Submit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Week_Reaward1, new CUIEventManager.OnUIEventHandler(this.OnTask_Week_Reaward1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Week_Reaward2, new CUIEventManager.OnUIEventHandler(this.OnTask_Week_Reaward2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Day_Reaward, new CUIEventManager.OnUIEventHandler(this.OnTask_Day_Reaward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_ClickLevelNode, new CUIEventManager.OnUIEventHandler(this.OnTask_ClickLevelNode));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_ClickGetLevelReward, new CUIEventManager.OnUIEventHandler(this.OnTask_ClickGetLevelReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_LevelElemntEnable, new CUIEventManager.OnUIEventHandler(this.OnTask_LevelElemntEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_JumpToReward, new CUIEventManager.OnUIEventHandler(this.OnTask_JumpToReward));
            Singleton<EventRouter>.instance.AddEventHandler("TASK_HUOYUEDU_Change", new System.Action(this.On_TASK_HUOYUEDU_Change));
            this._monthCardTaskID = 0;
            this._weekCardTaskID = 0;
            this._monthCardExpireTime = 0;
            this._weekCardExpireTime = 0;
            base.Init();
        }

        public void InitReport()
        {
            this.ClearReport();
            if (this.sendReportTimer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.sendReportTimer);
            }
            this.sendReportTimer = -1;
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x88);
            DebugHelper.Assert(dataByKey != null, "--- 分享统计上报时间间隔 找不到对应的配置项, 请检查...");
            this.sendReportTimer = Singleton<CTimerManager>.instance.AddTimer((int) (dataByKey.dwConfValue * 0x3e8), 0, new CTimer.OnTimeUpHandler(this.OnReportTimerEnd));
            Singleton<CTimerManager>.instance.ResumeTimer(this.sendReportTimer);
        }

        public bool IsReportAllZero()
        {
            for (int i = 0; i < this.reportInts.Length; i++)
            {
                if (this.reportInts[i] > 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void On_TASK_HUOYUEDU_Change()
        {
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh_Huoyue();
            }
        }

        private void On_Task_List_ElementEnable(CUIEvent uiEvent)
        {
            if (this.m_taskView != null)
            {
                this.m_taskView.On_List_ElementEnable(uiEvent);
            }
        }

        private void On_Task_Submit(CUIEvent uiEvent)
        {
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            DebugHelper.Assert(tagUInt > 0, "---ctask Submit task, taskid should > 0");
            if (tagUInt > 0)
            {
                TaskNetUT.Send_SubmitTask(tagUInt);
            }
        }

        public void On_TASKUPD_NTF(ref CSPkg pkg)
        {
            SCPKG_TASKUPD_NTF stTaskUdpNtf = pkg.stPkgData.stTaskUdpNtf;
            for (int i = 0; i < stTaskUdpNtf.dwUpdTaskCnt; i++)
            {
                SCDT_UPDTASKONE scdt_updtaskone = stTaskUdpNtf.astUpdTaskDetail[i];
                if (scdt_updtaskone != null)
                {
                    uint key = 0;
                    if (scdt_updtaskone.bUpdateType == 0)
                    {
                        key = scdt_updtaskone.stUpdTaskInfo.stUdpPrerequisite.dwTaskID;
                    }
                    if (this.model.task_Data.task_map.ContainsKey(key))
                    {
                        SCDT_UDPPREREQUISITE stUdpPrerequisite = scdt_updtaskone.stUpdTaskInfo.stUdpPrerequisite;
                        if (stUdpPrerequisite != null)
                        {
                            CTask task = this.model.GetTask(stUdpPrerequisite.dwTaskID);
                            if (task != null)
                            {
                                task.SetState(stUdpPrerequisite.bTaskState);
                                for (int j = 0; j < stUdpPrerequisite.bPrerequisiteNum; j++)
                                {
                                    int bPosInArray = stUdpPrerequisite.astPrerequisiteInfo[j].bPosInArray;
                                    bool flag = stUdpPrerequisite.astPrerequisiteInfo[j].bIsReach > 0;
                                    if (flag)
                                    {
                                        task.m_prerequisiteInfo[bPosInArray].m_value = task.m_prerequisiteInfo[bPosInArray].m_valueTarget;
                                    }
                                    else
                                    {
                                        task.m_prerequisiteInfo[bPosInArray].m_value = (int) stUdpPrerequisite.astPrerequisiteInfo[j].dwCnt;
                                    }
                                    task.m_prerequisiteInfo[bPosInArray].m_isReach = flag;
                                }
                            }
                        }
                    }
                }
            }
            this.UpdateTaskState();
            Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh();
            }
        }

        public void OnCloseTaskForm(CUIEvent uiEvent)
        {
            this.m_taskView.OnCloseTaskForm();
            Singleton<CMiShuSystem>.instance.OnCloseTalk(null);
            Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
        }

        public void OnHuoyue_Error_NTF(ref CSPkg pkg)
        {
            int iErrorCode = pkg.stPkgData.stHuoYueDuRewardErr.iErrorCode;
            string text = string.Empty;
            switch (iErrorCode)
            {
                case 0:
                    text = UT.GetText("CS_HUOYUEDUREWARD_SUCC");
                    break;

                case 1:
                    text = UT.GetText("CS_HUOYUEDUREWARD_ACNTNULL");
                    break;

                case 2:
                    text = UT.GetText("CS_HUOYUEDUREWARD_INFONULL");
                    break;

                case 3:
                    text = UT.GetText("CS_HUOYUEDUREWARD_NOTINTABLE");
                    break;

                case 4:
                    text = UT.GetText("CS_HUOYUEDUREWARD_NOTENOUGH");
                    break;

                case 5:
                    text = UT.GetText("CS_HUOYUEDUREWARD_GETED");
                    break;
            }
            if (!string.IsNullOrEmpty(text))
            {
                Singleton<CUIManager>.instance.OpenTips(text, false, 1.5f, null, new object[0]);
            }
        }

        public void OnHuoyue_Info_NTF(ref CSPkg pkg)
        {
            COMDT_HUOYUEDU_DATA stHuoYueDuInfo = pkg.stPkgData.stNtfHuoYueDuInfo.stHuoYueDuInfo;
            this.model.huoyue_data.Set(RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_WEEK, stHuoYueDuInfo.dwWeekHuoYue, stHuoYueDuInfo.iWeekRewardCnt, stHuoYueDuInfo.WeekReward);
            this.model.huoyue_data.Set(RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY, stHuoYueDuInfo.dwDayHuoYue, stHuoYueDuInfo.iDayRewardCnt, stHuoYueDuInfo.DayReward);
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh_Huoyue();
            }
        }

        public void OnHuoyue_Reward_RES(ref CSPkg pkg)
        {
            SCPKG_GETHUOYUEDUREWARD_RSP stHuoYueDuRewardRsp = pkg.stPkgData.stHuoYueDuRewardRsp;
            this.model.huoyue_data.Get_Reward((RES_HUOYUEDU_TYPE) stHuoYueDuRewardRsp.bRewardType, stHuoYueDuRewardRsp.wHuoYueDuId);
            if (stHuoYueDuRewardRsp.stRewardInfo.bNum > 0)
            {
                ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(stHuoYueDuRewardRsp.stRewardInfo);
                Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(useableListFromReward), null, false, enUIEventID.None, false, false, "Form_Award");
            }
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh_Huoyue();
            }
        }

        public void OnInitTask(CSPkg pkg)
        {
            SCPKG_CMD_GAMELOGINRSP stGameLoginRsp = pkg.stPkgData.stGameLoginRsp;
            for (int i = 0; i < stGameLoginRsp.stLoginTaskInfo.dwCurtaskNum; i++)
            {
                COMDT_ACNT_CURTASK comdt_acnt_curtask = stGameLoginRsp.stLoginTaskInfo.astCurtask[i];
                CTask task = TaskUT.Create_Task(comdt_acnt_curtask.dwBaseID);
                if (task != null)
                {
                    task.SetState(comdt_acnt_curtask.bTaskState);
                    TaskUT.Add_Task(task);
                    for (int j = 0; j < comdt_acnt_curtask.bPrerequisiteNum; j++)
                    {
                        int bPosInArray = comdt_acnt_curtask.astPrerequisiteInfo[j].bPosInArray;
                        bool flag = comdt_acnt_curtask.astPrerequisiteInfo[j].bIsReach > 0;
                        if (flag)
                        {
                            task.m_prerequisiteInfo[bPosInArray].m_value = task.m_prerequisiteInfo[bPosInArray].m_valueTarget;
                        }
                        else
                        {
                            task.m_prerequisiteInfo[bPosInArray].m_value = (int) comdt_acnt_curtask.astPrerequisiteInfo[j].dwCnt;
                        }
                        task.m_prerequisiteInfo[bPosInArray].m_isReach = flag;
                    }
                }
            }
            this.UpdateTaskState();
            this.model.ParseCltCalcCompletedTasks(ref stGameLoginRsp.stLoginTaskInfo.MainTaskIDs);
            Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh();
            }
        }

        public void OnLinkPveClick(CUIEvent uiEvent)
        {
            uint taskId = uiEvent.m_eventParams.taskId;
            uiEvent.m_eventID = enUIEventID.Adv_OpenChapterForm;
            ResTask dataByKey = GameDataMgr.taskDatabin.GetDataByKey(taskId);
            int iParam = 0;
            for (int i = 0; i < dataByKey.astPrerequisiteArray.Length; i++)
            {
                if (dataByKey.astPrerequisiteArray[i].dwPrerequisiteType == 2)
                {
                    iParam = dataByKey.astPrerequisiteArray[i].astPrerequisiteParam[1].iParam;
                    break;
                }
            }
            if (iParam == 0)
            {
                Singleton<CAdventureSys>.instance.currentChapter = 1;
                Singleton<CAdventureSys>.instance.currentLevelSeq = 1;
                Singleton<CAdventureSys>.instance.currentDifficulty = 1;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
            else
            {
                ResLevelCfgInfo info = GameDataMgr.levelDatabin.GetDataByKey((long) iParam);
                DebugHelper.Assert(info != null);
                if (info != null)
                {
                    Singleton<CAdventureSys>.instance.currentChapter = info.iChapterId;
                    Singleton<CAdventureSys>.instance.currentLevelSeq = info.bLevelNo;
                    Singleton<CAdventureSys>.instance.currentDifficulty = uiEvent.m_eventParams.tag;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
                }
            }
        }

        public void OnOpenTaskForm(CUIEvent uiEvent)
        {
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK))
            {
                this.m_taskView.OpenForm(uiEvent);
                TaskNetUT.Send_Update_Task(0);
                CMiShuSystem.SendReqCoinGetPathData();
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x10);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        public void OnRefreshTaskView()
        {
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh();
            }
        }

        private void OnReportTimerEnd(int timersequence)
        {
            if (!Singleton<BattleLogic>.instance.isRuning)
            {
                this.SendReport();
                this.ClearReport();
            }
        }

        public void OnSCID_NEWTASKGET_NTF(ref CSPkg pkg)
        {
            SCPKG_NEWTASKGET_NTF stNewTaskGet = pkg.stPkgData.stNewTaskGet;
            for (int i = 0; i < stNewTaskGet.dwTaskCnt; i++)
            {
                SCDT_NEWTASKGET scdt_newtaskget = stNewTaskGet.astNewTask[i];
                CTask task = TaskUT.Create_Task(scdt_newtaskget.dwTaskID);
                DebugHelper.Assert(task.m_taskType == scdt_newtaskget.dwTaskType, "OnSCID_NEWTASKGET_NTF task.m_taskType == info.dwTaskType");
                if (task != null)
                {
                    this.model.AddTask(task);
                }
            }
            Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh();
            }
        }

        private void OnTask_ClickGetLevelReward(CUIEvent uievent)
        {
            DebugHelper.Assert(this.model.curLevelRewardData != null, "OnTask_ClickGetLevelReward model.m_curLevelRewardData == null");
            if (this.model.curLevelRewardData != null)
            {
                TaskNetUT.Send_Get_Level_Reward_Request(this.model.curLevelRewardData.m_level);
            }
        }

        private void OnTask_ClickLevelNode(CUIEvent uievent)
        {
            int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
            LevelRewardData data = this.model.GetLevelRewardData_ByIndex(srcWidgetIndexInBelongedList);
            if ((data != null) && (this.model.curLevelRewardData != data))
            {
                this.model.curLevelRewardData = data;
            }
        }

        private void OnTask_Day_Reaward(CUIEvent uiEvent)
        {
            this._check(uiEvent, true);
        }

        public void OnTask_EngineCloseForm(CUIEvent uiEvent)
        {
            if (this.m_taskView != null)
            {
                this.m_taskView.OnEngineCloseForm();
            }
        }

        private void OnTask_JumpToReward(CUIEvent uievent)
        {
            if ((this.m_taskView != null) && (this.model.curLevelRewardData != null))
            {
                int levelIndex = this.model.GetLevelIndex(this.model.curLevelRewardData.m_level);
                if (levelIndex != -1)
                {
                    int nextlistIndex = 0;
                    LevelRewardData nextData = null;
                    if (this.model.CalcNextRewardNode(levelIndex, out nextlistIndex, out nextData))
                    {
                        this.model.curLevelRewardData = nextData;
                    }
                }
            }
        }

        private void OnTask_LevelElemntEnable(CUIEvent uievent)
        {
            if (this.m_taskView != null)
            {
                this.m_taskView.On_LevelRewardList_ElementEnable(uievent);
            }
        }

        private void OnTask_Week_Reaward1(CUIEvent uiEvent)
        {
            this._check(uiEvent, false);
        }

        private void OnTask_Week_Reaward2(CUIEvent uiEvent)
        {
            this._check(uiEvent, false);
        }

        private void OnTaskAwardClose(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_Award.prefab");
        }

        public void OnTaskChanged(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_taskView.On_Tab_Change(selectedIndex);
            switch (selectedIndex)
            {
                case 0:
                    CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_MainTaskBtn);
                    break;

                case 1:
                    CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_DayTaskBtn);
                    break;

                case 2:
                    CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_GetCoinBtn);
                    break;

                case 3:
                    CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_GetSymbolBtn);
                    break;

                case 4:
                    CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_GetHeroBtn);
                    break;
            }
        }

        public void OnTASKSUBMIT_RES(ref CSPkg pkg)
        {
            if (pkg.stPkgData.stSubmitTaskRes.bSubmitResult == 0)
            {
                uint dwTaskID = pkg.stPkgData.stSubmitTaskRes.dwTaskID;
                this._show_task_award(dwTaskID);
                CTask task = this.model.GetTask(dwTaskID);
                if (task != null)
                {
                    task.SetState((CTask.State) pkg.stPkgData.stSubmitTaskRes.bTaskState);
                }
            }
            Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh();
            }
        }

        public void OnUSUALTASK_RES(ref CSPkg pkg)
        {
            SCDT_USUTASKLIST stUsualTaskList = pkg.stPkgData.stUsualTaskRes.stUpdateDetail.stUsualTaskList;
            for (int i = 0; i < stUsualTaskList.bNewUsualTaskCnt; i++)
            {
                DT_USUTASKINFO dt_usutaskinfo = stUsualTaskList.astNewUsualTask[i];
                if (dt_usutaskinfo.bIsNew == 1)
                {
                    this.model.Remove(dt_usutaskinfo.dwTaskID);
                    CTask task = TaskUT.Create_Task(dt_usutaskinfo.dwTaskID);
                    if (task != null)
                    {
                        task.SetState(CTask.State.NewRefresh);
                        TaskUT.Add_Task(task);
                    }
                }
            }
            this.UpdateTaskState();
            Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
            if (this.m_taskView != null)
            {
                this.m_taskView.Refresh();
            }
        }

        public static void Send_Share_Task()
        {
            if (Singleton<CTaskSys>.instance.model.share_task_id > 0)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x5e1);
                msg.stPkgData.stClientReportTaskDone.dwTaskID = Singleton<CTaskSys>.instance.model.share_task_id;
                msg.stPkgData.stClientReportTaskDone.bEventType = 0;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
        }

        public void SendReport()
        {
            if (!this.IsReportAllZero())
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10d3);
                msg.stPkgData.stShareTLogReq.dwSecretaryNum = 7;
                for (int i = 0; i < this.reportInts.Length; i++)
                {
                    msg.stPkgData.stShareTLogReq.SecretaryDetail[i] = this.reportInts[i];
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
        }

        public void SetCardExpireTime(RES_PROP_VALFUNC_TYPE funcType, uint expireTime)
        {
            if (funcType == RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_MONTH_CARD)
            {
                this._monthCardExpireTime = expireTime;
            }
            else if (funcType == RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_WEEK_CARD)
            {
                this._weekCardExpireTime = expireTime;
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTaskForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseTaskForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_LinkPve, new CUIEventManager.OnUIEventHandler(this.OnLinkPveClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_TabChanged, new CUIEventManager.OnUIEventHandler(this.OnTaskChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_AwardClose, new CUIEventManager.OnUIEventHandler(this.OnTaskAwardClose));
            base.UnInit();
        }

        private void UpdateTaskState()
        {
            this.model.UpdateTaskState();
        }

        public uint monthCardExpireTime
        {
            get
            {
                return this._monthCardExpireTime;
            }
        }

        public uint monthCardTaskID
        {
            get
            {
                if (this._monthCardTaskID == 0)
                {
                    this._monthCardTaskID = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xa3).dwConfValue;
                }
                return this._monthCardTaskID;
            }
        }

        public uint weekCardExpireTime
        {
            get
            {
                return this._weekCardExpireTime;
            }
        }

        public uint weekCardTaskID
        {
            get
            {
                if (this._weekCardTaskID == 0)
                {
                    this._weekCardTaskID = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xa4).dwConfValue;
                }
                return this._weekCardTaskID;
            }
        }

        public enum SECRETARY_REPORT_INT_TYPE
        {
            SECRETARY_REPORT_INT_TYPE_CREATEKINGTIMEVIDEO = 6,
            SECRETARY_REPORT_INT_TYPE_MAX = 7,
            SECRETARY_REPORT_INT_TYPE_OPENRECORDKINGTIME = 5
        }
    }
}

