namespace tsf4g_tdr_csharp
{
    using System;
    using System.Runtime.InteropServices;

    public class TdrDate
    {
        public byte bDay;
        public byte bMon;
        public short nYear;

        public TdrDate()
        {
        }

        public TdrDate(uint date)
        {
            this.nYear = (short) (date & 0xffff);
            this.bMon = (byte) ((date >> 0x10) & 0xff);
            this.bDay = (byte) ((date >> 0x18) & 0xff);
        }

        public bool isValid()
        {
            DateTime time;
            if (!DateTime.TryParse(string.Format("{0:d4}-{1:d2}-{2:d2}", this.nYear, this.bMon, this.bDay), out time))
            {
                return false;
            }
            return true;
        }

        public TdrError.ErrorType parse(uint date)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.nYear = (short) (date & 0xffff);
            this.bMon = (byte) ((date >> 0x10) & 0xff);
            this.bDay = (byte) ((date >> 0x18) & 0xff);
            if (!this.isValid())
            {
                type = TdrError.ErrorType.TDR_ERR_INVALID_TDRTIME_VALUE;
            }
            return type;
        }

        public void toDate(out uint date)
        {
            date = (uint) ((((ushort) this.nYear) | (this.bMon << 0x10)) | (this.bDay << 0x18));
        }
    }
}

