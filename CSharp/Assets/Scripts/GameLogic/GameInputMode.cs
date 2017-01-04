namespace Assets.Scripts.GameLogic
{
    using System;

    public abstract class GameInputMode : BaseState
    {
        protected GameInput inputSys;

        public GameInputMode(GameInput InSys)
        {
            this.inputSys = InSys;
        }

        public virtual void StopInput()
        {
            this.inputSys.SendStopMove(false);
        }

        public abstract void Update();
    }
}

