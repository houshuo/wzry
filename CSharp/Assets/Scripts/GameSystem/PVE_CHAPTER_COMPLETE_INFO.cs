namespace Assets.Scripts.GameSystem
{
    using System;

    public class PVE_CHAPTER_COMPLETE_INFO
    {
        public byte bChapterNo;
        public byte bIsGetBonus;
        public byte bLevelNum;
        public PVE_LEVEL_COMPLETE_INFO[] LevelDetailList = new PVE_LEVEL_COMPLETE_INFO[CAdventureSys.LEVEL_PER_CHAPTER];

        public PVE_CHAPTER_COMPLETE_INFO()
        {
            for (int i = 0; i < CAdventureSys.LEVEL_PER_CHAPTER; i++)
            {
                this.LevelDetailList[i] = new PVE_LEVEL_COMPLETE_INFO();
            }
        }
    }
}

