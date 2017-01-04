namespace Assets.Scripts.Framework
{
    using AGE;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class GameTaskSys
    {
        private ActionHelper _actionHelper;
        private string _classSpace;
        private DictionaryView<uint, ListView<ResGameTaskGroup>> _groupData;
        private DatabinTable<ResGameTask, uint> _taskDB;
        private DictionaryView<uint, GameTask> _taskDict;
        private DatabinTable<ResGameTaskGroup, uint> _taskGroupDB;

        public event TaskEventDelegate OnTaskClose;

        public event TaskEventDelegate OnTaskGoing;

        public event TaskEventDelegate OnTaskReady;

        public event TaskEventDelegate OnTaskStart;

        internal void _OnTaskClose(GameTask gt)
        {
            if (null != this._actionHelper)
            {
                this._actionHelper.PlayAction(gt.CloseAction);
            }
            if (this.OnTaskClose != null)
            {
                this.OnTaskClose(gt);
            }
        }

        internal void _OnTaskGoing(GameTask gt)
        {
            if (this.OnTaskGoing != null)
            {
                this.OnTaskGoing(gt);
            }
        }

        internal void _OnTaskReady(GameTask gt)
        {
            if (this.OnTaskReady != null)
            {
                this.OnTaskReady(gt);
            }
        }

        internal void _OnTaskStart(GameTask gt)
        {
            if (null != this._actionHelper)
            {
                this._actionHelper.PlayAction(gt.StartAction);
            }
            if (this.OnTaskStart != null)
            {
                this.OnTaskStart(gt);
            }
        }

        public GameTask AddTask(uint taskId, bool autoStart)
        {
            if (this._taskDict.ContainsKey(taskId))
            {
                return this._taskDict[taskId];
            }
            GameTask task = this.CreateTask(taskId);
            if (task != null)
            {
                this._taskDict.Add(taskId, task);
                if (autoStart)
                {
                    task.Start();
                }
            }
            return task;
        }

        public void Clear()
        {
            if (this._taskDict != null)
            {
                DictionaryView<uint, GameTask>.Enumerator enumerator = this._taskDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, GameTask> current = enumerator.Current;
                    current.Value.Destroy();
                }
            }
        }

        private GameTask CreateTask(uint taskId)
        {
            ResGameTask dataByKey = this._taskDB.GetDataByKey(taskId);
            if (dataByKey == null)
            {
                return null;
            }
            GameTask task2 = null;
            string str = Utility.UTF8Convert(dataByKey.szType);
            if ("Group" == str)
            {
                task2 = new GameTaskGroup();
            }
            else
            {
                task2 = (GameTask) Assembly.GetExecutingAssembly().CreateInstance(this._classSpace + ".GameTask" + str);
            }
            task2.Initial(dataByKey, this);
            return task2;
        }

        internal ListView<ResGameTaskGroup> GetGroupData(uint groupTaskId)
        {
            if (this._groupData.ContainsKey(groupTaskId))
            {
                return this._groupData[groupTaskId];
            }
            return null;
        }

        public GameTask GetTask(uint taskId, bool addIfNone = false)
        {
            if (this._taskDict.ContainsKey(taskId))
            {
                return this._taskDict[taskId];
            }
            if (addIfNone)
            {
                return this.AddTask(taskId, false);
            }
            return null;
        }

        public void Initial(string classSpace, DatabinTable<ResGameTask, uint> taskDB, DatabinTable<ResGameTaskGroup, uint> taskGroupDB, ActionHelper actionHelper)
        {
            this._classSpace = classSpace;
            this._taskDB = taskDB;
            this._taskGroupDB = taskGroupDB;
            this._actionHelper = actionHelper;
            this._groupData = new DictionaryView<uint, ListView<ResGameTaskGroup>>();
            this._taskGroupDB.Accept(new Action<ResGameTaskGroup>(this.OnVisit));
            this._taskDict = new DictionaryView<uint, GameTask>();
        }

        private void OnVisit(ResGameTaskGroup InGroup)
        {
            ListView<ResGameTaskGroup> view = null;
            if (this._groupData.ContainsKey(InGroup.dwGroupTask))
            {
                view = this._groupData[InGroup.dwGroupTask];
            }
            else
            {
                view = new ListView<ResGameTaskGroup>();
                this._groupData.Add(InGroup.dwGroupTask, view);
            }
            view.Add(InGroup);
        }

        public bool HasTask
        {
            get
            {
                return (this._taskDict.Count > 0);
            }
        }

        public delegate void TaskEventDelegate(GameTask gt);
    }
}

