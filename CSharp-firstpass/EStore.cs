using System;

internal class EStore
{
    public readonly uint refKey;
    public readonly RestoreDelegate restoreDelegate;
    public readonly StoreDelegate storeDelegate;
    public readonly ushort typKey;
    public readonly object val;

    public EStore(object obj, ushort tpk, uint rfk, StoreDelegate sd, RestoreDelegate rd)
    {
        this.val = obj;
        this.typKey = tpk;
        this.refKey = rfk;
        this.storeDelegate = sd;
        this.restoreDelegate = rd;
    }
}

