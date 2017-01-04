using System;
using System.Collections;
using UnityEngine;

public static class StoreExtendPlugin
{
    [GameTypeAttr(0xa9, typeof(Vector2))]
    public static Vector2 _Restore_169(GameStore gs)
    {
        return new Vector2(gs.ReadSingle(), gs.ReadSingle());
    }

    [GameTypeAttr(0xad, typeof(Bounds))]
    public static Bounds _Restore_173(GameStore gs)
    {
        return new Bounds(_Restore_33(gs), _Restore_33(gs));
    }

    [GameTypeAttr(0x15, typeof(System.Type))]
    public static object _Restore_21(GameStore gs)
    {
        return System.Type.GetType(gs.ReadString());
    }

    [GameTypeAttr(0xdd, typeof(Color))]
    public static Color _Restore_221(GameStore gs)
    {
        return new Color(gs.ReadSingle(), gs.ReadSingle(), gs.ReadSingle(), gs.ReadSingle());
    }

    [GameTypeAttr(0x21, typeof(Vector3))]
    public static Vector3 _Restore_33(GameStore gs)
    {
        return new Vector3(gs.ReadSingle(), gs.ReadSingle(), gs.ReadSingle());
    }

    [GameTypeAttr(0x22, typeof(Quaternion))]
    public static Quaternion _Restore_34(GameStore gs)
    {
        return new Quaternion(gs.ReadSingle(), gs.ReadSingle(), gs.ReadSingle(), gs.ReadSingle());
    }

    [GameTypeAttr(0x23, typeof(Transform))]
    public static object _Restore_35(GameStore gs)
    {
        return null;
    }

    [GameTypeAttr(0xff8, typeof(DateTime))]
    public static DateTime _Restore_4088(GameStore gs)
    {
        return new DateTime(gs.ReadInt64());
    }

    [GameTypeAttr(0x1018, typeof(Matrix4x4))]
    public static Matrix4x4 _Restore_4120(GameStore gs)
    {
        return new Matrix4x4 { m00 = gs.ReadSingle(), m01 = gs.ReadSingle(), m02 = gs.ReadSingle(), m03 = gs.ReadSingle(), m10 = gs.ReadSingle(), m11 = gs.ReadSingle(), m12 = gs.ReadSingle(), m13 = gs.ReadSingle(), m20 = gs.ReadSingle(), m21 = gs.ReadSingle(), m22 = gs.ReadSingle(), m23 = gs.ReadSingle(), m30 = gs.ReadSingle(), m31 = gs.ReadSingle(), m32 = gs.ReadSingle(), m33 = gs.ReadSingle() };
    }

    [GameTypeAttr(0x101d, typeof(Rect))]
    public static Rect _Restore_4125(GameStore gs)
    {
        return new Rect(gs.ReadSingle(), gs.ReadSingle(), gs.ReadSingle(), gs.ReadSingle());
    }

    [GameTypeAttr(0x1026, typeof(BitArray))]
    public static object _Restore_4134(GameStore gs)
    {
        byte[] bytes = gs.ReadArray();
        if (bytes != null)
        {
            return new BitArray(bytes);
        }
        return null;
    }

    [GameTypeAttr(0x4b, typeof(object))]
    public static object _Restore_75(GameStore gs)
    {
        return gs.ReadObject();
    }

    [GameTypeAttr(0x5b, typeof(LayerMask))]
    public static LayerMask _Restore_91(GameStore gs)
    {
        return new LayerMask { value = gs.ReadInt32() };
    }

    public static void _Store_169(Vector2 val, GameStore gs)
    {
        gs.WriteSingle(val.x);
        gs.WriteSingle(val.y);
    }

    public static void _Store_173(Bounds val, GameStore gs)
    {
        _Store_33(val.center, gs);
        _Store_33(val.size, gs);
    }

    public static void _Store_21(object val, GameStore gs)
    {
        System.Type type = val as System.Type;
        gs.WriteString(type.GetPathName(true));
    }

    public static void _Store_221(Color val, GameStore gs)
    {
        gs.WriteSingle(val.r);
        gs.WriteSingle(val.g);
        gs.WriteSingle(val.b);
        gs.WriteSingle(val.a);
    }

    public static void _Store_33(Vector3 val, GameStore gs)
    {
        gs.WriteSingle(val.x);
        gs.WriteSingle(val.y);
        gs.WriteSingle(val.z);
    }

    public static void _Store_34(Quaternion val, GameStore gs)
    {
        gs.WriteSingle(val.x);
        gs.WriteSingle(val.y);
        gs.WriteSingle(val.z);
        gs.WriteSingle(val.w);
    }

    public static void _Store_35(object val, GameStore gs)
    {
        Transform transform = val as Transform;
    }

    public static void _Store_4088(DateTime val, GameStore gs)
    {
        gs.WriteInt64(val.Ticks);
    }

    public static void _Store_4120(Matrix4x4 val, GameStore gs)
    {
        gs.WriteSingle(val.m00);
        gs.WriteSingle(val.m01);
        gs.WriteSingle(val.m02);
        gs.WriteSingle(val.m03);
        gs.WriteSingle(val.m10);
        gs.WriteSingle(val.m11);
        gs.WriteSingle(val.m12);
        gs.WriteSingle(val.m13);
        gs.WriteSingle(val.m20);
        gs.WriteSingle(val.m21);
        gs.WriteSingle(val.m22);
        gs.WriteSingle(val.m23);
        gs.WriteSingle(val.m30);
        gs.WriteSingle(val.m31);
        gs.WriteSingle(val.m32);
        gs.WriteSingle(val.m33);
    }

    public static void _Store_4125(Rect val, GameStore gs)
    {
        gs.WriteSingle(val.x);
        gs.WriteSingle(val.y);
        gs.WriteSingle(val.width);
        gs.WriteSingle(val.height);
    }

    public static void _Store_4134(object val, GameStore gs)
    {
        BitArray array = val as BitArray;
        byte[] buffer = new byte[Mathf.CeilToInt(((float) array.Count) / 8f)];
        array.CopyTo(buffer, 0);
        gs.WriteArray(buffer);
    }

    public static void _Store_75(object val, GameStore gs)
    {
        gs.WriteObject(val);
    }

    public static void _Store_91(LayerMask val, GameStore gs)
    {
        gs.WriteInt32(val.value);
    }
}

