namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class KerningTable
    {
        public List<KerningPair> kerningPairs = new List<KerningPair>();

        public int AddKerningPair(int left, int right, float offset)
        {
            <AddKerningPair>c__AnonStorey47 storey = new <AddKerningPair>c__AnonStorey47 {
                left = left,
                right = right
            };
            if (this.kerningPairs.FindIndex(new Predicate<KerningPair>(storey.<>m__53)) == -1)
            {
                this.kerningPairs.Add(new KerningPair(storey.left, storey.right, offset));
                return 0;
            }
            return -1;
        }

        public void RemoveKerningPair(int index)
        {
            this.kerningPairs.RemoveAt(index);
        }

        public void RemoveKerningPair(int left, int right)
        {
            <RemoveKerningPair>c__AnonStorey48 storey = new <RemoveKerningPair>c__AnonStorey48 {
                left = left,
                right = right
            };
            int index = this.kerningPairs.FindIndex(new Predicate<KerningPair>(storey.<>m__54));
            if (index != -1)
            {
                this.kerningPairs.RemoveAt(index);
            }
        }

        public void SortKerningPairs()
        {
        }

        [CompilerGenerated]
        private sealed class <AddKerningPair>c__AnonStorey47
        {
            internal int left;
            internal int right;

            internal bool <>m__53(KerningPair item)
            {
                return ((item.AscII_Left == this.left) && (item.AscII_Right == this.right));
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveKerningPair>c__AnonStorey48
        {
            internal int left;
            internal int right;

            internal bool <>m__54(KerningPair item)
            {
                return ((item.AscII_Left == this.left) && (item.AscII_Right == this.right));
            }
        }
    }
}

