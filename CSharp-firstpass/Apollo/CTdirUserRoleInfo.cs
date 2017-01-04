namespace Apollo
{
    using System;

    public class CTdirUserRoleInfo : ApolloBufferBase
    {
        public byte[] appBuff;
        public uint appLen;
        public uint lastLoginTime;
        public ulong roleID;
        public string roleLevel;
        public string roleName;
        public int zoneID;

        public override void ReadFrom(ApolloBufferReader reader)
        {
            reader.Read(ref this.zoneID);
            reader.Read(ref this.roleID);
            reader.Read(ref this.lastLoginTime);
            reader.Read(ref this.roleName);
            reader.Read(ref this.roleLevel);
            reader.Read(ref this.appLen);
            reader.Read(ref this.appBuff);
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            writer.Write(this.zoneID);
            writer.Write(this.roleID);
            writer.Write(this.lastLoginTime);
            writer.Write(this.roleName);
            writer.Write(this.roleLevel);
            writer.Write(this.appLen);
            writer.Write(this.appBuff);
        }
    }
}

