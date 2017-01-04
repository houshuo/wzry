namespace tsf4g_tdr_csharp
{
    using System;
    using System.Runtime.InteropServices;

    public class TdrDateTime
    {
        public TdrDate tdrDate;
        public TdrTime tdrTime;

        public TdrDateTime()
        {
            this.tdrDate = new TdrDate();
            this.tdrTime = new TdrTime();
        }

        public TdrDateTime(ulong datetime)
        {
            this.tdrDate = new TdrDate((uint) (datetime & 0xffffffffL));
            this.tdrTime = new TdrTime((uint) ((datetime >> 0x20) & 0xffffffffL));
        }

        public bool isValid()
        {
            return (this.tdrDate.isValid() && this.tdrTime.isValid());
        }

        public TdrError.ErrorType parse(ulong datetime)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            uint date = (uint) (datetime & 0xffffffffL);
            uint time = (uint) ((datetime >> 0x20) & 0xffffffffL);
            type = this.tdrDate.parse(date);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.tdrTime.parse(time);
            }
            return type;
        }

        public void toDateTime(out ulong datetime)
        {
            uint date = 0;
            uint time = 0;
            this.tdrDate.toDate(out date);
            this.tdrTime.toTime(out time);
            datetime = date | (time << 0x20);
        }
    }
}

