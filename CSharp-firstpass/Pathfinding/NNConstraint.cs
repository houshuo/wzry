namespace Pathfinding
{
    using System;

    public class NNConstraint
    {
        public int area = -1;
        public bool constrainArea;
        public bool constrainDistance = true;
        public bool constrainTags = true;
        public bool constrainWalkability = true;
        public bool distanceXZ;
        public int graphMask = -1;
        public int tags = -1;
        public bool walkable = true;

        public virtual bool Suitable(GraphNode node)
        {
            if (this.constrainWalkability && (node.Walkable != this.walkable))
            {
                return false;
            }
            if ((this.constrainArea && (this.area >= 0)) && (node.Area != this.area))
            {
                return false;
            }
            if (this.constrainTags && (((this.tags >> node.Tag) & 1) == 0))
            {
                return false;
            }
            return true;
        }

        public virtual bool SuitableGraph(int graphIndex, NavGraph graph)
        {
            return (((this.graphMask >> graphIndex) & 1) != 0);
        }

        public static NNConstraint Default
        {
            get
            {
                return new NNConstraint();
            }
        }

        public static NNConstraint None
        {
            get
            {
                return new NNConstraint { constrainWalkability = false, constrainArea = false, constrainTags = false, constrainDistance = false, graphMask = -1 };
            }
        }
    }
}

