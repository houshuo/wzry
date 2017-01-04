namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public interface IPickupEffect
    {
        void Bind(DropItem item);
        bool CanPickup(PoolObjHandle<ActorRoot> InTarget);
        void OnPickup(PoolObjHandle<ActorRoot> InTarget);
    }
}

