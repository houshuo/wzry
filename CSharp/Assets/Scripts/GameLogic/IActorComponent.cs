namespace Assets.Scripts.GameLogic
{
    using System;

    public interface IActorComponent
    {
        void Born(ActorRoot owner);
        void UpdateLogic(int delta);
    }
}

