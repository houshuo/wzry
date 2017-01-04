namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class DropItemMgr : Singleton<DropItemMgr>, IUpdateLogic
    {
        protected HashSet<object> ActiveItems = new HashSet<object>();
        protected ListView<DropItem> DeprecatedItem = new ListView<DropItem>();

        public DropItem CreateItem(string InPrefab, IDropDownEffect InDropdownEffect, IPickupEffect InPickupEffect)
        {
            DropItem item = new DropItem(InPrefab, InDropdownEffect, InPickupEffect);
            this.ActiveItems.Add(item);
            return item;
        }

        public GameObject FindPrefabObject(string Prefab)
        {
            return (GameObject) Singleton<CResourceManager>.instance.GetResource(Prefab, typeof(GameObject), enResourceType.BattleScene, false, false).m_content;
        }

        public override void Init()
        {
        }

        public void RemoveItem(DropItem item)
        {
            this.DeprecatedItem.Add(item);
        }

        public void RemoveItemImmediate(DropItem item)
        {
            this.ActiveItems.Remove(item);
        }

        public void UpdateLogic(int delta)
        {
            HashSet<object>.Enumerator enumerator = this.ActiveItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DropItem current = (DropItem) enumerator.Current;
                if (current != null)
                {
                    current.UpdateLogic(delta);
                }
            }
            for (int i = 0; i < this.DeprecatedItem.Count; i++)
            {
                this.ActiveItems.Remove(this.DeprecatedItem[i]);
            }
            this.DeprecatedItem.Clear();
        }
    }
}

