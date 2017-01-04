using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class SProfiler : MonoSingleton<SProfiler>
{
    private static uint _curFrameIndex = 0;
    private static bool deferStartup = false;
    private static bool disableGameInput = false;
    private static EventSystem gui_eventSystem;
    private static float MaxSelfTimeDispearTime = 6f;
    private static double MaxSelfTimeThreshold = 15.0;
    public static ProfileObj Obj = new ProfileObj();
    private static bool paused = false;
    public static int profileFrameCount = 0;
    private static int requestPause = 0;
    private static bool requestReset = false;
    private static bool showColomn_AvgSelfTime = true;
    private static bool showColomn_AvgTime = true;
    private static bool showColomn_Count = true;
    private static bool showColomn_MaxSelfTime = true;
    private static bool showColomn_MaxTime = true;
    private static bool showColomn_SelfTime = true;
    private static bool showColomn_Time = true;
    private static bool showData = false;
    private static bool showFPSStatistic = false;
    private static bool showMaxOnly = false;
    public static long startFrameIndex = 0L;
    public static uint StartFrameIndex = 0;

    [Conditional("SGAME_PROFILE")]
    public static void BeginSample(string name)
    {
        Obj.Begin(name);
    }

    public static void Cleanup()
    {
        Obj.Cleanup();
    }

    private static void EnableEventSystem(bool enable)
    {
        if (gui_eventSystem == null)
        {
            gui_eventSystem = EventSystem.current;
        }
        if (gui_eventSystem != null)
        {
            gui_eventSystem.enabled = enable;
        }
    }

    [Conditional("SGAME_PROFILE")]
    public static void EndSample()
    {
        Obj.End();
    }

    private void LateUpdate()
    {
        if (requestReset)
        {
            requestReset = false;
            Reset();
        }
        if (requestPause != 0)
        {
            paused = requestPause == 1;
            requestPause = 0;
        }
        if (((profileFrameCount > 0) && !paused) && ((CurFrameIndex - StartFrameIndex) >= profileFrameCount))
        {
            profileFrameCount = 0;
            FPSStatistic.sampleFps = false;
            ActorsStatistic.sampleActors = false;
            showFPSStatistic = true;
            showData = true;
            paused = true;
            Obj.SaveProfileData();
        }
    }

    private void OnGUI()
    {
        if (showGUI)
        {
            if (showMaxOnly)
            {
                Obj.DrawGroups_MaxSelfTimeOnly();
            }
            else
            {
                Obj.DrawGroups();
            }
            if (showFPSStatistic)
            {
                FPSStatistic.Draw();
                ActorsStatistic.Draw();
            }
        }
    }

    public static void Pause()
    {
        paused = true;
    }

    public void RequestPause(bool pause)
    {
        requestPause = !pause ? -1 : 1;
    }

    public void RequestReset()
    {
        requestReset = true;
    }

    public static void Reset()
    {
        Obj.Reset();
    }

    public void ShowGUI(bool show)
    {
        if (showGUI != show)
        {
            showGUI = show;
            if (showGUI)
            {
                Cleanup();
                paused = false;
            }
            deferStartup = false;
        }
    }

    private static void StartProfile()
    {
        paused = false;
        showGUI = true;
        showFPSStatistic = false;
        showData = false;
        StartFrameIndex = CurFrameIndex;
        FPSStatistic.Start();
        ActorsStatistic.Start();
    }

    public void StartProfileNFrames(int frameCount, uint startIndex = 0)
    {
        Cleanup();
        profileFrameCount = frameCount;
        if (startIndex > 0)
        {
            if (CurFrameIndex > startIndex)
            {
                startIndex *= (CurFrameIndex / startIndex) + 1;
            }
            StartFrameIndex = startIndex;
            deferStartup = true;
            paused = true;
        }
        else
        {
            StartFrameIndex = CurFrameIndex;
            deferStartup = false;
            StartProfile();
        }
    }

    public void ToggleVisible()
    {
        this.ShowGUI(!showGUI);
    }

    public static uint CurFrameIndex
    {
        get
        {
            return _curFrameIndex;
        }
        set
        {
            _curFrameIndex = value;
            if ((deferStartup && (StartFrameIndex > 0)) && (_curFrameIndex == StartFrameIndex))
            {
                deferStartup = false;
                StartProfile();
            }
        }
    }

    public static bool showGUI
    {
        [CompilerGenerated]
        get
        {
            return <showGUI>k__BackingField;
        }
        [CompilerGenerated]
        private set
        {
            <showGUI>k__BackingField = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ColumnProp
    {
        public string name;
        public int width;
        public string data;
        public bool showInCompareMode;
        public int colorIndex;
        public bool hidden;
    }

    public class Group
    {
        public int count;
        public SProfiler.Group groupToCompare;
        public double maxSelfTime;
        public double maxTime;
        public string name;
        public List<SProfiler.Sample> samples = new List<SProfiler.Sample>();
        public double selfTime;
        public double selfTimePercent;
        public double time;
        public double timePercent;
        public float timeToClean;

        public void AppendStr_AvgSelfTime(ref string str_avgSelfTime)
        {
            str_avgSelfTime = str_avgSelfTime + ((this.count == 0) ? 0.0 : (this.selfTime / ((double) this.count))).ToString("F2");
            str_avgSelfTime = str_avgSelfTime + "\n";
        }

        public void AppendStr_AvgSelfTime_Diff(ref string str_avgSelfTime)
        {
            double num = (this.count == 0) ? 0.0 : (this.selfTime / ((double) this.count));
            double num2 = (this.groupToCompare.count == 0) ? 0.0 : (this.groupToCompare.selfTime / ((double) this.groupToCompare.count));
            str_avgSelfTime = str_avgSelfTime + ((num2 - num)).ToString("F2");
            str_avgSelfTime = str_avgSelfTime + "\n";
        }

        public void AppendStr_AvgTime(ref string str_avgTime)
        {
            str_avgTime = str_avgTime + ((this.count == 0) ? 0.0 : (this.time / ((double) this.count))).ToString("F2");
            str_avgTime = str_avgTime + "\n";
        }

        public void AppendStr_AvgTime_Diff(ref string str_avgTime)
        {
            double num = (this.count == 0) ? 0.0 : (this.time / ((double) this.count));
            double num2 = (this.groupToCompare.count == 0) ? 0.0 : (this.groupToCompare.time / ((double) this.groupToCompare.count));
            str_avgTime = str_avgTime + ((num2 - num)).ToString("F2");
            str_avgTime = str_avgTime + "\n";
        }

        public void AppendStr_Count(ref string str_count)
        {
            str_count = str_count + this.count;
            str_count = str_count + "\n";
        }

        public void AppendStr_Count_Diff(ref string str_count)
        {
            int num = this.groupToCompare.count - this.count;
            str_count = str_count + num;
            str_count = str_count + "\n";
        }

        public void AppendStr_MaxSelfTime(ref string str_maxSelfTime)
        {
            string str = this.maxSelfTime.ToString("F2");
            if (this.maxSelfTime > 5.0)
            {
                str_maxSelfTime = str_maxSelfTime + "<color=red>";
                str_maxSelfTime = str_maxSelfTime + str;
                str_maxSelfTime = str_maxSelfTime + "</color>";
            }
            else
            {
                str_maxSelfTime = str_maxSelfTime + str;
            }
            str_maxSelfTime = str_maxSelfTime + "\n";
        }

        public void AppendStr_MaxSelfTime_Diff(ref string str_maxSelfTime)
        {
            double num = this.groupToCompare.maxTime - this.maxSelfTime;
            str_maxSelfTime = str_maxSelfTime + this.maxSelfTime.ToString("F2");
            str_maxSelfTime = str_maxSelfTime + "\n";
        }

        public void AppendStr_MaxTime(ref string str_maxTime)
        {
            string str = this.maxTime.ToString("F2");
            if (this.maxTime > 5.0)
            {
                str_maxTime = str_maxTime + "<color=red>";
                str_maxTime = str_maxTime + str;
                str_maxTime = str_maxTime + "</color>";
            }
            else
            {
                str_maxTime = str_maxTime + str;
            }
            str_maxTime = str_maxTime + "\n";
        }

        public void AppendStr_MaxTime_Diff(ref string str_maxTime)
        {
            str_maxTime = str_maxTime + ((this.groupToCompare.maxTime - this.maxTime)).ToString("F2");
            str_maxTime = str_maxTime + "\n";
        }

        public void AppendStr_SelfTime(ref string str_selfTime, bool showPercent)
        {
            str_selfTime = str_selfTime + this.selfTime.ToString("F2");
            if (showPercent)
            {
                str_selfTime = str_selfTime + " (";
                str_selfTime = str_selfTime + this.selfTimePercent.ToString("F2");
                str_selfTime = str_selfTime + "%)";
            }
            str_selfTime = str_selfTime + "\n";
        }

        public void AppendStr_SelfTime_Diff(ref string str_selfTime, bool showPercent)
        {
            double num = this.groupToCompare.selfTime - this.selfTime;
            double num2 = this.groupToCompare.selfTimePercent - this.selfTimePercent;
            str_selfTime = str_selfTime + num.ToString("F2");
            str_selfTime = str_selfTime + "\n";
        }

        public void AppendStr_Time(ref string str_time, bool showPercent)
        {
            str_time = str_time + this.time.ToString("F2");
            if (showPercent)
            {
                str_time = str_time + " (";
                str_time = str_time + this.timePercent.ToString("F2");
                str_time = str_time + "%)";
            }
            str_time = str_time + "\n";
        }

        public void AppendStr_Time_Diff(ref string str_time, bool showPercent)
        {
            double num = this.groupToCompare.time - this.time;
            double num2 = this.groupToCompare.timePercent - this.timePercent;
            str_time = str_time + num.ToString("F2");
            str_time = str_time + "\n";
        }

        public void checkClean()
        {
            if ((Time.realtimeSinceStartup > this.timeToClean) && (this.timeToClean > 0f))
            {
                this.timeToClean = -1f;
                for (int i = 0; i < this.samples.Count; i++)
                {
                    SProfiler.Sample sample = this.samples[i];
                    sample.maxSelfTime = 0.0;
                    sample.maxTime = 0.0;
                }
            }
        }

        public void flush(double totalTime)
        {
            double maxSelfTime = this.maxSelfTime;
            this.count = 0;
            this.time = 0.0;
            this.selfTime = 0.0;
            this.maxTime = 0.0;
            this.maxSelfTime = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            for (int i = 0; i < this.samples.Count; i++)
            {
                SProfiler.Sample sample = this.samples[i];
                num2 += sample.profilerTime0;
                num3 += sample.profilerTime1;
                this.time += sample.time;
                this.selfTime += sample.stackTime;
                this.count += sample.count;
                this.maxTime = Math.Max(this.maxTime, sample.maxTime);
                this.maxSelfTime = Math.Max(this.maxSelfTime, sample.maxSelfTime);
            }
            this.selfTime = this.time - this.selfTime;
            this.time -= num2;
            this.selfTime -= num3;
            this.timePercent = (this.time * 100.0) / totalTime;
            this.selfTimePercent = (this.selfTime * 100.0) / totalTime;
            this.time *= 1000.0;
            this.selfTime *= 1000.0;
            this.maxTime *= 1000.0;
            this.maxSelfTime *= 1000.0;
            if ((this.maxSelfTime > SProfiler.MaxSelfTimeThreshold) && (this.maxSelfTime > maxSelfTime))
            {
                this.timeToClean = Time.realtimeSinceStartup + SProfiler.MaxSelfTimeDispearTime;
            }
        }
    }

    [Serializable]
    public class ProfileObj
    {
        [CompilerGenerated]
        private static Comparison<SProfiler.Group> <>f__am$cache12;
        [CompilerGenerated]
        private static Comparison<SProfiler.Group> <>f__am$cache13;
        [CompilerGenerated]
        private static Comparison<SProfiler.Group> <>f__am$cache14;
        private static Color[] column_colors;
        private static SProfiler.ColumnProp[] column_props;
        [NonSerialized]
        public bool compareMode;
        [NonSerialized]
        private SProfiler.ProfileObj compareObj;
        public SProfiler.SortType curSortType = SProfiler.SortType.Time;
        private const float drawGroup_buttonWidth = 88f;
        private const float drawGroup_marginX = 2f;
        private const float drawGroup_startX = 2f;
        private const float drawGroup_startY = 30f;
        [NonSerialized]
        public List<SProfiler.Group> groupList = new List<SProfiler.Group>();
        [NonSerialized]
        private List<SProfiler.Group> groupListForSort = new List<SProfiler.Group>();
        [NonSerialized]
        public Dictionary<string, SProfiler.Group> groups = new Dictionary<string, SProfiler.Group>();
        public bool highToLow = true;
        public int idAllocator;
        public Dictionary<long, int> idmap = new Dictionary<long, int>();
        private const int MaxStackCount = 0x20;
        [NonSerialized]
        public string[] profileFiles;
        public Dictionary<int, SProfiler.Sample> samples = new Dictionary<int, SProfiler.Sample>();
        public bool showPercent = true;
        [NonSerialized]
        public bool showProfileFiles;
        [NonSerialized]
        public int stackCount;
        [NonSerialized]
        public SProfiler.Sample[] stacks = new SProfiler.Sample[0x20];
        public double totalTime;

        static ProfileObj()
        {
            SProfiler.ColumnProp[] propArray1 = new SProfiler.ColumnProp[0x16];
            SProfiler.ColumnProp prop = new SProfiler.ColumnProp {
                name = "NAME",
                width = 260
            };
            propArray1[0] = prop;
            SProfiler.ColumnProp prop2 = new SProfiler.ColumnProp {
                name = "TIME",
                width = 140
            };
            propArray1[1] = prop2;
            SProfiler.ColumnProp prop3 = new SProfiler.ColumnProp {
                name = "TIME2",
                width = 140,
                showInCompareMode = true,
                colorIndex = 1
            };
            propArray1[2] = prop3;
            SProfiler.ColumnProp prop4 = new SProfiler.ColumnProp {
                name = "DIFF",
                width = 100,
                showInCompareMode = true,
                colorIndex = 2
            };
            propArray1[3] = prop4;
            SProfiler.ColumnProp prop5 = new SProfiler.ColumnProp {
                name = "SELF TIME",
                width = 140
            };
            propArray1[4] = prop5;
            SProfiler.ColumnProp prop6 = new SProfiler.ColumnProp {
                name = "SELF TIME 2",
                width = 140,
                showInCompareMode = true,
                colorIndex = 1
            };
            propArray1[5] = prop6;
            SProfiler.ColumnProp prop7 = new SProfiler.ColumnProp {
                name = "DIFF",
                width = 100,
                showInCompareMode = true,
                colorIndex = 2
            };
            propArray1[6] = prop7;
            SProfiler.ColumnProp prop8 = new SProfiler.ColumnProp {
                name = "AVG TIME",
                width = 80
            };
            propArray1[7] = prop8;
            SProfiler.ColumnProp prop9 = new SProfiler.ColumnProp {
                name = "AVG TIME2",
                width = 80,
                showInCompareMode = true,
                colorIndex = 1
            };
            propArray1[8] = prop9;
            SProfiler.ColumnProp prop10 = new SProfiler.ColumnProp {
                name = "AVG DIFF",
                width = 80,
                showInCompareMode = true,
                colorIndex = 2
            };
            propArray1[9] = prop10;
            SProfiler.ColumnProp prop11 = new SProfiler.ColumnProp {
                name = "AVG SELF",
                width = 80
            };
            propArray1[10] = prop11;
            SProfiler.ColumnProp prop12 = new SProfiler.ColumnProp {
                name = "AVG SELF 2",
                width = 80,
                showInCompareMode = true,
                colorIndex = 1
            };
            propArray1[11] = prop12;
            SProfiler.ColumnProp prop13 = new SProfiler.ColumnProp {
                name = "DIFF",
                width = 80,
                showInCompareMode = true,
                colorIndex = 2
            };
            propArray1[12] = prop13;
            SProfiler.ColumnProp prop14 = new SProfiler.ColumnProp {
                name = "MAX",
                width = 80
            };
            propArray1[13] = prop14;
            SProfiler.ColumnProp prop15 = new SProfiler.ColumnProp {
                name = "MAX2",
                width = 80,
                showInCompareMode = true,
                colorIndex = 1
            };
            propArray1[14] = prop15;
            SProfiler.ColumnProp prop16 = new SProfiler.ColumnProp {
                name = "DIFF",
                width = 80,
                showInCompareMode = true,
                colorIndex = 2
            };
            propArray1[15] = prop16;
            SProfiler.ColumnProp prop17 = new SProfiler.ColumnProp {
                name = "MAX SELF",
                width = 80
            };
            propArray1[0x10] = prop17;
            SProfiler.ColumnProp prop18 = new SProfiler.ColumnProp {
                name = "MAX SELF 2",
                width = 80,
                showInCompareMode = true,
                colorIndex = 1
            };
            propArray1[0x11] = prop18;
            SProfiler.ColumnProp prop19 = new SProfiler.ColumnProp {
                name = "DIFF",
                width = 80,
                showInCompareMode = true,
                colorIndex = 2
            };
            propArray1[0x12] = prop19;
            SProfiler.ColumnProp prop20 = new SProfiler.ColumnProp {
                name = "COUNT",
                width = 80
            };
            propArray1[0x13] = prop20;
            SProfiler.ColumnProp prop21 = new SProfiler.ColumnProp {
                name = "COUNT2",
                width = 80,
                showInCompareMode = true,
                colorIndex = 1
            };
            propArray1[20] = prop21;
            SProfiler.ColumnProp prop22 = new SProfiler.ColumnProp {
                name = "DIFF",
                width = 80,
                showInCompareMode = true,
                colorIndex = 2
            };
            propArray1[0x15] = prop22;
            column_props = propArray1;
            column_colors = new Color[] { Color.green, Color.cyan, Color.yellow };
        }

        public void Begin(string name)
        {
            if ((!string.IsNullOrEmpty(name) && !SProfiler.paused) && SProfiler.showGUI)
            {
                double currentTime = SProfiler.STimer.currentTime;
                int key = this.getIdentifity(name);
                SProfiler.Sample sample = null;
                if (!this.samples.TryGetValue(key, out sample))
                {
                    sample = new SProfiler.Sample {
                        name = name,
                        identity = key
                    };
                    this.samples.Add(sample.identity, sample);
                    SProfiler.Group group = null;
                    if (!this.groups.TryGetValue(name, out group))
                    {
                        group = new SProfiler.Group {
                            name = name
                        };
                        this.groups.Add(name, group);
                        this.groupList.Add(group);
                    }
                    group.samples.Add(sample);
                }
                if (this.stackCount >= this.stacks.Length)
                {
                    SProfiler.Sample[] destinationArray = new SProfiler.Sample[this.stacks.Length * 2];
                    Array.Copy(this.stacks, destinationArray, this.stacks.Length);
                    this.stacks = destinationArray;
                }
                this.stacks[this.stackCount++] = sample;
                sample.begin();
                if (this.stackCount > 1)
                {
                    currentTime = sample.start - currentTime;
                    SProfiler.Sample sample2 = this.stacks[this.stackCount - 2];
                    sample2.profilerTime1 += currentTime;
                    sample2.profilerTimeThisCall1 += currentTime;
                    for (int i = 0; i < (this.stackCount - 1); i++)
                    {
                        sample2 = this.stacks[i];
                        sample2.profilerTime0 += currentTime;
                        sample2.profilerTimeThisCall0 += currentTime;
                    }
                    this.totalTime -= currentTime;
                }
            }
        }

        private void BuildGroupColumns()
        {
            string str = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            string str5 = string.Empty;
            string str6 = string.Empty;
            string str7 = string.Empty;
            string str8 = string.Empty;
            string str9 = string.Empty;
            string str10 = string.Empty;
            string str11 = string.Empty;
            string str12 = string.Empty;
            string str13 = string.Empty;
            string str14 = string.Empty;
            string str15 = string.Empty;
            string str16 = string.Empty;
            string str17 = string.Empty;
            string str18 = string.Empty;
            string str19 = string.Empty;
            string str20 = string.Empty;
            string str21 = string.Empty;
            string str22 = string.Empty;
            for (int i = 0; i < this.groupList.Count; i++)
            {
                SProfiler.Group group = this.groupList[i];
                SProfiler.Group groupToCompare = group.groupToCompare;
                str = str + group.name + "\n";
                if (SProfiler.showColomn_Time)
                {
                    group.AppendStr_Time(ref str2, this.showPercent);
                    if (this.compareMode)
                    {
                        groupToCompare.AppendStr_Time(ref str9, this.showPercent);
                        group.AppendStr_Time_Diff(ref str16, this.showPercent);
                    }
                }
                if (SProfiler.showColomn_SelfTime)
                {
                    group.AppendStr_SelfTime(ref str3, this.showPercent);
                    if (this.compareMode)
                    {
                        groupToCompare.AppendStr_SelfTime(ref str10, this.showPercent);
                        group.AppendStr_SelfTime_Diff(ref str17, this.showPercent);
                    }
                }
                if (SProfiler.showColomn_AvgTime)
                {
                    group.AppendStr_AvgTime(ref str4);
                    if (this.compareMode)
                    {
                        groupToCompare.AppendStr_AvgTime(ref str11);
                        group.AppendStr_AvgTime_Diff(ref str18);
                    }
                }
                if (SProfiler.showColomn_AvgSelfTime)
                {
                    group.AppendStr_AvgSelfTime(ref str5);
                    if (this.compareMode)
                    {
                        groupToCompare.AppendStr_AvgSelfTime(ref str12);
                        group.AppendStr_AvgSelfTime_Diff(ref str19);
                    }
                }
                if (SProfiler.showColomn_MaxTime)
                {
                    group.AppendStr_MaxTime(ref str6);
                    if (this.compareMode)
                    {
                        groupToCompare.AppendStr_MaxTime(ref str13);
                        group.AppendStr_MaxTime_Diff(ref str20);
                    }
                }
                if (SProfiler.showColomn_MaxSelfTime)
                {
                    group.AppendStr_MaxSelfTime(ref str7);
                    if (this.compareMode)
                    {
                        groupToCompare.AppendStr_MaxSelfTime(ref str14);
                        group.AppendStr_MaxSelfTime_Diff(ref str21);
                    }
                }
                if (SProfiler.showColomn_Count)
                {
                    group.AppendStr_Count(ref str8);
                    if (this.compareMode)
                    {
                        groupToCompare.AppendStr_Count(ref str15);
                        group.AppendStr_Count_Diff(ref str22);
                    }
                }
            }
            column_props[0].data = str;
            column_props[1].data = str2;
            column_props[4].data = str3;
            column_props[7].data = str4;
            column_props[10].data = str5;
            column_props[13].data = str6;
            column_props[0x10].data = str7;
            column_props[0x13].data = str8;
            column_props[2].data = str9;
            column_props[5].data = str10;
            column_props[8].data = str11;
            column_props[11].data = str12;
            column_props[14].data = str13;
            column_props[0x11].data = str14;
            column_props[20].data = str15;
            column_props[3].data = str16;
            column_props[6].data = str17;
            column_props[9].data = str18;
            column_props[12].data = str19;
            column_props[15].data = str20;
            column_props[0x12].data = str21;
            column_props[0x15].data = str22;
        }

        public void Cleanup()
        {
            for (int i = 0; i < this.stacks.Length; i++)
            {
                this.stacks[i] = null;
            }
            this.stackCount = 0;
            this.samples.Clear();
            this.groups.Clear();
            this.groupList.Clear();
            this.totalTime = 0.0;
            this.compareMode = false;
            this.compareObj = null;
            SProfiler.profileFrameCount = 0;
            this.curSortType = SProfiler.SortType.Time;
            this.highToLow = true;
        }

        private void CopyFrom(SProfiler.ProfileObj src)
        {
            this.Cleanup();
            if (src != null)
            {
                this.samples = src.samples;
                this.groups = src.groups;
                this.groupList = src.groupList;
                this.totalTime = src.totalTime;
                this.idmap = src.idmap;
                this.idAllocator = src.idAllocator;
            }
        }

        private void DrawButtons()
        {
            float height = 26f;
            float num2 = 4f;
            float top = 0f;
            float left = 2f;
            GUI.color = Color.green;
            if (GUI.Button(new Rect(left, top, 88f, height), "MODE"))
            {
                SProfiler.showMaxOnly = !SProfiler.showMaxOnly;
                this.Reset();
            }
            top += height + num2;
            if (GUI.Button(new Rect(left, top, 88f, height), "RESET"))
            {
                this.Cleanup();
            }
            top += height + num2;
            Color color = GUI.color;
            GUI.color = !SProfiler.paused ? color : Color.red;
            if (GUI.Button(new Rect(left, top, 88f, height), "PAUSE"))
            {
                SProfiler.paused = !SProfiler.paused;
            }
            top += height + num2;
            GUI.color = color;
            GUI.color = !SProfiler.showData ? Color.red : color;
            if (GUI.Button(new Rect(left, top, 88f, height), "SHOW"))
            {
                SProfiler.showData = !SProfiler.showData;
            }
            top += height + num2;
            GUI.color = color;
            GUI.color = !SProfiler.disableGameInput ? color : Color.red;
            if (GUI.Button(new Rect(left, top, 88f, height), "EVENT SYS"))
            {
                SProfiler.disableGameInput = !SProfiler.disableGameInput;
                SProfiler.EnableEventSystem(!SProfiler.disableGameInput);
            }
            top += height + num2;
            GUI.color = color;
            if (GUI.Button(new Rect(left, top, 88f, height), "SHOW %"))
            {
                this.showPercent = !this.showPercent;
            }
            top += height + num2;
            if (GUI.Button(new Rect(left, top, 88f, height), "SAVE"))
            {
                this.SaveProfileData();
            }
            top += height + num2;
            if (GUI.Button(new Rect(left, top, 88f, height), "LOAD"))
            {
                this.RefreshProfileFiles();
                this.showProfileFiles = true;
            }
            top += height + num2;
            if (GUI.Button(new Rect(left, top, 88f, height), "COMPARE"))
            {
                this.RefreshProfileFiles();
                this.showProfileFiles = true;
                this.compareMode = true;
            }
            top += height + num2;
            GUI.color = !SProfiler.showFPSStatistic ? Color.red : color;
            if (GUI.Button(new Rect(left, top, 88f, height), "FPS_SHOW"))
            {
                SProfiler.showFPSStatistic = !SProfiler.showFPSStatistic;
            }
            top += height + num2;
            GUI.color = !FPSStatistic.sampleFps ? Color.red : color;
            if (GUI.Button(new Rect(left, top, 88f, height), "FPS_PAUSE"))
            {
                FPSStatistic.sampleFps = !FPSStatistic.sampleFps;
                ActorsStatistic.sampleActors = !ActorsStatistic.sampleActors;
            }
            top += height + num2;
            GUI.color = color;
            if (GUI.Button(new Rect(left, top, 88f, height), "FPS_RESET"))
            {
                FPSStatistic.Reset();
                ActorsStatistic.Reset();
            }
            top += height + num2;
            if (this.compareMode)
            {
                top += height + num2;
                GUI.color = !SProfiler.showColomn_Time ? Color.red : color;
                if (GUI.Button(new Rect(left, top, 88f, height), "[TIME]"))
                {
                    SProfiler.showColomn_Time = !SProfiler.showColomn_Time;
                    column_props[1].hidden = column_props[2].hidden = column_props[3].hidden = !SProfiler.showColomn_Time;
                }
                top += height + num2;
                GUI.color = !SProfiler.showColomn_SelfTime ? Color.red : color;
                if (GUI.Button(new Rect(left, top, 88f, height), "[SELF TIME]"))
                {
                    SProfiler.showColomn_SelfTime = !SProfiler.showColomn_SelfTime;
                    column_props[4].hidden = column_props[5].hidden = column_props[6].hidden = !SProfiler.showColomn_SelfTime;
                }
                top += height + num2;
                GUI.color = !SProfiler.showColomn_AvgTime ? Color.red : color;
                if (GUI.Button(new Rect(left, top, 88f, height), "[AVG TIME]"))
                {
                    SProfiler.showColomn_AvgTime = !SProfiler.showColomn_AvgTime;
                    column_props[7].hidden = column_props[8].hidden = column_props[9].hidden = !SProfiler.showColomn_AvgTime;
                }
                top += height + num2;
                GUI.color = !SProfiler.showColomn_AvgSelfTime ? Color.red : color;
                if (GUI.Button(new Rect(left, top, 88f, height), "[AVG SELF]"))
                {
                    SProfiler.showColomn_AvgSelfTime = !SProfiler.showColomn_AvgSelfTime;
                    column_props[10].hidden = column_props[11].hidden = column_props[12].hidden = !SProfiler.showColomn_AvgSelfTime;
                }
                top += height + num2;
                GUI.color = !SProfiler.showColomn_MaxTime ? Color.red : color;
                if (GUI.Button(new Rect(left, top, 88f, height), "[MAX TIME]"))
                {
                    SProfiler.showColomn_MaxTime = !SProfiler.showColomn_MaxTime;
                    column_props[13].hidden = column_props[14].hidden = column_props[15].hidden = !SProfiler.showColomn_MaxTime;
                }
                top += height + num2;
                GUI.color = !SProfiler.showColomn_MaxSelfTime ? Color.red : color;
                if (GUI.Button(new Rect(left, top, 88f, height), "[MAX SELF]"))
                {
                    SProfiler.showColomn_MaxSelfTime = !SProfiler.showColomn_MaxSelfTime;
                    column_props[0x10].hidden = column_props[0x11].hidden = column_props[0x12].hidden = !SProfiler.showColomn_MaxSelfTime;
                }
                top += height + num2;
                GUI.color = !SProfiler.showColomn_Count ? Color.red : color;
                if (GUI.Button(new Rect(left, top, 88f, height), "[COUNT]"))
                {
                    SProfiler.showColomn_Count = !SProfiler.showColomn_Count;
                    column_props[0x13].hidden = column_props[20].hidden = column_props[0x15].hidden = !SProfiler.showColomn_Count;
                }
                top += height + num2;
            }
        }

        private void DrawGroupColumns()
        {
            GUI.color = Color.green;
            float left = 92f;
            float top = 30f;
            float height = Screen.height - top;
            for (int i = 0; i < column_props.Length; i++)
            {
                SProfiler.ColumnProp prop = column_props[i];
                if ((!prop.showInCompareMode || this.compareMode) && (!this.compareMode || !prop.hidden))
                {
                    if (this.compareMode && (prop.colorIndex != -1))
                    {
                        GUI.color = column_colors[prop.colorIndex];
                    }
                    else
                    {
                        GUI.color = Color.green;
                    }
                    GUI.Box(new Rect(left, top, (float) prop.width, height), string.Empty);
                    GUI.Label(new Rect(left + 4f, top, (float) (prop.width - 8), height), prop.data);
                    if (GUI.Button(new Rect(left, 0f, (float) prop.width, top), prop.name))
                    {
                        if (this.curSortType == i)
                        {
                            this.highToLow = !this.highToLow;
                        }
                        else
                        {
                            this.curSortType = (SProfiler.SortType) i;
                        }
                    }
                    left += prop.width + 2f;
                }
            }
        }

        public void DrawGroups()
        {
            if (!this.showProfileFiles)
            {
                if (SProfiler.showData)
                {
                    this.SortGroups();
                    this.BuildGroupColumns();
                    this.DrawGroupColumns();
                }
                this.DrawButtons();
            }
            else
            {
                this.DrawProfileFiles();
            }
        }

        public void DrawGroups_MaxSelfTimeOnly()
        {
            this.groupListForSort.Clear();
            for (int i = 0; i < this.groupList.Count; i++)
            {
                SProfiler.Group item = this.groupList[i];
                item.flush(this.totalTime);
                item.checkClean();
                if (item.maxSelfTime > SProfiler.MaxSelfTimeThreshold)
                {
                    this.groupListForSort.Add(item);
                }
            }
            string text = string.Empty;
            if (this.groupListForSort.Count > 0)
            {
                if (<>f__am$cache14 == null)
                {
                    <>f__am$cache14 = (g0, g1) => g0.timeToClean.CompareTo(g1.timeToClean);
                }
                this.groupListForSort.Sort(<>f__am$cache14);
                StringBuilder builder = new StringBuilder();
                builder.Append("<size=24>");
                for (int j = 0; j < this.groupListForSort.Count; j++)
                {
                    SProfiler.Group group2 = this.groupListForSort[j];
                    builder.Append(group2.name);
                    builder.AppendFormat(":\t\t{0:F2}\n", group2.maxSelfTime);
                }
                builder.Append("</size>");
                text = builder.ToString();
            }
            float left = 20f;
            float height = 30f;
            float num7 = Screen.height - height;
            GUI.color = Color.green;
            float width = 80f;
            if (GUI.Button(new Rect(left, 0f, width, height), "RESET"))
            {
                this.Cleanup();
            }
            left += width + 10f;
            if (GUI.Button(new Rect(left, 0f, width, height), "MODE"))
            {
                SProfiler.showMaxOnly = !SProfiler.showMaxOnly;
                this.Cleanup();
            }
            left += width + 10f;
            GUI.color = !SProfiler.paused ? GUI.color : Color.red;
            if (GUI.Button(new Rect(left, 0f, width, height), "PAUSE"))
            {
                SProfiler.paused = !SProfiler.paused;
            }
            GUI.color = Color.yellow;
            GUI.Label(new Rect(0f, height, (float) Screen.width, num7), text);
            GUI.color = Color.white;
        }

        private void DrawProfileFiles()
        {
            GUI.color = Color.white;
            float top = 0f;
            float left = 2f;
            float height = 30f;
            float width = 500f;
            float num5 = 4f;
            if (GUI.Button(new Rect(left, top, 88f, height), "返回"))
            {
                this.showProfileFiles = false;
                if (this.compareObj == null)
                {
                    this.compareMode = false;
                }
            }
            left += 90f;
            if (GUI.Button(new Rect(left, top, 88f, height), "刷新"))
            {
                this.RefreshProfileFiles();
            }
            left = 2f;
            top += height + num5;
            if ((this.profileFiles != null) && (this.profileFiles.Length > 0))
            {
                int index = -1;
                for (int i = 0; i < this.profileFiles.Length; i++)
                {
                    string text = this.profileFiles[i];
                    int num8 = Mathf.Max(text.LastIndexOf('/'), text.LastIndexOf('\\'));
                    text = text.Substring(num8 + 1);
                    if (GUI.Button(new Rect(left, top, width, height), text))
                    {
                        index = i;
                    }
                    top += height + num5;
                }
                if (index != -1)
                {
                    this.showProfileFiles = false;
                    SProfiler.ProfileObj src = LoadFromFile(this.profileFiles[index]);
                    if (this.compareMode)
                    {
                        if (src == null)
                        {
                            this.compareMode = false;
                        }
                        this.compareObj = src;
                    }
                    else
                    {
                        this.CopyFrom(src);
                    }
                    SProfiler.Pause();
                }
            }
        }

        public void End()
        {
            if (((this.stackCount != 0) && !SProfiler.paused) && SProfiler.showGUI)
            {
                SProfiler.Sample sample = this.stacks[this.stackCount - 1];
                this.stacks[this.stackCount--] = null;
                double num = sample.end();
                if (this.stackCount > 0)
                {
                    SProfiler.Sample sample2 = this.stacks[this.stackCount - 1];
                    sample2.stackTime += num;
                    sample2.stackTimeThisCall += num;
                }
                else
                {
                    this.totalTime += num;
                }
            }
        }

        public int getIdentifity(string name)
        {
            long hashCode = name.GetHashCode();
            if (this.stackCount > 0)
            {
                long identity = this.stacks[this.stackCount - 1].identity;
                hashCode |= identity << 0x20;
            }
            int num3 = 0;
            if (!this.idmap.TryGetValue(hashCode, out num3))
            {
                num3 = ++this.idAllocator;
                this.idmap.Add(hashCode, num3);
            }
            return num3;
        }

        private static void LinkGroupList(SProfiler.ProfileObj obj1, SProfiler.ProfileObj obj2)
        {
            int count = obj1.groupList.Count;
            for (int i = 0; i < count; i++)
            {
                SProfiler.Group group = obj1.groupList[i];
                SProfiler.Group group2 = null;
                if (group.groupToCompare == null)
                {
                    if (!obj2.groups.TryGetValue(group.name, out group2))
                    {
                        group2 = new SProfiler.Group {
                            name = group.name
                        };
                        obj2.groups.Add(group2.name, group2);
                        obj2.groupList.Add(group2);
                    }
                    group2.groupToCompare = group;
                    group.groupToCompare = group2;
                }
            }
        }

        public static SProfiler.ProfileObj LoadFromFile(string path)
        {
            SProfiler.ProfileObj obj2 = null;
            FileStream serializationStream = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                serializationStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                obj2 = formatter.Deserialize(serializationStream) as SProfiler.ProfileObj;
                if (obj2 != null)
                {
                    obj2.PostLoad();
                }
            }
            catch (Exception exception)
            {
                obj2 = null;
                Debug.LogException(exception);
            }
            finally
            {
                if (serializationStream != null)
                {
                    serializationStream.Close();
                    serializationStream.Dispose();
                    serializationStream = null;
                }
            }
            return obj2;
        }

        public void PostLoad()
        {
            if (this.groups == null)
            {
                this.groups = new Dictionary<string, SProfiler.Group>();
            }
            if (this.groupList == null)
            {
                this.groupList = new List<SProfiler.Group>();
            }
            if (this.stacks == null)
            {
                this.stacks = new SProfiler.Sample[0x20];
            }
            foreach (SProfiler.Sample sample in this.samples.Values)
            {
                SProfiler.Group group = null;
                if (!this.groups.TryGetValue(sample.name, out group))
                {
                    group = new SProfiler.Group {
                        name = sample.name
                    };
                    this.groups.Add(sample.name, group);
                    this.groupList.Add(group);
                }
                group.samples.Add(sample);
            }
        }

        private void RefreshProfileFiles()
        {
            this.profileFiles = null;
            string logRootPath = DebugHelper.logRootPath;
            if (Directory.Exists(logRootPath))
            {
                this.profileFiles = Directory.GetFiles(logRootPath, "*.spf", SearchOption.TopDirectoryOnly);
            }
        }

        public void Reset()
        {
            Dictionary<int, SProfiler.Sample>.Enumerator enumerator = this.samples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, SProfiler.Sample> current = enumerator.Current;
                current.Value.reset();
            }
            this.totalTime = 0.0;
            this.compareMode = false;
            this.compareObj = null;
            SProfiler.profileFrameCount = 0;
        }

        public void SaveProfileData()
        {
            string str = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string path = string.Format("{0}/{1}.spf", DebugHelper.logRootPath, str);
            this.SaveToFile(path);
        }

        public void SaveToFile(string path)
        {
            FileStream serializationStream = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                serializationStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(serializationStream, this);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (serializationStream != null)
                {
                    serializationStream.Close();
                    serializationStream.Dispose();
                    serializationStream = null;
                }
            }
        }

        public void SortGroups()
        {
            <SortGroups>c__AnonStorey3D storeyd = new <SortGroups>c__AnonStorey3D();
            for (int i = 0; i < this.groupList.Count; i++)
            {
                this.groupList[i].flush(this.totalTime);
            }
            if (this.compareMode)
            {
                for (int j = 0; j < this.compareObj.groupList.Count; j++)
                {
                    this.compareObj.groupList[j].flush(this.compareObj.totalTime);
                }
                LinkGroupList(this, this.compareObj);
                LinkGroupList(this.compareObj, this);
            }
            storeyd.le = !this.highToLow ? -1 : 1;
            storeyd.ge = !this.highToLow ? 1 : -1;
            switch (this.curSortType)
            {
                case SProfiler.SortType.Name:
                    if (!this.highToLow)
                    {
                        if (<>f__am$cache13 == null)
                        {
                            <>f__am$cache13 = (x, y) => string.Compare(x.name, y.name);
                        }
                        this.groupList.Sort(<>f__am$cache13);
                        break;
                    }
                    if (<>f__am$cache12 == null)
                    {
                        <>f__am$cache12 = (x, y) => -string.Compare(x.name, y.name);
                    }
                    this.groupList.Sort(<>f__am$cache12);
                    break;

                case SProfiler.SortType.Time:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__35));
                    break;

                case SProfiler.SortType.Time_Diff:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__36));
                    break;

                case SProfiler.SortType.SelfTime:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__3B));
                    break;

                case SProfiler.SortType.SelfTime_Diff:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__3C));
                    break;

                case SProfiler.SortType.AvgTime:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__3F));
                    break;

                case SProfiler.SortType.AvgTime_Diff:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__40));
                    break;

                case SProfiler.SortType.AvgSelfTime:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__41));
                    break;

                case SProfiler.SortType.AvgSelfTime_Diff:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__42));
                    break;

                case SProfiler.SortType.MaxTime:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__37));
                    break;

                case SProfiler.SortType.MaxTime_Diff:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__38));
                    break;

                case SProfiler.SortType.MaxSelfTime:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__39));
                    break;

                case SProfiler.SortType.MaxSelfTime_Diff:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__3A));
                    break;

                case SProfiler.SortType.Count:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__3D));
                    break;

                case SProfiler.SortType.Count_Diff:
                    this.groupList.Sort(new Comparison<SProfiler.Group>(storeyd.<>m__3E));
                    break;
            }
        }

        [CompilerGenerated]
        private sealed class <SortGroups>c__AnonStorey3D
        {
            internal int ge;
            internal int le;

            internal int <>m__35(SProfiler.Group x, SProfiler.Group y)
            {
                if (x.time < y.time)
                {
                    return this.le;
                }
                if (x.time == y.time)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__36(SProfiler.Group x, SProfiler.Group y)
            {
                double num = x.time - x.groupToCompare.time;
                double num2 = y.time - y.groupToCompare.time;
                if (num < num2)
                {
                    return this.le;
                }
                if (num == num2)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__37(SProfiler.Group x, SProfiler.Group y)
            {
                if (x.maxTime < y.maxTime)
                {
                    return this.le;
                }
                if (x.maxTime == y.maxTime)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__38(SProfiler.Group x, SProfiler.Group y)
            {
                double num = x.maxTime - x.groupToCompare.maxTime;
                double num2 = y.maxTime - y.groupToCompare.maxTime;
                if (num < num2)
                {
                    return this.le;
                }
                if (num == num2)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__39(SProfiler.Group x, SProfiler.Group y)
            {
                if (x.maxSelfTime < y.maxSelfTime)
                {
                    return this.le;
                }
                if (x.maxSelfTime == y.maxSelfTime)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__3A(SProfiler.Group x, SProfiler.Group y)
            {
                double num = x.maxSelfTime - x.groupToCompare.maxSelfTime;
                double num2 = y.maxSelfTime - y.groupToCompare.maxSelfTime;
                if (num < num2)
                {
                    return this.le;
                }
                if (num == num2)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__3B(SProfiler.Group x, SProfiler.Group y)
            {
                if (x.selfTime < y.selfTime)
                {
                    return this.le;
                }
                if (x.selfTime == y.selfTime)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__3C(SProfiler.Group x, SProfiler.Group y)
            {
                double num = x.selfTime - x.groupToCompare.selfTime;
                double num2 = y.selfTime - y.groupToCompare.selfTime;
                if (num < num2)
                {
                    return this.le;
                }
                if (num == num2)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__3D(SProfiler.Group x, SProfiler.Group y)
            {
                if (x.count < y.count)
                {
                    return this.le;
                }
                if (x.count == y.count)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__3E(SProfiler.Group x, SProfiler.Group y)
            {
                int num = x.count - x.groupToCompare.count;
                int num2 = y.count - y.groupToCompare.count;
                if (num < num2)
                {
                    return this.le;
                }
                if (num == num2)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__3F(SProfiler.Group x, SProfiler.Group y)
            {
                double num = (x.count == 0) ? 0.0 : (x.time / ((double) x.count));
                double num2 = (y.count == 0) ? 0.0 : (y.time / ((double) y.count));
                if (num < num2)
                {
                    return this.le;
                }
                if (num == num2)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__40(SProfiler.Group x, SProfiler.Group y)
            {
                double num = (x.count == 0) ? 0.0 : (x.time / ((double) x.count));
                double num2 = (y.count == 0) ? 0.0 : (y.time / ((double) y.count));
                double num3 = (x.groupToCompare.count == 0) ? 0.0 : (x.groupToCompare.time / ((double) x.groupToCompare.count));
                double num4 = (y.groupToCompare.count == 0) ? 0.0 : (y.groupToCompare.time / ((double) y.groupToCompare.count));
                double num5 = num - num3;
                double num6 = num2 - num4;
                if (num5 < num6)
                {
                    return this.le;
                }
                if (num5 == num6)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__41(SProfiler.Group x, SProfiler.Group y)
            {
                double num = (x.count == 0) ? 0.0 : (x.selfTime / ((double) x.count));
                double num2 = (y.count == 0) ? 0.0 : (y.selfTime / ((double) y.count));
                if (num < num2)
                {
                    return this.le;
                }
                if (num == num2)
                {
                    return 0;
                }
                return this.ge;
            }

            internal int <>m__42(SProfiler.Group x, SProfiler.Group y)
            {
                double num = (x.count == 0) ? 0.0 : (x.selfTime / ((double) x.count));
                double num2 = (y.count == 0) ? 0.0 : (y.selfTime / ((double) y.count));
                double num3 = (x.groupToCompare.count == 0) ? 0.0 : (x.groupToCompare.selfTime / ((double) x.groupToCompare.count));
                double num4 = (y.groupToCompare.count == 0) ? 0.0 : (y.groupToCompare.selfTime / ((double) y.groupToCompare.count));
                double num5 = num - num3;
                double num6 = num2 - num4;
                if (num5 < num6)
                {
                    return this.le;
                }
                if (num5 == num6)
                {
                    return 0;
                }
                return this.ge;
            }
        }
    }

    [Serializable]
    public class Sample
    {
        public int count;
        public int identity;
        public double maxSelfTime;
        public double maxTime;
        public string name;
        public double profilerTime0;
        public double profilerTime1;
        public double profilerTimeThisCall0;
        public double profilerTimeThisCall1;
        public double stackTime;
        public double stackTimeThisCall;
        public double start;
        public double time;

        public void begin()
        {
            this.stackTimeThisCall = 0.0;
            this.profilerTimeThisCall0 = 0.0;
            this.profilerTimeThisCall1 = 0.0;
            this.start = SProfiler.STimer.currentTime;
        }

        public double end()
        {
            double num = SProfiler.STimer.currentTime - this.start;
            this.time += num;
            this.maxTime = Math.Max(this.maxTime, num - this.profilerTimeThisCall0);
            this.maxSelfTime = Math.Max(this.maxSelfTime, (num - this.stackTimeThisCall) - this.profilerTimeThisCall1);
            this.count++;
            return num;
        }

        public void reset()
        {
            this.time = 0.0;
            this.start = 0.0;
            this.stackTime = 0.0;
            this.stackTimeThisCall = 0.0;
            this.profilerTime0 = 0.0;
            this.profilerTime1 = 0.0;
            this.profilerTimeThisCall0 = 0.0;
            this.profilerTimeThisCall1 = 0.0;
            this.count = 0;
            this.maxTime = 0.0;
            this.maxSelfTime = 0.0;
        }
    }

    public enum SortType
    {
        Name,
        Time,
        Time2,
        Time_Diff,
        SelfTime,
        SelfTime2,
        SelfTime_Diff,
        AvgTime,
        AvgTime2,
        AvgTime_Diff,
        AvgSelfTime,
        AvgSelfTime2,
        AvgSelfTime_Diff,
        MaxTime,
        MaxTime2,
        MaxTime_Diff,
        MaxSelfTime,
        MaxSelfTime2,
        MaxSelfTime_Diff,
        Count,
        Count2,
        Count_Diff
    }

    private class STimer
    {
        public static double currentTime
        {
            get
            {
                return (double) Time.realtimeSinceStartup;
            }
        }
    }
}

