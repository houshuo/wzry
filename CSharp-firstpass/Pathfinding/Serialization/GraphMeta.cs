namespace Pathfinding.Serialization
{
    using System;

    internal class GraphMeta
    {
        public int graphs;
        public string[] guids;
        public int[] nodeCounts;
        public string[] typeNames;
        public Version version;

        public Type GetGraphType(int i)
        {
            if (this.typeNames[i] == null)
            {
                return null;
            }
            Type objA = UtilityPlugin.GetType(this.typeNames[i]);
            if (object.Equals(objA, null))
            {
                throw new Exception("No graph of type '" + this.typeNames[i] + "' could be created, type does not exist");
            }
            return objA;
        }
    }
}

