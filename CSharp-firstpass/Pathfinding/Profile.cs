namespace Pathfinding
{
    using System;
    using System.Diagnostics;
    using UnityEngine;

    public class Profile
    {
        private int control = 0x40000000;
        private int counter;
        private bool dontCountFirst;
        private long mem;
        public string name;
        private const bool PROFILE_MEM = false;
        private long smem;
        private Stopwatch w;

        public Profile(string name)
        {
            this.name = name;
            this.w = new Stopwatch();
        }

        [Conditional("PROFILE")]
        public void ConsoleLog()
        {
            Console.WriteLine(this.ToString());
        }

        [Conditional("PROFILE")]
        public void Control(Profile other)
        {
            if (this.ControlValue() != other.ControlValue())
            {
                object[] objArray1 = new object[] { "Control numbers do not match (", this.name, " ", other.name, ") ", this.ControlValue(), " != ", other.ControlValue() };
                throw new Exception(string.Concat(objArray1));
            }
        }

        public int ControlValue()
        {
            return this.control;
        }

        [Conditional("PROFILE")]
        public void Log()
        {
            Debug.Log(this.ToString());
        }

        [Conditional("PROFILE")]
        public void Start()
        {
            if (!this.dontCountFirst || (this.counter != 1))
            {
                this.w.Start();
            }
        }

        [Conditional("PROFILE")]
        public void Stop()
        {
            this.counter++;
            if (!this.dontCountFirst || (this.counter != 1))
            {
                this.w.Stop();
            }
        }

        [Conditional("PROFILE")]
        public void Stop(int control)
        {
            this.counter++;
            if (!this.dontCountFirst || (this.counter != 1))
            {
                this.w.Stop();
                if (this.control == 0x40000000)
                {
                    this.control = control;
                }
                else if (this.control != control)
                {
                    object[] objArray1 = new object[] { "Control numbers do not match ", this.control, " != ", control };
                    throw new Exception(string.Concat(objArray1));
                }
            }
        }

        public override string ToString()
        {
            object[] objArray1 = new object[] { this.name, " #", this.counter, " ", this.w.Elapsed.TotalMilliseconds.ToString("0.0 ms"), " avg: ", (this.w.Elapsed.TotalMilliseconds / ((double) this.counter)).ToString("0.00 ms") };
            return string.Concat(objArray1);
        }
    }
}

