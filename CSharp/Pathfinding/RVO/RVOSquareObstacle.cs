namespace Pathfinding.RVO
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/Local Avoidance/Square Obstacle")]
    public class RVOSquareObstacle : RVOObstacle
    {
        public VInt2 center = new VInt2(0x3e8, 0x3e8);
        public int height = 0x3e8;
        public VInt2 size = new VInt2(0x3e8, 0x3e8);

        protected override bool AreGizmosDirty()
        {
            return false;
        }

        protected override void CreateObstacles()
        {
            this.size.x = Mathf.Abs(this.size.x);
            this.size.y = Mathf.Abs(this.size.y);
            this.height = Mathf.Abs(this.height);
            VInt3[] vertices = new VInt3[] { new VInt3(1, 0, -1), new VInt3(1, 0, 1), new VInt3(-1, 0, 1), new VInt3(-1, 0, -1) };
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= new VInt3(this.size.x >> 1, 0, this.size.y >> 1);
                vertices[i] += new VInt3(this.center.x, 0, this.center.y);
            }
            base.AddObstacle(vertices, this.height);
        }

        protected override bool ExecuteInEditor
        {
            get
            {
                return true;
            }
        }

        protected override int Height
        {
            get
            {
                return this.height;
            }
        }

        protected override bool LocalCoordinates
        {
            get
            {
                return true;
            }
        }

        protected override bool StaticObstacle
        {
            get
            {
                return false;
            }
        }
    }
}

