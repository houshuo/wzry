namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine.UI;

    public class CBuyDiamondView
    {
        private CUIFormScript m_cuiForm;

        public void Draw()
        {
            CUIListScript component = this.m_cuiForm.GetWidget(0).GetComponent<CUIListScript>();
            int[] numArray = new int[] { 100, 200, 300, 400, 500, 600 };
            component.SetElementAmount(numArray.Length);
            for (byte i = 0; i < numArray.Length; i = (byte) (i + 1))
            {
                CUIListElementScript elemenet = component.GetElemenet(i);
                CUIEventScript script3 = elemenet.gameObject.GetComponent<CUIEventScript>();
                if (script3 == null)
                {
                    script3 = elemenet.gameObject.AddComponent<CUIEventScript>();
                    script3.Initialize(elemenet.m_belongedFormScript);
                }
                stUIEventParams eventParams = new stUIEventParams();
                int num2 = numArray[i] / 10;
                eventParams.tag = numArray[i];
                script3.SetUIEvent(enUIEventType.Click, enUIEventID.Purchase_BuyDiamond, eventParams);
                Text text = elemenet.gameObject.transform.Find("pnlPrice/txtQuantity").GetComponent<Text>();
                Text text2 = elemenet.gameObject.transform.Find("pnlPrice/txtPrice").GetComponent<Text>();
                text.text = numArray[i].ToString();
                text2.text = num2.ToString();
            }
        }

        public CUIFormScript Form
        {
            set
            {
                this.m_cuiForm = value;
                if (this.m_cuiForm != null)
                {
                    this.Draw();
                }
            }
        }

        public enum enBuyDiamondViewWidget
        {
            Charge_Items
        }
    }
}

