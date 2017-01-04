namespace PigeonCoopToolkit.Utillities
{
    using System;

    [Serializable]
    public class Range
    {
        public float Max;
        public float Min;

        public bool WithinRange(float value)
        {
            return ((this.Min <= value) && (this.Max >= value));
        }
    }
}

