using System;
using System.Text;
using UnityEngine;

public class UnityBasetypeSerializer
{
    private const float FloatFactor = 0.001f;
    private const float FloatPrecision = 1000f;
    private static StringBuilder m_sb;
    protected const string XML_ATTR_VALUE = "Value";

    public static byte[] BoundsToBytes(Bounds bounds)
    {
        byte[] data = new byte[0x18];
        WriteFloat(data, 0, bounds.center.x);
        WriteFloat(data, 4, bounds.center.y);
        WriteFloat(data, 8, bounds.center.z);
        WriteFloat(data, 12, bounds.extents.x);
        WriteFloat(data, 0x10, bounds.extents.y);
        WriteFloat(data, 20, bounds.extents.z);
        return data;
    }

    public static string BoundsToString(Bounds bounds)
    {
        object[] args = new object[] { bounds.center.x, bounds.center.y, bounds.center.z, bounds.extents.x, bounds.extents.y, bounds.extents.z };
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9},{4:G9},{5:G9}", args).ToString();
    }

    public static Bounds BytesToBounds(byte[] data)
    {
        Bounds bounds = new Bounds();
        BytesToBounds(ref bounds, data);
        return bounds;
    }

    public static void BytesToBounds(ref Bounds bounds, byte[] data)
    {
        bounds.center = new Vector3(ReadFloat(data, 0), ReadFloat(data, 4), ReadFloat(data, 8));
        bounds.extents = new Vector3(ReadFloat(data, 12), ReadFloat(data, 0x10), ReadFloat(data, 20));
    }

    public static Color BytesToColor(byte[] data)
    {
        Color color = new Color();
        BytesToColor(ref color, data);
        return color;
    }

    public static void BytesToColor(ref Color color, byte[] data)
    {
        color.r = ReadFloat(data, 0);
        color.g = ReadFloat(data, 4);
        color.b = ReadFloat(data, 8);
        color.a = ReadFloat(data, 12);
    }

    public static float BytesToFloat(byte[] data)
    {
        return ReadFloat(data, 0);
    }

    public static int BytesToInt(byte[] data)
    {
        return ReadInt(data, 0);
    }

    public static Matrix4x4 BytesToMatrix4x4(byte[] data)
    {
        Matrix4x4 matrix = new Matrix4x4();
        BytesToMatrix4x4(ref matrix, data);
        return matrix;
    }

    public static void BytesToMatrix4x4(ref Matrix4x4 matrix, byte[] data)
    {
        matrix.m00 = ReadFloat(data, 0);
        matrix.m01 = ReadFloat(data, 4);
        matrix.m02 = ReadFloat(data, 8);
        matrix.m03 = ReadFloat(data, 12);
        matrix.m10 = ReadFloat(data, 0x10);
        matrix.m11 = ReadFloat(data, 20);
        matrix.m12 = ReadFloat(data, 0x18);
        matrix.m13 = ReadFloat(data, 0x1c);
        matrix.m20 = ReadFloat(data, 0x20);
        matrix.m21 = ReadFloat(data, 0x24);
        matrix.m22 = ReadFloat(data, 40);
        matrix.m23 = ReadFloat(data, 0x2c);
        matrix.m30 = ReadFloat(data, 0x30);
        matrix.m31 = ReadFloat(data, 0x34);
        matrix.m32 = ReadFloat(data, 0x38);
        matrix.m33 = ReadFloat(data, 60);
    }

    public static Quaternion BytesToQuaternion(byte[] data)
    {
        Quaternion quaternion = new Quaternion();
        BytesToQuaternion(ref quaternion, data);
        return quaternion;
    }

    public static void BytesToQuaternion(ref Quaternion quaternion, byte[] data)
    {
        quaternion.x = ReadFloat(data, 0);
        quaternion.y = ReadFloat(data, 4);
        quaternion.z = ReadFloat(data, 8);
        quaternion.w = ReadFloat(data, 12);
    }

    public static Rect BytesToRect(byte[] data)
    {
        Rect rect = new Rect();
        BytesToRect(ref rect, data);
        return rect;
    }

    public static void BytesToRect(ref Rect rect, byte[] data)
    {
        rect.xMin = ReadFloat(data, 0);
        rect.xMax = ReadFloat(data, 4);
        rect.yMin = ReadFloat(data, 8);
        rect.yMax = ReadFloat(data, 12);
    }

    public static Vector2 BytesToVector2(byte[] data)
    {
        Vector2 vector = new Vector2();
        BytesToVector2(ref vector, data);
        return vector;
    }

    public static void BytesToVector2(ref Vector2 vector, byte[] data)
    {
        vector.x = ReadFloat(data, 0);
        vector.y = ReadFloat(data, 4);
    }

    public static Vector3 BytesToVector3(byte[] data)
    {
        Vector3 vector = new Vector3();
        BytesToVector3(ref vector, data);
        return vector;
    }

    public static void BytesToVector3(ref Vector3 vector, byte[] data)
    {
        vector.x = ReadFloat(data, 0);
        vector.y = ReadFloat(data, 4);
        vector.z = ReadFloat(data, 8);
    }

    public static Vector4 BytesToVector4(byte[] data)
    {
        Vector4 vector = new Vector4();
        BytesToVector4(ref vector, data);
        return vector;
    }

    public static void BytesToVector4(ref Vector4 vector, byte[] data)
    {
        vector.x = ReadFloat(data, 0);
        vector.y = ReadFloat(data, 4);
        vector.z = ReadFloat(data, 8);
        vector.w = ReadFloat(data, 12);
    }

    public static byte[] ColorToBytes(Color color)
    {
        byte[] data = new byte[0x10];
        WriteFloat(data, 0, color.r);
        WriteFloat(data, 4, color.g);
        WriteFloat(data, 8, color.b);
        WriteFloat(data, 12, color.a);
        return data;
    }

    public static string ColorToString(Color color)
    {
        object[] args = new object[] { color.r, color.g, color.b, color.a };
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", args).ToString();
    }

    public static byte[] FloatToBytes(float value)
    {
        byte[] data = new byte[4];
        WriteFloat(data, 0, value);
        return data;
    }

    private static StringBuilder GetStringBuilder()
    {
        if (m_sb == null)
        {
            m_sb = new StringBuilder();
        }
        m_sb.Length = 0;
        return m_sb;
    }

    public static byte[] IntToBytes(int value)
    {
        byte[] data = new byte[4];
        WriteInt(data, 0, value);
        return data;
    }

    public static byte[] Matrix4x4ToBytes(Matrix4x4 matrix)
    {
        byte[] data = new byte[0x40];
        WriteFloat(data, 0, matrix.m00);
        WriteFloat(data, 4, matrix.m01);
        WriteFloat(data, 8, matrix.m02);
        WriteFloat(data, 12, matrix.m03);
        WriteFloat(data, 0x10, matrix.m10);
        WriteFloat(data, 20, matrix.m11);
        WriteFloat(data, 0x18, matrix.m12);
        WriteFloat(data, 0x1c, matrix.m13);
        WriteFloat(data, 0x20, matrix.m20);
        WriteFloat(data, 0x24, matrix.m21);
        WriteFloat(data, 40, matrix.m22);
        WriteFloat(data, 0x2c, matrix.m23);
        WriteFloat(data, 0x30, matrix.m30);
        WriteFloat(data, 0x34, matrix.m31);
        WriteFloat(data, 0x38, matrix.m32);
        WriteFloat(data, 60, matrix.m33);
        return data;
    }

    public static string Matrix4x4ToString(Matrix4x4 matrix)
    {
        object[] args = new object[] { matrix.m00, matrix.m01, matrix.m02, matrix.m03, matrix.m10, matrix.m11, matrix.m12, matrix.m13, matrix.m20, matrix.m21, matrix.m22, matrix.m23, matrix.m30, matrix.m31, matrix.m32, matrix.m33 };
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9},{4:G9},{5:G9},{6:G9},{7:G9},{8:G9},{9:G9},{10:G9},{11:G9},{12:G9},{13:G9},{14:G9},{15:G9}", args).ToString();
    }

    public static byte[] QuaternionToBytes(Quaternion quaternion)
    {
        byte[] data = new byte[0x10];
        WriteFloat(data, 0, quaternion.x);
        WriteFloat(data, 4, quaternion.y);
        WriteFloat(data, 8, quaternion.z);
        WriteFloat(data, 12, quaternion.w);
        return data;
    }

    public static string QuaternionToString(Quaternion quaternion)
    {
        object[] args = new object[] { quaternion.x, quaternion.y, quaternion.z, quaternion.w };
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", args).ToString();
    }

    private static float ReadFloat(byte[] data, int offset)
    {
        return (ReadInt(data, offset) * 0.001f);
    }

    private static int ReadInt(byte[] data, int offset)
    {
        if (!BitConverter.IsLittleEndian)
        {
            byte[] destinationArray = new byte[4];
            Array.Copy(data, offset, destinationArray, 0, 4);
            Array.Reverse(destinationArray);
            return BitConverter.ToInt32(destinationArray, 0);
        }
        return BitConverter.ToInt32(data, offset);
    }

    public static byte[] RectToBytes(Rect rect)
    {
        byte[] data = new byte[0x10];
        WriteFloat(data, 0, rect.xMin);
        WriteFloat(data, 4, rect.xMax);
        WriteFloat(data, 8, rect.yMin);
        WriteFloat(data, 12, rect.yMax);
        return data;
    }

    public static string RectToString(Rect rect)
    {
        object[] args = new object[] { rect.xMin, rect.xMax, rect.yMin, rect.yMax };
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", args).ToString();
    }

    public static Bounds StringToBounds(string s)
    {
        Bounds bounds = new Bounds();
        StringToBounds(ref bounds, s);
        return bounds;
    }

    public static void StringToBounds(ref Bounds bounds, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        bounds.center = new Vector3(Convert.ToSingle(strArray[0]), Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]));
        bounds.extents = new Vector3(Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4]), Convert.ToSingle(strArray[5]));
    }

    public static Color StringToColor(string s)
    {
        Color color = new Color();
        StringToColor(ref color, s);
        return color;
    }

    public static void StringToColor(ref Color color, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        color.r = Convert.ToSingle(strArray[0]);
        color.g = Convert.ToSingle(strArray[1]);
        color.b = Convert.ToSingle(strArray[2]);
        color.a = Convert.ToSingle(strArray[3]);
    }

    public static Matrix4x4 StringToMatrix4x4(string s)
    {
        Matrix4x4 matrix = new Matrix4x4();
        StringToMatrix4x4(ref matrix, s);
        return matrix;
    }

    public static void StringToMatrix4x4(ref Matrix4x4 matrix, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        matrix.m00 = Convert.ToSingle(strArray[0]);
        matrix.m01 = Convert.ToSingle(strArray[1]);
        matrix.m02 = Convert.ToSingle(strArray[2]);
        matrix.m03 = Convert.ToSingle(strArray[3]);
        matrix.m10 = Convert.ToSingle(strArray[4]);
        matrix.m11 = Convert.ToSingle(strArray[5]);
        matrix.m12 = Convert.ToSingle(strArray[6]);
        matrix.m13 = Convert.ToSingle(strArray[7]);
        matrix.m20 = Convert.ToSingle(strArray[8]);
        matrix.m21 = Convert.ToSingle(strArray[9]);
        matrix.m22 = Convert.ToSingle(strArray[10]);
        matrix.m23 = Convert.ToSingle(strArray[11]);
        matrix.m30 = Convert.ToSingle(strArray[12]);
        matrix.m31 = Convert.ToSingle(strArray[13]);
        matrix.m32 = Convert.ToSingle(strArray[14]);
        matrix.m33 = Convert.ToSingle(strArray[15]);
    }

    public static Quaternion StringToQuaternion(string s)
    {
        Quaternion quaternion = new Quaternion();
        StringToQuaternion(ref quaternion, s);
        return quaternion;
    }

    public static void StringToQuaternion(ref Quaternion quaternion, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        quaternion.x = Convert.ToSingle(strArray[0]);
        quaternion.y = Convert.ToSingle(strArray[1]);
        quaternion.z = Convert.ToSingle(strArray[2]);
        quaternion.w = Convert.ToSingle(strArray[3]);
    }

    public static Rect StringToRect(string s)
    {
        Rect rect = new Rect();
        StringToRect(ref rect, s);
        return rect;
    }

    public static void StringToRect(ref Rect rect, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        rect.xMin = Convert.ToSingle(strArray[0]);
        rect.xMax = Convert.ToSingle(strArray[1]);
        rect.yMin = Convert.ToSingle(strArray[2]);
        rect.yMax = Convert.ToSingle(strArray[3]);
    }

    public static Vector2 StringToVector2(string s)
    {
        Vector2 vector = new Vector2();
        StringToVector2(ref vector, s);
        return vector;
    }

    public static void StringToVector2(ref Vector2 vector, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        vector.x = Convert.ToSingle(strArray[0]);
        vector.y = Convert.ToSingle(strArray[1]);
    }

    public static Vector3 StringToVector3(string s)
    {
        Vector3 vector = new Vector3();
        StringToVector3(ref vector, s);
        return vector;
    }

    public static void StringToVector3(ref Vector3 vector, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        vector.x = Convert.ToSingle(strArray[0]);
        vector.y = Convert.ToSingle(strArray[1]);
        vector.z = Convert.ToSingle(strArray[2]);
    }

    public static Vector4 StringToVector4(string s)
    {
        Vector4 vector = new Vector4();
        StringToVector4(ref vector, s);
        return vector;
    }

    public static void StringToVector4(ref Vector4 vector, string s)
    {
        char[] separator = new char[] { ',' };
        string[] strArray = s.Split(separator);
        vector.x = Convert.ToSingle(strArray[0]);
        vector.y = Convert.ToSingle(strArray[1]);
        vector.z = Convert.ToSingle(strArray[2]);
        vector.w = Convert.ToSingle(strArray[3]);
    }

    public static byte[] Vector2ToBytes(Vector2 vector)
    {
        byte[] data = new byte[8];
        WriteFloat(data, 0, vector.x);
        WriteFloat(data, 4, vector.y);
        return data;
    }

    public static string Vector2ToString(Vector2 vector)
    {
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9}", vector.x, vector.y).ToString();
    }

    public static byte[] Vector3ToBytes(Vector3 vector)
    {
        byte[] data = new byte[12];
        WriteFloat(data, 0, vector.x);
        WriteFloat(data, 4, vector.y);
        WriteFloat(data, 8, vector.z);
        return data;
    }

    public static string Vector3ToString(Vector3 vector)
    {
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9}", vector.x, vector.y, vector.z).ToString();
    }

    public static byte[] Vector4ToBytes(Vector4 vector)
    {
        byte[] data = new byte[0x10];
        WriteFloat(data, 0, vector.x);
        WriteFloat(data, 4, vector.y);
        WriteFloat(data, 8, vector.z);
        WriteFloat(data, 12, vector.w);
        return data;
    }

    public static string Vector4ToString(Vector4 vector)
    {
        object[] args = new object[] { vector.x, vector.y, vector.z, vector.w };
        return GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", args).ToString();
    }

    private static void WriteFloat(byte[] data, int offset, float value)
    {
        int num = (int) Mathf.Round(value * 1000f);
        WriteInt(data, offset, num);
    }

    private static void WriteInt(byte[] data, int offset, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        Array.Copy(bytes, 0, data, offset, 4);
    }
}

