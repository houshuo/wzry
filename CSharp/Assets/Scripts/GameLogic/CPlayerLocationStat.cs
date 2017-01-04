namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using System.Collections.Generic;

    public class CPlayerLocationStat
    {
        protected bool bShouldCare;
        protected DictionaryView<uint, List<VInt2>> StatData = new DictionaryView<uint, List<VInt2>>();

        public void Clear()
        {
            if (this.ShouldStatInThisGameMode())
            {
                this.StatData.Clear();
            }
            this.bShouldCare = false;
        }

        public VInt2 GetTimeLocation(uint playerID, int Index)
        {
            List<VInt2> list = null;
            if (this.StatData.TryGetValue(playerID, out list) && (Index < list.Count))
            {
                return list[Index];
            }
            return new VInt2();
        }

        private void OnStat(int TimeSeq)
        {
            try
            {
                List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.instance.GetAllPlayers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Player current = enumerator.Current;
                    if ((current != null) && (current.Captain != 0))
                    {
                        List<VInt2> list2 = null;
                        if (!this.StatData.TryGetValue(current.PlayerId, out list2))
                        {
                            list2 = new List<VInt2>();
                            this.StatData.Add(current.PlayerId, list2);
                        }
                        VInt3 location = current.Captain.handle.location;
                        list2.Add(new VInt2(location.x, location.z));
                    }
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "exception in player location stat:{0}", inParameters);
            }
        }

        public bool ShouldStatInThisGameMode()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (!curLvelContext.IsMobaModeWithOutGuide())
            {
                return false;
            }
            return (curLvelContext.m_pvpPlayerNum == 10);
        }

        public void StartRecord()
        {
            this.Clear();
            if (this.ShouldStatInThisGameMode())
            {
                this.bShouldCare = true;
                this.OnStat(0);
            }
        }

        public void UpdateLogic(int DeltaTime)
        {
            if ((this.bShouldCare && ((Singleton<FrameSynchr>.instance.CurFrameNum % 450) == 0)) && Singleton<BattleLogic>.instance.isFighting)
            {
                this.OnStat(0);
            }
        }
    }
}

