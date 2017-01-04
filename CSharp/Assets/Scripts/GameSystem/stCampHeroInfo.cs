namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct stCampHeroInfo
    {
        public string m_headIconPath;
        public uint m_objectID;
        public Rect m_headAreaInScreen;
        public stFan m_headAreaFan;
    }
}

