using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Matrix4x4))]
public class Matrix4x4Serializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
    public bool IsObjectTheSame(object o, object oPrefab)
    {
        return (o == oPrefab);
    }

    public void ObjectDeserialize(ref object o, BinaryNode node)
    {
        byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
        Matrix4x4 matrix = (Matrix4x4) o;
        UnityBasetypeSerializer.BytesToMatrix4x4(ref matrix, binaryAttribute);
        o = matrix;
    }
}

