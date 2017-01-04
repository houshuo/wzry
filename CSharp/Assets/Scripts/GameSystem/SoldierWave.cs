namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class SoldierWave
    {
        private int _countdown;
        private Text _countdownText;
        private Text _countdownTitle;
        private int _currentWave;
        private GameObject _root;
        private int _totalWave;
        private Text _waveText;

        private void CalcMinSec(int inMs, out int outSec, out int outMin)
        {
            outSec = inMs / 0x3e8;
            outMin = outSec / 60;
            outSec = outSec % 60;
        }

        public void Clear()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, new RefAction<SoldierWaveParam>(this.OnNextRepeat));
            Singleton<GameEventSys>.instance.RmvEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, new RefAction<SoldierWaveParam>(this.OnNextWave));
            this._totalWave = 0;
            this._currentWave = 0;
            this._countdown = 0;
        }

        public void Hide()
        {
            this._root.CustomSetActive(false);
        }

        public void Init(GameObject obj)
        {
            this._root = obj;
            this._waveText = Utility.GetComponetInChild<Text>(obj, "WavesContent");
            this._countdownText = Utility.GetComponetInChild<Text>(obj, "CounterContent");
            this._countdownTitle = Utility.GetComponetInChild<Text>(obj, "CounterTitle");
            SoldierRegion soldirRegion = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldirRegion();
            if (soldirRegion != null)
            {
                this._totalWave = soldirRegion.GetTotalCount();
            }
            this._waveText.text = string.Format("{0}/{1}", this._currentWave, this._totalWave);
            Singleton<GameEventSys>.instance.AddEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, new RefAction<SoldierWaveParam>(this.OnNextWave));
            Singleton<GameEventSys>.instance.AddEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, new RefAction<SoldierWaveParam>(this.OnNextRepeat));
        }

        private void OnNextRepeat(ref SoldierWaveParam inParam)
        {
            int num = this._currentWave;
            SoldierRegion soldirRegion = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldirRegion();
            if (soldirRegion != null)
            {
                this._currentWave = soldirRegion.GetRepeatCountTill(inParam.WaveIndex);
            }
            this._waveText.text = string.Format("{0}/{1}", this._currentWave, this._totalWave);
            if (num != this._currentWave)
            {
                if ((inParam.NextDuration >= 0) && this._countdownText.gameObject.activeSelf)
                {
                    this._countdown = inParam.NextDuration;
                }
                else
                {
                    this._countdownText.gameObject.CustomSetActive(false);
                    this._countdownTitle.gameObject.CustomSetActive(false);
                }
            }
        }

        private void OnNextWave(ref SoldierWaveParam inParam)
        {
            if ((inParam.NextDuration >= 0) && this._countdownText.gameObject.activeSelf)
            {
                this._countdown = inParam.NextDuration;
            }
            else
            {
                this._countdownText.gameObject.CustomSetActive(false);
                this._countdownTitle.gameObject.CustomSetActive(false);
            }
        }

        public void Show()
        {
            this._root.CustomSetActive(true);
        }

        public void Update()
        {
            MapWrapper mapLogic = Singleton<BattleLogic>.GetInstance().mapLogic;
            if (!mapLogic.DoesSoldierOverNum())
            {
                SoldierRegion soldirRegion = mapLogic.GetSoldirRegion();
                DebugHelper.Assert(soldirRegion != null, "region 不能为空");
                if ((soldirRegion != null) && soldirRegion.isStartup)
                {
                    int num = (int) (Time.deltaTime * 1000f);
                    this._countdown -= num;
                    if (this._countdown <= 0)
                    {
                        this._countdown = 0;
                    }
                }
            }
            if ((this._countdownText != null) && this._countdownText.gameObject.activeSelf)
            {
                int outMin = 0;
                int outSec = 0;
                this.CalcMinSec(this._countdown, out outSec, out outMin);
                this._countdownText.text = string.Format("{0:D2} : {1:D2}", outMin, outSec);
            }
        }
    }
}

