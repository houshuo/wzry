namespace Pathfinding
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Progress
    {
        public float progress;
        public string description;
        public Progress(float p, string d)
        {
            this.progress = p;
            this.description = d;
        }
    }
}

