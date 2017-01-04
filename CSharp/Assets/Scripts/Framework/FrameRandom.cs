namespace Assets.Scripts.Framework
{
    using System;

    public class FrameRandom
    {
        private const uint addValue = 0x3039;
        public static uint callNum = 0;
        private const uint maxShort = 0x10000;
        private const uint multiper = 0x472e396d;
        private static uint nSeed = ((uint) UnityEngine.Random.Range(0x7fff, 0x7fffffff));

        public static float fRandom()
        {
            callNum++;
            return (((float) Random(0x10000)) / 65536f);
        }

        public static int GetSeed()
        {
            return (int) nSeed;
        }

        public static ushort Random(uint nMax)
        {
            callNum++;
            nSeed = (nSeed * 0x472e396d) + 0x3039;
            return (ushort) ((nSeed >> 0x10) % nMax);
        }

        public static void ResetSeed(uint seed)
        {
            nSeed = seed;
            callNum = 0;
        }
    }
}

