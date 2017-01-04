namespace behaviac
{
    using System;

    public static class Config
    {
        private static bool ms_bDebugging = false;
        private static readonly bool ms_bIsDesktopEditor = ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.OSXPlayer));
        private static readonly bool ms_bIsDesktopPlayer = ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.OSXPlayer));
        private static bool ms_bIsLogging = false;
        private static bool ms_bIsSocketing = false;
        private static bool ms_bIsSuppressingNonPublicWarning;
        private static bool ms_bProfiling = false;

        public static bool IsDebugging
        {
            get
            {
                return ms_bDebugging;
            }
            set
            {
                if (ms_bDebugging)
                {
                }
            }
        }

        public static bool IsDesktop
        {
            get
            {
                return (ms_bIsDesktopPlayer || ms_bIsDesktopEditor);
            }
        }

        public static bool IsDesktopEditor
        {
            get
            {
                return ms_bIsDesktopEditor;
            }
        }

        public static bool IsDesktopPlayer
        {
            get
            {
                return ms_bIsDesktopPlayer;
            }
        }

        public static bool IsLogging
        {
            get
            {
                return (IsDesktop && ms_bIsLogging);
            }
            set
            {
                if (ms_bIsLogging)
                {
                }
            }
        }

        public static bool IsLoggingOrSocketing
        {
            get
            {
                return (IsLogging || IsSocketing);
            }
        }

        public static bool IsProfiling
        {
            get
            {
                return ms_bProfiling;
            }
            set
            {
                if (ms_bProfiling)
                {
                }
            }
        }

        public static bool IsSocketing
        {
            get
            {
                return ms_bIsSocketing;
            }
            set
            {
                if (ms_bIsLogging)
                {
                }
            }
        }

        public static bool IsSuppressingNonPublicWarning
        {
            get
            {
                return ms_bIsSuppressingNonPublicWarning;
            }
            set
            {
                ms_bIsSuppressingNonPublicWarning = value;
            }
        }
    }
}

