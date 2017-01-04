namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ReplayControl
    {
        private Text _currentTimeTxt;
        private GameObject _pauseBtn;
        private GameObject _playBtn;
        private Slider _progress;
        private GameObject _root;
        private GameObject _speedDownBtn;
        private Text _speedTxt;
        private GameObject _speedUpBtn;
        private Text _totalTimeTxt;

        public ReplayControl(GameObject root)
        {
            this._root = root;
            this._playBtn = Utility.FindChild(root, "PlayBtn");
            this._pauseBtn = Utility.FindChild(root, "PauseBtn");
            this._speedDownBtn = Utility.FindChild(root, "SpeedDownBtn");
            this._speedUpBtn = Utility.FindChild(root, "SpeedUpBtn");
            this._speedTxt = Utility.GetComponetInChild<Text>(root, "SpeedText");
            this._progress = Utility.GetComponetInChild<Slider>(root, "Progress");
            this._currentTimeTxt = Utility.GetComponetInChild<Text>(root, "CurrentTime");
            this._totalTimeTxt = Utility.GetComponetInChild<Text>(root, "TotalTime");
            this._speedTxt.text = Singleton<WatchController>.GetInstance().SpeedRate.ToString() + "X";
            this.ValidatePlayBtnState();
            this.ValidateSpeedBtnState();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickPlay, new CUIEventManager.OnUIEventHandler(this.OnClickPlay));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickSpeedUp, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickSpeedDown, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedDown));
        }

        public void Clear()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickPlay, new CUIEventManager.OnUIEventHandler(this.OnClickPlay));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickSpeedUp, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickSpeedDown, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedDown));
        }

        public void LateUpdate()
        {
            uint num = Singleton<WatchController>.GetInstance().CurFrameNo * Singleton<WatchController>.GetInstance().FrameDelta;
            uint num2 = Singleton<WatchController>.GetInstance().EndFrameNo * Singleton<WatchController>.GetInstance().FrameDelta;
            this._progress.value = (num2 <= 0) ? 0f : (((float) num) / ((float) num2));
            this._currentTimeTxt.text = string.Format("{0:D2}:{1:D2}", num / 0xea60, (num / 0x3e8) % 60);
            this._totalTimeTxt.text = string.Format("{0:D2}:{1:D2}", num2 / 0xea60, (num2 / 0x3e8) % 60);
            this._speedTxt.text = Singleton<WatchController>.GetInstance().SpeedRate.ToString() + "X";
        }

        private void OnClickPlay(CUIEvent evt)
        {
            Singleton<WatchController>.GetInstance().IsRunning = !Singleton<WatchController>.GetInstance().IsRunning;
            this.ValidatePlayBtnState();
        }

        private void OnClickSpeedDown(CUIEvent evt)
        {
            WatchController instance = Singleton<WatchController>.GetInstance();
            instance.SpeedRate = (byte) (instance.SpeedRate - 1);
            this.ValidateSpeedBtnState();
        }

        private void OnClickSpeedUp(CUIEvent evt)
        {
            WatchController instance = Singleton<WatchController>.GetInstance();
            instance.SpeedRate = (byte) (instance.SpeedRate + 1);
            this.ValidateSpeedBtnState();
        }

        private void ValidatePlayBtnState()
        {
            bool flag = !Singleton<WatchController>.GetInstance().IsLiveCast;
            if (Singleton<WatchController>.GetInstance().IsRunning)
            {
                this._pauseBtn.CustomSetActive(true);
                this._pauseBtn.GetComponent<Button>().interactable = flag;
                this._pauseBtn.GetComponent<CUIEventScript>().enabled = flag;
                this._playBtn.CustomSetActive(false);
            }
            else
            {
                this._playBtn.CustomSetActive(true);
                this._playBtn.GetComponent<Button>().interactable = flag;
                this._playBtn.GetComponent<CUIEventScript>().enabled = flag;
                this._pauseBtn.CustomSetActive(false);
            }
        }

        private void ValidateSpeedBtnState()
        {
            bool flag = !Singleton<WatchController>.GetInstance().IsLiveCast && (Singleton<WatchController>.GetInstance().SpeedRate < Singleton<WatchController>.GetInstance().SpeedRateMax);
            this._speedUpBtn.GetComponent<Button>().interactable = flag;
            this._speedUpBtn.GetComponent<CUIEventScript>().enabled = flag;
            bool flag2 = !Singleton<WatchController>.GetInstance().IsLiveCast && (Singleton<WatchController>.GetInstance().SpeedRate > Singleton<WatchController>.GetInstance().SpeedRateMin);
            this._speedDownBtn.GetComponent<Button>().interactable = flag2;
            this._speedDownBtn.GetComponent<CUIEventScript>().enabled = flag2;
        }

        public GameObject Root
        {
            get
            {
                return this._root;
            }
        }
    }
}

