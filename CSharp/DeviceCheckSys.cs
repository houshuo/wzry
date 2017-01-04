using System;
using UnityEngine;

public class DeviceCheckSys
{
    public static int cpuCoreNum;
    public static string cpuName;
    public static string deviceName;
    public static int height;
    public const int LowCoreNum = 1;
    public const int LowEneterMemorySize = 200;
    public const int LowHeight = 480;
    public const int LowMemorySize = 700;
    public const int LowWidth = 800;
    public static int sysMemorySize;
    public static int width;

    public static bool CheckAvailMemory()
    {
        return true;
    }

    public static bool CheckCPU()
    {
        cpuCoreNum = SystemInfo.processorCount;
        if (cpuCoreNum < 1)
        {
            return false;
        }
        return true;
    }

    public static bool CheckDeviceIsValid()
    {
        return true;
    }

    private static bool checkGPU_Adreno(string[] tokens)
    {
        int val = 0;
        for (int i = 1; i < tokens.Length; i++)
        {
            if (TryGetInt(ref val, tokens[i]))
            {
                if (val < 200)
                {
                    return false;
                }
                if (val < 300)
                {
                    return (val > 220);
                }
                if (val < 400)
                {
                    return ((val >= 320) || (val >= 0x131));
                }
                if (val >= 400)
                {
                    return ((val < 410) || true);
                }
            }
        }
        return false;
    }

    private static bool checkGPU_Mali(string[] tokens)
    {
        int val = 0;
        for (int i = 1; i < tokens.Length; i++)
        {
            string str = tokens[i];
            if (str.Length >= 3)
            {
                int num3 = str.LastIndexOf("mp");
                bool flag = str[0] == 't';
                if (num3 > 0)
                {
                    int startIndex = !flag ? 0 : 1;
                    str = str.Substring(startIndex, num3 - startIndex);
                    TryGetInt(ref val, str);
                }
                else
                {
                    if (flag)
                    {
                        str = str.Substring(1);
                    }
                    if (TryGetInt(ref val, str))
                    {
                        for (int j = i + 1; j < tokens.Length; j++)
                        {
                            str = tokens[j];
                            if (str.IndexOf("mp") >= 0)
                            {
                                break;
                            }
                        }
                    }
                }
                if (val > 0)
                {
                    if (val < 400)
                    {
                        return false;
                    }
                    if (val < 500)
                    {
                        return ((val == 400) || (val == 450));
                    }
                    if (val < 700)
                    {
                        if (!flag)
                        {
                            return false;
                        }
                        return ((val < 620) || true);
                    }
                    if (!flag)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private static bool checkGPU_PowerVR(string[] tokens)
    {
        bool flag = false;
        bool flag2 = false;
        int val = 0;
        for (int i = 1; i < tokens.Length; i++)
        {
            string str = tokens[i];
            switch (str)
            {
                case "sgx":
                {
                    flag = true;
                    continue;
                }
                case "rogue":
                    flag2 = true;
                    goto Label_01A4;

                default:
                {
                    if (!flag)
                    {
                        goto Label_00F4;
                    }
                    int index = str.IndexOf("mp");
                    if (index > 0)
                    {
                        TryGetInt(ref val, str.Substring(0, index));
                    }
                    else if (TryGetInt(ref val, str))
                    {
                        for (int j = i + 1; j < tokens.Length; j++)
                        {
                            if (tokens[j].ToLower().IndexOf("mp") >= 0)
                            {
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            if (val <= 0)
            {
                continue;
            }
            if (val < 0x21f)
            {
                return false;
            }
            if ((val == 0x21f) || (val != 0x220))
            {
            }
            return true;
        Label_00F4:
            if (str.Length > 4)
            {
                char ch = str[0];
                char ch2 = str[1];
                if (ch == 'g')
                {
                    if ((ch2 >= '0') && (ch2 <= '9'))
                    {
                        TryGetInt(ref val, str.Substring(1));
                    }
                    else
                    {
                        TryGetInt(ref val, str.Substring(2));
                    }
                    if (val > 0)
                    {
                        if (val < 0x1b58)
                        {
                            if (val < 0x1770)
                            {
                                return false;
                            }
                            if (val < 0x17d4)
                            {
                                return false;
                            }
                            if (val < 0x1900)
                            {
                                return true;
                            }
                        }
                        return true;
                    }
                }
            }
        }
    Label_01A4:
        if (flag2)
        {
            return true;
        }
        return false;
    }

    private static bool checkGPU_Tegra(string[] tokens)
    {
        bool flag = false;
        int val = 0;
        for (int i = 1; i < tokens.Length; i++)
        {
            if (TryGetInt(ref val, tokens[i]))
            {
                flag = true;
                if (val >= 4)
                {
                    return true;
                }
                if (val == 3)
                {
                    return true;
                }
            }
            else
            {
                string str = tokens[i];
                if (str == "k1")
                {
                    return true;
                }
            }
        }
        return !flag;
    }

    public static bool CheckMemory()
    {
        sysMemorySize = SystemInfo.systemMemorySize;
        if (sysMemorySize < 700)
        {
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("MemoryNotEnough", null, true);
            return false;
        }
        return true;
    }

    public static bool CheckResolution()
    {
        width = Screen.width;
        height = Screen.height;
        if ((width >= 800) && (height >= 480))
        {
            return true;
        }
        Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("ResolutionNotValid", null, true);
        return false;
    }

    public static int GetAvailMemory()
    {
        return 0;
    }

    public static int GetAvailMemoryMegaBytes()
    {
        return 0;
    }

    public static int GetTotalMemoryMegaBytes()
    {
        return SystemInfo.systemMemorySize;
    }

    private static bool TryGetInt(ref int val, string str)
    {
        val = 0;
        try
        {
            val = Convert.ToInt32(str);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

