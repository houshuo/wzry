namespace com.tencent.pandora
{
    using System;

    public class ControllerCommand : ICommand
    {
        public virtual void Execute(IMessage message)
        {
        }
    }
}

