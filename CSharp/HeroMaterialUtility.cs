using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class HeroMaterialUtility
{
    private const string token_occlusion = " (Occlusion)";
    private const string token_shadow = " (Shadow)";
    private const string token_translucent = " (Translucent)";

    public static void GetShaderProperty(string name, out bool shadow, out bool translucent, out bool occlusion)
    {
        shadow = name.IndexOf(" (Shadow)") != -1;
        translucent = name.IndexOf(" (Translucent)") != -1;
        occlusion = name.IndexOf(" (Occlusion)") != -1;
    }

    public static bool IsHeroBattleShader(Material m)
    {
        return (((m != null) && (m.shader != null)) && (m.shader.name.IndexOf("S_Game_Hero/Hero_Battle") == 0));
    }

    public static string MakeShaderName(string name, bool shadow, bool translucent, bool occlusion)
    {
        string str;
        int length = name.Length;
        int index = name.IndexOf(" (Shadow)");
        if (index != -1)
        {
            length = Mathf.Min(index, length);
        }
        index = name.IndexOf(" (Translucent)");
        if (index != -1)
        {
            length = Mathf.Min(index, length);
        }
        index = name.IndexOf(" (Occlusion)");
        if (index != -1)
        {
            length = Mathf.Min(index, length);
        }
        if (length == name.Length)
        {
            str = name;
        }
        else
        {
            str = name.Substring(0, length);
        }
        if (shadow)
        {
            str = str + " (Shadow)";
        }
        if (translucent)
        {
            str = str + " (Translucent)";
        }
        if (occlusion)
        {
            str = str + " (Occlusion)";
        }
        return str;
    }
}

