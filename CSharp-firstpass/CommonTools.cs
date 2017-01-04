using CSProtocol;
using System;

public static class CommonTools
{
    public static CSDT_VECTOR3 CSDTFromVector3(VInt3 InVector3)
    {
        return new CSDT_VECTOR3 { X = InVector3.x, Y = InVector3.y, Z = InVector3.z };
    }

    public static FRAMEDT_VECTOR3 FromVector3(VInt3 InVector3)
    {
        return new FRAMEDT_VECTOR3 { X = InVector3.x, Y = InVector3.y, Z = InVector3.z };
    }

    public static VInt3 ToVector3(CSDT_VECTOR3 InFrameVector3)
    {
        return new VInt3(InFrameVector3.X, InFrameVector3.Y, InFrameVector3.Z);
    }

    public static VInt3 ToVector3(FRAMEDT_VECTOR3 InFrameVector3)
    {
        return new VInt3(InFrameVector3.X, InFrameVector3.Y, InFrameVector3.Z);
    }
}

