namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class Track
    {
        private bool bAverageStep;
        private bool bFixedRange;
        public static readonly float Clip = 0.1f;
        public static readonly int DefaultSampling = 100;
        public bool isVisiable;
        protected int MaxSampling;

        public Track(string InTag, Color InColor)
        {
            this.isVisiable = true;
            this.bAverageStep = true;
            this.samples = new List<float>(DefaultSampling);
            this.tag = InTag;
            this.drawColor = InColor;
            this.MaxSampling = DefaultSampling;
        }

        public Track(string InTag, Color InColor, float InMin, float InMax) : this(InTag, InColor)
        {
            this.SetFixedRange(true, InMin, InMax);
        }

        public void AddSample(float InValue)
        {
            this.samples.Add(InValue);
            this.CollapseSamplings();
        }

        private float CalcMax(float InMax)
        {
            return (!this.bFixedRange ? InMax : this.fixedMaxSampleValue);
        }

        private float CalcMin(float InMin)
        {
            return (!this.bFixedRange ? InMin : this.fixedMinSampleValue);
        }

        private float CalcX(int InIndex)
        {
            if (this.bAverageStep)
            {
                return (((InIndex * Screen.width) * (1f - (Clip * 2f))) / ((float) this.maxSampling));
            }
            return (((InIndex * Screen.width) * (1f - (Clip * 2f))) / ((float) this.samples.Count));
        }

        private float CalcY(float InPercent)
        {
            return ((InPercent * Screen.height) * (1f - (Clip * 2f)));
        }

        private void CollapseSamplings()
        {
            if (this.samples.Count > this.maxSampling)
            {
                this.samples.RemoveRange(0, this.samples.Count - this.maxSampling);
            }
        }

        public static void DrawLine(Vector2 InStart, Vector2 InEnd, Color InColor, float InWidth)
        {
            Drawing.DrawLine(new Vector2(InStart.x + (Screen.width * Clip), (Screen.height * (1f - Clip)) - InStart.y), new Vector2(InEnd.x + (Screen.width * Clip), (Screen.height * (1f - Clip)) - InEnd.y), InColor, InWidth, true);
        }

        public void OnRender()
        {
            this.OnRender(this.minValue, this.maxValue);
        }

        public void OnRender(float InMinValue, float InMaxValue)
        {
            float num = this.CalcMax(InMaxValue) - this.CalcMin(InMinValue);
            Vector2 inStart = new Vector2();
            for (int i = 0; i < this.samples.Count; i++)
            {
                float num3 = this.samples[i] - this.CalcMin(InMinValue);
                float inPercent = num3 / num;
                if (i == 0)
                {
                    inStart = new Vector2(this.CalcX(i), this.CalcY(inPercent));
                }
                else
                {
                    Vector2 inEnd = new Vector2(this.CalcX(i), this.CalcY(inPercent));
                    DrawLine(inStart, inEnd, this.drawColor, 1f);
                    inStart = inEnd;
                }
            }
        }

        public void SetFixedRange(bool bInFixed, float InMin, float InMax)
        {
            this.bFixedRange = bInFixed;
            this.fixedMinSampleValue = InMin;
            this.fixedMaxSampleValue = InMax;
        }

        public void SetSample(float InValue, int reverseIndex)
        {
            if ((reverseIndex >= 0) && ((this.samples.Count - reverseIndex) > 0))
            {
                this.samples[(this.samples.Count - reverseIndex) - 1] = InValue;
            }
        }

        public Color drawColor { get; set; }

        public float fixedMaxSampleValue { get; protected set; }

        public float fixedMinSampleValue { get; protected set; }

        public bool hasSamples
        {
            get
            {
                return (this.samples.Count > 0);
            }
        }

        public bool isFixedRange
        {
            get
            {
                return this.bFixedRange;
            }
        }

        public int maxSampling
        {
            get
            {
                return this.MaxSampling;
            }
            set
            {
                this.MaxSampling = value;
                this.CollapseSamplings();
            }
        }

        public float maxValue
        {
            get
            {
                float minValue = float.MinValue;
                for (int i = 0; i < this.samples.Count; i++)
                {
                    if (this.samples[i] > minValue)
                    {
                        minValue = this.samples[i];
                    }
                }
                return minValue;
            }
        }

        public float minValue
        {
            get
            {
                float maxValue = float.MaxValue;
                for (int i = 0; i < this.samples.Count; i++)
                {
                    if (this.samples[i] < maxValue)
                    {
                        maxValue = this.samples[i];
                    }
                }
                return maxValue;
            }
        }

        public List<float> samples { get; protected set; }

        public string tag { get; protected set; }
    }
}

