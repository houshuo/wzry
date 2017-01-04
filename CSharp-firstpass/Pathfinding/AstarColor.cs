namespace Pathfinding
{
    using System;
    using UnityEngine;

    [Serializable]
    public class AstarColor
    {
        public Color[] _AreaColors;
        public Color _BoundsHandles = new Color(0.29f, 0.454f, 0.741f, 0.9f);
        public Color _ConnectionHighLerp = new Color(1f, 0f, 0f, 0.5f);
        public Color _ConnectionLowLerp = new Color(0f, 1f, 0f, 0.5f);
        public Color _MeshColor = new Color(0.125f, 0.686f, 0f, 0.19f);
        public Color _MeshEdgeColor = new Color(0f, 0f, 0f, 0.5f);
        public Color _NodeConnection = new Color(1f, 1f, 1f, 0.9f);
        public Color _UnwalkableNode = new Color(1f, 0f, 0f, 0.5f);
        private static Color[] AreaColors;
        public static Color BoundsHandles = new Color(0.29f, 0.454f, 0.741f, 0.9f);
        public static Color ConnectionHighLerp = new Color(1f, 0f, 0f, 0.5f);
        public static Color ConnectionLowLerp = new Color(0f, 1f, 0f, 0.5f);
        public static Color MeshColor = new Color(0f, 0f, 0f, 0.5f);
        public static Color MeshEdgeColor = new Color(0f, 0f, 0f, 0.5f);
        public static Color NodeConnection = new Color(1f, 1f, 1f, 0.9f);
        public static Color UnwalkableNode = new Color(1f, 0f, 0f, 0.5f);

        public static Color GetAreaColor(uint area)
        {
            if ((AreaColors != null) && (area < AreaColors.Length))
            {
                return AreaColors[area];
            }
            return AstarMath.IntToColor((int) area, 1f);
        }

        public void OnEnable()
        {
            NodeConnection = this._NodeConnection;
            UnwalkableNode = this._UnwalkableNode;
            BoundsHandles = this._BoundsHandles;
            ConnectionLowLerp = this._ConnectionLowLerp;
            ConnectionHighLerp = this._ConnectionHighLerp;
            MeshEdgeColor = this._MeshEdgeColor;
            MeshColor = this._MeshColor;
            AreaColors = this._AreaColors;
        }
    }
}

