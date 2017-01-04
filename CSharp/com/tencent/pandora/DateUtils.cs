namespace com.tencent.pandora
{
    using System;

    internal class DateUtils
    {
        public static int ConvertDateTimeInt(DateTime time)
        {
            DateTime time2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(0x7b2, 1, 1));
            TimeSpan span = (TimeSpan) (time - time2);
            return (int) span.TotalSeconds;
        }

        public static int GetCurTimestamp()
        {
            return ConvertDateTimeInt(DateTime.Now);
        }

        public static string UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
            return time.AddSeconds((double) unixTimeStamp).ToLocalTime().ToString();
        }
    }
}

