using System;
using UnityEngine;

public interface ICustomizedComponentSerializer
{
    void ComponentDeserialize(Component cmp, BinaryNode node);
    bool IsComponentSame(Component cmp, Component cmpPrefab);
}

