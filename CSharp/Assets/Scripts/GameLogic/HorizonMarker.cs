namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;

    public class HorizonMarker : HorizonMarkerBase
    {
        private CampMarker[] _campMarkers;
        private bool _enabled;
        private int _jungleHideMarkCount;
        private bool _needTranslucent;

        public override void AddHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, int count)
        {
            if (this._campMarkers != null)
            {
                if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
                {
                    for (int i = 0; i < this._campMarkers.Length; i++)
                    {
                        this._campMarkers[i].AddHideMark(hm, count);
                        this.StatHideMark(hm, count);
                    }
                }
                else
                {
                    int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
                    if ((index >= 0) && (index < this._campMarkers.Length))
                    {
                        this._campMarkers[index].AddHideMark(hm, count);
                        this.StatHideMark(hm, count);
                    }
                }
            }
        }

        public override void AddShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm, int count)
        {
            if (this._campMarkers != null)
            {
                if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
                {
                    for (int i = 0; i < this._campMarkers.Length; i++)
                    {
                        this._campMarkers[i].AddShowMark(sm, count);
                    }
                }
                else
                {
                    int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
                    if ((index >= 0) && (index < this._campMarkers.Length))
                    {
                        this._campMarkers[index].AddShowMark(sm, count);
                    }
                }
            }
        }

        public override bool HasHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm)
        {
            if (this._campMarkers == null)
            {
                return false;
            }
            int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
            return (((index >= 0) && (index < this._campMarkers.Length)) && this._campMarkers[index].HasHideMark(hm));
        }

        public override bool HasShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm)
        {
            if (this._campMarkers == null)
            {
                return false;
            }
            int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
            return (((index >= 0) && (index < this._campMarkers.Length)) && this._campMarkers[index].HasShowMark(sm));
        }

        public override void Init()
        {
            base.Init();
            Horizon.EnableMethod horizonEnableMethod = Singleton<BattleLogic>.instance.GetCurLvelContext().m_horizonEnableMethod;
            this._enabled = (horizonEnableMethod == Horizon.EnableMethod.EnableAll) || (horizonEnableMethod == Horizon.EnableMethod.EnableMarkNoMist);
            if (this._enabled)
            {
                this._campMarkers = new CampMarker[2];
                for (int i = 0; i < this._campMarkers.Length; i++)
                {
                    this._campMarkers[i] = new CampMarker(this, BattleLogic.MapOtherCampType(base.actor.TheActorMeta.ActorCamp, i));
                }
            }
            else
            {
                this._campMarkers = null;
            }
            this._jungleHideMarkCount = 0;
            this._needTranslucent = false;
        }

        protected override bool IsEnabled()
        {
            return (this._enabled && (null != this._campMarkers));
        }

        public override bool IsSightVisited(COM_PLAYERCAMP targetCamp)
        {
            if (this._campMarkers != null)
            {
                int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
                if ((index >= 0) && (index < this._campMarkers.Length))
                {
                    return (Singleton<FrameSynchr>.instance.CurFrameNum == this._campMarkers[index].sightFrame);
                }
            }
            return true;
        }

        public override bool IsVisibleFor(COM_PLAYERCAMP targetCamp)
        {
            if (this.IsEnabled() && (this._campMarkers != null))
            {
                int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
                if ((index >= 0) && (index < this._campMarkers.Length))
                {
                    return (this._campMarkers[index].Visible || ((targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID) && this._campMarkers[index].IsHideMarkOnly(HorizonConfig.HideMark.Jungle)));
                }
            }
            return true;
        }

        public override void OnUse()
        {
            base.OnUse();
            this._enabled = false;
            this._campMarkers = null;
            this._jungleHideMarkCount = 0;
            this._needTranslucent = false;
        }

        public override byte QueryMarker(COM_PLAYERCAMP camp, HorizonConfig.HideMark mark)
        {
            int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, camp);
            if ((index >= 0) && (index < this._campMarkers.Length))
            {
                return (byte) this._campMarkers[index]._hideMarks[(int) mark];
            }
            return 0;
        }

        public override byte QueryMarker(COM_PLAYERCAMP camp, HorizonConfig.ShowMark mark)
        {
            int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, camp);
            if ((index >= 0) && (index < this._campMarkers.Length))
            {
                return (byte) this._campMarkers[index]._showMarks[(int) mark];
            }
            return 0;
        }

        public override void Reactive()
        {
            base.Reactive();
            this.ResetSight();
        }

        private void RefreshVisible(COM_PLAYERCAMP targetCamp)
        {
            if (!Singleton<WatchController>.GetInstance().IsWatching && (base.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ))
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if ((hostPlayer != null) && (targetCamp == hostPlayer.PlayerCamp))
                {
                    base.actor.Visible = this.IsVisibleFor(targetCamp);
                }
            }
        }

        public override void ResetSight()
        {
            if (this._campMarkers != null)
            {
                for (int i = 0; i < this._campMarkers.Length; i++)
                {
                    this._campMarkers[i].sightFrame = 0;
                }
            }
        }

        public override void SetEnabled(bool bEnabled)
        {
            if (bEnabled != this._enabled)
            {
                this._enabled = bEnabled;
                if (!this._enabled)
                {
                    base.actor.Visible = true;
                }
            }
        }

        public override bool SetExposeMark(bool exposed, COM_PLAYERCAMP targetCamp, bool bIgnoreAlreadyLit)
        {
            if (this._campMarkers == null)
            {
                return false;
            }
            if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
            {
                for (int i = 0; i < this._campMarkers.Length; i++)
                {
                    this._campMarkers[i].Exposed = exposed;
                }
            }
            else
            {
                int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
                if ((index >= 0) && (index < this._campMarkers.Length))
                {
                    this._campMarkers[index].Exposed = exposed;
                }
            }
            this.StatHideMark(HorizonConfig.HideMark.INVALID, 0);
            return true;
        }

        private void StatHideMark(HorizonConfig.HideMark hm, int count)
        {
            if (hm == HorizonConfig.HideMark.Jungle)
            {
                int num = this._jungleHideMarkCount;
                this._jungleHideMarkCount += count;
                if ((num <= 0) && (this._jungleHideMarkCount > 0))
                {
                    base.actor.HudControl.ShowStatus(StatusHudType.InJungle);
                }
                else if ((num > 0) && (this._jungleHideMarkCount <= 0))
                {
                    base.actor.HudControl.HideStatus(StatusHudType.InJungle);
                }
            }
            int num2 = 0;
            bool flag = false;
            for (int i = 0; i < this._campMarkers.Length; i++)
            {
                CampMarker marker = this._campMarkers[i];
                num2 += marker.HideMarkTotal;
                flag |= marker.Exposed;
            }
            bool flag2 = ((num2 > 0) && !flag) || (this._jungleHideMarkCount > 0);
            if (this._needTranslucent != flag2)
            {
                this._needTranslucent = flag2;
                base.actor.MatHurtEffect.SetTranslucent(this._needTranslucent);
            }
        }

        public override void UpdateLogic(int delta)
        {
            if (this.IsEnabled())
            {
                if (this._campMarkers != null)
                {
                    for (int i = 0; i < this._campMarkers.Length; i++)
                    {
                        this._campMarkers[i].UpdateLogic();
                    }
                }
                if (!Singleton<WatchController>.GetInstance().IsWatching)
                {
                    Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if (((hostPlayer != null) && (base.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)) && (base.actor.TheActorMeta.ActorCamp != hostPlayer.PlayerCamp))
                    {
                        if (base.actor.VisibleIniting)
                        {
                            base.actor.Visible = this.IsVisibleFor(hostPlayer.PlayerCamp);
                        }
                        else if (base.actor.Visible && !this.IsVisibleFor(hostPlayer.PlayerCamp))
                        {
                            base.actor.Visible = false;
                        }
                    }
                }
            }
        }

        public override void VisitSight(COM_PLAYERCAMP targetCamp)
        {
            if (this._campMarkers != null)
            {
                int index = BattleLogic.MapOtherCampIndex(base.actor.TheActorMeta.ActorCamp, targetCamp);
                if ((index >= 0) && (index < this._campMarkers.Length))
                {
                    this._campMarkers[index].sightFrame = Singleton<FrameSynchr>.instance.CurFrameNum;
                    this.RefreshVisible(targetCamp);
                }
            }
        }

        internal class CampMarker
        {
            private COM_PLAYERCAMP _camp;
            private bool _exposed;
            private uint _exposeFrame;
            private HorizonMarker _owner;
            public const uint EXPOSE_COOL_FRAMES = 0x2d;
            public uint sightFrame;

            public CampMarker(HorizonMarker owner, COM_PLAYERCAMP camp)
            {
                this._owner = owner;
                this._camp = camp;
                this.sightFrame = 0;
                this._hideMarks = new int[2];
                this._showMarks = new int[3];
                this._exposed = false;
                this._exposeFrame = 0;
                this.RuleVisible = true;
            }

            public void AddHideMark(HorizonConfig.HideMark hm, int count)
            {
                this._hideMarks[(int) hm] += count;
                if (this._hideMarks[(int) hm] < 0)
                {
                    this._hideMarks[(int) hm] = 0;
                }
                if (count > 0)
                {
                    this._exposed = false;
                    this._exposeFrame = 0;
                }
                this.ApplyVisibleRules();
            }

            public void AddShowMark(HorizonConfig.ShowMark sm, int count)
            {
                this._showMarks[(int) sm] += count;
                if (this._showMarks[(int) sm] < 0)
                {
                    this._showMarks[(int) sm] = 0;
                }
                this.ApplyVisibleRules();
            }

            private bool ApplyVisibleRules()
            {
                bool ruleVisible = this.RuleVisible;
                this.RuleVisible = true;
                if (!this._exposed)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (this._hideMarks[i] <= 0)
                        {
                            continue;
                        }
                        bool flag2 = false;
                        for (int j = 0; j < 3; j++)
                        {
                            if (HorizonConfig.RelationMap[j, i] && (this._showMarks[j] > 0))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            this.RuleVisible = false;
                            break;
                        }
                    }
                }
                if (ruleVisible != this.RuleVisible)
                {
                    this._owner.RefreshVisible(this._camp);
                }
                return this.RuleVisible;
            }

            public bool HasHideMark(HorizonConfig.HideMark hm)
            {
                return (this._hideMarks[(int) hm] > 0);
            }

            public bool HasShowMark(HorizonConfig.ShowMark sm)
            {
                return (this._showMarks[(int) sm] > 0);
            }

            public bool IsHideMarkOnly(HorizonConfig.HideMark hm)
            {
                int num = 0;
                int num2 = 0;
                for (int i = 0; i < this._hideMarks.Length; i++)
                {
                    if (i == hm)
                    {
                        num2 += this._hideMarks[i];
                    }
                    num += this._hideMarks[i];
                }
                return ((num2 > 0) && (num2 == num));
            }

            public void UpdateLogic()
            {
                if ((this._exposeFrame > 0) && (Singleton<FrameSynchr>.GetInstance().CurFrameNum > (this._exposeFrame + 0x2d)))
                {
                    this.Exposed = false;
                }
            }

            public int[] _hideMarks { get; private set; }

            public int[] _showMarks { get; private set; }

            public COM_PLAYERCAMP Camp
            {
                get
                {
                    return this._camp;
                }
            }

            public bool Exposed
            {
                get
                {
                    return this._exposed;
                }
                set
                {
                    this._exposeFrame = !value ? 0 : Singleton<FrameSynchr>.GetInstance().CurFrameNum;
                    if (value != this._exposed)
                    {
                        this._exposed = value;
                        this.ApplyVisibleRules();
                    }
                }
            }

            public int HideMarkTotal
            {
                get
                {
                    int num = 0;
                    for (int i = 0; i < this._hideMarks.Length; i++)
                    {
                        num += this._hideMarks[i];
                    }
                    return num;
                }
            }

            public bool RuleVisible { get; private set; }

            public bool Visible
            {
                get
                {
                    return (this.RuleVisible && (!Singleton<BattleLogic>.GetInstance().horizon.Enabled || ((this.sightFrame > 0) && (Singleton<FrameSynchr>.instance.CurFrameNum <= (this.sightFrame + 8)))));
                }
            }
        }
    }
}

