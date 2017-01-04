using System;

[ObjectTypeSerializer(typeof(byte[]))]
public class ByteArraySerializer : BaseArrayTypeSerializer, ICustomizedObjectSerializer, ICustomInstantiate
{
    public object Instantiate(BinaryNode node)
    {
        return new byte[int.Parse(GameSerializer.GetNodeAttr(node, "Size"))];
    }

    public bool IsObjectTheSame(object o, object oPrefab)
    {
        byte[] buffer = (byte[]) o;
        byte[] buffer2 = (byte[]) oPrefab;
        if (buffer != buffer2)
        {
            if ((buffer == null) || (buffer2 == null))
            {
                return false;
            }
            if (buffer.Length != buffer2.Length)
            {
                return false;
            }
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != buffer2[i])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ObjectDeserialize(ref object o, BinaryNode node)
    {
        o = node.GetChild(0).GetValue();
    }
}

