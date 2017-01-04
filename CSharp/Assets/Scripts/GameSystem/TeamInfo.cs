namespace Assets.Scripts.GameSystem
{
    using System;

    public class TeamInfo
    {
        public ListView<TeamMember> MemInfoList = new ListView<TeamMember>();
        public PlayerUniqueID stSelfInfo = new PlayerUniqueID();
        public TeamAttrib stTeamInfo = new TeamAttrib();
        public PlayerUniqueID stTeamMaster = new PlayerUniqueID();
        public int TeamEntity;
        public ulong TeamFeature;
        public uint TeamId;
        public uint TeamSeq;

        public static TeamMember GetMemberInfo(TeamInfo teamInfo, int Pos)
        {
            for (int i = 0; i < teamInfo.MemInfoList.Count; i++)
            {
                if (teamInfo.MemInfoList[i].dwPosOfTeam == (Pos - 1))
                {
                    return teamInfo.MemInfoList[i];
                }
            }
            return null;
        }
    }
}

