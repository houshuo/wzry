namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void TalkerMessageWithReceiptHandler<TResp, TReceipt>(TResp resp, ref TReceipt receipt) where TResp: IUnpackable where TReceipt: IPackable;
}

