namespace com.tencent.pandora
{
    using System;

    public class MsgPacket
    {
        public LuaStringBuffer data;
        public ushort errno;
        public ushort id;
        public int seq;
    }
}

