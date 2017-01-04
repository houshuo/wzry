namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StarCondition
    {
        public string ConditionName;
        public bool bCompelete;
    }
}

