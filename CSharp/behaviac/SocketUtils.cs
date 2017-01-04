namespace behaviac
{
    using System;

    public static class SocketUtils
    {
        public static void Flush()
        {
        }

        public static int GetMemoryOverhead()
        {
            return 0;
        }

        public static int GetNumTrackedThreads()
        {
            return 0;
        }

        public static bool ReadText(ref string text)
        {
            return false;
        }

        public static void SendText(string text)
        {
        }

        public static void SendWorkspace(string text)
        {
        }

        public static void SendWorkspaceSettings()
        {
        }

        public static bool SetupConnection(bool bBlocking)
        {
            return false;
        }

        public static bool SetupConnection(bool bBlocking, ushort port)
        {
            return false;
        }

        public static void ShutdownConnection()
        {
        }

        public static void UpdatePacketsStats()
        {
        }
    }
}

