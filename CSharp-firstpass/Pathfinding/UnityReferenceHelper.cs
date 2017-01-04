namespace Pathfinding
{
    using Pathfinding.Util;
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class UnityReferenceHelper : MonoBehaviour
    {
        [HideInInspector, SerializeField]
        private string guid;

        public void Awake()
        {
            this.Reset();
        }

        public string GetGUID()
        {
            return this.guid;
        }

        public void Reset()
        {
            if ((this.guid == null) || (this.guid == string.Empty))
            {
                this.guid = Pathfinding.Util.Guid.NewGuid().ToString();
                Debug.Log("Created new GUID - " + this.guid);
            }
            else
            {
                foreach (UnityReferenceHelper helper in UnityEngine.Object.FindObjectsOfType(typeof(UnityReferenceHelper)) as UnityReferenceHelper[])
                {
                    if ((helper != this) && (this.guid == helper.guid))
                    {
                        this.guid = Pathfinding.Util.Guid.NewGuid().ToString();
                        Debug.Log("Created new GUID - " + this.guid);
                        return;
                    }
                }
            }
        }
    }
}

