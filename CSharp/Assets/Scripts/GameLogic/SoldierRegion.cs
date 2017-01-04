namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using UnityEngine;

    public class SoldierRegion : FuncRegion
    {
        private SoldierWave _lastWave;
        private int _toSwitchWaveID;
        public GameObject AttackRoute;
        public static bool bFirstSpawnEvent;
        public bool bForceCompleteSpawn = true;
        private bool bInited;
        private bool bShouldReset;
        private bool bShouldWait;
        [NonSerialized, HideInInspector]
        public bool bTriggerEvent;
        [NonSerialized, HideInInspector]
        public SoldierWave CurrentWave;
        private int curTick;
        public GameObject finalTarget;
        public int RouteID;
        private int waitTick;
        public int WaveID;
        [NonSerialized, HideInInspector]
        public ListView<SoldierWave> Waves = new ListView<SoldierWave>();

        public void Awake()
        {
            this.LoadWave(this.WaveID);
            this._toSwitchWaveID = 0;
        }

        private SoldierWave FindNextValidWave()
        {
            if (this.CurrentWave == null)
            {
                this.CurrentWave = (this.Waves.Count <= 0) ? null : this.Waves[0];
            }
            else
            {
                this.CurrentWave = (this.CurrentWave.Index >= (this.Waves.Count - 1)) ? null : this.Waves[this.CurrentWave.Index + 1];
            }
            if ((this.CurrentWave != null) && (this._lastWave != null))
            {
                this.CurrentWave.CloneState(this._lastWave);
                this._lastWave = null;
            }
            if (this.bTriggerEvent)
            {
                int inWaveIndex = (this.CurrentWave == null) ? (this.Waves.Count - 1) : this.CurrentWave.Index;
                SoldierWaveParam prm = new SoldierWaveParam(inWaveIndex, 0, this.GetNextRepeatTime(true));
                Singleton<GameEventSys>.instance.SendEvent<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, ref prm);
            }
            return this.CurrentWave;
        }

        public int GetNextRepeatTime(bool bWaveOrRepeat)
        {
            if (this.CurrentWave != null)
            {
                if (bWaveOrRepeat)
                {
                    return (int) this.CurrentWave.WaveInfo.dwStartWatiTick;
                }
                if (this.CurrentWave.repeatCount < this.CurrentWave.WaveInfo.dwRepeatNum)
                {
                    return (int) (this.CurrentWave.WaveInfo.dwIntervalTick + (this.CurrentWave.Selector.StatTotalCount * MonoSingleton<GlobalConfig>.instance.SoldierWaveInterval));
                }
                if ((this.CurrentWave.Index + 1) < this.Waves.Count)
                {
                    SoldierWave wave = this.Waves[this.CurrentWave.Index + 1];
                    return (int) (((this.CurrentWave.Selector.StatTotalCount * MonoSingleton<GlobalConfig>.instance.SoldierWaveInterval) + this.CurrentWave.WaveInfo.dwIntervalTick) + wave.WaveInfo.dwStartWatiTick);
                }
            }
            return -1;
        }

        public int GetRepeatCountTill(int inWaveIndex)
        {
            int num = 0;
            int num2 = 0;
            ListView<SoldierWave>.Enumerator enumerator = this.Waves.GetEnumerator();
            while (enumerator.MoveNext() && (num < inWaveIndex))
            {
                SoldierWave current = enumerator.Current;
                if (current.WaveInfo.dwRepeatNum == 0)
                {
                    return 0;
                }
                num2 += (int) current.WaveInfo.dwRepeatNum;
                num++;
            }
            int repeatCount = this.Waves[inWaveIndex].repeatCount;
            return (num2 + repeatCount);
        }

        public int GetTotalCount()
        {
            int num = 0;
            ListView<SoldierWave>.Enumerator enumerator = this.Waves.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SoldierWave current = enumerator.Current;
                if (current.WaveInfo.dwRepeatNum == 0)
                {
                    return 0;
                }
                num += (int) current.WaveInfo.dwRepeatNum;
            }
            return num;
        }

        public int GetTotalTime()
        {
            int num = 0;
            ListView<SoldierWave>.Enumerator enumerator = this.Waves.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SoldierWave current = enumerator.Current;
                if (current.WaveInfo.dwRepeatNum == 0)
                {
                    return 0;
                }
                num += (int) current.WaveInfo.dwStartWatiTick;
                num += (int) (current.WaveInfo.dwIntervalTick * (current.WaveInfo.dwRepeatNum - 1));
            }
            return num;
        }

        private void LoadWave(int theWaveID)
        {
            DebugHelper.Assert(GameDataMgr.soldierWaveDatabin != null);
            ResSoldierWaveInfo dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey((uint) theWaveID);
            int num = 0;
            while (dataByKey != null)
            {
                this.Waves.Add(new SoldierWave(this, dataByKey, num++));
                dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey(dataByKey.dwNextSoldierWaveID);
            }
        }

        public void ResetRegion()
        {
            this.CurrentWave = null;
            this.bShouldReset = true;
            this.waitTick = 0;
            this.curTick = 0;
            if (this.Waves != null)
            {
                for (int i = 0; i < this.Waves.Count; i++)
                {
                    if (this.Waves[i] != null)
                    {
                        this.Waves[i].Reset();
                    }
                }
            }
        }

        public override void Startup()
        {
            base.Startup();
            if (!this.bInited)
            {
                this.FindNextValidWave();
                this.bInited = true;
            }
        }

        public void SwitchWave(int newWaveID)
        {
            if ((this.CurrentWave == null) || this.CurrentWave.IsInIdle)
            {
                this._lastWave = this.CurrentWave;
                this.CurrentWave = null;
                this.bShouldReset = true;
                this.waitTick = 0;
                this.curTick = 0;
                this.Waves.Clear();
                this.LoadWave(newWaveID);
                this._toSwitchWaveID = 0;
            }
            else
            {
                this._toSwitchWaveID = newWaveID;
            }
        }

        public SoldierSpawnResult UpdateLogicSpec(int delta)
        {
            if (!base.isStartup)
            {
                return SoldierSpawnResult.UnStarted;
            }
            if ((this.CurrentWave == null) && !this.bShouldReset)
            {
                return SoldierSpawnResult.Completed;
            }
            if (((this._toSwitchWaveID > 0) && (this.CurrentWave != null)) && this.CurrentWave.IsInIdle)
            {
                this.SwitchWave(this._toSwitchWaveID);
            }
            if (this.bShouldWait || this.bShouldReset)
            {
                this.bShouldReset = false;
                this.curTick += delta;
                if (this.curTick > this.waitTick)
                {
                    this.FindNextValidWave();
                    this.bShouldWait = false;
                    this.curTick = 0;
                    this.waitTick = 0;
                    if (this.CurrentWave == null)
                    {
                        return SoldierSpawnResult.Completed;
                    }
                }
                else
                {
                    return SoldierSpawnResult.ShouldWaitInterval;
                }
            }
            SoldierSpawnResult result = this.CurrentWave.Update(delta);
            if (result > SoldierSpawnResult.ThresholdShouldWait)
            {
                this.bShouldWait = true;
                this.curTick = 0;
                this.waitTick = (int) this.CurrentWave.WaveInfo.dwIntervalTick;
            }
            return result;
        }
    }
}

