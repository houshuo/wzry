namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CShenFuStat
    {
        private List<ShenFuRecord> m_recordList = new List<ShenFuRecord>();

        public void Clear()
        {
            this.m_recordList.Clear();
            Singleton<EventRouter>.instance.RemoveEventHandler<COM_PLAYERCAMP, uint, uint>(EventID.BATTLE_SHENFU_EFFECT_CHANGED, new Action<COM_PLAYERCAMP, uint, uint>(this.OnShenFuEffect));
        }

        public List<ShenFuRecord> GetShenFuRecord(COM_PLAYERCAMP playerCamp)
        {
            List<ShenFuRecord> list = new List<ShenFuRecord>();
            for (int i = 0; i < this.m_recordList.Count; i++)
            {
                ShenFuRecord record = this.m_recordList[i];
                if (record.playerCamp == playerCamp)
                {
                    list.Add(this.m_recordList[i]);
                }
            }
            return list;
        }

        public List<ShenFuRecord> GetShenFuRecord(uint playerId)
        {
            List<ShenFuRecord> list = new List<ShenFuRecord>();
            for (int i = 0; i < this.m_recordList.Count; i++)
            {
                ShenFuRecord record = this.m_recordList[i];
                if (record.playerId == playerId)
                {
                    list.Add(this.m_recordList[i]);
                }
            }
            return list;
        }

        public List<ShenFuRecord> GetShenFuRecord(COM_PLAYERCAMP playerCamp, uint shenFuId)
        {
            List<ShenFuRecord> list = new List<ShenFuRecord>();
            for (int i = 0; i < this.m_recordList.Count; i++)
            {
                ShenFuRecord record = this.m_recordList[i];
                if (record.playerCamp == playerCamp)
                {
                    ShenFuRecord record2 = this.m_recordList[i];
                    if (record2.shenFuId == shenFuId)
                    {
                        list.Add(this.m_recordList[i]);
                    }
                }
            }
            return list;
        }

        public List<ShenFuRecord> GetShenFuRecord(uint playerId, uint shenFuId)
        {
            List<ShenFuRecord> list = new List<ShenFuRecord>();
            for (int i = 0; i < this.m_recordList.Count; i++)
            {
                ShenFuRecord record = this.m_recordList[i];
                if (record.playerId == playerId)
                {
                    ShenFuRecord record2 = this.m_recordList[i];
                    if (record2.shenFuId == shenFuId)
                    {
                        list.Add(this.m_recordList[i]);
                    }
                }
            }
            return list;
        }

        private void OnShenFuEffect(COM_PLAYERCAMP playerCamp, uint playerId, uint shenFuId)
        {
            this.m_recordList.Add(new ShenFuRecord(playerCamp, playerId, shenFuId, (uint) Singleton<FrameSynchr>.instance.LogicFrameTick));
        }

        public void StartRecord()
        {
            this.Clear();
            Singleton<EventRouter>.instance.AddEventHandler<COM_PLAYERCAMP, uint, uint>(EventID.BATTLE_SHENFU_EFFECT_CHANGED, new Action<COM_PLAYERCAMP, uint, uint>(this.OnShenFuEffect));
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ShenFuRecord
        {
            public COM_PLAYERCAMP playerCamp;
            public uint playerId;
            public uint shenFuId;
            public uint onEffectTime;
            public ShenFuRecord(COM_PLAYERCAMP playerCamp, uint playerId, uint shenFuId, uint onEffectTime)
            {
                this.playerCamp = playerCamp;
                this.playerId = playerId;
                this.shenFuId = shenFuId;
                this.onEffectTime = onEffectTime;
            }
        }
    }
}

