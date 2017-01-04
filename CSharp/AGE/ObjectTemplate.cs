namespace AGE
{
    using System;
    using UnityEngine;

    public sealed class ObjectTemplate : Attribute
    {
        public System.Type[] dependencies;
        public bool dynamicObject;

        public ObjectTemplate(bool _dynamicObject)
        {
            this.dynamicObject = _dynamicObject;
        }

        public ObjectTemplate(params System.Type[] _dependencies)
        {
            this.dependencies = _dependencies;
        }

        public bool CheckForDependencies(GameObject _gameObject)
        {
            foreach (System.Type type in this.dependencies)
            {
                if (_gameObject.GetComponent(type) == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

