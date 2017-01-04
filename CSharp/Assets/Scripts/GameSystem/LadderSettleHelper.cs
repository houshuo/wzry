namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;

    public class LadderSettleHelper : MonoBehaviour
    {
        public void OnLevelDownEndAnimationOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderLevelDownEndOver();
        }

        public void OnLevelDownStartAnimationOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderLevelDownStartOver();
        }

        public void OnLevelUpEndAnimationOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderLevelUpEndOver();
        }

        public void OnLevelUpStartAnimationOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderLevelUpStartOver();
        }

        public void OnShowInAnimationOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderShowInOver();
        }

        public void OnWangZheXingAnimationEndOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderWangZheXingEndOver();
        }

        public void OnWangZheXingAnimationStartOver()
        {
        }

        public void OnXingDownAnimationOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderXingDownOver();
        }

        public void OnXingUpAnimationOver()
        {
            Singleton<SettlementSystem>.instance.OnLadderXingUpOver();
        }
    }
}

