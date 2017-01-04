namespace Assets.Scripts.GameLogic
{
    using System;

    public enum SoldierSpawnResult
    {
        ShouldWaitStart,
        ShouldWaitInterval,
        ShouldWaitSoldierInterval,
        ThresholdShouldWait,
        Finish,
        Error,
        UnStarted,
        Completed
    }
}

