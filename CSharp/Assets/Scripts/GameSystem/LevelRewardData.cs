namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;

    public class LevelRewardData
    {
        public ResDT_LevelReward_UnlockInfo[] astLockInfo = new ResDT_LevelReward_UnlockInfo[UNLOCK_MAX_COUNT];
        public bool m_bHasGetReward;
        public int m_level;
        public ResPvpLevelReward m_resLevelReward;
        public static int REWARD_MAX_COUNT = 2;
        public ResTask[] task_ids = new ResTask[TASK_MAX_COUNT];
        public static int TASK_MAX_COUNT = 2;
        public static int TASK_REWARD_MAX_COUNT = 2;
        public static int UNLOCK_MAX_COUNT = 2;

        public void Clear()
        {
            this.m_resLevelReward = null;
            for (int i = 0; i < this.astLockInfo.Length; i++)
            {
                this.astLockInfo[i] = null;
            }
            for (int j = 0; j < this.task_ids.Length; j++)
            {
                this.task_ids[j] = null;
            }
        }

        public int GetConfigRewardCount()
        {
            if (this.m_resLevelReward == null)
            {
                return 0;
            }
            int num = 0;
            for (int i = 0; i < this.m_resLevelReward.astRewardInfo.Length; i++)
            {
                ResDT_LevelReward_Info info = this.m_resLevelReward.astRewardInfo[i];
                if (info.dwRewardNum != 0)
                {
                    num++;
                }
            }
            return num;
        }

        public int GetFristNullResTaskIndex()
        {
            for (int i = 0; i < this.task_ids.Length; i++)
            {
                if (this.task_ids[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetHaveDoneTaskCount()
        {
            int num = 0;
            for (int i = 0; i < this.task_ids.Length; i++)
            {
                ResTask task = this.task_ids[i];
                if (task != null)
                {
                    CTask task2 = Singleton<CTaskSys>.instance.model.GetTask(task.dwTaskID);
                    if (((task2 != null) && (task2.m_taskSubType != 0)) && (task2.m_taskState == 1))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public int GetResTaskIDIndex(uint id)
        {
            for (int i = 0; i < this.task_ids.Length; i++)
            {
                ResTask task = this.task_ids[i];
                if ((task != null) && (task.dwTaskID == id))
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetValidTaskCount()
        {
            int num = 0;
            for (int i = 0; i < this.task_ids.Length; i++)
            {
                ResTask task = this.task_ids[i];
                if ((task != null) && (Singleton<CTaskSys>.instance.model.GetTask(task.dwTaskID) != null))
                {
                    num++;
                }
            }
            return num;
        }

        public bool IsAllLevelTask()
        {
            for (int i = 0; i < this.task_ids.Length; i++)
            {
                ResTask task = this.task_ids[i];
                if (((task != null) && (task != null)) && (task.bTaskSubType != 0))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsConfigTaskAllEmpty()
        {
            for (int i = 0; i < this.task_ids.Length; i++)
            {
                if (this.task_ids[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsEqual(LevelRewardData data)
        {
            if (data == null)
            {
                return false;
            }
            if ((this.m_resLevelReward != null) && (data.m_resLevelReward != null))
            {
                return ((this.m_resLevelReward.iLevel == data.m_resLevelReward.iLevel) && (this.m_level == data.m_level));
            }
            return (((this.m_resLevelReward == null) && (data.m_resLevelReward == null)) && (this.m_level == data.m_level));
        }

        public bool IsHasCltCalcCompeletedTask()
        {
            for (int i = 0; i < this.task_ids.Length; i++)
            {
                ResTask task = this.task_ids[i];
                if ((task != null) && Singleton<CTaskSys>.instance.model.IsInCltCalcCompletedTasks(task.dwTaskID))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

