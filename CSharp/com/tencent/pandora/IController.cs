namespace com.tencent.pandora
{
    using System;

    public interface IController
    {
        void ExecuteCommand(IMessage message);
        bool HasCommand(string messageName);
        void RegisterCommand(string messageName, System.Type commandType);
        void RegisterViewCommand(IView view, string[] commandNames);
        void RemoveCommand(string messageName);
        void RemoveViewCommand(IView view, string[] commandNames);
    }
}

