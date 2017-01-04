namespace Assets.Scripts.GameSystem
{
    using System;

    public class PVE_ADV_COMPLETE_INFO
    {
        public PVE_CHAPTER_COMPLETE_INFO[] ChapterDetailList = new PVE_CHAPTER_COMPLETE_INFO[CAdventureSys.CHAPTER_NUM];

        public PVE_ADV_COMPLETE_INFO()
        {
            for (int i = 0; i < CAdventureSys.CHAPTER_NUM; i++)
            {
                this.ChapterDetailList[i] = new PVE_CHAPTER_COMPLETE_INFO();
            }
        }
    }
}

