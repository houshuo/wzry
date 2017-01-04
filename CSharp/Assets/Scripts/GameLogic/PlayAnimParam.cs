namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PlayAnimParam
    {
        public string animName;
        public bool cancelCurrent;
        public bool cancelAll;
        public int layer;
        public float blendTime;
        public bool loop;
        public bool bNoTimeScale;
        public float speed;
    }
}

