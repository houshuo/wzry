namespace Pathfinding
{
    using System;

    [Flags]
    public enum ModifierData
    {
        All = -1,
        NodePath = 2,
        Nodes = 3,
        None = 0,
        Original = 0x10,
        StrictNodePath = 1,
        StrictVectorPath = 4,
        Vector = 12,
        VectorPath = 8
    }
}

