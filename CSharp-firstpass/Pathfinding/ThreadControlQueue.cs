namespace Pathfinding
{
    using System;
    using System.Threading;

    public class ThreadControlQueue
    {
        private ManualResetEvent block = new ManualResetEvent(true);
        private bool blocked;
        private int blockedReceivers;
        private Path head;
        private object lockObj = new object();
        private int numReceivers;
        private bool starving;
        private Path tail;
        private bool terminate;

        public ThreadControlQueue(int numReceivers)
        {
            this.numReceivers = numReceivers;
        }

        public void Block()
        {
            this.blocked = true;
            this.block.Reset();
        }

        public void Lock()
        {
            Monitor.Enter(this.lockObj);
        }

        public Path Pop()
        {
            lock (this.lockObj)
            {
                if (this.terminate)
                {
                    this.blockedReceivers++;
                    throw new QueueTerminationException();
                }
                if (this.head == null)
                {
                    this.Starving();
                }
                while (this.blocked || this.starving)
                {
                    this.blockedReceivers++;
                    if (this.terminate)
                    {
                        throw new QueueTerminationException();
                    }
                    if ((this.blockedReceivers != this.numReceivers) && (this.blockedReceivers > this.numReceivers))
                    {
                        object[] objArray1 = new object[] { "More receivers are blocked than specified in constructor (", this.blockedReceivers, " > ", this.numReceivers, ")" };
                        throw new InvalidOperationException(string.Concat(objArray1));
                    }
                    Monitor.Exit(this.lockObj);
                    this.block.WaitOne();
                    Monitor.Enter(this.lockObj);
                    this.blockedReceivers--;
                    if (this.head == null)
                    {
                        this.Starving();
                    }
                }
                Path head = this.head;
                DebugHelper.Assert(this.head != null);
                if (this.head.next == null)
                {
                    this.tail = null;
                }
                this.head = this.head.next;
                return head;
            }
        }

        public Path PopNoBlock(bool blockedBefore)
        {
            lock (this.lockObj)
            {
                if (this.terminate)
                {
                    this.blockedReceivers++;
                    throw new QueueTerminationException();
                }
                if (this.head == null)
                {
                    this.Starving();
                }
                if (this.blocked || this.starving)
                {
                    if (!blockedBefore)
                    {
                        this.blockedReceivers++;
                        if (this.terminate)
                        {
                            throw new QueueTerminationException();
                        }
                        if ((this.blockedReceivers != this.numReceivers) && (this.blockedReceivers > this.numReceivers))
                        {
                            object[] objArray1 = new object[] { "More receivers are blocked than specified in constructor (", this.blockedReceivers, " > ", this.numReceivers, ")" };
                            throw new InvalidOperationException(string.Concat(objArray1));
                        }
                    }
                    return null;
                }
                if (blockedBefore)
                {
                    this.blockedReceivers--;
                }
                Path head = this.head;
                DebugHelper.Assert(this.head != null);
                if (this.head.next == null)
                {
                    this.tail = null;
                }
                this.head = this.head.next;
                return head;
            }
        }

        public void Push(Path p)
        {
            if (!this.terminate)
            {
                if (this.tail == null)
                {
                    this.head = p;
                    this.tail = p;
                    if (this.starving && !this.blocked)
                    {
                        this.starving = false;
                        this.block.Set();
                    }
                    else
                    {
                        this.starving = false;
                    }
                }
                else
                {
                    this.tail.next = p;
                    this.tail = p;
                }
            }
        }

        public void PushFront(Path p)
        {
            if (!this.terminate)
            {
                if (this.tail == null)
                {
                    this.head = p;
                    this.tail = p;
                    if (this.starving && !this.blocked)
                    {
                        this.starving = false;
                        this.block.Set();
                    }
                    else
                    {
                        this.starving = false;
                    }
                }
                else
                {
                    p.next = this.head;
                    this.head = p;
                }
            }
        }

        public void ReceiverTerminated()
        {
            Monitor.Enter(this.lockObj);
            this.blockedReceivers++;
            Monitor.Exit(this.lockObj);
        }

        private void Starving()
        {
            this.starving = true;
            this.block.Reset();
        }

        public void TerminateReceivers()
        {
            this.terminate = true;
            this.block.Set();
        }

        public void Unblock()
        {
            this.blocked = false;
            this.block.Set();
        }

        public void Unlock()
        {
            Monitor.Exit(this.lockObj);
        }

        public bool AllReceiversBlocked
        {
            get
            {
                return (this.blocked && (this.blockedReceivers == this.numReceivers));
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.head == null);
            }
        }

        public bool IsTerminating
        {
            get
            {
                return this.terminate;
            }
        }

        public class QueueTerminationException : Exception
        {
        }
    }
}

