namespace PigeonCoopToolkit.Effects.Trails
{
    using System;
    using UnityEngine;

    public class SmokeTrailPoint : PCTrailPoint
    {
        public Vector3 RandomVec;

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            base.Position += (Vector3) (this.RandomVec * deltaTime);
        }
    }
}

