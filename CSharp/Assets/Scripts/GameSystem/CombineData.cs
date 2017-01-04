namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;

    public class CombineData
    {
        public DictionaryView<uint, CTask> task_map = new DictionaryView<uint, CTask>();
        public ListView<CTask>[] type_taskList_array = new ListView<CTask>[3];

        public CombineData()
        {
            this.type_taskList_array[0] = new ListView<CTask>();
            this.type_taskList_array[1] = new ListView<CTask>();
            this.type_taskList_array[2] = new ListView<CTask>();
        }

        private int _getIndex(CTask task)
        {
            if (task == null)
            {
                return -1;
            }
            ListView<CTask> listView = this.GetListView(task.m_taskType);
            if (listView == null)
            {
                return -1;
            }
            return listView.IndexOf(task);
        }

        private void _insert(uint uid, CTask task)
        {
            if (((uid != 0) && (task != null)) && !this.task_map.ContainsKey(uid))
            {
                this.task_map.Add(uid, task);
                ListView<CTask> listView = this.GetListView(task.m_taskType);
                if (listView != null)
                {
                    listView.Add(task);
                }
            }
        }

        private void _remove(uint uid)
        {
            CTask task;
            this.task_map.TryGetValue(uid, out task);
            if (task != null)
            {
                this.task_map.Remove(uid);
                ListView<CTask> listView = this.GetListView(task.m_taskType);
                if (listView != null)
                {
                    listView.Remove(task);
                }
            }
        }

        private int _sort_main(CTask l, CTask r)
        {
            if (l == r)
            {
                return 0;
            }
            if ((l == null) || (r == null))
            {
                return 0;
            }
            if (r.m_taskState == 1)
            {
                if (l.m_taskState == 1)
                {
                    return (int) (r.m_baseId - l.m_baseId);
                }
                return 1;
            }
            if (r.m_taskState == 0)
            {
                if (l.m_taskState == 1)
                {
                    return -1;
                }
                if (l.m_taskState == 0)
                {
                    return (int) (r.m_baseId - l.m_baseId);
                }
                return 1;
            }
            if (r.m_taskState != 3)
            {
                return -1;
            }
            if (l.m_taskState != 3)
            {
                return -1;
            }
            return (int) (r.m_baseId - l.m_baseId);
        }

        public void Add(uint uid, CTask task)
        {
            this._insert(uid, task);
        }

        public void Clear()
        {
            this.task_map.Clear();
            for (int i = 0; i < this.type_taskList_array.Length; i++)
            {
                ListView<CTask> view = this.type_taskList_array[i];
                if (view != null)
                {
                    view.Clear();
                }
            }
        }

        public ListView<CTask> GetListView(RES_TASK_TYPE type)
        {
            if ((type >= RES_TASK_TYPE.RES_TASKTYPE_MAIN) && (type < RES_TASK_TYPE.RES_TASKTYPE_MAX))
            {
                return this.type_taskList_array[(int) ((IntPtr) type)];
            }
            return null;
        }

        public ListView<CTask> GetListView(uint type)
        {
            if ((type >= 0) && (type < 3))
            {
                return this.type_taskList_array[type];
            }
            return null;
        }

        public CTask GetMaxIndex_TaskID_InState(RES_TASK_TYPE type, CTask.State state)
        {
            CTask task = null;
            uint dwMiShuIndex = 0;
            CTask task2 = null;
            ListView<CTask> listView = this.GetListView(type);
            if (listView != null)
            {
                for (int i = 0; i < listView.Count; i++)
                {
                    task2 = listView[i];
                    if (((task2 != null) && (task2.m_taskState == ((byte) state))) && (task2.m_resTask.dwMiShuIndex > dwMiShuIndex))
                    {
                        task = task2;
                        dwMiShuIndex = task.m_resTask.dwMiShuIndex;
                    }
                }
            }
            return task;
        }

        public CTask GetTask(uint uid)
        {
            CTask task;
            this.task_map.TryGetValue(uid, out task);
            return task;
        }

        public int GetTask_Count(RES_TASK_TYPE type, CTask.State state)
        {
            CTask task = null;
            int num = 0;
            ListView<CTask> listView = this.GetListView(type);
            if (listView != null)
            {
                for (int i = 0; i < listView.Count; i++)
                {
                    task = listView[i];
                    if ((task != null) && (task.m_taskState == ((byte) state)))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public void Remove(uint uid)
        {
            if (uid != 0)
            {
                this._remove(uid);
            }
        }

        public void Sort(RES_TASK_TYPE type)
        {
            ListView<CTask> listView = this.GetListView(type);
            if (listView != null)
            {
                listView.Sort(new Comparison<CTask>(this._sort_main));
            }
        }
    }
}

