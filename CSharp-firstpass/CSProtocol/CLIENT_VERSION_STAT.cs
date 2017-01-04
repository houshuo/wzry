namespace CSProtocol
{
    using System;

    public enum CLIENT_VERSION_STAT
    {
        VERSION_BAD = -1,
        VERSION_CANUPDATE = 2,
        VERSION_CONFIG_BAD = -2,
        VERSION_FORCEUPDATE = 1,
        VERSION_IN_REVIEW = 3,
        VERSION_NEWEST = 0
    }
}

