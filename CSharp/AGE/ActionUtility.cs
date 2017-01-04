namespace AGE
{
    using System;
    using UnityEngine;

    public static class ActionUtility
    {
        public static float MsToSec(int ms)
        {
            return (ms * 0.001f);
        }

        public static int SecToMs(float s)
        {
            return Mathf.RoundToInt(s * 1000f);
        }
    }
}

