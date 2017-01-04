namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;

    public class CPlayerSoulLevelStat
    {
        private List<SoulLevelDetail> playerSoulLevelDetail = new List<SoulLevelDetail>();

        public void Clear()
        {
            this.playerSoulLevelDetail.Clear();
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnSoulLvlChange));
        }

        public uint GetPlayerLevelChangedTime(uint playerId, uint soulLevel)
        {
            for (int i = 0; i < this.playerSoulLevelDetail.Count; i++)
            {
                if (((this.playerSoulLevelDetail[i].playerId == playerId) && (soulLevel <= ValueProperty.GetMaxSoulLvl())) && (soulLevel > 0))
                {
                    return this.playerSoulLevelDetail[i].changeTime[(int) ((IntPtr) (soulLevel - 1))];
                }
            }
            return 0;
        }

        public int GetPlayerSoulLevelAtTime(uint playerID, int time)
        {
            SoulLevelDetail detail = null;
            for (int i = 0; i < this.playerSoulLevelDetail.Count; i++)
            {
                if (this.playerSoulLevelDetail[i].playerId == playerID)
                {
                    detail = this.playerSoulLevelDetail[i];
                }
            }
            int num2 = 0;
            if (detail != null)
            {
                for (int j = 0; j < detail.changeTime.Length; j++)
                {
                    if (detail.changeTime[j] > time)
                    {
                        return num2;
                    }
                    num2 = j + 1;
                }
            }
            return num2;
        }

        private void OnSoulLvlChange(PoolObjHandle<ActorRoot> act, int curSoulLevel)
        {
            if (((curSoulLevel <= ValueProperty.GetMaxSoulLvl()) && (curSoulLevel != 0)) && (act != 0))
            {
                SoulLevelDetail item = null;
                uint playerId = act.handle.TheActorMeta.PlayerId;
                bool flag = false;
                for (int i = 0; i < this.playerSoulLevelDetail.Count; i++)
                {
                    if (this.playerSoulLevelDetail[i].playerId == playerId)
                    {
                        flag = true;
                        item = this.playerSoulLevelDetail[i];
                        break;
                    }
                }
                if (!flag)
                {
                    item = new SoulLevelDetail(playerId);
                    this.playerSoulLevelDetail.Add(item);
                }
                if ((curSoulLevel <= ValueProperty.GetMaxSoulLvl()) && (curSoulLevel > 0))
                {
                    item.changeTime[curSoulLevel - 1] = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
            }
        }

        public void StartRecord()
        {
            this.Clear();
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnSoulLvlChange));
        }

        private class SoulLevelDetail
        {
            public uint[] changeTime;
            public uint playerId;

            public SoulLevelDetail(uint playerId)
            {
                this.playerId = playerId;
                this.changeTime = new uint[ValueProperty.GetMaxSoulLvl()];
            }
        }
    }
}

