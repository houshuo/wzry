namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public class PassiveConditionAttribute : PassivetAttribute
    {
        public PassiveConditionAttribute(PassiveConditionType _type)
        {
            base.attributeType = (int) _type;
        }
    }
}

