namespace Assets.Scripts.Framework
{
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class GameTaskGroup : GameTask
    {
        private ListView<TaskNode> _taskList;

        internal void _OnChildClosed(GameTask et)
        {
            int num = 0;
            int num2 = 0;
            int num3 = -1;
            bool flag = false;
            for (int i = 0; i < this._taskList.Count; i++)
            {
                TaskNode node = this._taskList[i];
                if (node.task.Closed)
                {
                    num2++;
                    if (node.task.Achieving == node.achieve)
                    {
                        num++;
                    }
                }
                if (node.task == et)
                {
                    num3 = i;
                    flag = et.Achieving == node.achieve;
                }
            }
            if (num3 != -1)
            {
                base.Current = num;
                if (!base.Closed)
                {
                    if ((num2 >= this._taskList.Count) || (et.Closed && ((flag && (this.CloseType == RES_GAME_TASK_GROUP_CLOSE_TYPE.ACHIEVE_ONE)) || (!flag && (this.CloseType == RES_GAME_TASK_GROUP_CLOSE_TYPE.ACHIEVE_ALL)))))
                    {
                        base.Close();
                    }
                    else if ((this.StartType == RES_GAME_TASK_GROUP_START_TYPE.ONEBYONE) && (++num3 < this._taskList.Count))
                    {
                        TaskNode node2 = this._taskList[num3];
                        node2.task.Start();
                    }
                }
            }
        }

        protected override void OnInitial()
        {
            ListView<ResGameTaskGroup> groupData = base.RootSys.GetGroupData(base.ID);
            DebugHelper.Assert(null != groupData, "GameTaskGroup.groupData must not be null!");
            this._taskList = new ListView<TaskNode>();
            if (groupData != null)
            {
                for (int i = 0; i < groupData.Count; i++)
                {
                    ResGameTaskGroup group = groupData[i];
                    TaskNode item = new TaskNode {
                        task = base.RootSys.GetTask(group.dwChildTask, true),
                        achieve = group.bIsAchieve == 0
                    };
                    this._taskList.Add(item);
                    if (item.task != null)
                    {
                        item.task._AddOwnerGroup(this);
                    }
                }
            }
            DebugHelper.Assert(this._taskList.Count > 0, "GameTaskGroup.taskList.Count must > 0!");
        }

        protected override void OnStart()
        {
            if (this._taskList.Count >= 1)
            {
                if (this.StartType == RES_GAME_TASK_GROUP_START_TYPE.ONEBYONE)
                {
                    TaskNode node = this._taskList[0];
                    node.task.Start();
                }
                else
                {
                    for (int i = 0; i < this._taskList.Count; i++)
                    {
                        TaskNode node2 = this._taskList[i];
                        node2.task.Start();
                    }
                }
            }
        }

        public GameTask ActiveChild
        {
            get
            {
                for (int i = 0; i < this._taskList.Count; i++)
                {
                    TaskNode node = this._taskList[i];
                    GameTask task = node.task;
                    if (task.Active)
                    {
                        return task;
                    }
                }
                return null;
            }
        }

        public RES_GAME_TASK_GROUP_CLOSE_TYPE CloseType
        {
            get
            {
                return (RES_GAME_TASK_GROUP_CLOSE_TYPE) base.Config.iParam2;
            }
        }

        public override bool IsGroup
        {
            get
            {
                return true;
            }
        }

        public RES_GAME_TASK_GROUP_START_TYPE StartType
        {
            get
            {
                return (RES_GAME_TASK_GROUP_START_TYPE) base.Config.iParam1;
            }
        }

        public override int Target
        {
            get
            {
                return ((this.CloseType != RES_GAME_TASK_GROUP_CLOSE_TYPE.ACHIEVE_ONE) ? this._taskList.Count : 1);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TaskNode
        {
            public GameTask task;
            public bool achieve;
        }
    }
}

