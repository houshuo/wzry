namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class RankingItemHelper : MonoBehaviour
    {
        public GameObject AddFriend;
        public GameObject FindBtn;
        public GameObject GuestIcon;
        public GameObject HeadIcon;
        public GameObject HeadIconFrame;
        public GameObject LadderGo;
        public GameObject LadderXing;
        public GameObject No1;
        public GameObject No1BG;
        public GameObject No1IconFrame;
        public GameObject No2;
        public GameObject No3;
        public GameObject NoRankingText;
        public GameObject Online;
        public GameObject PvpRankingIcon;
        public GameObject QqIcon;
        public GameObject QqVip;
        public GameObject RankingNumText;
        public GameObject RankingUpDownIcon;
        public GameObject Selected;
        public GameObject SendCoin;
        public GameObject VipIcon;
        public GameObject WxIcon;

        public void OnChatHideAnimationEnd()
        {
            Singleton<CChatController>.instance.OnClosingAnimEnd();
        }

        public void OnHideAnimationEnd()
        {
            Singleton<RankingSystem>.instance.OnHideAnimationEnd();
        }

        public void ShowSendButton(bool bEnable)
        {
            if (this.SendCoin != null)
            {
                Button component = this.SendCoin.GetComponent<Button>();
                if (component != null)
                {
                    component.gameObject.CustomSetActive(true);
                    CUICommonSystem.SetButtonEnableWithShader(component, bEnable, true);
                }
            }
        }
    }
}

