using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class CheatCommandEntryMethodAttribute : AutoRegisterAttribute
{
    public CheatCommandEntryMethodAttribute(string InComment, bool bInSupportInEditor, bool bHiddenInMobile = false)
    {
        this.comment = InComment;
        this.isSupportInEditor = bInSupportInEditor;
        this.isHiddenInMobile = bHiddenInMobile;
    }

    public string comment { get; protected set; }

    public bool isHiddenInMobile { get; protected set; }

    public bool isSupportInEditor { get; protected set; }
}

