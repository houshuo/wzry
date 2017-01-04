namespace Assets.Scripts.GameLogic
{
    using System;

    public class PassivetAttribute : Attribute
    {
        public int attributeType;

        public PassivetAttribute()
        {
        }

        public PassivetAttribute(int _type)
        {
            this.attributeType = _type;
        }
    }
}

