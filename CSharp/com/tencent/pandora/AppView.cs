namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class AppView : View
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        private string message;

        private void Awake()
        {
            base.RemoveMessage(this, this.MessageList);
            base.RegisterMessage(this, this.MessageList);
        }

        public override void OnMessage(IMessage message)
        {
            string name = message.Name;
            object body = message.Body;
            string key = name;
            if (key != null)
            {
                int num;
                if (<>f__switch$map0 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                    dictionary.Add("UpdateMessage", 0);
                    dictionary.Add("UpdateExtract", 1);
                    dictionary.Add("UpdateDownload", 2);
                    dictionary.Add("UpdateProgress", 3);
                    <>f__switch$map0 = dictionary;
                }
                if (<>f__switch$map0.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            this.UpdateMessage(body.ToString());
                            break;

                        case 1:
                            this.UpdateExtract(body.ToString());
                            break;

                        case 2:
                            this.UpdateDownload(body.ToString());
                            break;

                        case 3:
                            this.UpdateProgress(body.ToString());
                            break;
                    }
                }
            }
        }

        public void UpdateDownload(string data)
        {
            this.message = data;
        }

        public void UpdateExtract(string data)
        {
            this.message = data;
        }

        public void UpdateMessage(string data)
        {
            this.message = data;
        }

        public void UpdateProgress(string data)
        {
            this.message = data;
        }

        private List<string> MessageList
        {
            get
            {
                return new List<string> { "UpdateMessage", "UpdateExtract", "UpdateDownload", "UpdateProgress" };
            }
        }
    }
}

