namespace Assets.Scripts.Framework
{
    using System;
    using UnityEngine;

    public class DetectRenderQuality
    {
        public static SGameRenderQuality check_Android()
        {
            SGameRenderQuality q = checkGPU_Android(SystemInfo.graphicsDeviceName);
            checkDevice_Android(ref q);
            return q;
        }

        private static void checkDevice_Android(ref SGameRenderQuality q)
        {
            switch (SystemInfo.deviceModel.ToLower())
            {
                case "samsung gt-s7568i":
                    q = SGameRenderQuality.Low;
                    break;

                case "xiaomi 1s":
                    q = SGameRenderQuality.Medium;
                    break;

                case "xiaomi 2013022":
                    q = SGameRenderQuality.Medium;
                    break;

                case "samsung sch-i959":
                    q = SGameRenderQuality.Medium;
                    break;

                case "xiaomi mi 3":
                    q = SGameRenderQuality.High;
                    break;

                case "xiaomi mi 2a":
                    q = SGameRenderQuality.Medium;
                    break;

                case "xiaomi hm 1sc":
                    q = SGameRenderQuality.Low;
                    break;
            }
        }

        private static SGameRenderQuality checkGPU_Adreno(string[] tokens)
        {
            int val = 0;
            for (int i = 1; i < tokens.Length; i++)
            {
                if (TryGetInt(ref val, tokens[i]))
                {
                    if (val < 200)
                    {
                        return SGameRenderQuality.Low;
                    }
                    if (val < 300)
                    {
                        if (val > 220)
                        {
                            return SGameRenderQuality.Low;
                        }
                        return SGameRenderQuality.Low;
                    }
                    if (val < 400)
                    {
                        if (val >= 330)
                        {
                            return SGameRenderQuality.High;
                        }
                        if (val >= 320)
                        {
                            return SGameRenderQuality.Medium;
                        }
                        return SGameRenderQuality.Low;
                    }
                    if (val >= 400)
                    {
                        if (val < 420)
                        {
                            return SGameRenderQuality.Medium;
                        }
                        return SGameRenderQuality.High;
                    }
                }
            }
            return SGameRenderQuality.Low;
        }

        private static SGameRenderQuality checkGPU_Android(string gpuName)
        {
            SGameRenderQuality low = SGameRenderQuality.Low;
            if (SystemInfo.systemMemorySize < 0x5dc)
            {
                return SGameRenderQuality.Low;
            }
            gpuName = gpuName.ToLower();
            char[] separator = new char[] { ' ', '\t', '\r', '\n', '+', '-', ':', '\0' };
            string[] tokens = gpuName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if ((tokens == null) || (tokens.Length == 0))
            {
                return SGameRenderQuality.Low;
            }
            if (tokens[0].Contains("vivante"))
            {
                return SGameRenderQuality.Low;
            }
            if (tokens[0] == "adreno")
            {
                return checkGPU_Adreno(tokens);
            }
            if (((tokens[0] == "powervr") || (tokens[0] == "imagination")) || (tokens[0] == "sgx"))
            {
                return checkGPU_PowerVR(tokens);
            }
            if (((tokens[0] == "arm") || (tokens[0] == "mali")) || ((tokens.Length > 1) && (tokens[1] == "mali")))
            {
                return checkGPU_Mali(tokens);
            }
            if (!(tokens[0] == "tegra") && !(tokens[0] == "nvidia"))
            {
                return low;
            }
            return checkGPU_Tegra(tokens);
        }

        private static SGameRenderQuality checkGPU_Mali(string[] tokens)
        {
            int val = 0;
            SGameRenderQuality low = SGameRenderQuality.Low;
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
                            return SGameRenderQuality.Low;
                        }
                        if (val < 500)
                        {
                            if ((val != 400) && (val == 450))
                            {
                                return SGameRenderQuality.Medium;
                            }
                            return SGameRenderQuality.Low;
                        }
                        if (val < 700)
                        {
                            if (!flag)
                            {
                                return SGameRenderQuality.Low;
                            }
                            if (val < 620)
                            {
                                return SGameRenderQuality.Low;
                            }
                            if (val < 0x274)
                            {
                                return SGameRenderQuality.Medium;
                            }
                            return SGameRenderQuality.High;
                        }
                        if (!flag)
                        {
                            return SGameRenderQuality.Low;
                        }
                        return SGameRenderQuality.High;
                    }
                }
            }
            return low;
        }

        private static SGameRenderQuality checkGPU_PowerVR(string[] tokens)
        {
            bool flag = false;
            bool flag2 = false;
            SGameRenderQuality low = SGameRenderQuality.Low;
            int val = 0;
            for (int i = 1; i < tokens.Length; i++)
            {
                bool flag3;
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
                        goto Label_01EB;

                    default:
                    {
                        if (!flag)
                        {
                            goto Label_011F;
                        }
                        flag3 = false;
                        int index = str.IndexOf("mp");
                        if (index > 0)
                        {
                            TryGetInt(ref val, str.Substring(0, index));
                            flag3 = true;
                        }
                        else if (TryGetInt(ref val, str))
                        {
                            for (int j = i + 1; j < tokens.Length; j++)
                            {
                                if (tokens[j].ToLower().IndexOf("mp") >= 0)
                                {
                                    flag3 = true;
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
                    low = SGameRenderQuality.Low;
                }
                else if (val == 0x21f)
                {
                    low = SGameRenderQuality.Low;
                }
                else if (val == 0x220)
                {
                    low = SGameRenderQuality.Low;
                    if (flag3)
                    {
                        low = SGameRenderQuality.Medium;
                    }
                }
                else
                {
                    low = SGameRenderQuality.Medium;
                }
                break;
            Label_011F:
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
                            if (val >= 0x1b58)
                            {
                                low = SGameRenderQuality.High;
                            }
                            else if (val >= 0x1770)
                            {
                                if (val < 0x17d4)
                                {
                                    low = SGameRenderQuality.Low;
                                }
                                else if (val < 0x1900)
                                {
                                    low = SGameRenderQuality.Medium;
                                }
                                else
                                {
                                    low = SGameRenderQuality.High;
                                }
                            }
                            else
                            {
                                low = SGameRenderQuality.Low;
                            }
                            break;
                        }
                    }
                }
            }
        Label_01EB:
            if (flag2)
            {
                low = SGameRenderQuality.High;
            }
            return low;
        }

        private static SGameRenderQuality checkGPU_Tegra(string[] tokens)
        {
            bool flag = false;
            int val = 0;
            SGameRenderQuality low = SGameRenderQuality.Low;
            for (int i = 1; i < tokens.Length; i++)
            {
                if (TryGetInt(ref val, tokens[i]))
                {
                    flag = true;
                    if (val >= 4)
                    {
                        low = SGameRenderQuality.High;
                    }
                    else
                    {
                        if (val != 3)
                        {
                            continue;
                        }
                        low = SGameRenderQuality.Medium;
                    }
                    break;
                }
                string str = tokens[i];
                if (str == "k1")
                {
                    low = SGameRenderQuality.High;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                low = SGameRenderQuality.Medium;
            }
            return low;
        }

        public static void test()
        {
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
}

