namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class NotificationCenter : MonoBehaviour
    {
        public static bool blNewsOpen;
        public static bool blPLNewsOpen;
        private static NotificationCenter defaultCenter;
        private Hashtable notifications;

        public void AddObserver(Component observer, string name)
        {
            this.AddObserver(observer, name, null);
        }

        public void AddObserver(Component observer, string name, Component sender)
        {
            if ((name == null) || (name == string.Empty))
            {
                com.tencent.pandora.Logger.d("在AddObserver函数中指定的是空名称!.");
            }
            else
            {
                if (this.notifications[name] == null)
                {
                    this.notifications[name] = new List<Component>();
                }
                List<Component> list = this.notifications[name] as List<Component>;
                if (!list.Contains(observer))
                {
                    list.Add(observer);
                }
            }
        }

        private void Awake()
        {
            this.notifications = new Hashtable();
        }

        public static void Clean()
        {
        }

        public static NotificationCenter DefaultCenter()
        {
            if (Pandora.stopConnectAll)
            {
                return null;
            }
            if (null == defaultCenter)
            {
                GameObject obj2 = new GameObject("TPlay Notification Center");
                if (Pandora.GetInstance().gameObject != null)
                {
                    obj2.transform.parent = Pandora.GetInstance().gameObject.transform;
                }
                defaultCenter = obj2.AddComponent<NotificationCenter>();
            }
            return defaultCenter;
        }

        public void PostNotification(Notification aNotification)
        {
            if ((aNotification.name == null) || (aNotification.name == string.Empty))
            {
                com.tencent.pandora.Logger.d("Null name sent to PostNotification.");
            }
            else
            {
                List<Component> list = this.notifications[aNotification.name] as List<Component>;
                if (list == null)
                {
                    com.tencent.pandora.Logger.d("在PostNotification的通知列表中未找到: " + aNotification.name);
                }
                else
                {
                    List<Component> list2 = new List<Component>();
                    int count = list.Count;
                    for (int i = 0; i < list.Count; i++)
                    {
                        Component item = list[i];
                        if (item == null)
                        {
                            list2.Add(item);
                        }
                        else
                        {
                            item.SendMessage(aNotification.name, aNotification, SendMessageOptions.DontRequireReceiver);
                        }
                        if (count > list.Count)
                        {
                            i--;
                            count = list.Count;
                        }
                    }
                    foreach (Component component2 in list2)
                    {
                        list.Remove(component2);
                    }
                }
            }
        }

        public void PostNotification(Component aSender, string aName)
        {
            this.PostNotification(aSender, aName, null);
        }

        public void PostNotification(Component aSender, string aName, object aData)
        {
            this.PostNotification(new Notification(aSender, aName, aData));
        }

        public static void RemoveObserver(Component observer, string name)
        {
            if (defaultCenter != null)
            {
                List<Component> list = defaultCenter.notifications[name] as List<Component>;
                if (list != null)
                {
                    if (list.Contains(observer))
                    {
                        list.Remove(observer);
                    }
                    if (list.Count == 0)
                    {
                        defaultCenter.notifications.Remove(name);
                    }
                }
            }
        }
    }
}

