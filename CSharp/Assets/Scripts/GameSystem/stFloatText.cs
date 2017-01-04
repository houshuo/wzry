namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct stFloatText
    {
        public int m_floatTextSequence;
        public CUIAnimatorScript m_floatTextAnimatorScript;
        public Vector3 m_worldPosition;
    }
}

