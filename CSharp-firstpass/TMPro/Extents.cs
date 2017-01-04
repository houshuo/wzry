namespace TMPro
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct Extents
    {
        public Vector2 min;
        public Vector2 max;
        public Extents(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            string[] textArray1 = new string[] { "Min (", this.min.x.ToString("f2"), ", ", this.min.y.ToString("f2"), ")   Max (", this.max.x.ToString("f2"), ", ", this.max.y.ToString("f2"), ")" };
            return string.Concat(textArray1);
        }
    }
}

