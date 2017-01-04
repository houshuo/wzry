namespace PigeonCoopToolkit.Effects.Trails
{
    using System;
    using UnityEngine;

    public class PCTrailPoint
    {
        private float _distance;
        private float _timeActive;
        public Vector3 Forward;
        public int PointNumber;
        public Vector3 Position;

        public float GetDistanceFromStart()
        {
            return this._distance;
        }

        public void SetDistanceFromStart(float distance)
        {
            this._distance = distance;
        }

        public void SetTimeActive(float time)
        {
            this._timeActive = time;
        }

        public float TimeActive()
        {
            return this._timeActive;
        }

        public virtual void Update(float deltaTime)
        {
            this._timeActive += deltaTime;
        }
    }
}

