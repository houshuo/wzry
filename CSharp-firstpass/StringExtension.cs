using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class StringExtension
{
    public static readonly string asset_str = "Assets/";

    public static string AssetPathToFullPath(this string s)
    {
        if (s == null)
        {
            return null;
        }
        if (!s.StartsWith(asset_str))
        {
            return null;
        }
        return (Application.dataPath + "/" + s.Remove(0, asset_str.Length));
    }

    public static string FullPathToAssetPath(this string s)
    {
        if (s == null)
        {
            return null;
        }
        return (asset_str + s.Substring(Application.dataPath.Length + 1)).Replace('\\', '/');
    }

    public static string GetFileExtension(this string s)
    {
        int num = s.LastIndexOf('.');
        if (num == -1)
        {
            return null;
        }
        return s.Substring(num + 1);
    }

    public static string GetFileExtensionUpper(this string s)
    {
        string fileExtension = s.GetFileExtension();
        if (fileExtension == null)
        {
            return null;
        }
        return fileExtension.ToUpper();
    }

    public static string GetHierarchyName(this GameObject go)
    {
        if (go == null)
        {
            return "<null>";
        }
        string name = string.Empty;
        while (go != null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = go.name;
            }
            else
            {
                name = go.name + "." + name;
            }
            Transform parent = go.transform.parent;
            go = (parent == null) ? null : parent.gameObject;
        }
        return name;
    }

    public static string RemoveExtension(this string s)
    {
        if (s == null)
        {
            return null;
        }
        int length = s.LastIndexOf('.');
        if (length == -1)
        {
            return s;
        }
        return s.Substring(0, length);
    }
}

