using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class VCollisionBox : VCollisionShape
{
    private VInt3 _tempDist;
    private long _tempRadius;
    private VInt3[] axis = new VInt3[] { new VInt3(0x3e8, 0, 0), new VInt3(0, 0x3e8, 0), new VInt3(0, 0, 0x3e8) };
    [SerializeField, HideInInspector]
    private VInt3 localPos = VInt3.zero;
    [SerializeField, HideInInspector]
    private VInt3 size = VInt3.one;
    private VInt3 worldExtends = VInt3.half;
    private VInt3 worldPos = VInt3.zero;
    private int worldRadius;

    public VCollisionBox()
    {
        base.isBox = true;
    }

    public VInt3 ClosestPoint(VInt3 targetPoint)
    {
        VInt3 lhs = targetPoint - this.worldPos;
        VInt3 worldPos = this.worldPos;
        int max = this.worldExtends.x * 0x3e8;
        int num4 = this.worldExtends.y * 0x3e8;
        int num5 = this.worldExtends.z * 0x3e8;
        worldPos += IntMath.Divide(this.axis[0], (long) Mathf.Clamp(VInt3.Dot(ref lhs, ref this.axis[0]), -max, max), 0xf4240L);
        worldPos += IntMath.Divide(this.axis[1], (long) Mathf.Clamp(VInt3.Dot(ref lhs, ref this.axis[1]), -num4, num4), 0xf4240L);
        return (worldPos + IntMath.Divide(this.axis[2], (long) Mathf.Clamp(VInt3.Dot(ref lhs, ref this.axis[2]), -num5, num5), 0xf4240L));
    }

    public override bool EdgeIntersects(VCollisionBox s)
    {
        return false;
    }

    public override bool EdgeIntersects(VCollisionSphere s)
    {
        return false;
    }

    public override void GetAabb2D(out VInt2 origin, out VInt2 size)
    {
        int worldRadius = this.worldRadius;
        origin.x = this.WorldPos.x - worldRadius;
        origin.y = this.WorldPos.z - worldRadius;
        size.x = worldRadius + worldRadius;
        size.y = size.x;
    }

    public Vector3[] GetPoints()
    {
        Vector3[] vectorArray = new Vector3[] { new Vector3(-1f, 1f, -1f), new Vector3(1f, 1f, -1f), new Vector3(1f, 1f, 1f), new Vector3(-1f, 1f, 1f), new Vector3(-1f, -1f, -1f), new Vector3(1f, -1f, -1f), new Vector3(1f, -1f, 1f), new Vector3(-1f, -1f, 1f) };
        Vector3 zero = Vector3.zero;
        Vector3 worldExtends = (Vector3) this.worldExtends;
        Vector3 worldPos = (Vector3) this.worldPos;
        Vector3 vector5 = (Vector3) this.axis[0];
        Vector3 vector6 = (Vector3) this.axis[1];
        Vector3 vector7 = (Vector3) this.axis[2];
        for (int i = 0; i < 8; i++)
        {
            zero.x = vectorArray[i].x * worldExtends.x;
            zero.y = vectorArray[i].y * worldExtends.y;
            zero.z = vectorArray[i].z * worldExtends.z;
            Vector3 vector2 = (Vector3) (vector5 * zero.x);
            vector2 += (Vector3) (vector6 * zero.y);
            vector2 += (Vector3) (vector7 * zero.z);
            vectorArray[i] = vector2 + worldPos;
        }
        return vectorArray;
    }

    public override CollisionShapeType GetShapeType()
    {
        return CollisionShapeType.Box;
    }

    public override bool Intersects(VCollisionBox obb)
    {
        return this.IntersectsOBB(obb);
    }

    public override bool Intersects(VCollisionSphere s)
    {
        base.ConditionalUpdateShape();
        s.ConditionalUpdateShape();
        this._tempRadius = this.worldRadius + s.WorldRadius;
        this._tempDist = this.worldPos - s.WorldPos;
        if (this._tempDist.sqrMagnitudeLong > (this._tempRadius * this._tempRadius))
        {
            return false;
        }
        VInt3 worldPos = s.WorldPos;
        long worldRadius = s.WorldRadius;
        VInt3 num3 = this.ClosestPoint(worldPos);
        VInt3 num4 = worldPos - num3;
        return (num4.sqrMagnitudeLong <= (worldRadius * worldRadius));
    }

    public bool IntersectsOBB(VCollisionBox b)
    {
        base.ConditionalUpdateShape();
        b.ConditionalUpdateShape();
        long num = this.worldRadius + b.worldRadius;
        VInt3 num11 = this.worldPos - b.worldPos;
        if (num11.sqrMagnitudeLong > (num * num))
        {
            return false;
        }
        VInt3 num2 = new VInt3(IntMath.Divide(VInt3.Dot(ref this.axis[0], ref b.axis[0]), 0x3e8), IntMath.Divide(VInt3.Dot(ref this.axis[0], ref b.axis[1]), 0x3e8), IntMath.Divide(VInt3.Dot(ref this.axis[0], ref b.axis[2]), 0x3e8));
        VInt3 num3 = new VInt3(IntMath.Divide(VInt3.Dot(ref this.axis[1], ref b.axis[0]), 0x3e8), IntMath.Divide(VInt3.Dot(ref this.axis[1], ref b.axis[1]), 0x3e8), IntMath.Divide(VInt3.Dot(ref this.axis[1], ref b.axis[2]), 0x3e8));
        VInt3 num4 = new VInt3(IntMath.Divide(VInt3.Dot(ref this.axis[2], ref b.axis[0]), 0x3e8), IntMath.Divide(VInt3.Dot(ref this.axis[2], ref b.axis[1]), 0x3e8), IntMath.Divide(VInt3.Dot(ref this.axis[2], ref b.axis[2]), 0x3e8));
        VInt3 abs = num2.abs;
        VInt3 rhs = num3.abs;
        VInt3 num7 = num4.abs;
        VInt3 lhs = b.worldPos - this.worldPos;
        int introduced11 = IntMath.Divide(VInt3.Dot(ref lhs, ref this.axis[0]), 0x3e8);
        int introduced12 = IntMath.Divide(VInt3.Dot(ref lhs, ref this.axis[1]), 0x3e8);
        lhs = new VInt3(introduced11, introduced12, IntMath.Divide(VInt3.Dot(ref lhs, ref this.axis[2]), 0x3e8));
        int num9 = this.worldExtends.x * 0x3e8;
        int num10 = VInt3.Dot(ref b.worldExtends, ref abs);
        if ((Mathf.Abs(lhs.x) * 0x3e8) > (num9 + num10))
        {
            return false;
        }
        num9 = this.worldExtends.y * 0x3e8;
        num10 = VInt3.Dot(ref b.worldExtends, ref rhs);
        if ((Mathf.Abs(lhs.y) * 0x3e8) > (num9 + num10))
        {
            return false;
        }
        num9 = this.worldExtends.z * 0x3e8;
        num10 = VInt3.Dot(ref b.worldExtends, ref num7);
        if ((Mathf.Abs(lhs.z) * 0x3e8) > (num9 + num10))
        {
            return false;
        }
        num9 = ((this.worldExtends.x * abs.x) + (this.worldExtends.y * rhs.x)) + (this.worldExtends.z * num7.x);
        num10 = b.worldExtends.x * 0x3e8;
        if (Math.Abs((int) (((lhs.x * num2.x) + (lhs.y * num3.x)) + (lhs.z * num4.x))) > (num9 + num10))
        {
            return false;
        }
        num9 = ((this.worldExtends.x * abs.y) + (this.worldExtends.y * rhs.y)) + (this.worldExtends.z * num7.y);
        num10 = b.worldExtends.y * 0x3e8;
        if (Math.Abs((int) (((lhs.x * num2.y) + (lhs.y * num3.y)) + (lhs.z * num4.y))) > (num9 + num10))
        {
            return false;
        }
        num9 = ((this.worldExtends.x * abs.z) + (this.worldExtends.y * rhs.z)) + (this.worldExtends.z * num7.z);
        num10 = b.worldExtends.z * 0x3e8;
        if (Math.Abs((int) (((lhs.x * num2.z) + (lhs.y * num3.z)) + (lhs.z * num4.z))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.y * num7.x) + (this.worldExtends.z * rhs.x);
        num10 = (b.worldExtends.y * abs.z) + (b.worldExtends.z * abs.y);
        if (Mathf.Abs((int) ((lhs.z * num3.x) - (lhs.y * num4.x))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.y * num7.y) + (this.worldExtends.z * rhs.y);
        num10 = (b.worldExtends.x * abs.z) + (b.worldExtends.z * abs.x);
        if (Mathf.Abs((int) ((lhs.z * num3.y) - (lhs.y * num4.y))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.y * num7.z) + (this.worldExtends.z * rhs.z);
        num10 = (b.worldExtends.x * abs.y) + (b.worldExtends.y * abs.x);
        if (Mathf.Abs((int) ((lhs.z * num3.z) - (lhs.y * num4.z))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.x * num7.x) + (this.worldExtends.z * abs.x);
        num10 = (b.worldExtends.y * rhs.z) + (b.worldExtends.z * rhs.y);
        if (Mathf.Abs((int) ((lhs.x * num4.x) - (lhs.z * num2.x))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.x * num7.y) + (this.worldExtends.z * abs.y);
        num10 = (b.worldExtends.x * rhs.z) + (b.worldExtends.z * rhs.x);
        if (Mathf.Abs((int) ((lhs.x * num4.y) - (lhs.z * num2.y))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.x * num7.z) + (this.worldExtends.z * abs.z);
        num10 = (b.worldExtends.x * rhs.y) + (b.worldExtends.y * rhs.x);
        if (Mathf.Abs((int) ((lhs.x * num4.z) - (lhs.z * num2.z))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.x * rhs.x) + (this.worldExtends.y * abs.x);
        num10 = (b.worldExtends.y * num7.z) + (b.worldExtends.z * num7.y);
        if (Mathf.Abs((int) ((lhs.y * num2.x) - (lhs.x * num3.x))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.x * rhs.y) + (this.worldExtends.y * abs.y);
        num10 = (b.worldExtends.x * num7.z) + (b.worldExtends.z * num7.x);
        if (Mathf.Abs((int) ((lhs.y * num2.y) - (lhs.x * num3.y))) > (num9 + num10))
        {
            return false;
        }
        num9 = (this.worldExtends.x * rhs.z) + (this.worldExtends.y * abs.z);
        num10 = (b.worldExtends.x * num7.y) + (b.worldExtends.y * num7.x);
        if (Mathf.Abs((int) ((lhs.y * num2.z) - (lhs.x * num3.z))) > (num9 + num10))
        {
            return false;
        }
        return true;
    }

    public override void UpdateShape(VInt3 location, VInt3 forward)
    {
        this.axis[2] = forward;
        this.axis[0] = VInt3.Cross(ref this.axis[1], ref this.axis[2]);
        this.worldPos = IntMath.Transform(ref this.localPos, ref this.axis[0], ref this.axis[1], ref this.axis[2], ref location);
        this.worldExtends.x = this.size.x >> 1;
        this.worldExtends.y = this.size.y >> 1;
        this.worldExtends.z = this.size.z >> 1;
        this.worldRadius = Mathf.Max(this.worldExtends.x, Mathf.Max(this.worldExtends.y, this.worldExtends.z));
        base.dirty = false;
    }

    public override int AvgCollisionRadius
    {
        get
        {
            base.ConditionalUpdateShape();
            int num = this.worldExtends.x + this.worldExtends.z;
            return (num >> 1);
        }
    }

    public VInt3 AxisX
    {
        get
        {
            return this.axis[0];
        }
    }

    public VInt3 AxisY
    {
        get
        {
            return this.axis[1];
        }
    }

    public VInt3 AxisZ
    {
        get
        {
            return this.axis[2];
        }
    }

    [CollisionProperty]
    public VInt3 Pos
    {
        get
        {
            return this.localPos;
        }
        set
        {
            this.localPos = value;
            base.dirty = true;
        }
    }

    [CollisionProperty]
    public VInt3 Size
    {
        get
        {
            return this.size;
        }
        set
        {
            this.size = value;
            base.dirty = true;
        }
    }

    public VInt3 WorldExtends
    {
        get
        {
            base.ConditionalUpdateShape();
            return this.worldExtends;
        }
    }

    public VInt3 WorldPos
    {
        get
        {
            base.ConditionalUpdateShape();
            return this.worldPos;
        }
    }
}

