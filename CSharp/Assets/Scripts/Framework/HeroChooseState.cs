namespace Assets.Scripts.Framework
{
    using System;

    [GameState]
    public class HeroChooseState : BaseState
    {
        private void OnHeroChooseSceneCompleted()
        {
        }

        public override void OnStateEnter()
        {
            Singleton<HeroChooseLogic>.GetInstance().OpenInitChooseHeroForm();
            Singleton<ResourceLoader>.GetInstance().LoadScene("ChooseHero", new ResourceLoader.LoadCompletedDelegate(this.OnHeroChooseSceneCompleted));
        }

        public override void OnStateLeave()
        {
            Singleton<HeroChooseLogic>.GetInstance().CloseInitChooseHeroForm();
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
        }
    }
}

