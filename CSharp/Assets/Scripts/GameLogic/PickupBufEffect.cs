namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    public class PickupBufEffect : BasePickupEffect
    {
        private ResBufConfigInfo BufDropInfo;

        public PickupBufEffect(ResBufConfigInfo InBufDropInfo)
        {
            this.BufDropInfo = InBufDropInfo;
        }

        public void ApplyBuff(PoolObjHandle<ActorRoot> InTarget)
        {
            new BufConsumer((int) this.BufDropInfo.dwBufID, InTarget, InTarget).Use();
        }

        public override void OnPickup(PoolObjHandle<ActorRoot> InTarget)
        {
            DebugHelper.Assert((bool) InTarget);
            this.ApplyBuff(InTarget);
            base.OnPickup(InTarget);
        }
    }
}

