namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CTaskModel
    {
        public string Daily_Quest_Career = string.Empty;
        public string Daily_Quest_NeedGrowing = string.Empty;
        public string Daily_Quest_NeedHero = string.Empty;
        public string Daily_Quest_NeedMoney = string.Empty;
        public string Daily_Quest_NeedSeal = string.Empty;
        public HuoyueData huoyue_data = new HuoyueData();
        public Dictionary<int, List<uint>> m_cltCalcCompletedTasks = new Dictionary<int, List<uint>>();
        private LevelRewardData m_curLevelRewardData;
        public ListView<LevelRewardData> m_levelRewardDataMap = new ListView<LevelRewardData>();
        private ulong m_levelRewardFlag;
        public uint share_task_id;
        public CombineData task_Data = new CombineData();

        public void AddTask(CTask task)
        {
            if (task != null)
            {
                this.task_Data.Add(task.m_baseId, task);
            }
        }

        public bool AnyTaskOfState(COM_TASK_STATE state, RES_TASK_TYPE taskType, out CTask outTask)
        {
            outTask = null;
            ListView<CTask> listView = this.task_Data.GetListView(taskType);
            if (listView != null)
            {
                for (int i = 0; i < listView.Count; i++)
                {
                    CTask task = listView[i];
                    if ((task != null) && (task.m_taskState == ((byte) state)))
                    {
                        outTask = task;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CalcNextRewardNode(int startIndex, out int nextlistIndex, out LevelRewardData nextData)
        {
            nextlistIndex = 0;
            nextData = null;
            ListView<LevelRewardData> levelRewardDataMap = Singleton<CTaskSys>.instance.model.m_levelRewardDataMap;
            for (int i = startIndex + 1; i < levelRewardDataMap.Count; i++)
            {
                LevelRewardData levelRewardData = levelRewardDataMap[i];
                if ((levelRewardData != null) && this.IsLevelNode_RedDot(levelRewardData))
                {
                    nextlistIndex = i;
                    nextData = levelRewardData;
                    return true;
                }
            }
            for (int j = 0; j < startIndex; j++)
            {
                LevelRewardData data2 = levelRewardDataMap[j];
                if ((data2 != null) && this.IsLevelNode_RedDot(data2))
                {
                    nextlistIndex = j;
                    nextData = data2;
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            this.task_Data.Clear();
            this.huoyue_data.Clear();
        }

        public int GetLevelIndex(int level)
        {
            for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
            {
                LevelRewardData data = this.m_levelRewardDataMap[i];
                if (data.m_level == level)
                {
                    return i;
                }
            }
            return -1;
        }

        public LevelRewardData GetLevelRewardData(int level)
        {
            LevelRewardData data = null;
            for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
            {
                data = this.m_levelRewardDataMap[i];
                if ((data != null) && (data.m_level == level))
                {
                    return data;
                }
            }
            return null;
        }

        public LevelRewardData GetLevelRewardData_ByIndex(int index)
        {
            DebugHelper.Assert(index < this.m_levelRewardDataMap.Count, "CTaskModel GetLevelRewardData_ByIndex, index > count, check out...");
            if (index < this.m_levelRewardDataMap.Count)
            {
                return this.m_levelRewardDataMap[index];
            }
            return null;
        }

        public int GetLevelRewardData_Index(LevelRewardData data)
        {
            if (data != null)
            {
                for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
                {
                    LevelRewardData data2 = this.m_levelRewardDataMap[i];
                    if (data2 == data)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int GetMainTask_RedDotCount()
        {
            int num = 0;
            for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
            {
                LevelRewardData data = this.m_levelRewardDataMap[i];
                if (data != null)
                {
                    bool flag = false;
                    if (!Singleton<CTaskSys>.instance.model.IsGetLevelReward(data.m_level) && (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= data.m_level))
                    {
                        flag = true;
                    }
                    if (flag || (data.GetHaveDoneTaskCount() > 0))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public CTask GetMaxIndex_TaskID_InState(RES_TASK_TYPE type, CTask.State state)
        {
            return this.task_Data.GetMaxIndex_TaskID_InState(type, state);
        }

        public CTask GetTask(uint TaskId)
        {
            return this.task_Data.GetTask(TaskId);
        }

        public ListView<CTask> GetTasks(RES_TASK_TYPE type)
        {
            return this.task_Data.GetListView(type);
        }

        public int GetTasks_Count(RES_TASK_TYPE type, CTask.State state)
        {
            ListView<CTask> listView = this.task_Data.GetListView(type);
            DebugHelper.Assert(listView != null);
            if (listView == null)
            {
                return 0;
            }
            int num = 0;
            for (int i = 0; i < listView.Count; i++)
            {
                CTask task = listView[i];
                if ((task != null) && (task.m_taskState == ((byte) state)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetTotalTaskOfState(RES_TASK_TYPE type, COM_TASK_STATE state)
        {
            ListView<CTask> listView = this.task_Data.GetListView(type);
            if (listView == null)
            {
                return 0;
            }
            int num = 0;
            for (int i = 0; i < listView.Count; i++)
            {
                if (((COM_TASK_STATE) listView[i].m_taskState) == state)
                {
                    num++;
                }
            }
            return num;
        }

        private void InsertCltCalcCompletedTasks(uint taskid, int task_sub_type)
        {
            List<uint> list = null;
            if (this.m_cltCalcCompletedTasks.ContainsKey(task_sub_type))
            {
                this.m_cltCalcCompletedTasks.TryGetValue(task_sub_type, out list);
                if (list == null)
                {
                    list = new List<uint>();
                    this.m_cltCalcCompletedTasks[task_sub_type] = list;
                }
            }
            else
            {
                list = new List<uint>();
                this.m_cltCalcCompletedTasks.Add(task_sub_type, list);
            }
            if (!list.Contains(taskid))
            {
                list.Add(taskid);
            }
        }

        public bool IsAnyTaskInState(RES_TASK_TYPE type, CTask.State state)
        {
            ListView<CTask> listView = this.task_Data.GetListView(type);
            DebugHelper.Assert(listView != null);
            if (listView != null)
            {
                for (int i = 0; i < listView.Count; i++)
                {
                    CTask task = listView[i];
                    if ((task != null) && (task.m_taskState == ((byte) state)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsGetLevelReward(int level)
        {
            return this.IsGetLevelReward(this.m_levelRewardFlag, level);
        }

        private bool IsGetLevelReward(ulong flagdata, int level)
        {
            ulong num = ((ulong) 1L) << (level - 1);
            return ((flagdata & num) != 0L);
        }

        public bool IsInCltCalcCompletedTasks(uint taskid)
        {
            Dictionary<int, List<uint>>.Enumerator enumerator = this.m_cltCalcCompletedTasks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, List<uint>> current = enumerator.Current;
                List<uint> list = current.Value;
                if ((list != null) && list.Contains(taskid))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsInCltCalcCompletedTasks(uint taskid, int task_type)
        {
            if (taskid == 0)
            {
                return false;
            }
            List<uint> list = null;
            this.m_cltCalcCompletedTasks.TryGetValue(task_type, out list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(taskid);
        }

        public bool IsLevelNode_RedDot(LevelRewardData levelRewardData)
        {
            if (levelRewardData != null)
            {
                bool flag = false;
                if (!Singleton<CTaskSys>.instance.model.IsGetLevelReward(levelRewardData.m_level) && (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= levelRewardData.m_level))
                {
                    flag = true;
                }
                if (flag || (levelRewardData.GetHaveDoneTaskCount() > 0))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsShowMainTaskTab_RedDotCount()
        {
            for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
            {
                LevelRewardData levelRewardData = this.m_levelRewardDataMap[i];
                if ((levelRewardData != null) && this.IsLevelNode_RedDot(levelRewardData))
                {
                    return true;
                }
            }
            return false;
        }

        public void Load_Share_task()
        {
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.taskDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResTask task = (ResTask) current.Value;
                if (task.astPrerequisiteArray[0].dwPrerequisiteType == 0x13)
                {
                    this.share_task_id = task.dwTaskID;
                    break;
                }
            }
        }

        public void Load_Task_Tab_String()
        {
            this.Daily_Quest_Career = UT.GetText("Daily_Quest_Career");
            this.Daily_Quest_NeedGrowing = UT.GetText("Daily_Quest_NeedGrowing");
            this.Daily_Quest_NeedMoney = UT.GetText("Daily_Quest_NeedMoney");
            this.Daily_Quest_NeedSeal = UT.GetText("Daily_Quest_NeedSeal");
            this.Daily_Quest_NeedHero = UT.GetText("Daily_Quest_NeedHero");
        }

        public void ParseCltCalcCompletedTasks(ref uint[] taskids)
        {
            Dictionary<int, List<uint>>.Enumerator enumerator = this.m_cltCalcCompletedTasks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, List<uint>> current = enumerator.Current;
                List<uint> list = current.Value;
                if (list != null)
                {
                    list.Clear();
                }
            }
            this.m_cltCalcCompletedTasks.Clear();
            Dictionary<byte, uint> dictionary = new Dictionary<byte, uint>();
            for (int i = 0; i < taskids.Length; i++)
            {
                uint key = taskids[i];
                if (key != 0)
                {
                    ResTask dataByKey = GameDataMgr.taskDatabin.GetDataByKey(key);
                    object[] inParameters = new object[] { key };
                    DebugHelper.Assert(dataByKey != null, "ParseCltCalcCompletedTasks, taskDatabin.GetDataByKey({0}) is null", inParameters);
                    if (dataByKey != null)
                    {
                        if (dictionary.ContainsKey(dataByKey.bTaskSubType))
                        {
                            if (key < dictionary[dataByKey.bTaskSubType])
                            {
                                dictionary[dataByKey.bTaskSubType] = key;
                            }
                        }
                        else
                        {
                            dictionary.Add(dataByKey.bTaskSubType, key);
                        }
                    }
                }
            }
            Dictionary<long, object>.Enumerator enumerator2 = GameDataMgr.taskDatabin.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                KeyValuePair<long, object> pair2 = enumerator2.Current;
                ResTask task2 = (ResTask) pair2.Value;
                if (((task2 != null) && (task2.dwTaskType == 0)) && (dictionary.ContainsKey(task2.bTaskSubType) && (task2.dwTaskID < dictionary[task2.bTaskSubType])))
                {
                    this.InsertCltCalcCompletedTasks(task2.dwTaskID, task2.bTaskSubType);
                }
            }
            Dictionary<byte, uint>.Enumerator enumerator3 = dictionary.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                KeyValuePair<byte, uint> pair3 = enumerator3.Current;
                if (this.GetTask(pair3.Value) == null)
                {
                    KeyValuePair<byte, uint> pair4 = enumerator3.Current;
                    ResTask task4 = GameDataMgr.taskDatabin.GetDataByKey(pair4.Value);
                    this.InsertCltCalcCompletedTasks(task4.dwTaskID, task4.bTaskSubType);
                }
            }
        }

        public void ParseLevelRewardData()
        {
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.resPvpLevelRewardDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResPvpLevelReward reward = (ResPvpLevelReward) current.Value;
                if (reward != null)
                {
                    LevelRewardData item = null;
                    if (this.GetLevelRewardData(reward.iLevel) == null)
                    {
                        item = new LevelRewardData {
                            m_level = reward.iLevel,
                            m_resLevelReward = reward
                        };
                        DebugHelper.Assert(reward.astLockInfo.Length <= 2, "ParseLevelRewardData 等级奖励配置表配 解锁数量不该超过2个, 翔哥 check out...");
                        for (int i = 0; i < reward.astLockInfo.Length; i++)
                        {
                            ResDT_LevelReward_UnlockInfo info = reward.astLockInfo[i];
                            if (info != null)
                            {
                                item.astLockInfo[i] = info;
                            }
                        }
                        this.m_levelRewardDataMap.Add(item);
                    }
                }
            }
            Dictionary<long, object>.Enumerator enumerator2 = GameDataMgr.taskDatabin.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                KeyValuePair<long, object> pair2 = enumerator2.Current;
                ResTask task = (ResTask) pair2.Value;
                if ((task != null) && (task.dwOpenType == 2))
                {
                    int iParam = task.astOpenTaskParam[0].iParam;
                    LevelRewardData levelRewardData = this.GetLevelRewardData(iParam);
                    if ((levelRewardData != null) && (levelRewardData.GetResTaskIDIndex(task.dwTaskID) == -1))
                    {
                        int fristNullResTaskIndex = levelRewardData.GetFristNullResTaskIndex();
                        if (fristNullResTaskIndex != -1)
                        {
                            levelRewardData.task_ids[fristNullResTaskIndex] = task;
                        }
                    }
                }
            }
        }

        public void Remove(CTask task)
        {
            if (task != null)
            {
                this.task_Data.Remove(task.m_baseId);
            }
        }

        public void Remove(uint id)
        {
            this.task_Data.Remove(id);
        }

        public void SyncServerLevelRewardFlagData(ulong num)
        {
            this.m_levelRewardFlag = num;
            LevelRewardData data = null;
            for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
            {
                data = this.m_levelRewardDataMap[i];
                if (data != null)
                {
                    data.m_bHasGetReward = this.IsGetLevelReward(this.m_levelRewardFlag, data.m_level);
                    if (!data.m_bHasGetReward)
                    {
                    }
                }
            }
        }

        public void UpdateTaskState()
        {
            if (this.task_Data != null)
            {
                DictionaryView<uint, CTask>.Enumerator enumerator = this.task_Data.task_map.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, CTask> current = enumerator.Current;
                    CTask task = current.Value;
                    if (task != null)
                    {
                        task.UpdateState();
                    }
                }
            }
        }

        public LevelRewardData curLevelRewardData
        {
            get
            {
                return this.m_curLevelRewardData;
            }
            set
            {
                this.m_curLevelRewardData = value;
                CTaskView taskView = Singleton<CTaskSys>.instance.m_taskView;
                if (taskView != null)
                {
                    taskView.RefreshLevelList();
                }
                if (taskView != null)
                {
                    taskView.ShowLevelRightSide(this.curLevelRewardData);
                }
                if ((taskView != null) && (this.m_curLevelRewardData != null))
                {
                    taskView.MoveElementInScrollArea(Singleton<CTaskSys>.instance.model.GetLevelIndex(this.m_curLevelRewardData.m_level));
                }
            }
        }
    }
}

