namespace Pathfinding.Util
{
    using Pathfinding;
    using System;

    public class LockFreeStack
    {
        public Path head;

        public Path PopAll()
        {
            Path head = this.head;
            this.head = null;
            return head;
        }

        public void Push(Path p)
        {
            p.next = this.head;
            this.head = p;
        }
    }
}

