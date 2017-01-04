namespace com.tencent.pandora
{
    using System;

    public class LuaScriptException : LuaException
    {
        private bool isNet;
        private readonly string source;

        public LuaScriptException(Exception innerException, string source) : base(innerException.Message, innerException)
        {
            this.source = source;
            this.IsNetException = true;
        }

        public LuaScriptException(string message, string source) : base(message)
        {
            this.source = source;
        }

        public override string ToString()
        {
            return (this.GetType().FullName + ": " + this.source + this.Message);
        }

        public bool IsNetException
        {
            get
            {
                return this.isNet;
            }
            set
            {
                this.isNet = value;
            }
        }

        public override string Source
        {
            get
            {
                return this.source;
            }
        }
    }
}

