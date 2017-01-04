using System;
using System.IO;

public static class FrameTracer
{
    private static BinaryWriter _bw;
    private static uint _mask;
    private static MemoryStream _ms;

    public static void Close()
    {
    }

    public static void Destroy()
    {
        _mask = 0;
        if (_bw != null)
        {
            _bw.Close();
            _bw = null;
        }
        if (_ms != null)
        {
            _ms.Close();
            _ms = null;
        }
    }

    public static MemoryStream[] GetReports()
    {
        return new MemoryStream[] { _ms };
    }

    public static void Initial(uint mask, uint bufferLength)
    {
        Destroy();
        _mask = mask;
        if (_mask > 0)
        {
            _ms = new MemoryStream((int) bufferLength);
            _bw = new BinaryWriter(_ms);
        }
    }

    public static void Start()
    {
        _bw.Seek(0, SeekOrigin.Begin);
    }

    public static void Trace(short srcFileId, short srcLineNo)
    {
        if (((((int) 1) << (srcFileId % 0x20)) & _mask) > 0)
        {
            _bw.Write((uint) (((uint) srcFileId) | (((uint) srcLineNo) << 15)));
        }
    }

    public static void Trace(short srcFileId, short srcLineNo, string withData)
    {
        if (((((int) 1) << (srcFileId % 0x20)) & _mask) > 0)
        {
            _bw.Write((uint) ((((uint) srcFileId) | (((uint) srcLineNo) << 15)) | 0x80000000));
            _bw.Write(withData);
        }
    }

    public static void Trace(short srcFileId, short srcLineNo, uint withData)
    {
        if (((((int) 1) << (srcFileId % 0x20)) & _mask) > 0)
        {
            _bw.Write((ulong) (((ulong) ((srcFileId | (srcLineNo << 15)) | 0x40000000L)) | (withData << 0x20)));
        }
    }

    public static void Trace(short srcFileId, short srcLineNo, byte[] withData)
    {
        if (((((int) 1) << (srcFileId % 0x20)) & _mask) > 0)
        {
            _bw.Write((ulong) ((((ulong) (srcFileId | (srcLineNo << 15))) | 0xc0000000L) | (((ulong) withData.Length) << 0x20)));
            _bw.Write(withData);
        }
    }
}

