namespace com.tencent.pandora
{
    using System;
    using UnityEngine;

    public class Notification
    {
        public object data;
        public string name;
        public Component sender;

        public Notification(Component aSender, string aName)
        {
            this.sender = aSender;
            this.name = aName;
            this.data = null;
        }

        public Notification(Component aSender, string aName, object aData)
        {
            this.sender = aSender;
            this.name = aName;
            this.data = aData;
        }
    }
}

