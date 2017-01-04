using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ReadonlyContext<T>
{
    private List<T> Reference;
    public ReadonlyContext(List<T> InReference)
    {
        this.Reference = InReference;
    }

    public Enumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this.Reference);
    }

    public bool isValidReference
    {
        get
        {
            return (this.Reference != null);
        }
    }
    public T this[int index]
    {
        get
        {
            return this.Reference[index];
        }
    }
    public int Count
    {
        get
        {
            return this.Reference.Count;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        private List<T> Reference;
        private List<T>.Enumerator IterReference;
        public Enumerator(List<T> InRefernce)
        {
            this.Reference = InRefernce;
            this.IterReference = InRefernce.GetEnumerator();
        }

        object IEnumerator.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public T Current
        {
            get
            {
                return this.IterReference.Current;
            }
        }
        public void Reset()
        {
            this.IterReference = this.Reference.GetEnumerator();
        }

        public void Dispose()
        {
            this.IterReference.Dispose();
            this.Reference = null;
        }

        public bool MoveNext()
        {
            return this.IterReference.MoveNext();
        }
    }
}

