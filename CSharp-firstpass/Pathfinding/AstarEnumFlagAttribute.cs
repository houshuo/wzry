namespace Pathfinding
{
    using System;
    using UnityEngine;

    public class AstarEnumFlagAttribute : PropertyAttribute
    {
        public string enumName;

        public AstarEnumFlagAttribute()
        {
        }

        public AstarEnumFlagAttribute(string name)
        {
            this.enumName = name;
        }
    }
}

