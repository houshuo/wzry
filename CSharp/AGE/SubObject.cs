namespace AGE
{
    using System;
    using UnityEngine;

    public sealed class SubObject : Attribute
    {
        public static GameObject FindSubObject(GameObject _targetObject, string _subObjectNamePath)
        {
            if (_subObjectNamePath.IndexOf('/') >= 0)
            {
                Transform transform = _targetObject.transform.Find(_subObjectNamePath);
                if (transform != null)
                {
                    return transform.gameObject;
                }
                return null;
            }
            Transform transform2 = _targetObject.transform.Find(_subObjectNamePath);
            if (transform2 != null)
            {
                return transform2.gameObject;
            }
            for (int i = 0; i < _targetObject.transform.childCount; i++)
            {
                GameObject obj2 = FindSubObject(_targetObject.transform.GetChild(i).gameObject, _subObjectNamePath);
                if (obj2 != null)
                {
                    return obj2;
                }
            }
            return null;
        }
    }
}

