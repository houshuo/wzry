using System;

public interface IMessage
{
    string ToString();

    object Body { get; set; }

    string Name { get; }

    string Type { get; set; }
}

