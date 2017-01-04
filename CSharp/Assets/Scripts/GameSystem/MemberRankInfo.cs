namespace Assets.Scripts.GameSystem
{
    using System;

    public class MemberRankInfo
    {
        public uint assistCnt;
        public uint byGameRankPoint;
        public uint deadCnt;
        public bool isSigned;
        public uint killCnt;
        public uint maxRankPoint;
        public uint totalRankPoint;
        public uint weekRankPoint;

        public MemberRankInfo()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.maxRankPoint = 0;
            this.totalRankPoint = 0;
            this.killCnt = 0;
            this.deadCnt = 0;
            this.assistCnt = 0;
            this.weekRankPoint = 0;
            this.byGameRankPoint = 0;
            this.isSigned = false;
        }
    }
}

