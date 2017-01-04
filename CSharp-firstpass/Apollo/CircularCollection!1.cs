namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class CircularCollection<T>
    {
        private int _nReadIndex;
        private List<object> dataCollection;

        public CircularCollection()
        {
            this.nWriteIndex = 0;
            this.nReadIndex = 0;
            this.dataCollection = new List<object>(50);
        }

        public CircularCollection(int capacity)
        {
            this.nWriteIndex = 0;
            this.nReadIndex = 0;
            this.dataCollection = new List<object>(capacity);
        }

        public void Add(T aData)
        {
            if (this.nWriteIndex >= this.dataCollection.Capacity)
            {
                this.nWriteIndex = 0;
            }
            if (this.dataCollection.Count < this.dataCollection.Capacity)
            {
                this.dataCollection.Add(aData);
            }
            else
            {
                this.dataCollection[this.nWriteIndex] = aData;
            }
            this.nWriteIndex++;
        }

        public int Capacity
        {
            get
            {
                return this.dataCollection.Capacity;
            }
        }

        public int Count
        {
            get
            {
                if (this.nWriteIndex >= this.nReadIndex)
                {
                    return (this.nWriteIndex - this.nReadIndex);
                }
                return ((this.dataCollection.Capacity + this.nWriteIndex) - this.nReadIndex);
            }
        }

        public int Free
        {
            get
            {
                if (this.nReadIndex > this.nWriteIndex)
                {
                    return (this.nReadIndex - this.nWriteIndex);
                }
                return ((this.dataCollection.Capacity + this.nReadIndex) - this.nWriteIndex);
            }
        }

        public T Next
        {
            get
            {
                int num;
                if (this.nReadIndex == this.nWriteIndex)
                {
                    return default(T);
                }
                if (this.nReadIndex >= this.dataCollection.Capacity)
                {
                    this.nReadIndex = 0;
                }
                this.nReadIndex = (num = this.nReadIndex) + 1;
                object obj2 = this.dataCollection[num];
                return ((obj2 == null) ? default(T) : ((T) obj2));
            }
        }

        private int nReadIndex
        {
            get
            {
                return this._nReadIndex;
            }
            set
            {
                this._nReadIndex = value;
            }
        }

        private int nWriteIndex { get; set; }
    }
}

