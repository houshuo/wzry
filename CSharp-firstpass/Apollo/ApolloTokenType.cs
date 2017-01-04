namespace Apollo
{
    using System;

    public enum ApolloTokenType
    {
        Access = 1,
        [Obsolete("Obsolete since 1.1.13, using Access instead.")]
        GuestAccess = 1,
        None = 0,
        Pay = 3,
        [Obsolete("Obsolete since 1.1.13, using Access instead.")]
        QQAccess = 1,
        [Obsolete("Obsolete since 1.1.13, using Pay instead.")]
        QQPay = 3,
        Refresh = 2,
        STSignature = 4,
        [Obsolete("Obsolete since 1.1.13, using Access instead.")]
        WXAccess = 1,
        [Obsolete("Obsolete since 1.1.13, using Refresh instead.")]
        WXRefresh = 2
    }
}

