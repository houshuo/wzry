namespace Assets.Scripts.GameLogic
{
    using System;

    public class CoutofControlInfo
    {
        public int combId;
        public int leftTime;
        public int totalTime;

        public CoutofControlInfo(int _combineID, int _totalTime, int _leftTime)
        {
            this.combId = _combineID;
            this.totalTime = _totalTime;
            this.leftTime = _leftTime;
        }
    }
}

