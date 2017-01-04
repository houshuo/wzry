namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;

    internal abstract class CBuyPanelBase
    {
        protected bool bOpen;
        protected bool bShopping;
        protected string FormPath = string.Empty;
        protected bool m_bShowCheckRight;
        protected GameObject[] m_Buttons;
        protected GameObject m_CheckRightButton;
        protected int m_CurBuyTime;
        protected CUIFormScript m_FormScript;
        protected int m_MaxBuyTime;

        protected CBuyPanelBase()
        {
        }

        public virtual void BuyRsp(CSPkg msg)
        {
            this.close();
            this.bShopping = true;
            this.CurBuyTime = msg.stPkgData.stShopBuyRsp.iBuySubType;
            switch (msg.stPkgData.stShopBuyRsp.iBuyType)
            {
                case 4:
                {
                    object[] replaceArr = new object[] { msg.stPkgData.stShopBuyRsp.iChgValue.ToString() };
                    Singleton<CUIManager>.GetInstance().OpenTips("BuyAct_BuyApSuccess", true, 1f, null, replaceArr);
                    break;
                }
                case 5:
                {
                    object[] objArray2 = new object[] { msg.stPkgData.stShopBuyRsp.iChgValue.ToString() };
                    Singleton<CUIManager>.GetInstance().OpenTips("BuyAct_BuySpSuccess", true, 1f, null, objArray2);
                    break;
                }
            }
        }

        public virtual void close()
        {
            if (this.bOpen)
            {
                this.bOpen = false;
                Singleton<CUIManager>.GetInstance().CloseForm(this.FormPath);
            }
        }

        protected abstract uint GetRequireDianquan(int times);
        public virtual void initPanel(CUIFormScript form)
        {
            this.bShopping = false;
        }

        protected bool IsHaveEnoughDianQuan(int times)
        {
            return (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().DianQuan >= this.GetRequireDianquan(times));
        }

        protected bool IsHaveEnoughTimes(int TimesToBuy)
        {
            return ((this.m_MaxBuyTime - this.CurBuyTime) >= TimesToBuy);
        }

        public virtual void open()
        {
            if (!this.bOpen)
            {
                this.m_FormScript = Singleton<CUIManager>.GetInstance().OpenForm(this.FormPath, false, true);
                this.bOpen = true;
                this.initPanel(this.m_FormScript);
            }
        }

        protected void showVipButton(bool bShowCheckRight)
        {
            this.m_bShowCheckRight = bShowCheckRight;
            if (this.m_bShowCheckRight)
            {
                if (this.m_Buttons != null)
                {
                    for (int i = 0; i < this.m_Buttons.Length; i++)
                    {
                        this.m_Buttons[i].CustomSetActive(false);
                    }
                }
                if (this.m_CheckRightButton != null)
                {
                    this.m_CheckRightButton.CustomSetActive(true);
                }
            }
            else
            {
                if (this.m_Buttons != null)
                {
                    for (int j = 0; j < this.m_Buttons.Length; j++)
                    {
                        this.m_Buttons[j].CustomSetActive(true);
                    }
                }
                if (this.m_CheckRightButton != null)
                {
                    this.m_CheckRightButton.CustomSetActive(false);
                }
            }
        }

        public abstract bool bCanBuy { get; }

        public bool bTimesOut
        {
            get
            {
                return (this.m_CurBuyTime >= this.m_MaxBuyTime);
            }
        }

        public abstract int CurBuyTime { get; set; }
    }
}

