namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;

    public class CampaignFormView : ActivityView
    {
        private const float WIDGET_SPACING_Y = 5f;

        public CampaignFormView(GameObject node, ActivityForm actvForm, Activity actv) : base(node, actvForm, actv)
        {
            this.SetActivity(actv);
        }

        public override void Clear()
        {
            base.Clear();
            for (int i = 0; i < 0x10; i++)
            {
                Utility.FindChildSafe(base.root, ((WidgetDefine) i).ToString()).CustomSetActive(false);
            }
        }

        public void SetActivity(Activity actv)
        {
            this.Clear();
            base.activity = actv;
            if (actv != null)
            {
                char[] separator = new char[] { ' ', ',' };
                string[] strArray = actv.Wigets.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                float val = 0f;
                int[] numArray = new int[0x10];
                int num2 = 0;
                for (int i = 0; i < strArray.Length; i++)
                {
                    int result = 0;
                    if ((!int.TryParse(strArray[i], out result) || (result <= 0)) || (result >= 0x10))
                    {
                        object[] inParameters = new object[] { actv.ID };
                        DebugHelper.Assert(false, "[CampaignFormView][Activity:{0}] widgets config error!", inParameters);
                        continue;
                    }
                    WidgetDefine define = (WidgetDefine) result;
                    GameObject node = Utility.FindChildSafe(base.root, define.ToString());
                    if (null != node)
                    {
                        numArray[num2++] = result;
                        node.SetActive(true);
                        ActivityWidget item = null;
                        switch (define)
                        {
                            case WidgetDefine.Introduction:
                                item = new IntrodWidget(node, this);
                                break;

                            case WidgetDefine.Banner:
                                item = new BannerWidget(node, this);
                                break;

                            case WidgetDefine.Progress:
                                item = new ProgressWidget(node, this);
                                break;

                            case WidgetDefine.Rewards:
                                item = new RewardWidget(node, this);
                                break;

                            case WidgetDefine.MultiGain:
                                item = new MultiGainWgt(node, this);
                                break;

                            case WidgetDefine.CheckIn:
                                item = new CheckInWidget(node, this);
                                break;

                            case WidgetDefine.Notice:
                                item = new NoticeWidget(node, this);
                                break;

                            case WidgetDefine.Exchange:
                                item = new ExchangeWgt(node, this);
                                break;

                            case WidgetDefine.PointsExchange:
                                item = new PointsExchangeWgt(node, this);
                                break;
                        }
                        if (item != null)
                        {
                            item.SetPosY(val);
                            val -= item.Height + 5f;
                            base.WidgetList.Add(item);
                        }
                    }
                }
                for (int j = 1; j < 0x10; j++)
                {
                    bool flag = false;
                    for (int m = 0; m < num2; m++)
                    {
                        if (numArray[m] == j)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Utility.FindChildSafe(base.root, ((WidgetDefine) j).ToString()).CustomSetActive(false);
                    }
                }
                base.root.GetComponent<RectTransform>().sizeDelta = new Vector2(base.root.GetComponent<RectTransform>().sizeDelta.x, -val);
                for (int k = 0; k < base.WidgetList.Count; k++)
                {
                    base.WidgetList[k].OnShow();
                }
            }
        }

        public enum WidgetDefine
        {
            Banner = 2,
            CheckIn = 12,
            EMPTY = 0,
            Exchange = 14,
            Introduction = 1,
            MAX = 0x10,
            MultiGain = 11,
            Notice = 13,
            PointsExchange = 15,
            Progress = 3,
            Rewards = 4
        }
    }
}

