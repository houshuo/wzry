namespace Pathfinding
{
    using System;

    public class QuadtreeNodeHolder
    {
        public QuadtreeNodeHolder c0;
        public QuadtreeNodeHolder c1;
        public QuadtreeNodeHolder c2;
        public QuadtreeNodeHolder c3;
        public QuadtreeNode node;

        public void GetNodes(GraphNodeDelegateCancelable del)
        {
            if (this.node != null)
            {
                del(this.node);
            }
            else
            {
                this.c0.GetNodes(del);
                this.c1.GetNodes(del);
                this.c2.GetNodes(del);
                this.c3.GetNodes(del);
            }
        }
    }
}

