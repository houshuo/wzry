namespace Assets.Scripts.Framework
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.Sound;
    using System;

    [GameState]
    public class LoadingState : BaseState
    {
        private string LastLevelBank;

        public override void OnStateEnter()
        {
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            Singleton<CUILoadingSystem>.instance.ShowLoading();
            Singleton<CSoundManager>.GetInstance().PostEvent("Login_Stop", null);
            Singleton<CSoundManager>.GetInstance().PostEvent("Play_Hall_Ending", null);
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            string str = (curLvelContext == null) ? string.Empty : curLvelContext.m_musicBankResName;
            if ((str != this.LastLevelBank) && !string.IsNullOrEmpty(this.LastLevelBank))
            {
                Singleton<CSoundManager>.instance.UnLoadBank(this.LastLevelBank, CSoundManager.BankType.LevelMusic);
            }
            if (!string.IsNullOrEmpty(str))
            {
                this.LastLevelBank = str;
                Singleton<CSoundManager>.instance.LoadBank(str, CSoundManager.BankType.LevelMusic);
            }
            CUICommonSystem.OpenFps();
        }

        public override void OnStateLeave()
        {
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
        }
    }
}

