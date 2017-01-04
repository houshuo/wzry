using CSProtocol;
using System;

public class FrameCSSYNCCommandClassAttribute : Attribute, IIdentifierAttribute<CSSYNC_TYPE_DEF>
{
    public CSSYNC_TYPE_DEF CmdID;

    public FrameCSSYNCCommandClassAttribute(CSSYNC_TYPE_DEF InCmdID)
    {
        this.CmdID = InCmdID;
    }

    public CSSYNC_TYPE_DEF[] AdditionalIdList
    {
        get
        {
            return null;
        }
    }

    public CSSYNC_TYPE_DEF ID
    {
        get
        {
            return this.CmdID;
        }
    }
}

