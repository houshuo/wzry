using System;

public interface IStore
{
    void __Restore(GameStore gs);
    void __Store(GameStore gs);

    uint __RefKey { get; }

    ushort __TypKey { get; }
}

