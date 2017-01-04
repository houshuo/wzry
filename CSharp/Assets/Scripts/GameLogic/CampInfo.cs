namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class CampInfo
    {
        private int _campScore;
        public int allHurtHp;
        public COM_PLAYERCAMP campType;
        public int destoryTowers;
        private int m_headPoints;
        public int numDeadSoldier;

        public event CampInfoValueChanged onCampScoreChanged;

        public CampInfo(COM_PLAYERCAMP CmpType)
        {
            this.campType = CmpType;
        }

        public int GetScore(RES_STAR_CONDITION_DATA_SUB_TYPE inDataSubType)
        {
            if (inDataSubType == RES_STAR_CONDITION_DATA_SUB_TYPE.RES_STAR_CONDITION_DATA_HEAD_POINTS)
            {
                return this.HeadPoints;
            }
            if (inDataSubType == RES_STAR_CONDITION_DATA_SUB_TYPE.RES_STAR_CONDITION_DATA_HEADS)
            {
                return this.campScore;
            }
            return -1;
        }

        public void IncCampScore(PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker)
        {
            this._campScore++;
            this.OnUpdateCampPts(true, false, inSrc, inAtker);
        }

        public void IncHeadPoints(int deltaVal, PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker)
        {
            this.m_headPoints += deltaVal;
            this.OnUpdateCampPts(false, true, inSrc, inAtker);
        }

        private void OnUpdateCampPts(bool bUpdateScore, bool bUpdateHeadPts, PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker)
        {
            int inCampScore = this._campScore;
            if (!bUpdateScore)
            {
                inCampScore = -1;
            }
            int headPoints = this.m_headPoints;
            if (!bUpdateHeadPts)
            {
                headPoints = -1;
            }
            if (this.onCampScoreChanged != null)
            {
                this.onCampScoreChanged(this.campType, inCampScore, headPoints);
            }
            SCampScoreUpdateParam prm = new SCampScoreUpdateParam(inCampScore, headPoints, inSrc, inAtker, this.campType);
            Singleton<GameEventSys>.instance.SendEvent<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, ref prm);
        }

        public int campScore
        {
            get
            {
                return this._campScore;
            }
        }

        public int HeadPoints
        {
            get
            {
                return this.m_headPoints;
            }
        }

        public delegate void CampInfoValueChanged(COM_PLAYERCAMP campType, int inCampScore, int inHeadPts);
    }
}

