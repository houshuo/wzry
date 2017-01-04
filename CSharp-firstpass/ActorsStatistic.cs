using System;
using System.Text;
using UnityEngine;

public class ActorsStatistic
{
    public static long _heroCount;
    public static long _organCount;
    public static long _soldierCount;
    public static long _towerCount;
    public static int frameCount;
    public static bool sampleActors;

    public static void AddSample(int heroCount, int organCount, int towerCount, int soldierCount)
    {
        if (sampleActors)
        {
            frameCount++;
            _heroCount += heroCount;
            _organCount += organCount;
            _towerCount += towerCount;
            _soldierCount += soldierCount;
        }
    }

    public static void Draw()
    {
        double num = (frameCount == 0) ? 0.0 : (1.0 / ((double) frameCount));
        StringBuilder builder = new StringBuilder();
        builder.Append("<size=24><color=red>");
        builder.AppendLine("Actors Statistics:");
        builder.AppendLine(string.Format("Hero:    {0:F2}", num * _heroCount));
        builder.AppendLine(string.Format("Organ:   {0:F2}", num * _organCount));
        builder.AppendLine(string.Format("Tower:   {0:F2}", num * _towerCount));
        builder.AppendLine(string.Format("Soldier: {0:F2}", num * _soldierCount));
        builder.Append("</color></size>");
        GUI.Label(new Rect((float) (Screen.width / 2), 20f, (float) Screen.width, (float) Screen.height), builder.ToString());
    }

    public static void Reset()
    {
        frameCount = 0;
        _heroCount = 0L;
        _organCount = 0L;
        _towerCount = 0L;
        _soldierCount = 0L;
    }

    public static void Start()
    {
        Reset();
        sampleActors = true;
    }

    public static void Stop()
    {
        sampleActors = false;
    }
}

