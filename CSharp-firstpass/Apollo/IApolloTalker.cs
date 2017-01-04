namespace Apollo
{
    using ApolloTdr;
    using System;

    public interface IApolloTalker
    {
        ApolloResult Register(RawMessageHandler handler);
        ApolloResult Register<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp: IUnpackable;
        ApolloResult Register<TResp, TReceipt>(TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp: IUnpackable where TReceipt: IPackable;
        ApolloResult RegisterMessage<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp: IUnpackable;
        ApolloResult Send(IPackable request);
        ApolloResult Send(byte[] data, int usedSize);
        ApolloResult Send<TResp>(IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp: IUnpackable;
        ApolloResult SendMessage(IPackable request);
        void Unregister<TResp>();
        void Unregister(string cmd);
        void UnregisterRawMessageHandler();
        void Update(int num);

        bool AutoUpdate { get; set; }
    }
}

