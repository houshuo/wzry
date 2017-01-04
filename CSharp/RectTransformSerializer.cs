using System;
using UnityEngine;

[ComponentTypeSerializer(typeof(RectTransform))]
public class RectTransformSerializer : ICustomizedComponentSerializer
{
    public void ComponentDeserialize(Component o, BinaryNode node)
    {
        RectTransform transform = o as RectTransform;
        transform.localScale = new Vector3(float.Parse(GameSerializer.GetAttribute(node, "SX")), float.Parse(GameSerializer.GetAttribute(node, "SY")), float.Parse(GameSerializer.GetAttribute(node, "SZ")));
        transform.localRotation = new Quaternion(float.Parse(GameSerializer.GetAttribute(node, "RX")), float.Parse(GameSerializer.GetAttribute(node, "RY")), float.Parse(GameSerializer.GetAttribute(node, "RZ")), float.Parse(GameSerializer.GetAttribute(node, "RW")));
        transform.anchorMin = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "anchorMinX")), float.Parse(GameSerializer.GetAttribute(node, "anchorMinY")));
        transform.anchorMax = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "anchorMaxX")), float.Parse(GameSerializer.GetAttribute(node, "anchorMaxY")));
        transform.offsetMin = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "offsetMinX")), float.Parse(GameSerializer.GetAttribute(node, "offsetMinY")));
        transform.offsetMax = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "offsetMaxX")), float.Parse(GameSerializer.GetAttribute(node, "offsetMaxY")));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        RectTransform transform = cmp as RectTransform;
        RectTransform transform2 = cmpPrefab as RectTransform;
        return (((((transform.localScale == transform2.localScale) && (transform.localRotation == transform2.localRotation)) && ((transform.anchorMin == transform2.anchorMin) && (transform.anchorMax == transform2.anchorMax))) && (transform.offsetMin == transform2.offsetMin)) && (transform.offsetMax == transform2.offsetMax));
    }
}

