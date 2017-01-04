namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public interface ITreasureChestStrategy
    {
        event OnDropTreasureChestDelegate OnDropTreasure;

        void Initialize(int InMaxCount);
        void NotifyDropEvent(PoolObjHandle<ActorRoot> actor);
        void NotifyMatchEnd();
        void Stop();

        int droppedCount { get; }

        bool isSupportDrop { get; }

        int maxCount { get; }
    }
}

