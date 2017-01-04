namespace behaviac
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public class MemberMetaInfoAttribute : TypeMetaInfoAttribute
    {
        private float m_range;

        public MemberMetaInfoAttribute()
        {
            this.m_range = 1f;
        }

        public MemberMetaInfoAttribute(string displayName, string description) : this(displayName, description, 1f)
        {
        }

        public MemberMetaInfoAttribute(string displayName, string description, float range) : base(displayName, description)
        {
            this.m_range = 1f;
            this.m_range = range;
        }

        public static string GetEnumDescription(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            string description = getEnumName(obj);
            Attribute[] customAttributes = (Attribute[]) obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
            if (customAttributes.Length > 0)
            {
                description = ((MemberMetaInfoAttribute) customAttributes[0]).Description;
            }
            return description;
        }

        public static string GetEnumDisplayName(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            string displayName = getEnumName(obj);
            Attribute[] customAttributes = (Attribute[]) obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
            if (customAttributes.Length > 0)
            {
                displayName = ((MemberMetaInfoAttribute) customAttributes[0]).DisplayName;
            }
            return displayName;
        }

        private static string getEnumName(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            Type enumType = obj.GetType();
            if (!enumType.IsEnum)
            {
                return string.Empty;
            }
            string name = Enum.GetName(enumType, obj);
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            return name;
        }

        public float Range
        {
            get
            {
                return this.m_range;
            }
        }
    }
}

