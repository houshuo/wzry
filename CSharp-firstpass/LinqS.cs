using System;
using System.Collections.Generic;

public static class LinqS
{
    public static bool Contains<T>(T[] InArray, T InTest)
    {
        if ((InArray != null) && (InArray.Length != 0))
        {
            for (int i = 0; i < InArray.Length; i++)
            {
                if (object.Equals(InArray[i], InTest))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static string[] Skip(string[] InStringArray, int InCount)
    {
        if (InStringArray == null)
        {
            return null;
        }
        int num = InStringArray.Length - InCount;
        string[] strArray = new string[num];
        for (int i = InCount; i < InStringArray.Length; i++)
        {
            strArray[i - InCount] = InStringArray[i];
        }
        return strArray;
    }

    public static string[] Take(string[] InStringArray, int InCount)
    {
        if (InStringArray == null)
        {
            return null;
        }
        string[] strArray = new string[InCount];
        for (int i = 0; (i < InCount) && (i < InStringArray.Length); i++)
        {
            strArray[i] = InStringArray[i];
        }
        return strArray;
    }

    public static T[] ToArray<T>(ListView<T> InList)
    {
        if (InList == null)
        {
            return null;
        }
        T[] localArray = new T[InList.Count];
        for (int i = 0; i < InList.Count; i++)
        {
            localArray[i] = InList[i];
        }
        return localArray;
    }

    public static List<string> ToStringList(string[] InStringArray)
    {
        if (InStringArray == null)
        {
            return null;
        }
        List<string> list = new List<string>(InStringArray.Length);
        for (int i = 0; i < InStringArray.Length; i++)
        {
            list.Add(InStringArray[i]);
        }
        return list;
    }

    public static string[] Where(string[] InStringArray, Func<string, bool> InPredicate)
    {
        if (InStringArray == null)
        {
            return null;
        }
        List<string> list = new List<string>(InStringArray.Length);
        for (int i = 0; i < InStringArray.Length; i++)
        {
            if (InPredicate(InStringArray[i]))
            {
                list.Add(InStringArray[i]);
            }
        }
        return list.ToArray();
    }
}

