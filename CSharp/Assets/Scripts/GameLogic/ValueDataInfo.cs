namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class ValueDataInfo
    {
        private CrypticInt32 _AddRatio;
        private CrypticInt32 _AddValue;
        private CrypticInt32 _AddValueOffRatio;
        private CrypticInt32 _BaseValue;
        private CrypticInt32 _DecRatio;
        private CrypticInt32 _DecValue;
        private CrypticInt32 _DynamicId = 0;
        private CrypticInt32 _GrowValue;
        private CrypticInt32 _MaxLimitValue;
        private CrypticInt32 _TotalEftRatio = 0;
        private CrypticInt32 _TotalEftRatioByMgc = 0;
        private CrypticInt32 _TotalOldEftRatioByMgc = 0;
        private RES_FUNCEFT_TYPE _type;
        private ValueCalculator Calculator;

        public event ValueChangeDelegate ChangeEvent;

        public ValueDataInfo(RES_FUNCEFT_TYPE type, int nValue, int nGrow, ValueCalculator calc, int dynamicCfg = 0, int nMaxLimitValue = 0)
        {
            this._type = type;
            this.baseValue = nValue;
            this.growValue = nGrow;
            this.addValue = 0;
            this.decValue = 0;
            this.addRatio = 0;
            this.decRatio = 0;
            this.dynamicId = dynamicCfg;
            this.Calculator = calc;
            this.maxLimitValue = nMaxLimitValue;
            this._TotalEftRatio = 0;
            this._TotalEftRatioByMgc = 0;
        }

        public static void ChangeValueData(ref ValueDataInfo valueInfo, RES_VALUE_TYPE type, int val, bool bOffRatio)
        {
            if (type == RES_VALUE_TYPE.TYPE_VALUE)
            {
                if (bOffRatio)
                {
                    valueInfo.addValueOffRatio += val;
                }
                else
                {
                    valueInfo.addValue += val;
                }
            }
            else if (type == RES_VALUE_TYPE.TYPE_PERCENT)
            {
                valueInfo.addRatio += val;
            }
        }

        public static ValueDataInfo operator +(ValueDataInfo lhs, int rhs)
        {
            lhs.addValue += rhs;
            return lhs;
        }

        public static ValueDataInfo operator <<(ValueDataInfo lhs, int rhs)
        {
            lhs.addRatio += rhs;
            return lhs;
        }

        public static ValueDataInfo operator >>(ValueDataInfo lhs, int rhs)
        {
            lhs.addRatio -= rhs;
            return lhs;
        }

        public static ValueDataInfo operator -(ValueDataInfo lhs, int rhs)
        {
            lhs.addValue -= rhs;
            return lhs;
        }

        public int addRatio
        {
            get
            {
                return (int) this._AddRatio;
            }
            set
            {
                this._AddRatio = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int addValue
        {
            get
            {
                return (int) this._AddValue;
            }
            set
            {
                this._AddValue = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int addValueOffRatio
        {
            get
            {
                return (int) this._AddValueOffRatio;
            }
            set
            {
                this._AddValueOffRatio = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int basePropertyValue
        {
            get
            {
                if (this.Calculator != null)
                {
                    return this.Calculator(this, ValueDataType.TYPE_BASE);
                }
                return this.baseValue;
            }
        }

        public int baseValue
        {
            get
            {
                return (int) this._BaseValue;
            }
            set
            {
                this._BaseValue = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int decRatio
        {
            get
            {
                return (int) this._DecRatio;
            }
            set
            {
                this._DecRatio = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int decValue
        {
            get
            {
                return (int) this._DecValue;
            }
            set
            {
                this._DecValue = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int dynamicId
        {
            get
            {
                return (int) this._DynamicId;
            }
            set
            {
                this._DynamicId = value;
            }
        }

        public int extraPropertyValue
        {
            get
            {
                return (this.totalValue - this.basePropertyValue);
            }
        }

        public int growValue
        {
            get
            {
                return (int) this._GrowValue;
            }
            set
            {
                this._GrowValue = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int maxLimitValue
        {
            get
            {
                return (int) this._MaxLimitValue;
            }
            set
            {
                this._MaxLimitValue = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int totalEftRatio
        {
            get
            {
                return (int) this._TotalEftRatio;
            }
            set
            {
                this._TotalEftRatio = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int totalEftRatioByMgc
        {
            get
            {
                return (int) this._TotalEftRatioByMgc;
            }
            set
            {
                this.totalOldEftRatioByMgc = (int) this._TotalEftRatioByMgc;
                this._TotalEftRatioByMgc = value;
                if (this.ChangeEvent != null)
                {
                    this.ChangeEvent();
                }
            }
        }

        public int totalOldEftRatioByMgc
        {
            get
            {
                return (int) this._TotalOldEftRatioByMgc;
            }
            set
            {
                this._TotalOldEftRatioByMgc = value;
            }
        }

        public int totalValue
        {
            get
            {
                int num = 0;
                if (this.Calculator != null)
                {
                    num = this.Calculator(this, ValueDataType.TYPE_TOTAL);
                }
                else
                {
                    long num2 = ((((this.baseValue + this.addValue) - this.decValue) * ((0x2710 + this.addRatio) - this.decRatio)) / 0x2710L) + this.addValueOffRatio;
                    num = (int) num2;
                }
                num = (num * (0x2710 + this.totalEftRatio)) / 0x2710;
                if ((this._type != RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD) && (this._type != RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE))
                {
                    num = (num <= 0) ? 0 : num;
                }
                if (this.maxLimitValue > 0)
                {
                    num = (num <= this.maxLimitValue) ? num : this.maxLimitValue;
                }
                return num;
            }
        }

        public RES_FUNCEFT_TYPE Type
        {
            get
            {
                return this._type;
            }
        }
    }
}

