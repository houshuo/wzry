namespace Apollo.Plugins.Msdk
{
    using Apollo;
    using System;

    public class AccountInitInfo : ApolloBufferBase
    {
        public uint Permission;

        public AccountInitInfo(uint permission)
        {
            this.Permission = permission;
        }

        public override void ReadFrom(ApolloBufferReader reader)
        {
            reader.Read(ref this.Permission);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            writer.Write(this.Permission);
        }
    }
}

