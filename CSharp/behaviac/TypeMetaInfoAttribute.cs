namespace behaviac
{
    using System;

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class TypeMetaInfoAttribute : Attribute
    {
        private string desc_;
        private string displayName_;

        public TypeMetaInfoAttribute()
        {
        }

        public TypeMetaInfoAttribute(string displayName, string description)
        {
            this.displayName_ = displayName;
            this.desc_ = description;
        }

        public string Description
        {
            get
            {
                return this.desc_;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName_;
            }
        }
    }
}

