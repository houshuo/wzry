namespace com.tencent.pandora
{
    using System;

    public interface IView
    {
        void OnMessage(IMessage message);
    }
}

