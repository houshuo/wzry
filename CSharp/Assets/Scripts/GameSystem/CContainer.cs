namespace Assets.Scripts.GameSystem
{
    using System;

    public class CContainer
    {
        protected enCONTAINER_TYPE m_type = enCONTAINER_TYPE.UNKNOWN;

        public enCONTAINER_TYPE GetContainerType()
        {
            return this.m_type;
        }

        public virtual void Init(enCONTAINER_TYPE type)
        {
        }
    }
}

