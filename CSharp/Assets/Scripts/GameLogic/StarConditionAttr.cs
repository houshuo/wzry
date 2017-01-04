namespace Assets.Scripts.GameLogic
{
    using System;

    [StarCondition(2)]
    internal class StarConditionAttr : StarConditionProxy
    {
        protected static StarSystemFactory Factory = new StarSystemFactory(typeof(StarConditionAttrContextAttribute), typeof(IStarCondition));

        public override IStarCondition CreateStarCondition()
        {
            IStarCondition condition = Factory.Create(this.attrID) as IStarCondition;
            object[] inParameters = new object[] { this.attrID };
            DebugHelper.Assert(condition != null, "can't create Attr id {0}", inParameters);
            if (condition != null)
            {
                condition.Initialize(base.ConditionInfo);
            }
            return condition;
        }

        public virtual int attrID
        {
            get
            {
                return base.ConditionInfo.KeyDetail[0];
            }
        }
    }
}

