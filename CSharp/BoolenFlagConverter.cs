using System;

public abstract class BoolenFlagConverter
{
    protected BoolenFlagConverter()
    {
    }

    public static bool ToBool(EBooleanFlag InFlag)
    {
        return (InFlag == EBooleanFlag.是);
    }
}

