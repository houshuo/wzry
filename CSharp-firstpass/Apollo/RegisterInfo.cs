namespace Apollo
{
    using System;

    public class RegisterInfo : ApolloBufferBase
    {
        public byte enableLog;
        public string environment;
        [Obsolete("Deprecated since 1.1.12")]
        public string offerId;
        [Obsolete("Deprecated since 1.1.12")]
        public string propUnit;

        public override void ReadFrom(ApolloBufferReader reader)
        {
            reader.Read(ref this.offerId);
            reader.Read(ref this.environment);
            reader.Read(ref this.propUnit);
            reader.Read(ref this.enableLog);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            writer.Write(this.offerId);
            writer.Write(this.environment);
            writer.Write(this.propUnit);
            writer.Write(this.enableLog);
        }
    }
}

