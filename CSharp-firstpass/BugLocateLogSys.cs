using System;
using System.IO;
using UnityEngine;

public class BugLocateLogSys : Singleton<BugLocateLogSys>
{
    private StreamWriter m_sw;

    public static void Log(string msg)
    {
        Singleton<BugLocateLogSys>.instance.WriteLine<string>(msg);
        Debug.Log(msg);
    }

    public void WriteLine<T>(T content)
    {
        try
        {
            this.SW.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + content.ToString());
        }
        catch (Exception)
        {
        }
    }

    private StreamWriter SW
    {
        get
        {
            if (this.m_sw == null)
            {
                string path = string.Format("{0}/{1}.log", CFileManager.GetCachePath(), DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
                this.m_sw = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate));
                try
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                }
                catch (Exception)
                {
                }
                this.m_sw.AutoFlush = true;
            }
            return this.m_sw;
        }
    }
}

