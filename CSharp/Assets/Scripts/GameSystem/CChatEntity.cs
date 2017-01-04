namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public class CChatEntity
    {
        public bool bHasReaded;
        public float final_height;
        public float final_width;
        public string head_url;
        public uint iLogicWorldID;
        public string level;
        public string name;
        public uint numLine = 1;
        public COMDT_GAME_VIP_CLIENT stGameVip;
        public string text;
        public ListView<CTextImageNode> TextObjList = new ListView<CTextImageNode>();
        public int time;
        public EChaterType type;
        public ulong ullUid;

        public void Clear()
        {
            this.ullUid = 0L;
            this.iLogicWorldID = 0;
            this.name = this.head_url = this.level = this.text = string.Empty;
            this.TextObjList.Clear();
            this.stGameVip = null;
            this.final_width = 0f;
            this.type = EChaterType.None;
            this.bHasReaded = true;
        }
    }
}

