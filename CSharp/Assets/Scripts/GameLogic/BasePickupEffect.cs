namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class BasePickupEffect : IPickupEffect
    {
        private DropItem Item;

        public virtual void Bind(DropItem item)
        {
            this.Item = item;
        }

        public virtual bool CanPickup(PoolObjHandle<ActorRoot> InTarget)
        {
            return true;
        }

        public virtual void OnPickup(PoolObjHandle<ActorRoot> InTarget)
        {
            Singleton<DropItemMgr>.instance.RemoveItem(this.Item);
            this.Item.Destroy();
        }
    }
}

