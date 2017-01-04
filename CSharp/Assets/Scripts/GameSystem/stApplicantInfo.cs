namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stApplicantInfo
    {
        public stGuildMemBriefInfo stBriefInfo;
        public int dwApplyTime;
        public void Reset()
        {
            this.stBriefInfo.Reset();
            this.dwApplyTime = 0;
        }
    }
}

