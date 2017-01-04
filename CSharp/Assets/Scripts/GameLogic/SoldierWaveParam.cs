namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SoldierWaveParam
    {
        public int WaveIndex;
        public int RepeatCount;
        public int NextDuration;
        public SoldierWaveParam(int inWaveIndex, int inRepeatCount, int inNextDuration)
        {
            this.WaveIndex = inWaveIndex;
            this.RepeatCount = inRepeatCount;
            this.NextDuration = inNextDuration;
        }
    }
}

