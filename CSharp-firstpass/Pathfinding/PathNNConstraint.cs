namespace Pathfinding
{
    using System;

    public class PathNNConstraint : NNConstraint
    {
        public virtual void SetStart(GraphNode node)
        {
            if (node != null)
            {
                base.area = (int) node.Area;
            }
            else
            {
                base.constrainArea = false;
            }
        }

        public static PathNNConstraint Default
        {
            get
            {
                return new PathNNConstraint { constrainArea = true };
            }
        }
    }
}

