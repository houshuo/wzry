namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class DropItem
    {
        protected Transform CachedItemTransform;
        protected IDropDownEffect DropDownEffect;
        protected GameObject ItemObject;
        protected IPickupEffect PickupEffect;
        protected string Prefab;

        public DropItem(string InPrefab, IDropDownEffect InDropdownEffect, IPickupEffect InPickupEffect)
        {
            this.Prefab = InPrefab;
            this.DropDownEffect = InDropdownEffect;
            this.PickupEffect = InPickupEffect;
            if (!string.IsNullOrEmpty(this.Prefab))
            {
                GameObject original = Singleton<DropItemMgr>.instance.FindPrefabObject(this.Prefab);
                if (original != null)
                {
                    this.ItemObject = (GameObject) UnityEngine.Object.Instantiate(original);
                    this.CachedItemTransform = (this.ItemObject == null) ? null : this.ItemObject.transform;
                }
            }
            if (InDropdownEffect != null)
            {
                InDropdownEffect.Bind(this);
            }
            if (InPickupEffect != null)
            {
                InPickupEffect.Bind(this);
            }
        }

        private void CheckTouch()
        {
            int num = MonoSingleton<GlobalConfig>.instance.PickupRange * MonoSingleton<GlobalConfig>.instance.PickupRange;
            DebugHelper.Assert(this.dropDownEffect != null);
            DebugHelper.Assert(this.pickupEffect != null);
            VInt3 location = this.dropDownEffect.location;
            List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
            int count = heroActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = heroActors[i];
                VInt3 num6 = handle.handle.location - location;
                if ((num6.sqrMagnitude <= num) && this.pickupEffect.CanPickup(heroActors[i]))
                {
                    this.pickupEffect.OnPickup(heroActors[i]);
                    break;
                }
            }
        }

        public void Destroy()
        {
            if (this.ItemObject != null)
            {
                UnityEngine.Object.DestroyObject(this.ItemObject);
                this.ItemObject = null;
            }
        }

        public void SetLocation(VInt3 Pos)
        {
            if (this.CachedItemTransform != null)
            {
                this.CachedItemTransform.position = (Vector3) Pos;
            }
        }

        public void UpdateLogic(int delta)
        {
            if ((this.dropDownEffect != null) && !this.dropDownEffect.isFinished)
            {
                this.dropDownEffect.OnUpdate(delta);
            }
            if ((this.DropDownEffect != null) && this.DropDownEffect.isFinished)
            {
                this.CheckTouch();
            }
        }

        public IDropDownEffect dropDownEffect
        {
            get
            {
                return this.DropDownEffect;
            }
        }

        public bool isMoving
        {
            get
            {
                return ((this.DropDownEffect != null) && !this.DropDownEffect.isFinished);
            }
        }

        public IPickupEffect pickupEffect
        {
            get
            {
                return this.PickupEffect;
            }
        }
    }
}

