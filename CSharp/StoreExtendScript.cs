using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class StoreExtendScript
{
    [GameTypeAttr(0x75, typeof(ArrayList))]
    public static object _Restore_117(GameStore gs)
    {
        int capacity = gs.ReadInt32();
        ArrayList list = new ArrayList(capacity);
        for (int i = 0; i < capacity; i++)
        {
            list.Add(StoreExtendPlugin._Restore_75(gs));
        }
        return list;
    }

    [GameTypeAttr(0x11, typeof(GameObject))]
    public static object _Restore_17(GameStore gs)
    {
        return null;
    }

    [GameTypeAttr(0xba, typeof(ValueType))]
    public static object _Restore_186(GameStore gs)
    {
        return null;
    }

    [GameTypeAttr(0xe2, typeof(KeyValuePair<uint, ulong>))]
    public static KeyValuePair<uint, ulong> _Restore_226(GameStore gs)
    {
        return new KeyValuePair<uint, ulong>(gs.ReadUInt32(), gs.ReadUInt64());
    }

    [GameTypeAttr(0xf7, typeof(FieldInfo))]
    public static object _Restore_247(GameStore gs)
    {
        System.Type type = (System.Type) StoreExtendPlugin._Restore_21(gs);
        return type.GetField(gs.ReadString());
    }

    [GameTypeAttr(0x102, typeof(MethodBase))]
    public static object _Restore_258(GameStore gs)
    {
        System.Type type = (System.Type) StoreExtendPlugin._Restore_21(gs);
        return type.GetMethod(gs.ReadString());
    }

    [GameTypeAttr(0x10c1, typeof(KeyValuePair<uint, int>))]
    public static KeyValuePair<uint, int> _Restore_4289(GameStore gs)
    {
        return new KeyValuePair<uint, int>(gs.ReadUInt32(), gs.ReadInt32());
    }

    [GameTypeAttr(0x1645, typeof(DatabinTable<ResGameTask, uint>))]
    public static object _Restore_5701(GameStore gs)
    {
        return null;
    }

    [GameTypeAttr(0x1647, typeof(DatabinTable<ResGameTaskGroup, uint>))]
    public static object _Restore_5703(GameStore gs)
    {
        return null;
    }

    [GameTypeAttr(0x1654, typeof(MultiValueListDictionary<uint, TriggerActionBase>))]
    public static object _Restore_5716(GameStore gs)
    {
        int num = gs.ReadInt32();
        MultiValueListDictionary<uint, TriggerActionBase> dictionary = new MultiValueListDictionary<uint, TriggerActionBase>();
        for (int i = 0; i < num; i++)
        {
            uint key = gs.ReadUInt32();
            int num4 = gs.ReadInt32();
            for (int j = 0; j < num4; j++)
            {
                dictionary.Add(key, gs.ReadIStore() as TriggerActionBase);
            }
        }
        return dictionary;
    }

    public static void _Store_117(object val, GameStore gs)
    {
        ArrayList list = val as ArrayList;
        gs.WriteInt32(list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            StoreExtendPlugin._Store_75(list[i], gs);
        }
    }

    public static void _Store_17(object val, GameStore gs)
    {
        GameObject obj2 = val as GameObject;
    }

    public static void _Store_186(object val, GameStore gs)
    {
        ValueType type = val as ValueType;
    }

    public static void _Store_226(KeyValuePair<uint, ulong> val, GameStore gs)
    {
        gs.WriteUInt32(val.Key);
        gs.WriteUInt64(val.Value);
    }

    public static void _Store_247(object val, GameStore gs)
    {
        FieldInfo info = val as FieldInfo;
        StoreExtendPlugin._Store_21(info.DeclaringType, gs);
        gs.WriteString(info.Name);
    }

    public static void _Store_258(object val, GameStore gs)
    {
        MethodBase base2 = val as MethodBase;
        StoreExtendPlugin._Store_21(base2.DeclaringType, gs);
        gs.WriteString(base2.Name);
    }

    public static void _Store_4289(KeyValuePair<uint, int> val, GameStore gs)
    {
        gs.WriteUInt32(val.Key);
        gs.WriteInt32(val.Value);
    }

    public static void _Store_5701(object val, GameStore gs)
    {
        DatabinTable<ResGameTask, uint> table = val as DatabinTable<ResGameTask, uint>;
    }

    public static void _Store_5703(object val, GameStore gs)
    {
        DatabinTable<ResGameTaskGroup, uint> table = val as DatabinTable<ResGameTaskGroup, uint>;
    }

    public static void _Store_5716(object val, GameStore gs)
    {
        MultiValueListDictionary<uint, TriggerActionBase> dictionary = val as MultiValueListDictionary<uint, TriggerActionBase>;
        gs.WriteInt32(dictionary.Count);
        DictionaryView<uint, ListView<TriggerActionBase>>.Enumerator enumerator = dictionary.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<uint, ListView<TriggerActionBase>> current = enumerator.Current;
            gs.WriteUInt32(current.Key);
            KeyValuePair<uint, ListView<TriggerActionBase>> pair2 = enumerator.Current;
            int count = pair2.Value.Count;
            gs.WriteInt32(count);
            for (int i = 0; i < count; i++)
            {
            }
        }
    }
}

