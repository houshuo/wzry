namespace Apollo
{
    using System;

    public sealed class NoneAccountInitInfo : ApolloBufferBase
    {
        public string OpenId;

        public NoneAccountInitInfo(string openID)
        {
            this.OpenId = openID;
        }

        public override void ReadFrom(ApolloBufferReader reader)
        {
            reader.Read(ref this.OpenId);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            writer.Write(this.OpenId);
        }
    }
}

