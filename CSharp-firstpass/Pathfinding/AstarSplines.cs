namespace Pathfinding
{
    using System;
    using UnityEngine;

    internal class AstarSplines
    {
        public static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime)
        {
            float num = elapsedTime;
            float num2 = num * num;
            float num3 = num2 * num;
            return (Vector3) ((((previous * (((-0.5f * num3) + num2) - (0.5f * num))) + (start * (((1.5f * num3) + (-2.5f * num2)) + 1f))) + (end * (((-1.5f * num3) + (2f * num2)) + (0.5f * num)))) + (next * ((0.5f * num3) - (0.5f * num2))));
        }

        public static Vector3 CatmullRomOLD(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime)
        {
            float num = elapsedTime;
            float num2 = num * num;
            float num3 = num2 * num;
            return (Vector3) ((((previous * (((-0.5f * num3) + num2) - (0.5f * num))) + (start * (((1.5f * num3) + (-2.5f * num2)) + 1f))) + (end * (((-1.5f * num3) + (2f * num2)) + (0.5f * num)))) + (next * ((0.5f * num3) - (0.5f * num2))));
        }
    }
}

