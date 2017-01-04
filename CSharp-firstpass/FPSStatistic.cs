using System;
using System.Text;
using UnityEngine;

public class FPSStatistic
{
    public static int count = 0;
    public static Distribute[] distributes;
    public static float MaxFps = -9999f;
    public static float MinFps = 9999f;
    public static float profiler_fps;
    public static double profiler_lastTime;
    public static bool sampleFps;
    private static double total_ = 0.0;

    static FPSStatistic()
    {
        Distribute[] distributeArray1 = new Distribute[12];
        Distribute distribute = new Distribute {
            MinValue = 55f
        };
        distributeArray1[0] = distribute;
        distribute = new Distribute {
            MinValue = 50f
        };
        distributeArray1[1] = distribute;
        distribute = new Distribute {
            MinValue = 45f
        };
        distributeArray1[2] = distribute;
        distribute = new Distribute {
            MinValue = 40f
        };
        distributeArray1[3] = distribute;
        distribute = new Distribute {
            MinValue = 35f
        };
        distributeArray1[4] = distribute;
        distribute = new Distribute {
            MinValue = 30f
        };
        distributeArray1[5] = distribute;
        distribute = new Distribute {
            MinValue = 25f
        };
        distributeArray1[6] = distribute;
        distribute = new Distribute {
            MinValue = 20f
        };
        distributeArray1[7] = distribute;
        distribute = new Distribute {
            MinValue = 15f
        };
        distributeArray1[8] = distribute;
        distribute = new Distribute {
            MinValue = 10f
        };
        distributeArray1[9] = distribute;
        distribute = new Distribute {
            MinValue = 5f
        };
        distributeArray1[10] = distribute;
        distributeArray1[11] = new Distribute();
        distributes = distributeArray1;
        sampleFps = true;
        profiler_lastTime = -10.0;
        profiler_fps = 0f;
        for (int i = 0; i < distributes.Length; i++)
        {
            if (i == 0)
            {
                distributes[i].name = ">= " + distributes[i].MinValue;
            }
            else
            {
                distributes[i].name = distributes[i].MinValue + "-" + distributes[i - 1].MinValue;
            }
        }
    }

    private static void AddSample(float fps)
    {
        total_ += fps;
        count++;
        if (MinFps > fps)
        {
            MinFps = fps;
        }
        if (MaxFps < fps)
        {
            MaxFps = fps;
        }
        for (int i = 0; i < distributes.Length; i++)
        {
            Distribute distribute = distributes[i];
            if (fps >= distribute.MinValue)
            {
                distribute.count++;
                break;
            }
        }
    }

    public static void Draw()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("<size=24>");
        builder.AppendLine("FPS Statistics:");
        builder.AppendLine(string.Format("Frame: {0}", count));
        builder.AppendLine(string.Format("Avg:   {0:F2}", avgFps));
        builder.AppendLine(string.Format("Min:   {0:F2}", MinFps));
        builder.AppendLine(string.Format("Max:   {0:F2}", MaxFps));
        bool flag = false;
        for (int i = 0; i < distributes.Length; i++)
        {
            Distribute distribute = distributes[i];
            if ((distribute.count != 0) || flag)
            {
                flag = true;
                builder.AppendLine(string.Format("{0}: {1} ({2:F2}%)", distribute.name, distribute.count, (distribute.count * 100.0) / ((double) count)));
            }
        }
        builder.Append("</size>");
        Color color = GUI.color;
        GUI.color = Color.red;
        GUI.Label(new Rect(20f, 20f, (float) Screen.width, (float) Screen.height), builder.ToString());
        GUI.color = color;
    }

    public static void Reset()
    {
        count = 0;
        MinFps = 9999f;
        MaxFps = -9999f;
        total_ = 0.0;
        for (int i = 0; i < distributes.Length; i++)
        {
            distributes[i].count = 0;
        }
        profiler_lastTime = -10.0;
        profiler_fps = 0f;
    }

    public static void Start()
    {
        Reset();
        sampleFps = true;
    }

    public static void Stop()
    {
        sampleFps = false;
    }

    public static void Update()
    {
        if (sampleFps)
        {
            double realtimeSinceStartup = Time.realtimeSinceStartup;
            if (profiler_lastTime < 0.0)
            {
                profiler_lastTime = realtimeSinceStartup;
            }
            else
            {
                profiler_fps = 1f / ((float) (realtimeSinceStartup - profiler_lastTime));
                profiler_lastTime = realtimeSinceStartup;
            }
            AddSample(profiler_fps);
        }
    }

    public static float avgFps
    {
        get
        {
            return (float) (total_ / ((double) count));
        }
    }

    public class Distribute
    {
        public int count;
        public float MinValue;
        public string name;
    }
}

