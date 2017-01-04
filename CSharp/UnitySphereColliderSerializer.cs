using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(SphereCollider))]
public class UnitySphereColliderSerializer : ICustomizedComponentSerializer
{
    private const string XML_ATTR_CENTER = "center";
    private const string XML_ATTR_R = "radius";
    private const string XML_IS_TRIGGER = "trigger";

    public void ComponentDeserialize(Component cmp, BinaryNode node)
    {
        SphereCollider collider = cmp as SphereCollider;
        collider.center = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center"));
        collider.radius = UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "radius"));
        collider.isTrigger = bool.Parse(GameSerializer.GetAttribute(node, "trigger"));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        SphereCollider collider = cmp as SphereCollider;
        SphereCollider collider2 = cmpPrefab as SphereCollider;
        return (((collider.center == collider2.center) && (collider.radius == collider2.radius)) && (collider.isTrigger == collider2.isTrigger));
    }
}

