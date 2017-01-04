using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(BoxCollider))]
public class UnityBoxColliderSerializer : ICustomizedComponentSerializer
{
    private const string XML_ATTR_CENTER = "center";
    private const string XML_ATTR_SIZE = "size";
    private const string XML_IS_TRIGGER = "trigger";

    public void ComponentDeserialize(Component cmp, BinaryNode node)
    {
        BoxCollider collider = cmp as BoxCollider;
        collider.center = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center"));
        collider.size = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "size"));
        collider.isTrigger = bool.Parse(GameSerializer.GetAttribute(node, "trigger"));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        BoxCollider collider = cmp as BoxCollider;
        BoxCollider collider2 = cmpPrefab as BoxCollider;
        return (((collider.center == collider2.center) && (collider.size == collider2.size)) && (collider.isTrigger == collider2.isTrigger));
    }
}

