namespace Pathfinding
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PathThreadInfo
    {
        public int threadIndex;
        public AstarPath astar;
        public PathHandler runData;
        private object _lock;
        public PathThreadInfo(int index, AstarPath astar, PathHandler runData)
        {
            this.threadIndex = index;
            this.astar = astar;
            this.runData = runData;
            this._lock = new object();
        }

        public object Lock
        {
            get
            {
                return this._lock;
            }
        }
    }
}

