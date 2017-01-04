using System;

public class MessageHandlerAttribute : Attribute, IIdentifierAttribute<uint>
{
    public int MessageID;

    public MessageHandlerAttribute(int InMessageID)
    {
        this.MessageID = InMessageID;
    }

    public uint[] AdditionalIdList
    {
        get
        {
            return null;
        }
    }

    public uint ID
    {
        get
        {
            return (uint) this.MessageID;
        }
    }
}

