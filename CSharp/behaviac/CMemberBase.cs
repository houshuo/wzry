namespace behaviac
{
    using System;
    using System.Reflection;

    public class CMemberBase
    {
        private FieldInfo field_;
        private CStringID m_id = new CStringID();
        private string m_instaceName;
        private float m_range = 1f;

        public CMemberBase(FieldInfo f, MemberMetaInfoAttribute a)
        {
            this.field_ = f;
            this.m_id.SetId(this.field_.Name);
            if (a != null)
            {
                this.m_range = a.Range;
            }
            else
            {
                this.m_range = 1f;
            }
        }

        public virtual CMemberBase clone()
        {
            return null;
        }

        public virtual Property CreateProperty(string defaultValue, bool bConst)
        {
            Property property = new Property(this, bConst);
            if (!string.IsNullOrEmpty(defaultValue))
            {
                property.SetDefaultValue(defaultValue);
            }
            return property;
        }

        public virtual object Get(object agentFrom)
        {
            if (this.ISSTATIC())
            {
                return this.field_.GetValue(null);
            }
            return this.field_.GetValue(agentFrom);
        }

        public string GetClassNameString()
        {
            return this.field_.DeclaringType.FullName;
        }

        public CStringID GetId()
        {
            return this.m_id;
        }

        public string GetInstanceNameString()
        {
            return this.m_instaceName;
        }

        public string GetName()
        {
            return this.field_.Name;
        }

        public ParentType GetParentType()
        {
            return ParentType.PT_INVALID;
        }

        public float GetRange()
        {
            return this.m_range;
        }

        public virtual int GetTypeId()
        {
            return 0;
        }

        public bool ISSTATIC()
        {
            return this.field_.IsStatic;
        }

        public virtual void Load(Agent parent, ISerializableNode node)
        {
        }

        public virtual void Save(Agent parent, ISerializableNode node)
        {
        }

        public void Set(object objectFrom, object v)
        {
            this.field_.SetValue(objectFrom, v);
        }

        public void SetInstanceNameString(string name)
        {
            this.m_instaceName = name;
        }

        public Type MemberType
        {
            get
            {
                return this.field_.FieldType;
            }
        }
    }
}

