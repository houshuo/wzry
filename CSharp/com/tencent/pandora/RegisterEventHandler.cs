namespace com.tencent.pandora
{
    using System;
    using System.Reflection;

    internal class RegisterEventHandler
    {
        private EventInfo eventInfo;
        private EventHandlerContainer pendingEvents;
        private object target;

        public RegisterEventHandler(EventHandlerContainer pendingEvents, object target, EventInfo eventInfo)
        {
            this.target = target;
            this.eventInfo = eventInfo;
            this.pendingEvents = pendingEvents;
        }

        public Delegate Add(LuaFunction function)
        {
            Delegate handler = CodeGeneration.Instance.GetDelegate(this.eventInfo.EventHandlerType, function);
            this.eventInfo.AddEventHandler(this.target, handler);
            this.pendingEvents.Add(handler, this);
            return handler;
        }

        public void Remove(Delegate handlerDelegate)
        {
            this.RemovePending(handlerDelegate);
            this.pendingEvents.Remove(handlerDelegate);
        }

        internal void RemovePending(Delegate handlerDelegate)
        {
            this.eventInfo.RemoveEventHandler(this.target, handlerDelegate);
        }
    }
}

