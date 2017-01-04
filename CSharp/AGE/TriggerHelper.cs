namespace AGE
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class TriggerHelper : MonoBehaviour
    {
        private DictionaryObjectView<GameObject, float> collisionSet = new DictionaryObjectView<GameObject, float>();

        public List<GameObject> GetCollisionSet()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (KeyValuePair<GameObject, float> pair in this.collisionSet)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        private void OnTriggerEnter(Collider _other)
        {
            GameObject gameObject = _other.gameObject;
            if (!this.collisionSet.ContainsKey(gameObject))
            {
                this.collisionSet.Add(gameObject, Time.realtimeSinceStartup);
            }
        }

        private void OnTriggerExit(Collider _other)
        {
            GameObject gameObject = _other.gameObject;
            if (this.collisionSet.ContainsKey(gameObject))
            {
                this.collisionSet.Remove(gameObject);
            }
        }
    }
}

