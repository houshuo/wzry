using System;
using System.Collections;
using UnityEngine;

[ComponentTypeSerializer(typeof(Animation))]
public class AnimationSerializer : ICustomizedComponentSerializer
{
    private const string DOM_ANIMNAME = "AniName";
    private const string DOM_ATTR_VALUE = "Value";

    public void ComponentDeserialize(Component o, BinaryNode node)
    {
        Animation animation = o as Animation;
        for (int i = 0; i < node.GetChildNum(); i++)
        {
            BinaryNode child = node.GetChild(i);
            if (child.GetName() == "AniName")
            {
                string nodeAttr = GameSerializer.GetNodeAttr(child, "Value");
                AnimationClip resource = (AnimationClip) GameSerializer.GetResource(nodeAttr, typeof(AnimationClip));
                if (null == resource)
                {
                    Debug.LogError("Cannot find Animation: " + nodeAttr);
                    return;
                }
                if ((nodeAttr != null) && (nodeAttr.Length != 0))
                {
                    animation.AddClip(resource, resource.name);
                }
            }
        }
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        Animation animation = cmp as Animation;
        Animation animation2 = cmpPrefab as Animation;
        if (animation.GetClipCount() != animation2.GetClipCount())
        {
            return false;
        }
        IEnumerator enumerator = animation.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                AnimationState current = (AnimationState) enumerator.Current;
                if (animation.GetClip(current.name) != animation2.GetClip(current.name))
                {
                    return false;
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        return true;
    }
}

