namespace Apollo
{
    using System;

    public enum ApolloPlatform
    {
        AutoLogin = 6,
        Guest = 5,
        Kakao = 0x3e8,
        None = 0,
        QQ = 2,
        QR = 0x100,
        QRQQ = 0x102,
        QRWechat = 0x101,
        Wechat = 1,
        [Obsolete("Obsolete since 1.1.13, using Wechat instead.")]
        Weixin = 1,
        WTLogin = 3
    }
}

