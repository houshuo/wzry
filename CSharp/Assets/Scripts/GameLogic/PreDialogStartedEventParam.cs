namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PreDialogStartedEventParam
    {
        public int PreDialogId;
        public PreDialogStartedEventParam(int inPreDialogId)
        {
            this.PreDialogId = inPreDialogId;
        }
    }
}

