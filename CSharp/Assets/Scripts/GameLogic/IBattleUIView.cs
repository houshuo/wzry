namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public interface IBattleUIView
    {
        void Clear();
        void Hide();
        void Init(GameObject obj);
        void Show();
    }
}

