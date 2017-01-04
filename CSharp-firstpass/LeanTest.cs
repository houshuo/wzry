using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class LeanTest
{
    public static int expected;
    private static int passes;
    private static int tests;

    public static void debug(string name, bool didPass, string failExplaination = null)
    {
        float num = printOutLength(name);
        int totalWidth = 40 - ((int) (num * 1.05f));
        string str = string.Empty.PadRight(totalWidth, "_"[0]);
        string[] textArray1 = new string[] { formatB(name), " ", str, " [ ", !didPass ? formatC("fail", "red") : formatC("pass", "green"), " ]" };
        string message = string.Concat(textArray1);
        if (!didPass && (failExplaination != null))
        {
            message = message + " - " + failExplaination;
        }
        Debug.Log(message);
        if (didPass)
        {
            passes++;
        }
        tests++;
        if (tests == expected)
        {
            Debug.Log(formatB("Final Report:") + " _____________________ PASSED: " + formatBC(string.Empty + passes, "green") + " FAILED: " + formatBC(string.Empty + (tests - passes), "red") + " ");
        }
        else if (tests > expected)
        {
            Debug.Log(formatB("Too many tests for a final report!") + " set LeanTest.expected = " + tests);
        }
    }

    public static string formatB(string str)
    {
        return ("<b>" + str + "</b>");
    }

    public static string formatBC(string str, string color)
    {
        return formatC(formatB(str), color);
    }

    public static string formatC(string str, string color)
    {
        string[] textArray1 = new string[] { "<color=", color, ">", str, "</color>" };
        return string.Concat(textArray1);
    }

    public static void overview()
    {
    }

    public static string padRight(int len)
    {
        string str = string.Empty;
        for (int i = 0; i < len; i++)
        {
            str = str + "_";
        }
        return str;
    }

    public static float printOutLength(string str)
    {
        float num = 0f;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == "I"[0])
            {
                num += 0.5f;
            }
            else if (str[i] == "J"[0])
            {
                num += 0.85f;
            }
            else
            {
                num++;
            }
        }
        return num;
    }
}

