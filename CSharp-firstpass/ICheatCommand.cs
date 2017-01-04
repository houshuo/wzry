using System;
using System.Runtime.InteropServices;

public interface ICheatCommand
{
    bool CheckArguments(string[] InArguments, out string OutMessage);
    string StartProcess(string[] InArguments);

    string[] arguments { get; }

    ArgumentDescriptionAttribute[] argumentsTypes { get; }

    CheatCommandName command { get; }

    string comment { get; }

    string description { get; }

    string fullyHelper { get; }

    bool isHiddenInMobile { get; }

    bool isSupportInEditor { get; }
}

