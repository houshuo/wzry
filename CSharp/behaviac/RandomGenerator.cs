namespace behaviac
{
    using System;

    internal class RandomGenerator
    {
        private static RandomGenerator Instance;
        private uint m_seed;

        protected RandomGenerator(uint seed)
        {
            this.m_seed = seed;
        }

        ~RandomGenerator()
        {
        }

        public static RandomGenerator GetInstance()
        {
            if (Instance == null)
            {
                Instance = new RandomGenerator(0);
            }
            return Instance;
        }

        public float GetRandom()
        {
            this.m_seed = (0x343fd * this.m_seed) + 0x269ec3;
            return (this.m_seed * 2.328306E-10f);
        }

        public float InRange(float low, float high)
        {
            return ((this.GetRandom() * (high - low)) + low);
        }

        public void SetSeed(uint seed)
        {
            this.m_seed = seed;
        }
    }
}

