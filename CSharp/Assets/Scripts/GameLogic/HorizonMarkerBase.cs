namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;
    using UnityEngine;

    public class HorizonMarkerBase : LogicComponent
    {
        private int _sightRadius;

        public virtual void AddHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, int count)
        {
        }

        public virtual void AddShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm, int count)
        {
        }

        public virtual int[] GetExposedCamps()
        {
            return null;
        }

        public virtual VInt3[] GetExposedPos()
        {
            return null;
        }

        public virtual int GetExposedRadius()
        {
            return 0;
        }

        public virtual bool HasHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm)
        {
            return false;
        }

        public virtual bool HasShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm)
        {
            return false;
        }

        protected virtual bool IsEnabled()
        {
            return false;
        }

        public virtual bool IsSightVisited(COM_PLAYERCAMP targetCamp)
        {
            return false;
        }

        public virtual bool IsVisibleFor(COM_PLAYERCAMP targetCamp)
        {
            return false;
        }

        public override void OnUse()
        {
            base.OnUse();
            this._sightRadius = -1;
        }

        public virtual byte QueryMarker(COM_PLAYERCAMP camp, HorizonConfig.HideMark mark)
        {
            return 0;
        }

        public virtual byte QueryMarker(COM_PLAYERCAMP camp, HorizonConfig.ShowMark mark)
        {
            return 0;
        }

        public virtual void ResetSight()
        {
        }

        public virtual void SetEnabled(bool bEnabled)
        {
        }

        public virtual bool SetExposeMark(bool exposed, COM_PLAYERCAMP targetCamp, bool bIgnoreAlreadyLit)
        {
            return false;
        }

        public virtual void SetHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, bool bSet)
        {
        }

        public virtual void SetTranslucentMark(HorizonConfig.HideMark hm, bool bSet)
        {
        }

        public virtual void VisitSight(COM_PLAYERCAMP targetCamp)
        {
        }

        public int SightRadius
        {
            get
            {
                if (this._sightRadius <= 0)
                {
                    this._sightRadius = (int) Horizon.QueryGlobalSight();
                }
                return this._sightRadius;
            }
            set
            {
                if (this._sightRadius != value)
                {
                    this._sightRadius = Mathf.Clamp(value, 0, (int) Horizon.QueryGlobalSight());
                }
            }
        }
    }
}

