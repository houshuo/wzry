namespace Apollo
{
    using System;
    using System.IO;

    public class ADebug
    {
        private static ADebugImplement implement = new ADebugImplement();
        public static LogPriority Level = LogPriority.Error;

        public static void Log(object message)
        {
            if (Level <= LogPriority.Info)
            {
                implement.Log(message);
            }
        }

        public static void LogError(object message)
        {
            if (Level <= LogPriority.Error)
            {
                implement.LogError(message);
            }
        }

        public static void LogException(Exception exception)
        {
            if (Level <= LogPriority.Error)
            {
                implement.LogException(exception);
            }
        }

        public static void LogHex(string prefix, byte[] data)
        {
            if ((Level <= LogPriority.Info) && (data != null))
            {
                string str = string.Empty;
                foreach (byte num in data)
                {
                    str = str + num.ToString("X") + " ";
                }
                Log(prefix + "[" + str + "]");
            }
        }

        private class ADebugImplement : ApolloObject
        {
            private FileStream fileStream;
            private StreamWriter streamWriter;

            public ADebugImplement() : base(false, true)
            {
                this.init();
            }

            private string formatMessage(object message)
            {
                return (DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:fff] ") + message);
            }

            private void init()
            {
                if (this.fileStream == null)
                {
                }
            }

            public void Log(object message)
            {
                string log = this.formatMessage(message);
                this.writeToFile(log);
            }

            public void LogError(object message)
            {
                string log = this.formatMessage(message);
                this.writeToFile(log);
            }

            public void LogException(Exception exception)
            {
                this.writeToFile(exception.ToString());
            }

            public override void OnDisable()
            {
            }

            private void writeToFile(string log)
            {
            }
        }

        public enum LogPriority
        {
            Info,
            Error,
            None
        }
    }
}

