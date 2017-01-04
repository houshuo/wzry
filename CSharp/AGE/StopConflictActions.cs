namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [EventCategory("ActionControl")]
    public class StopConflictActions : TickEvent
    {
        private int[] gameObjectIds = new int[0];

        public override BaseEvent Clone()
        {
            StopConflictActions actions = ClassObjPool<StopConflictActions>.Get();
            actions.CopyData(this);
            return actions;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            StopConflictActions actions = src as StopConflictActions;
            this.gameObjectIds = actions.gameObjectIds;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.gameObjectIds = new int[0];
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (int num in this.gameObjectIds)
            {
                list.Add(_action.GetGameObject(num));
            }
            ListView<AGE.Action> view = new ListView<AGE.Action>();
            foreach (GameObject obj2 in list)
            {
                foreach (AGE.Action action in ActionManager.Instance.objectReferenceSet[obj2])
                {
                    if ((action != _action) && !action.unstoppable)
                    {
                        view.Add(action);
                    }
                }
            }
            foreach (AGE.Action action2 in view)
            {
                action2.Stop(false);
            }
        }
    }
}

