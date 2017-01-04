namespace Apollo
{
    using System;

    public class Pay4MonthInfo : PayInfo
    {
        public byte autoPay;
        public string remark;
        public string serviceCode;
        public string serviceName;
        public APO_PAY_MONTH_TYPE serviceType;

        public Pay4MonthInfo()
        {
            base.Name = 1;
            base.Action = 0x10;
            this.serviceType = APO_PAY_MONTH_TYPE.APO_SERVICETYPE_NORMAL;
            this.autoPay = 0;
        }

        public override void ReadFrom(ApolloBufferReader reader)
        {
            int v = 0;
            base.ReadFrom(reader);
            reader.Read(ref this.serviceCode);
            reader.Read(ref this.serviceName);
            reader.Read(ref this.remark);
            reader.Read(ref v);
            this.serviceType = (APO_PAY_MONTH_TYPE) v;
            reader.Read(ref this.autoPay);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(this.serviceCode);
            writer.Write(this.serviceName);
            writer.Write(this.remark);
            writer.Write((int) this.serviceType);
            writer.Write(this.autoPay);
        }
    }
}

