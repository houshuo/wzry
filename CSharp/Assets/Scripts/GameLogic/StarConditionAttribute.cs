namespace Assets.Scripts.GameLogic
{
    using System;

    public class StarConditionAttribute : AutoRegisterAttribute, IIdentifierAttribute<int>
    {
        public int CondType;

        public StarConditionAttribute(int InCondType)
        {
            this.CondType = InCondType;
        }

        public int[] AdditionalIdList
        {
            get
            {
                return null;
            }
        }

        public int ID
        {
            get
            {
                return this.CondType;
            }
        }
    }
}

