namespace Apollo
{
    using System;

    public class CTdirServiceTable : ApolloBufferBase
    {
        public byte[] appBuff;
        public uint appLen;
        public uint bitMap;
        public uint updateTime;
        public uint userAttr;
        public int zoneID;

        public override void ReadFrom(ApolloBufferReader reader)
        {
            reader.Read(ref this.updateTime);
            reader.Read(ref this.bitMap);
            reader.Read(ref this.userAttr);
            reader.Read(ref this.zoneID);
            reader.Read(ref this.appLen);
            reader.Read(ref this.appBuff);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            writer.Write(this.updateTime);
            writer.Write(this.bitMap);
            writer.Write(this.userAttr);
            writer.Write(this.zoneID);
            writer.Write(this.appLen);
            writer.Write(this.appBuff);
        }
    }
}

