namespace TMPro
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct TMP_Vertex
    {
        public Vector3 position;
        public Vector2 uv;
        public Vector2 uv2;
        public Color32 color;
        public Vector3 normal;
        public Vector4 tangent;
    }
}

