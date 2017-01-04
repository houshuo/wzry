namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class TowerHitMgr
    {
        private DictionaryView<uint, TowerHit> _data = new DictionaryView<uint, TowerHit>();

        public void Clear()
        {
            DictionaryView<uint, TowerHit>.Enumerator enumerator = this._data.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, TowerHit> current = enumerator.Current;
                TowerHit hit = current.Value;
                if (hit != null)
                {
                    hit.Clear();
                }
            }
            this._data.Clear();
        }

        public void Init()
        {
        }

        public void Register(uint objid, RES_ORGAN_TYPE type)
        {
            if (!this._data.ContainsKey(objid))
            {
                this._data.Add(objid, new TowerHit(type));
            }
            else
            {
                this._data[objid].Clear();
                this._data[objid] = new TowerHit(type);
            }
        }

        public void Remove(uint id)
        {
            TowerHit hit = null;
            this._data.TryGetValue(id, out hit);
            if (hit != null)
            {
                hit.Clear();
            }
            this._data.Remove(id);
        }

        public void TryActive(uint id, GameObject target = null)
        {
            if (target != null)
            {
                TowerHit hit = null;
                this._data.TryGetValue(id, out hit);
                if (hit != null)
                {
                    hit.TryActive(target);
                }
            }
        }
    }
}

