namespace Assets.Scripts.GameLogic
{
    using System;

    public class TreasureChestStrategyAttribute : AutoRegisterAttribute, IIdentifierAttribute<int>
    {
        public int KeyType;

        public TreasureChestStrategyAttribute(int InKeyType)
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

