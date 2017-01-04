namespace tsf4g_tdr_csharp
{
    using System;
    using System.Diagnostics;

    public class TdrDebug
    {
        public static void tdrTrace()
        {
            StackTrace trace = new StackTrace(true);
            for (int i = 1; i < trace.FrameCount; i++)
            {
                if (trace.GetFrame(i).GetFileName() != null)
                {
                    Console.WriteLine("TSF4G_TRACE:  " + trace.GetFrame(i).ToString());
                }
            }
        }
    }
}

