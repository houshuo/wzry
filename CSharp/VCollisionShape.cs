using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public abstract class VCollisionShape
{
    [NonSerialized, HideInInspector]
    public bool dirty = true;
    [NonSerialized, HideInInspector]
    public bool isBox;
    [NonSerialized, HideInInspector]
    public PoolObjHandle<ActorRoot> owner;

    protected VCollisionShape()
    {
    }

    public virtual void Born(ActorRoot actor)
    {
        actor.shape = this;
        this.owner = new PoolObjHandle<ActorRoot>(actor);
    }

    public void ConditionalUpdateShape()
    {
        if (this.dirty)
        {
            ActorRoot handle = this.owner.handle;
            this.UpdateShape(handle.location, handle.forward);
        }
    }

    public static VCollisionShape createFromCollider(GameObject gameObject)
    {
        DebugHelper.Assert((!Singleton<BattleLogic>.instance.isFighting || Singleton<GameLogic>.instance.bInLogicTick) || Singleton<FrameSynchr>.instance.isCmdExecuting);
        Collider collider = (gameObject.GetComponent<Collider>() == null) ? null : gameObject.GetComponent<Collider>();
        if (collider == null)
        {
            return null;
        }
        VCollisionShape shape = null;
        if (collider is BoxCollider)
        {
            BoxCollider collider2 = collider as BoxCollider;
            return new VCollisionBox { Pos = (VInt3) collider2.center, Size = (VInt3) collider2.size };
        }
        if (collider is CapsuleCollider)
        {
            CapsuleCollider collider3 = collider as CapsuleCollider;
            VCollisionSphere sphere = new VCollisionSphere();
            Vector3 center = collider3.center;
            center.y -= collider3.height * 0.5f;
            sphere.Pos = (VInt3) center;
            VInt radius = (VInt) collider3.radius;
            sphere.Radius = radius.i;
            return sphere;
        }
        if (collider is SphereCollider)
        {
            SphereCollider collider4 = collider as SphereCollider;
            VCollisionSphere sphere2 = new VCollisionSphere {
                Pos = (VInt3) collider4.center
            };
            VInt num2 = (VInt) collider4.radius;
            sphere2.Radius = num2.i;
            shape = sphere2;
        }
        return shape;
    }

    public abstract bool EdgeIntersects(VCollisionBox obb);
    public bool EdgeIntersects(VCollisionShape shape)
    {
        bool flag = false;
        if (shape != null)
        {
            if (shape is VCollisionSphere)
            {
                return this.EdgeIntersects(shape as VCollisionSphere);
            }
            if (shape is VCollisionBox)
            {
                flag = this.EdgeIntersects(shape as VCollisionBox);
            }
        }
        return flag;
    }

    public abstract bool EdgeIntersects(VCollisionSphere s);
    public abstract void GetAabb2D(out VInt2 lt, out VInt2 size);
    public abstract CollisionShapeType GetShapeType();
    public static VCollisionShape InitActorCollision(ActorRoot actor)
    {
        VCollisionShape shape = null;
        if (actor.shape == null)
        {
            SCollisionComponent component = actor.gameObject.GetComponent<SCollisionComponent>();
            if (component != null)
            {
                shape = component.CreateShape();
            }
            else if ((actor.CharInfo != null) && (actor.CharInfo.collisionType != CollisionShapeType.None))
            {
                shape = actor.CharInfo.CreateCollisionShape();
            }
            else
            {
                shape = createFromCollider(actor.gameObject);
            }
            if (shape != null)
            {
                shape.Born(actor);
            }
        }
        return actor.shape;
    }

    public static VCollisionShape InitActorCollision(ActorRoot actor, GameObject gameObj, string actionName)
    {
        VCollisionShape shape = null;
        if (actor.shape == null)
        {
            SCollisionComponent component = (gameObj == null) ? null : gameObj.GetComponent<SCollisionComponent>();
            if (component != null)
            {
                shape = component.CreateShape();
            }
            else if ((actor.CharInfo != null) && (actor.CharInfo.collisionType != CollisionShapeType.None))
            {
                shape = actor.CharInfo.CreateCollisionShape();
            }
            else if (gameObj != null)
            {
                shape = createFromCollider(gameObj);
            }
            if (shape != null)
            {
                shape.Born(actor);
            }
        }
        return actor.shape;
    }

    public abstract bool Intersects(VCollisionBox obb);
    public bool Intersects(VCollisionShape shape)
    {
        bool flag = false;
        if (shape == null)
        {
            return flag;
        }
        if (shape.isBox)
        {
            return this.Intersects((VCollisionBox) shape);
        }
        return this.Intersects((VCollisionSphere) shape);
    }

    public abstract bool Intersects(VCollisionSphere s);
    public void OnEnable()
    {
        this.dirty = true;
        this.owner.Release();
    }

    public abstract void UpdateShape(VInt3 location, VInt3 forward);

    public abstract int AvgCollisionRadius { get; }
}

