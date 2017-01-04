namespace Apollo
{
    using System;

    public class Pay4MPItemInfo : PayInfo
    {
        public string reqType;

        public Pay4MPItemInfo()
        {
            base.Name = 1;
            base.Action = 8;
            this.reqType = "mp";
        }

        public override void ReadFrom(ApolloBufferReader reader)
        {
            base.ReadFrom(reader);
            reader.Read(ref this.reqType);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(this.reqType);
        }
    }
}

