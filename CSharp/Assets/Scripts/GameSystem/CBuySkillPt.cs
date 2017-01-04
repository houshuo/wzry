namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine.UI;

    internal class CBuySkillPt : CBuyPanelBase
    {
        private Text m_content;
        private int m_CostDiamond;
        private int m_Gain;

        public void Buy()
        {
            if (this.bCanBuy)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
                msg.stPkgData.stShopBuyReq.iBuyType = 5;
                msg.stPkgData.stShopBuyReq.iBuySubType = this.CurBuyTime + 1;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
            else if (!base.IsHaveEnoughDianQuan(1))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("没有足够的钻石！", false, 1.5f, null, new object[0]);
            }
            else if (!this.bSkillPointZero)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("当前技能点不为零，不能继续购买！", false, 1.5f, null, new object[0]);
            }
        }

        public override void BuyRsp(CSPkg msg)
        {
            base.BuyRsp(msg);
        }

        private void calcComsume()
        {
            ResShopInfo dataByKey = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint) ((ushort) ((500 + base.m_CurBuyTime) + 1)));
            if (dataByKey != null)
            {
                this.m_CostDiamond = (int) dataByKey.dwCoinPrice;
                this.m_Gain = (int) dataByKey.dwValue;
            }
            else
            {
                this.m_CostDiamond = 0;
                this.m_Gain = 0;
            }
        }

        protected override uint GetRequireDianquan(int times)
        {
            uint num = 0;
            uint dwCoinPrice = 0;
            for (int i = 0; i < times; i++)
            {
                dwCoinPrice = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint) ((ushort) (((500 + base.m_CurBuyTime) + 1) + i))).dwCoinPrice;
                if ((dwCoinPrice + num) > 0xffffffffL)
                {
                    return uint.MaxValue;
                }
                num += dwCoinPrice;
            }
            return num;
        }

        public void init()
        {
            base.FormPath = "UGUI/Form/System/Purchase/Form_BuySkillPoint.prefab";
            base.m_MaxBuyTime = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x1b).dwConfValue;
        }

        public override void initPanel(CUIFormScript form)
        {
            base.initPanel(form);
            this.m_content = base.m_FormScript.transform.FindChild("Content").GetComponent<Text>();
            base.m_Buttons = new GameObject[] { base.m_FormScript.transform.Find("ButtonGroup/Button_Confirm").gameObject, base.m_FormScript.transform.Find("ButtonGroup/Button_Cancel").gameObject };
            base.m_CheckRightButton = base.m_FormScript.transform.Find("ButtonGroup/Button_CheckRight").gameObject;
            this.RefreshUI();
            Singleton<EventRouter>.GetInstance().AddEventHandler("MasterAttributesChanged", new System.Action(this.onAtrrChange));
        }

        private void onAtrrChange()
        {
            this.RefreshUI();
        }

        private void RefreshUI()
        {
            if (base.bOpen)
            {
                if (!base.bTimesOut)
                {
                    string[] args = new string[] { this.m_Gain.ToString(), this.m_CostDiamond.ToString(), this.CurBuyTime.ToString() };
                    this.m_content.text = Singleton<CTextManager>.GetInstance().GetText("BuyAct_NoSkillPoint", args);
                }
                else
                {
                    string[] textArray2 = new string[] { this.CurBuyTime.ToString(), this.m_MaxBuyTime.ToString() };
                    this.m_content.text = Singleton<CTextManager>.GetInstance().GetText("BuyAct_NoMore", textArray2);
                }
                this.updateButtonState();
            }
        }

        public void SetSvrData(ref CSDT_ACNT_SHOPBUY_INFO ShopBuyInfo)
        {
            this.CurBuyTime = base.m_MaxBuyTime - ShopBuyInfo.LeftShopBuyCnt[2];
        }

        public void unInit()
        {
        }

        private void updateButtonState()
        {
            if (!base.bTimesOut)
            {
                base.showVipButton(false);
            }
            else
            {
                base.showVipButton(true);
            }
        }

        public override bool bCanBuy
        {
            get
            {
                return ((base.IsHaveEnoughDianQuan(1) && base.IsHaveEnoughTimes(1)) && this.bSkillPointZero);
            }
        }

        private bool bSkillPointZero
        {
            get
            {
                return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_skillPoint <= 0);
            }
        }

        public override int CurBuyTime
        {
            get
            {
                return base.m_CurBuyTime;
            }
            set
            {
                base.m_CurBuyTime = value;
                this.calcComsume();
                this.RefreshUI();
            }
        }
    }
}

