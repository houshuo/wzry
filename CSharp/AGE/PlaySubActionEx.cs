namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class PlaySubActionEx : DurationEvent
    {
        [AssetReference(AssetRefType.Action)]
        public string actionName = string.Empty;
        public bool pushGameObjs;
        public bool pushRefParams;
        private AGE.Action subAction;

        public override BaseEvent Clone()
        {
            PlaySubActionEx ex = ClassObjPool<PlaySubActionEx>.Get();
            ex.CopyData(this);
            return ex;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PlaySubActionEx ex = src as PlaySubActionEx;
            this.actionName = ex.actionName;
            this.pushGameObjs = ex.pushGameObjs;
            this.pushRefParams = ex.pushRefParams;
            this.subAction = ex.subAction;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            if (this.subAction != null)
            {
                this.subAction.Stop(false);
            }
            if (this.pushGameObjs)
            {
                this.subAction = ActionManager.Instance.PlaySubAction(_action, this.actionName, (float) base.length, _action.GetGameObjectList().ToArray());
            }
            else
            {
                this.subAction = ActionManager.Instance.PlaySubAction(_action, this.actionName, (float) base.length, new GameObject[0]);
            }
            if ((this.subAction != null) && this.pushRefParams)
            {
                this.subAction.InheritRefParams(_action);
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.subAction != null)
            {
                this.subAction.Stop(false);
                this.subAction = null;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.actionName = string.Empty;
            this.pushGameObjs = false;
            this.pushRefParams = false;
            this.subAction = null;
        }
    }
}

