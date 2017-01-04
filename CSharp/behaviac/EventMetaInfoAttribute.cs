namespace behaviac
{
    using System;

    [AttributeUsage(AttributeTargets.Delegate, AllowMultiple=false, Inherited=false)]
    public class EventMetaInfoAttribute : MethodMetaInfoAttribute
    {
        public EventMetaInfoAttribute()
        {
        }

        public EventMetaInfoAttribute(string displayName, string description) : base(displayName, description)
        {
        }

        public override bool IsNamedEvent
        {
            get
            {
                return true;
            }
        }
    }
}

