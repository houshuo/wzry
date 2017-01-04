namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;

    public class GameTaskKill : GameTask
    {
        protected override void OnClose()
        {
            DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
            if (campStat.ContainsKey((uint) this.SubjCamp))
            {
                CampInfo info = campStat[(uint) this.SubjCamp];
                info.onCampScoreChanged -= new CampInfo.CampInfoValueChanged(this.ValidateKill);
            }
        }

        protected override void OnDestroy()
        {
            this.OnClose();
        }

        protected override void OnInitial()
        {
        }

        protected override void OnStart()
        {
            DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
            if (campStat.ContainsKey((uint) this.SubjCamp))
            {
                CampInfo info = campStat[(uint) this.SubjCamp];
                info.onCampScoreChanged += new CampInfo.CampInfoValueChanged(this.ValidateKill);
                base.Current = info.campScore;
            }
        }

        private void ValidateKill(COM_PLAYERCAMP campType, int inCampScore, int inHeadPts)
        {
            if (inCampScore >= 0)
            {
                base.Current = inCampScore;
            }
        }

        protected COM_PLAYERCAMP SubjCamp
        {
            get
            {
                return (COM_PLAYERCAMP) base.Config.iParam1;
            }
        }
    }
}

