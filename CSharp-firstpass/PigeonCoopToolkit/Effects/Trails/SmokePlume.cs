namespace PigeonCoopToolkit.Effects.Trails
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Pigeon Coop Toolkit/Effects/Smoke Plume")]
    public class SmokePlume : TrailRenderer_Base
    {
        private float _timeSincePoint;
        public Vector3 ConstantForce = ((Vector3) (Vector3.up * 0.5f));
        public int MaxNumberOfPoints = 50;
        public float RandomForceScale = 0.05f;
        public float TimeBetweenPoints = 0.1f;

        protected override int GetMaxNumberOfPoints()
        {
            return this.MaxNumberOfPoints;
        }

        protected override void InitialiseNewPoint(PCTrailPoint newPoint)
        {
            ((SmokeTrailPoint) newPoint).RandomVec = (Vector3) (UnityEngine.Random.onUnitSphere * this.RandomForceScale);
        }

        protected void OnEnable()
        {
            base.Start();
            base.ClearSystem(true);
            this._timeSincePoint = 0f;
        }

        protected override void OnStartEmit()
        {
            this._timeSincePoint = 0f;
        }

        protected override void Reset()
        {
            base.Reset();
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.5f, 0.2f), new Keyframe(1f, 0.2f) };
            base.TrailData.SizeOverLife = new AnimationCurve(keys);
            base.TrailData.Lifetime = 6f;
            this.ConstantForce = (Vector3) (Vector3.up * 0.5f);
            this.TimeBetweenPoints = 0.1f;
            this.RandomForceScale = 0.05f;
            this.MaxNumberOfPoints = 50;
        }

        protected override void Update()
        {
            if (base._emit)
            {
                this._timeSincePoint += !base._noDecay ? Time.deltaTime : 0f;
                if (this._timeSincePoint >= this.TimeBetweenPoints)
                {
                    base.AddPoint(new SmokeTrailPoint(), base._t.position);
                    this._timeSincePoint = 0f;
                }
            }
            base.Update();
        }

        protected override void UpdateTrail(PCTrail trail, float deltaTime)
        {
            if (!base._noDecay)
            {
                IEnumerator<PCTrailPoint> enumerator = trail.Points.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        PCTrailPoint current = enumerator.Current;
                        current.Position += (Vector3) (this.ConstantForce * deltaTime);
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
        }
    }
}

