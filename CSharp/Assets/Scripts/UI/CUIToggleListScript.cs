namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CUIToggleListScript : CUIListScript
    {
        public bool m_isMultiSelected;
        private bool[] m_multiSelected;
        private int m_selected;

        public bool[] GetMultiSelected()
        {
            return this.m_multiSelected;
        }

        public int GetSelected()
        {
            return this.m_selected;
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                if (this.m_isMultiSelected)
                {
                    this.m_multiSelected = new bool[base.m_elementAmount];
                    for (int i = 0; i < base.m_elementAmount; i++)
                    {
                        this.m_multiSelected[i] = false;
                    }
                }
                else
                {
                    this.m_selected = -1;
                }
                base.Initialize(formScript);
            }
        }

        public override bool IsSelectedIndex(int index)
        {
            return (!this.m_isMultiSelected ? (index == this.m_selected) : this.m_multiSelected[index]);
        }

        public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            if (this.m_isMultiSelected)
            {
                bool selected = this.m_multiSelected[index];
                selected = !selected;
                this.m_multiSelected[index] = selected;
                CUIListElementScript elemenet = base.GetElemenet(index);
                if (elemenet != null)
                {
                    elemenet.ChangeDisplay(selected);
                }
                base.DispatchElementSelectChangedEvent();
            }
            else if (index == this.m_selected)
            {
                if (base.m_alwaysDispatchSelectedChangeEvent)
                {
                    base.DispatchElementSelectChangedEvent();
                }
            }
            else
            {
                if (this.m_selected >= 0)
                {
                    CUIListElementScript script2 = base.GetElemenet(this.m_selected);
                    if (script2 != null)
                    {
                        script2.ChangeDisplay(false);
                    }
                }
                this.m_selected = index;
                if (this.m_selected >= 0)
                {
                    CUIListElementScript script3 = base.GetElemenet(this.m_selected);
                    if (script3 != null)
                    {
                        script3.ChangeDisplay(true);
                    }
                }
                base.DispatchElementSelectChangedEvent();
            }
        }

        public override void SetElementAmount(int amount, List<Vector2> elementsSize)
        {
            if (this.m_isMultiSelected && ((this.m_multiSelected == null) || (this.m_multiSelected.Length < amount)))
            {
                bool[] flagArray = new bool[amount];
                for (int i = 0; i < amount; i++)
                {
                    if ((this.m_multiSelected != null) && (i < this.m_multiSelected.Length))
                    {
                        flagArray[i] = this.m_multiSelected[i];
                    }
                    else
                    {
                        flagArray[i] = false;
                    }
                }
                this.m_multiSelected = flagArray;
            }
            base.SetElementAmount(amount, elementsSize);
        }

        public void SetMultiSelected(int index, bool selected)
        {
            if ((index >= 0) && (index < base.m_elementAmount))
            {
                this.m_multiSelected[index] = selected;
                for (int i = 0; i < base.m_elementScripts.Count; i++)
                {
                    base.m_elementScripts[i].ChangeDisplay(this.IsSelectedIndex(base.m_elementScripts[i].m_index));
                }
            }
        }

        public void SetSelected(int selected)
        {
            this.m_selected = selected;
            for (int i = 0; i < base.m_elementScripts.Count; i++)
            {
                base.m_elementScripts[i].ChangeDisplay(this.IsSelectedIndex(base.m_elementScripts[i].m_index));
            }
        }
    }
}

