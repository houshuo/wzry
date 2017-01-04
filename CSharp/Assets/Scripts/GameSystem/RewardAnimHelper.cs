namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;

    internal class RewardAnimHelper : MonoBehaviour
    {
        public void EndBasicAward()
        {
            Singleton<PVESettleSys>.instance.OnAwardDisplayEnd();
        }

        public void EndPageSlideIn()
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_slide02", null);
        }

        public void TreasureBoxShaking()
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_box", null);
        }
    }
}

