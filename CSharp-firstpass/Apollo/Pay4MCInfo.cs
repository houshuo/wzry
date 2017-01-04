namespace Apollo
{
    using System;

    public class Pay4MCInfo : Pay4MonthInfo
    {
        public string productId;

        public Pay4MCInfo()
        {
            base.Name = 1;
            base.Action = 2;
            base.serviceType = APO_PAY_MONTH_TYPE.APO_SERVICETYPE_NORMAL;
            base.autoPay = 0;
        }

        public override void ReadFrom(ApolloBufferReader reader)
        {
            base.ReadFrom(reader);
            reader.Read(ref this.productId);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(this.productId);
        }
    }
}

