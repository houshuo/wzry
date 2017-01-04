namespace Assets.Scripts.GameLogic
{
    using System;

    public class StarConditionAttrContextAttribute : AutoRegisterAttribute, IIdentifierAttribute<int>
    {
        public int KeyType;

        public StarConditionAttrContextAttribute(int InKeyType)
        {
            this.KeyType = InKeyType;
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
                return this.KeyType;
            }
        }
    }
}

