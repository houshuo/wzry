namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    internal abstract class PVEExpItemBase
    {
        protected Image animaBar;
        protected uint ExpToAdd;
        public uint m_exp;
        protected Image m_ExpBar1;
        protected Text m_ExpTxt;
        public int m_level;
        protected Text m_LevelTxt;
        protected uint m_maxExp;
        protected string m_Name;
        protected Text m_NameText;
        protected GameObject m_Root;
        private float TweenDstVal;

        protected PVEExpItemBase()
        {
        }

        public virtual void addExp(uint addVal)
        {
            this.animaBar = this.m_ExpBar1;
            this.m_maxExp = this.calcMaxExp();
            this.ExpToAdd = addVal;
            this.SetUI();
            if ((this.ExpToAdd + this.m_exp) > this.m_maxExp)
            {
                this.TweenExpBar((float) this.m_maxExp, true);
            }
            else if (this.ExpToAdd > 0)
            {
                this.TweenExpBar((float) (this.m_exp + this.ExpToAdd), false);
            }
        }

        protected abstract uint calcMaxExp();
        protected string getExpString(uint exp)
        {
            return string.Format("+{0}", exp);
        }

        protected virtual void SetUI()
        {
            this.m_NameText.text = this.m_Name;
            this.m_ExpTxt.text = this.getExpString(this.ExpToAdd);
            this.m_LevelTxt.text = string.Format("Lv{0}", this.m_level);
            this.m_ExpBar1.CustomFillAmount(((float) this.m_exp) / ((float) this.m_maxExp));
        }

        protected virtual void TweenEnd(float val)
        {
            if (this.m_maxExp == this.m_exp)
            {
                this.Level++;
                this.m_exp = 0;
                this.m_ExpBar1.CustomFillAmount(0f);
                this.m_LevelTxt.text = string.Format("Lv{0}", this.m_level);
                if ((this.ExpToAdd + this.m_exp) > this.m_maxExp)
                {
                    this.TweenExpBar((float) this.m_maxExp, true);
                }
                else if (this.ExpToAdd > 0)
                {
                    this.TweenExpBar((float) (this.m_exp + this.ExpToAdd), true);
                }
            }
        }

        protected void TweenExpBar(float dst, bool bUpdate)
        {
            this.TweenDstVal = dst;
            this.ExpToAdd -= this.m_maxExp - this.m_exp;
            LeanTween.value(this.m_Root, new Action<float>(this.TweenUpdate), (float) this.m_exp, dst, !bUpdate ? 1.6f : 1f).setEase(LeanTweenType.linear);
        }

        protected void TweenUpdate(float val)
        {
            this.m_exp = (uint) val;
            this.animaBar.CustomFillAmount(val / ((float) this.m_maxExp));
            if (this.TweenDstVal == this.m_exp)
            {
                this.TweenEnd(val);
            }
        }

        public int Level
        {
            get
            {
                return this.m_level;
            }
            set
            {
                this.m_level = value;
                this.m_maxExp = this.calcMaxExp();
            }
        }
    }
}

