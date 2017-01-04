namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class TMPro_ExtensionMethods
    {
        public static string ArrayToString(this char[] chars)
        {
            string str = string.Empty;
            for (int i = 0; (i < chars.Length) && (chars[i] != '\0'); i++)
            {
                str = str + chars[i];
            }
            return str;
        }

        public static bool Compare(this Color a, Color b)
        {
            return ((((a.r == b.r) && (a.g == b.g)) && (a.b == b.b)) && (a.a == b.a));
        }

        public static bool Compare(this Color32 a, Color32 b)
        {
            return ((((a.r == b.r) && (a.g == b.g)) && (a.b == b.b)) && (a.a == b.a));
        }

        public static bool Compare(this Quaternion q1, Quaternion q2, int accuracy)
        {
            bool flag = ((int) (q1.x * accuracy)) == ((int) (q2.x * accuracy));
            bool flag2 = ((int) (q1.y * accuracy)) == ((int) (q2.y * accuracy));
            bool flag3 = ((int) (q1.z * accuracy)) == ((int) (q2.z * accuracy));
            bool flag4 = ((int) (q1.w * accuracy)) == ((int) (q2.w * accuracy));
            return (((flag && flag2) && flag3) && flag4);
        }

        public static bool Compare(this Vector3 v1, Vector3 v2, int accuracy)
        {
            bool flag = ((int) (v1.x * accuracy)) == ((int) (v2.x * accuracy));
            bool flag2 = ((int) (v1.y * accuracy)) == ((int) (v2.y * accuracy));
            bool flag3 = ((int) (v1.z * accuracy)) == ((int) (v2.z * accuracy));
            return ((flag && flag2) && flag3);
        }

        public static bool CompareRGB(this Color a, Color b)
        {
            return (((a.r == b.r) && (a.g == b.g)) && (a.b == b.b));
        }

        public static bool CompareRGB(this Color32 a, Color32 b)
        {
            return (((a.r == b.r) && (a.g == b.g)) && (a.b == b.b));
        }

        public static int FindInstanceID<T>(this List<T> list, T target) where T: UnityEngine.Object
        {
            int instanceID = target.GetInstanceID();
            for (int i = 0; i < list.Count; i++)
            {
                T local = list[i];
                if (local.GetInstanceID() == instanceID)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

