namespace Pathfinding
{
    using System;
    using UnityEngine;

    public class ModifierConverter
    {
        public static bool AllBits(ModifierData a, ModifierData b)
        {
            return ((a & b) == b);
        }

        public static bool AnyBits(ModifierData a, ModifierData b)
        {
            return ((a & b) != ModifierData.None);
        }

        public static bool CanConvert(ModifierData input, ModifierData output)
        {
            ModifierData b = CanConvertTo(input);
            return AnyBits(output, b);
        }

        public static ModifierData CanConvertTo(ModifierData a)
        {
            if (a == ~ModifierData.None)
            {
                return ~ModifierData.None;
            }
            ModifierData data = a;
            if (AnyBits(a, ModifierData.Nodes))
            {
                data |= ModifierData.VectorPath;
            }
            if (AnyBits(a, ModifierData.StrictNodePath))
            {
                data |= ModifierData.StrictVectorPath;
            }
            if (AnyBits(a, ModifierData.StrictVectorPath))
            {
                data |= ModifierData.VectorPath;
            }
            return data;
        }

        public static ModifierData Convert(Path p, ModifierData input, ModifierData output)
        {
            if (!CanConvert(input, output))
            {
                Debug.LogError(string.Concat(new object[] { "Can't convert ", input, " to ", output }));
                return ModifierData.None;
            }
            if (AnyBits(input, output))
            {
                return input;
            }
            if (AnyBits(input, ModifierData.Nodes) && AnyBits(output, ModifierData.Vector))
            {
                p.vectorPath.Clear();
                for (int i = 0; i < p.vectorPath.Count; i++)
                {
                    p.vectorPath.Add(p.path[i].position);
                }
                return (ModifierData.VectorPath | (!AnyBits(input, ModifierData.StrictNodePath) ? ModifierData.None : ModifierData.StrictVectorPath));
            }
            Debug.LogError(string.Concat(new object[] { "This part should not be reached - Error in ModifierConverted\nInput: ", input, " (", (int) input, ")\nOutput: ", output, " (", (int) output, ")" }));
            return ModifierData.None;
        }
    }
}

