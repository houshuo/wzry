namespace PigeonCoopToolkit.Effects.Trails
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PCTrailRendererData
    {
        public Gradient ColorOverLife;
        public Vector3 ForwardOverride;
        public bool ForwardOverrideRelative;
        public float Lifetime = 1f;
        public float MaterialTileLength;
        public Color SimpleColorOverLifeEnd;
        public Color SimpleColorOverLifeStart;
        public float SimpleSizeOverLifeEnd;
        public float SimpleSizeOverLifeStart;
        public AnimationCurve SizeOverLife = new AnimationCurve();
        public bool StretchColorToFit;
        public bool StretchSizeToFit;
        public Material TrailMaterial;
        public bool UseForwardOverride;
        public bool UsingSimpleColor;
        public bool UsingSimpleSize;
    }
}

