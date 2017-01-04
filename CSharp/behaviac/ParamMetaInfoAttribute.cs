namespace behaviac
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false, Inherited=false)]
    public class ParamMetaInfoAttribute : TypeMetaInfoAttribute
    {
        private string defaultValue_;
        private float rangeMax_;
        private float rangeMin_;

        public ParamMetaInfoAttribute()
        {
            this.rangeMin_ = float.MinValue;
            this.rangeMax_ = float.MaxValue;
        }

        public ParamMetaInfoAttribute(string displayName, string description, string defaultValue) : base(displayName, description)
        {
            this.rangeMin_ = float.MinValue;
            this.rangeMax_ = float.MaxValue;
            this.defaultValue_ = defaultValue;
            this.rangeMin_ = float.MinValue;
            this.rangeMax_ = float.MaxValue;
        }

        public ParamMetaInfoAttribute(string displayName, string description, string defaultValue, float rangeMin, float rangeMax) : base(displayName, description)
        {
            this.rangeMin_ = float.MinValue;
            this.rangeMax_ = float.MaxValue;
            this.defaultValue_ = defaultValue;
            this.rangeMin_ = rangeMin;
            this.rangeMax_ = rangeMax;
        }

        public string DefaultValue
        {
            get
            {
                return this.defaultValue_;
            }
        }

        public float RangeMax
        {
            get
            {
                return this.rangeMax_;
            }
        }

        public float RangeMin
        {
            get
            {
                return this.rangeMin_;
            }
        }
    }
}

