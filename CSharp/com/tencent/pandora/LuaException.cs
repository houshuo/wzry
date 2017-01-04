namespace com.tencent.pandora
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class LuaException : Exception
    {
        public LuaException()
        {
        }

        public LuaException(string message) : base(message)
        {
        }

        protected LuaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public LuaException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

