namespace ApolloTdr
{
    using System;
    using System.Net;
    using System.Runtime.InteropServices;

    public class TdrTypeUtil
    {
        public static int cstrlen(byte[] str)
        {
            byte num = 0;
            int num2 = 0;
            for (int i = 0; i < str.GetLength(0); i++)
            {
                if (num == str[i])
                {
                    return num2;
                }
                num2++;
            }
            return num2;
        }

        public static TdrError.ErrorType str2TdrDate(out uint date, string strDate)
        {
            DateTime time;
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            TdrDate date2 = new TdrDate();
            if (DateTime.TryParse(strDate, out time))
            {
                date2.nYear = (short) time.Year;
                date2.bMon = (byte) time.Month;
                date2.bDay = (byte) time.Day;
                date2.toDate(out date);
                return type;
            }
            date = 0;
            return TdrError.ErrorType.TDR_ERR_INVALID_TDRDATE_VALUE;
        }

        public static TdrError.ErrorType str2TdrDateTime(out ulong datetime, string strDateTime)
        {
            DateTime time;
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            TdrDateTime time2 = new TdrDateTime();
            if (DateTime.TryParse(strDateTime, out time))
            {
                time2.tdrDate.nYear = (short) time.Year;
                time2.tdrDate.bMon = (byte) time.Month;
                time2.tdrDate.bDay = (byte) time.Day;
                time2.tdrTime.nHour = (short) time.TimeOfDay.Hours;
                time2.tdrTime.bMin = (byte) time.TimeOfDay.Minutes;
                time2.tdrTime.bSec = (byte) time.TimeOfDay.Seconds;
                time2.toDateTime(out datetime);
                return type;
            }
            datetime = 0L;
            return TdrError.ErrorType.TDR_ERR_INVALID_TDRDATETIME_VALUE;
        }

        public static TdrError.ErrorType str2TdrIP(out uint ip, string strip)
        {
            IPAddress address;
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if (IPAddress.TryParse(strip, out address))
            {
                byte[] addressBytes = address.GetAddressBytes();
                ip = (uint) ((((addressBytes[3] << 0x18) | (addressBytes[2] << 0x10)) | (addressBytes[1] << 8)) | addressBytes[0]);
                return type;
            }
            ip = 0;
            return TdrError.ErrorType.TDR_ERR_INVALID_TDRIP_VALUE;
        }

        public static TdrError.ErrorType str2TdrTime(out uint time, string strTime)
        {
            DateTime time2;
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            TdrTime time3 = new TdrTime();
            if (DateTime.TryParse(strTime, out time2))
            {
                time3.nHour = (short) time2.TimeOfDay.Hours;
                time3.bMin = (byte) time2.TimeOfDay.Minutes;
                time3.bSec = (byte) time2.TimeOfDay.Seconds;
                time3.toTime(out time);
                return type;
            }
            time = 0;
            return TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
        }

        public static TdrError.ErrorType tdrDate2Str(ref TdrVisualBuf buf, uint date)
        {
            TdrDate date2 = new TdrDate();
            if (date2.parse(date) == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { date2.nYear, date2.bMon, date2.bDay };
                return buf.sprintf("{0:d4}-{1:d2}-{2:d2}", args);
            }
            return TdrError.ErrorType.TDR_ERR_INVALID_TDRDATE_VALUE;
        }

        public static TdrError.ErrorType tdrDateTime2Str(ref TdrVisualBuf buf, ulong datetime)
        {
            TdrDateTime time = new TdrDateTime();
            if (time.parse(datetime) == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { time.tdrDate.nYear, time.tdrDate.bMon, time.tdrDate.bDay, time.tdrTime.nHour, time.tdrTime.bMin, time.tdrTime.bSec };
                return buf.sprintf("{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}", args);
            }
            return TdrError.ErrorType.TDR_ERR_INVALID_TDRDATETIME_VALUE;
        }

        public static TdrError.ErrorType tdrIP2Str(ref TdrVisualBuf buf, uint ip)
        {
            string str = new IPAddress((long) ip).ToString();
            object[] args = new object[] { str };
            return buf.sprintf("{0}", args);
        }

        public static TdrError.ErrorType tdrTime2Str(ref TdrVisualBuf buf, uint time)
        {
            TdrTime time2 = new TdrTime();
            if (time2.parse(time) == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { time2.nHour, time2.bMin, time2.bSec };
                return buf.sprintf("{0:d2}:{1:d2}:{2:d2}", args);
            }
            return TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
        }

        public static int wstrlen(short[] str)
        {
            short num = 0;
            int num2 = 0;
            for (int i = 0; i < str.GetLength(0); i++)
            {
                if (num == str[i])
                {
                    return num2;
                }
                num2++;
            }
            return num2;
        }
    }
}

