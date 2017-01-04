namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct stRect
    {
        public int m_width;
        public int m_height;
        public int m_top;
        public int m_bottom;
        public int m_left;
        public int m_right;
        public Vector2 m_center;
    }
}

