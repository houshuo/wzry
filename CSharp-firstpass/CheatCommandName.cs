using System;
using System.Runtime.CompilerServices;

public class CheatCommandName
{
    public CheatCommandName(string InName)
    {
        DebugHelper.Assert(!string.IsNullOrEmpty(InName));
        this.rawName = InName;
        char[] separator = new char[] { '/' };
        string[] inStringArray = InName.Split(separator);
        if ((inStringArray != null) && (inStringArray.Length > 1))
        {
            this.baseName = inStringArray[inStringArray.Length - 1];
            this.groupName = inStringArray[inStringArray.Length - 2];
            this.groupHierarchies = LinqS.Take(inStringArray, inStringArray.Length - 1);
        }
        else
        {
            this.baseName = InName;
            this.groupName = "通用";
            this.groupHierarchies = new string[] { this.groupName };
        }
    }

    public string baseName { get; protected set; }

    public string[] groupHierarchies { get; protected set; }

    public string groupName { get; protected set; }

    public string rawName { get; protected set; }
}

