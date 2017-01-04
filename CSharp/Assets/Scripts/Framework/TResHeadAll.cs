namespace Assets.Scripts.Framework
{
    using System;
    using tsf4g_tdr_csharp;

    public class TResHeadAll
    {
        public TResHead mHead = new TResHead();
        public resHeadExt mResHeadExt = new resHeadExt();
        public const int TRES_ENCORDING_LEN = 0x20;
        public const int TRES_TRANSLATE_METALIB_HASH_LEN = 0x24;

        public void load(ref TdrReadBuf srcBuf)
        {
            srcBuf.disableEndian();
            srcBuf.readInt32(ref this.mHead.iMagic);
            srcBuf.readInt32(ref this.mHead.iVersion);
            srcBuf.readInt32(ref this.mHead.iUint);
            srcBuf.readInt32(ref this.mHead.iCount);
            srcBuf.readCString(ref this.mHead.szMetalibHash, this.mHead.szMetalibHash.Length);
            srcBuf.readInt32(ref this.mHead.iResVersion);
            srcBuf.readUInt64(ref this.mHead.ullCreateTime);
            srcBuf.readCString(ref this.mHead.szResEncording, this.mHead.szResEncording.Length);
            srcBuf.readCString(ref this.mHead.szContentHash, this.mHead.szContentHash.Length);
            srcBuf.readInt32(ref this.mResHeadExt.iDataOffset);
            srcBuf.readInt32(ref this.mResHeadExt.iBuff);
        }

        public class resHeadExt
        {
            public int iBuff;
            public int iDataOffset;
        }

        public class TResHead
        {
            public int iCount;
            public int iMagic;
            public int iResVersion;
            public int iUint;
            public int iVersion;
            public byte[] szContentHash = new byte[0x24];
            public byte[] szMetalibHash = new byte[0x24];
            public byte[] szResEncording = new byte[0x20];
            public ulong ullCreateTime;
        }
    }
}

