namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Collections.Generic;

    public class COfflineChatIndex
    {
        public uint dwLogicWorldId;
        public List<int> indexList;
        public ulong ullUid;

        public COfflineChatIndex(ulong ullUid, uint dwLogicWorldId)
        {
            this.ullUid = ullUid;
            this.dwLogicWorldId = dwLogicWorldId;
            this.indexList = new List<int>();
        }
    }
}

