namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class ApolloBufferBase
    {
        protected ApolloBufferBase()
        {
        }

        protected virtual void BeforeDecode(ApolloBufferReader reader)
        {
        }

        protected virtual void BeforeEncode(ApolloBufferWriter writer)
        {
        }

        public bool Decode(byte[] data)
        {
            if (data != null)
            {
                try
                {
                    ApolloBufferReader reader = new ApolloBufferReader(data);
                    this.BeforeDecode(reader);
                    this.ReadFrom(reader);
                    return true;
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                    return false;
                }
            }
            return false;
        }

        public bool Encode(out byte[] buffer)
        {
            try
            {
                ApolloBufferWriter writer = new ApolloBufferWriter();
                this.BeforeEncode(writer);
                this.WriteTo(writer);
                buffer = writer.GetBufferData();
                return true;
            }
            catch (Exception exception)
            {
                buffer = null;
                ADebug.LogException(exception);
                return false;
            }
        }

        public abstract void ReadFrom(ApolloBufferReader reader);
        public abstract void WriteTo(ApolloBufferWriter writer);
    }
}

