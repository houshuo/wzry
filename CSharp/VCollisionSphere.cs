using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class VCollisionSphere : VCollisionShape
{
    [HideInInspector, SerializeField]
    private VInt3 localPos = VInt3.zero;
    [HideInInspector, SerializeField]
    private int localRadius = 500;
    private VInt3 worldPos = VInt3.zero;
    private int worldRadius = 500;

    public VCollisionSphere()
    {
        base.dirty = true;
    }

    public override bool EdgeIntersects(VCollisionBox s)
    {
        return false;
    }

    public override bool EdgeIntersects(VCollisionSphere s)
    {
        base.ConditionalUpdateShape();
        s.ConditionalUpdateShape();
        long num = this.worldRadius + s.worldRadius;
        long num2 = this.worldRadius - s.worldRadius;
        VInt3 num4 = this.worldPos - s.worldPos;
        long num3 = num4.sqrMagnitudeLong2D;
        return ((num3 <= (num * num)) && (num3 >= (num2 * num2)));
    }

    public override void GetAabb2D(out VInt2 origin, out VInt2 size)
    {
        origin = this.WorldPos.xz;
        origin.x -= this.localRadius;
        origin.y -= this.localRadius;
        size.x = this.localRadius + this.localRadius;
        size.y = size.x;
    }

    public override CollisionShapeType GetShapeType()
    {
        return CollisionShapeType.Sphere;
    }

    public override bool Intersects(VCollisionBox obb)
    {
        return obb.Intersects(this);
    }

    public override bool Intersects(VCollisionSphere s)
    {
        base.ConditionalUpdateShape();
        s.ConditionalUpdateShape();
        long num = this.worldRadius + s.worldRadius;
        VInt3 num2 = this.worldPos - s.worldPos;
        return (num2.sqrMagnitudeLong <= (num * num));
    }

    public override void UpdateShape(VInt3 location, VInt3 forward)
    {
        if ((this.localPos.x == 0) && (this.localPos.z == 0))
        {
            this.worldPos.x = this.localPos.x + location.x;
            this.worldPos.y = this.localPos.y + location.y;
            this.worldPos.z = this.localPos.z + location.z;
        }
        else
        {
            VInt3 up = VInt3.up;
            VInt3 rhs = forward;
            VInt3 num3 = VInt3.Cross(ref up, ref rhs);
            VInt3 trans = location;
            this.worldPos = IntMath.Transform(ref this.localPos, ref num3, ref up, ref rhs, ref trans);
        }
        this.worldRadius = this.localRadius;
        base.dirty = false;
    }

    public override int AvgCollisionRadius
    {
        get
        {
            return this.WorldRadius;
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
            base.dirty = false;
        }
    }

    [CollisionProperty]
    public int Radius
    {
        get
        {
            return this.localRadius;
        }
        set
        {
            this.localRadius = value;
            base.dirty = true;
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

    public int WorldRadius
    {
        get
        {
            base.ConditionalUpdateShape();
            return this.worldRadius;
        }
    }
}

