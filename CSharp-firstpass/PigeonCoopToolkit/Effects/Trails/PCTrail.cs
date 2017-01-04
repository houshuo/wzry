namespace PigeonCoopToolkit.Effects.Trails
{
    using PigeonCoopToolkit.Utillities;
    using System;
    using UnityEngine;

    public class PCTrail : IDisposable
    {
        public int activePointCount;
        public Color[] colors;
        public int[] indicies;
        public bool IsActiveTrail;
        public UnityEngine.Mesh Mesh = new UnityEngine.Mesh();
        public Vector3[] normals;
        public CircularBuffer<PCTrailPoint> Points;
        public Vector2[] uvs;
        public Vector3[] verticies;

        public PCTrail(int numPoints)
        {
            this.Mesh.MarkDynamic();
            this.verticies = new Vector3[2 * numPoints];
            this.normals = new Vector3[2 * numPoints];
            this.uvs = new Vector2[2 * numPoints];
            this.colors = new Color[2 * numPoints];
            this.indicies = new int[(2 * numPoints) * 3];
            this.Points = new CircularBuffer<PCTrailPoint>(numPoints);
        }

        public void Dispose()
        {
            if (this.Mesh != null)
            {
                if (Application.isEditor)
                {
                    UnityEngine.Object.DestroyImmediate(this.Mesh, true);
                }
                else
                {
                    UnityEngine.Object.Destroy(this.Mesh);
                }
            }
            this.Points.Clear();
            this.Points = null;
        }
    }
}

