namespace Assets.Scripts.GameSystem
{
    using System;

    public static class SimpleNumericString
    {
        private static bool bIsInitialized = false;
        private static string[] RawString = new string[240];

        public static string GetNumeric(int InValue)
        {
            if (!bIsInitialized)
            {
                bIsInitialized = true;
                Init();
            }
            if ((InValue >= 0) && (InValue < RawString.Length))
            {
                return RawString[InValue];
            }
            return string.Format("{0}", InValue);
        }

        private static void Init()
        {
            for (int i = 0; i < RawString.Length; i++)
            {
                RawString[i] = string.Format("{0}", i);
            }
        }
    }
}

