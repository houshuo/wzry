namespace tsf4g_tdr_csharp
{
    using System;
    using System.Runtime.InteropServices;

    public class TdrTime
    {
        public byte bMin;
        public byte bSec;
        public short nHour;

        public TdrTime()
        {
        }

        public TdrTime(uint time)
        {
            this.nHour = (short) (time & 0xffff);
            this.bMin = (byte) ((time >> 0x10) & 0xff);
            this.bSec = (byte) ((time >> 0x18) & 0xff);
        }

        public bool isValid()
        {
            DateTime time;
            if (!DateTime.TryParse(string.Format("{0:d2}:{1:d2}:{2:d2}", this.nHour, this.bMin, this.bSec), out time))
            {
                return false;
            }
            return true;
        }

        public TdrError.ErrorType parse(uint time)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.nHour = (short) (time & 0xffff);
            this.bMin = (byte) ((time >> 0x10) & 0xff);
            this.bSec = (byte) ((time >> 0x18) & 0xff);
            if (!this.isValid())
            {
                type = TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
            }
            return type;
        }

        public void toTime(out uint time)
        {
            time = (uint) ((((ushort) this.nHour) | (this.bMin << 0x10)) | (this.bSec << 0x18));
        }
    }
}

