namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void TalkerMessageWithoutReceiptHandler<TResp>(TResp resp) where TResp: IUnpackable;
}

