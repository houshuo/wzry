namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class CBattleBuffStat
    {
        private List<int> m_buffRecordCamp1 = new List<int>();
        private List<int> m_buffRecordCamp2 = new List<int>();
        private ulong m_lastRecordFrameTick;
        private int m_timerSeq = -1;

        public void AddTimerEvent()
        {
            if (this.m_timerSeq == -1)
            {
                this.m_timerSeq = Singleton<CTimerManager>.instance.AddTimer(0x3e8, 0, new CTimer.OnTimeUpHandler(this.OnCheckPlayerBuff));
            }
        }

        public int GetDataByIndex(COM_PLAYERCAMP camp, int index)
        {
            if (index < this.GetRecordCnt())
            {
                if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    return this.m_buffRecordCamp1[index];
                }
                if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                {
                    return this.m_buffRecordCamp2[index];
                }
            }
            return 0;
        }

        public int GetRecordCnt()
        {
            return Math.Min(this.m_buffRecordCamp1.Count, this.m_buffRecordCamp2.Count);
        }

        private void OnCheckPlayerBuff(int timerSequence)
        {
            if ((Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_lastRecordFrameTick) >= 0x7530L)
            {
                this.m_lastRecordFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
                int dragonBuffId = Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_BIG_DRAGON);
                int item = 0;
                int num3 = 0;
                List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.instance.GetAllPlayers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Player current = enumerator.Current;
                    ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator2 = current.GetAllHeroes().GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current != 0)
                        {
                            PoolObjHandle<ActorRoot> handle = enumerator2.Current;
                            if (handle.handle.BuffHolderComp.FindBuff(dragonBuffId) != null)
                            {
                                if (current.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                                {
                                    item++;
                                }
                                else if (current.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                                {
                                    num3++;
                                }
                            }
                            continue;
                        }
                    }
                }
                this.m_buffRecordCamp1.Add(item);
                this.m_buffRecordCamp2.Add(num3);
            }
        }

        public void RemoveTimerEvent()
        {
            if (this.m_timerSeq != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSeq);
                this.m_timerSeq = -1;
            }
        }

        public void StartRecord()
        {
            this.m_buffRecordCamp1.Clear();
            this.m_buffRecordCamp2.Clear();
            this.m_lastRecordFrameTick = 0L;
            this.RemoveTimerEvent();
            this.AddTimerEvent();
        }
    }
}

