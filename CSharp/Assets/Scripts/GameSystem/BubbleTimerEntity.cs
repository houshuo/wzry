namespace Assets.Scripts.GameSystem
{
    using System;

    public class BubbleTimerEntity : CDEntity
    {
        public uint heroid;
        public ulong playerid;

        public BubbleTimerEntity(ulong playerid, uint heroid, int cd_time) : base(cd_time, 0)
        {
            this.playerid = playerid;
            this.heroid = heroid;
        }

        public override void On_Timer_End(int timer)
        {
            base.On_Timer_End(timer);
            InBattleMsgUT.ShowBubble(this.playerid, this.heroid, string.Empty);
        }
    }
}

