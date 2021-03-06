﻿namespace Pathfinding
{
    using System;

    [Serializable]
    public class TagMask
    {
        public int tagsChange;
        public int tagsSet;

        public TagMask()
        {
        }

        public TagMask(int change, int set)
        {
            this.tagsChange = change;
            this.tagsSet = set;
        }

        public void SetValues(object boxedTagMask)
        {
            TagMask mask = (TagMask) boxedTagMask;
            this.tagsChange = mask.tagsChange;
            this.tagsSet = mask.tagsSet;
        }

        public override string ToString()
        {
            return (string.Empty + Convert.ToString(this.tagsChange, 2) + "\n" + Convert.ToString(this.tagsSet, 2));
        }
    }
}

