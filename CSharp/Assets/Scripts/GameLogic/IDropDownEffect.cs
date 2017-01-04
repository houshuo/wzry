namespace Assets.Scripts.GameLogic
{
    using System;

    public interface IDropDownEffect
    {
        void Bind(DropItem item);
        void OnUpdate(int delta);

        bool isFinished { get; }

        VInt3 location { get; }
    }
}

