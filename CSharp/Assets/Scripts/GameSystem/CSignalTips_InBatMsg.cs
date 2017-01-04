namespace Assets.Scripts.GameSystem
{
    using System;

    public class CSignalTips_InBatMsg : CSignalTipsElement
    {
        public string content;
        public uint heroID;
        public ulong playerID;
        public string sound;

        public CSignalTips_InBatMsg(ulong playerID, uint heroID, string content, string sound) : base(CSignalTipsElement.EType.InBattleMsg)
        {
            this.playerID = playerID;
            this.heroID = heroID;
            this.content = content;
            this.sound = sound;
        }
    }
}

