using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(CapsuleCollider))]
public class UnityCapsuleColliderrSerializer : ICustomizedComponentSerializer
{
    private const string XML_ATTR_CENTER = "center";
    private const string XML_ATTR_D = "dir";
    private const string XML_ATTR_H = "height";
    private const string XML_ATTR_R = "radius";
    private const string XML_IS_TRIGGER = "trigger";

    public void ComponentDeserialize(Component cmp, BinaryNode node)
    {
        CapsuleCollider collider = cmp as CapsuleCollider;
        collider.center = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center"));
        collider.radius = UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "radius"));
        collider.height = UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "height"));
        collider.direction = UnityBasetypeSerializer.BytesToInt(GameSerializer.GetBinaryAttribute(node, "dir"));
        collider.isTrigger = bool.Parse(GameSerializer.GetAttribute(node, "trigger"));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        CapsuleCollider collider = cmp as CapsuleCollider;
        CapsuleCollider collider2 = cmpPrefab as CapsuleCollider;
        return ((((collider.center == collider2.center) && (collider.radius == collider2.radius)) && ((collider.height == collider2.height) && (collider.direction == collider2.direction))) && (collider.isTrigger == collider2.isTrigger));
    }
}

