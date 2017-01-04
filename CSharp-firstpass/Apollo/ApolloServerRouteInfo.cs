namespace Apollo
{
    using System;

    public class ApolloServerRouteInfo : ApolloRouteInfoBase
    {
        public ulong ServerId;

        public ApolloServerRouteInfo() : base(ApolloRouteType.Server)
        {
            this.ServerId = 0L;
        }

        public ApolloServerRouteInfo(ulong serverId) : base(ApolloRouteType.Server)
        {
            this.ServerId = serverId;
        }

        public ApolloServerRouteInfo FromString(string data)
        {
            this.ServerId = new ApolloStringParser(data).GetUInt32("ServerId");
            return this;
        }

        protected override ApolloRouteInfoBase onCopyInstance()
        {
            return new ApolloServerRouteInfo { ServerId = this.ServerId };
        }

        public override void ReadFrom(ApolloBufferReader reader)
        {
            base.ReadFrom(reader);
            reader.Read(ref this.ServerId);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(this.ServerId);
        }
    }
}

