namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public class PassiveEventAttribute : PassivetAttribute
    {
        public PassiveEventAttribute(PassiveEventType _type)
        {
            base.attributeType = (int) _type;
        }
    }
}

