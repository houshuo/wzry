namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    internal class CampDataStamp
    {
        public CampData[] CampGolds = new CampData[3];

        public CampDataStamp()
        {
            for (int i = 0; i < this.CampGolds.Length; i++)
            {
                this.CampGolds[i] = new CampData();
            }
        }

        public int CalcCampStat(COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo)
        {
            return (this.CampGolds[(int) InFrom].Golds - this.CampGolds[(int) InTo].Golds);
        }

        public void Clear()
        {
            for (int i = 0; i < this.CampGolds.Length; i++)
            {
                this.CampGolds[i].Clear();
            }
        }

        public void GetMaxCampStat(COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo, out int OutMaxPositive, out int OutMaxNegative)
        {
            OutMaxPositive = this.CampGolds[(int) InFrom].PositiveGolds[(int) InTo];
            OutMaxNegative = this.CampGolds[(int) InFrom].NegativeGolds[(int) InTo];
        }

        public void OnHeroGoldCoinChanged(ref PoolObjHandle<ActorRoot> InActor, int InChangedValue, int InCurrentValue, bool bInIsIncome)
        {
            byte actorCamp = (byte) InActor.handle.TheActorMeta.ActorCamp;
            if (actorCamp < this.CampGolds.Length)
            {
                CampData data1 = this.CampGolds[actorCamp];
                data1.Golds += InChangedValue;
                this.RefreshFlags(actorCamp);
            }
        }

        private void RefreshFlags(byte InChangedIndex)
        {
            CampData data = this.CampGolds[InChangedIndex];
            for (int i = 0; i < this.CampGolds.Length; i++)
            {
                if (InChangedIndex != i)
                {
                    CampData data2 = this.CampGolds[i];
                    int num2 = data.Golds - data2.Golds;
                    if ((num2 < 0) && (num2 < data.NegativeGolds[i]))
                    {
                        data.NegativeGolds[i] = num2;
                        data2.PositiveGolds[InChangedIndex] = num2 * -1;
                    }
                    else if ((num2 > 0) && (num2 > data.PositiveGolds[i]))
                    {
                        data.PositiveGolds[i] = num2;
                        data2.NegativeGolds[InChangedIndex] = num2 * -1;
                    }
                }
            }
        }
    }
}

