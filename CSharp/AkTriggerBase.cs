using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class AkTriggerBase : MonoBehaviour
{
    public Trigger triggerDelegate;

    protected AkTriggerBase()
    {
    }

    public static Dictionary<uint, string> GetAllDerivedTypes()
    {
        System.Type c = typeof(AkTriggerBase);
        System.Type[] types = c.Assembly.GetTypes();
        Dictionary<uint, string> dictionary = new Dictionary<uint, string>();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsClass && (types[i].IsSubclassOf(c) || (c.IsAssignableFrom(types[i]) && (c != types[i]))))
            {
                string name = types[i].Name;
                dictionary.Add(AkUtilities.ShortIDGenerator.Compute(name), name);
            }
        }
        dictionary.Add(AkUtilities.ShortIDGenerator.Compute("Awake"), "Awake");
        dictionary.Add(AkUtilities.ShortIDGenerator.Compute("Start"), "Start");
        dictionary.Add(AkUtilities.ShortIDGenerator.Compute("Destroy"), "Destroy");
        return dictionary;
    }

    public delegate void Trigger(GameObject in_gameObject);
}

