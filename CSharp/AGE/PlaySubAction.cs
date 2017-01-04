namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("Utility")]
    public class PlaySubAction : DurationEvent
    {
        [AssetReference(AssetRefType.Action)]
        public string actionName = string.Empty;
        public int[] gameObjectIds = new int[0];
        private GameObject[] gameObjects;
        private AGE.Action subAction;

        public override BaseEvent Clone()
        {
            PlaySubAction action = ClassObjPool<PlaySubAction>.Get();
            action.CopyData(this);
            return action;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PlaySubAction action = src as PlaySubAction;
            this.actionName = action.actionName;
            this.gameObjectIds = action.gameObjectIds;
            this.gameObjects = action.gameObjects;
            this.subAction = action.subAction;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            if (this.subAction != null)
            {
                this.subAction.Stop(false);
            }
            if (this.gameObjects == null)
            {
                this.gameObjects = new GameObject[this.gameObjectIds.Length];
                for (int i = 0; i < this.gameObjectIds.Length; i++)
                {
                    this.gameObjects[i] = _action.GetGameObject(this.gameObjectIds[i]);
                }
            }
            this.subAction = ActionManager.Instance.PlaySubAction(_action, this.actionName, (float) base.length, this.gameObjects);
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
            this.gameObjectIds = new int[0];
            this.gameObjects = null;
            this.subAction = null;
        }
    }
}

