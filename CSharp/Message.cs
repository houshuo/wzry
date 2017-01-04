using System;

public class Message : IMessage
{
    private object m_body;
    private string m_name;
    private string m_type;

    public Message(string name) : this(name, null, null)
    {
    }

    public Message(string name, object body) : this(name, body, null)
    {
    }

    public Message(string name, object body, string type)
    {
        this.m_name = name;
        this.m_body = body;
        this.m_type = type;
    }

    public override string ToString()
    {
        return ((("Notification Name: " + this.Name) + "\nBody:" + ((this.Body != null) ? this.Body.ToString() : "null")) + "\nType:" + ((this.Type != null) ? this.Type : "null"));
    }

    public virtual object Body
    {
        get
        {
            return this.m_body;
        }
        set
        {
            this.m_body = value;
        }
    }

    public virtual string Name
    {
        get
        {
            return this.m_name;
        }
    }

    public virtual string Type
    {
        get
        {
            return this.m_type;
        }
        set
        {
            this.m_type = value;
        }
    }
}

