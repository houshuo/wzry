namespace behaviac
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
    public class MethodMetaInfoAttribute : TypeMetaInfoAttribute
    {
        public MethodMetaInfoAttribute()
        {
        }

        public MethodMetaInfoAttribute(string displayName, string description) : base(displayName, description)
        {
        }

        public virtual bool IsNamedEvent
        {
            get
            {
                return false;
            }
        }
    }
}

