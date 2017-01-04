using System;

public interface IConsoleLogger
{
    void AddMessage(string InMessage);
    void Clear();

    string message { get; }
}

