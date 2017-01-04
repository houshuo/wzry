namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ChangeAnimParam
    {
        public string originalAnimName;
        public string changedAnimName;
    }
}

