namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;

    internal class EventHandlerContainer : IDisposable
    {
        private Dictionary<Delegate, RegisterEventHandler> dict = new Dictionary<Delegate, RegisterEventHandler>();

        public void Add(Delegate handler, RegisterEventHandler eventInfo)
        {
            this.dict.Add(handler, eventInfo);
        }

        public void Dispose()
        {
            foreach (KeyValuePair<Delegate, RegisterEventHandler> pair in this.dict)
            {
                pair.Value.RemovePending(pair.Key);
            }
            this.dict.Clear();
        }

        public void Remove(Delegate handler)
        {
            bool flag = this.dict.Remove(handler);
        }
    }
}

