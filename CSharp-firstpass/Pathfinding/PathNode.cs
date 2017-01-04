namespace Pathfinding
{
    using System;

    public class PathNode
    {
        private const uint CostMask = 0xfffffff;
        private const uint Flag1Mask = 0x10000000;
        private const int Flag1Offset = 0x1c;
        private const uint Flag2Mask = 0x20000000;
        private const int Flag2Offset = 0x1d;
        private uint flags;
        private uint g;
        private uint h;
        public GraphNode node;
        public PathNode parent;
        public ushort pathID;

        public uint cost
        {
            get
            {
                return (this.flags & 0xfffffff);
            }
            set
            {
                this.flags = (this.flags & 0xf0000000) | value;
            }
        }

        public uint F
        {
            get
            {
                return (this.g + this.h);
            }
        }

        public bool flag1
        {
            get
            {
                return ((this.flags & 0x10000000) != 0);
            }
            set
            {
                this.flags = (this.flags & 0xefffffff) | (!value ? 0 : 0x10000000);
            }
        }

        public bool flag2
        {
            get
            {
                return ((this.flags & 0x20000000) != 0);
            }
            set
            {
                this.flags = (this.flags & 0xdfffffff) | (!value ? 0 : 0x20000000);
            }
        }

        public uint G
        {
            get
            {
                return this.g;
            }
            set
            {
                this.g = value;
            }
        }

        public uint H
        {
            get
            {
                return this.h;
            }
            set
            {
                this.h = value;
            }
        }
    }
}

