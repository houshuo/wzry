namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;

    public interface IBattleForm
    {
        void BattleStart();
        void CloseForm();
        void LateUpdate();
        bool OpenForm();
        void Update();
        void UpdateLogic(int delta);

        CUIFormScript FormScript { get; }
    }
}

