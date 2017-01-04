namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void TalkerMessageHandler<T>(object request, TalkerEventArgs<T> e) where T: IUnpackable;
}

