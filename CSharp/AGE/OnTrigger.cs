namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    [EventCategory("Condition")]
    public class OnTrigger : DurationCondition
    {
        public string methodName = "GetCollisionSet";
        public string scriptName = "TriggerHelper";
        public string[] tags = new string[0];
        [ObjectTemplate(new System.Type[] { typeof(Collider) })]
        public int targetId;

        public override bool Check(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if (gameObject != null)
            {
                Component target = gameObject.GetComponent(this.scriptName);
                if (target == null)
                {
                    return false;
                }
                System.Type type = target.GetType();
                object[] args = new object[0];
                List<GameObject> list = type.InvokeMember(this.methodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, target, args) as List<GameObject>;
                if (this.tags.Length > 0)
                {
                    foreach (GameObject obj3 in list)
                    {
                        if (obj3 != null)
                        {
                            foreach (string str in this.tags)
                            {
                                if (obj3.tag == str)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (GameObject obj4 in list)
                    {
                        if (obj4 != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override BaseEvent Clone()
        {
            OnTrigger trigger = ClassObjPool<OnTrigger>.Get();
            trigger.CopyData(this);
            return trigger;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            OnTrigger trigger = src as OnTrigger;
            this.targetId = trigger.targetId;
            this.scriptName = trigger.scriptName;
            this.methodName = trigger.methodName;
            this.tags = trigger.tags;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.scriptName = "TriggerHelper";
            this.methodName = "GetCollisionSet";
            this.tags = new string[0];
        }
    }
}

