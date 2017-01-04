using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Pathfinding;
using Pathfinding.RVO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class MPathfinding
{
    protected PoolObjHandle<ActorRoot> actor = new PoolObjHandle<ActorRoot>();
    private bool canMove;
    public bool checkNavNode;
    public bool closestOnPathCheck = true;
    protected int currentWaypointIndex;
    public bool enabled = true;
    public float endReachedDistance = 0.05f;
    public VInt forwardLook = 0x3e8;
    protected VInt3 lastFoundWaypointPosition;
    protected float lastFoundWaypointTime = -9999f;
    protected Path path;
    public int pickNextWaypointDist = 0x7d0;
    public float rotationSpeed = 5f;
    protected Seeker seeker;
    public int speed = 0xbb8;
    protected VInt3 targetDirection;
    protected VInt3 targetPoint;
    private VInt3 targetPos = VInt3.zero;
    private bool targetPosIsValid;
    public VInt3 targetSearchPos = VInt3.zero;

    protected VInt3 CaculateDir(VInt3 currentPosition)
    {
        if (((this.path == null) || (this.path.vectorPath == null)) || (this.path.vectorPath.Count == 0))
        {
            return VInt3.zero;
        }
        List<VInt3> vectorPath = this.path.vectorPath;
        if (vectorPath.Count == 1)
        {
            vectorPath.Insert(0, currentPosition);
        }
        if (this.currentWaypointIndex >= vectorPath.Count)
        {
            this.currentWaypointIndex = vectorPath.Count - 1;
        }
        if (this.currentWaypointIndex <= 1)
        {
            this.currentWaypointIndex = 1;
        }
    Label_0088:
        if (this.currentWaypointIndex < (vectorPath.Count - 1))
        {
            long num = vectorPath[this.currentWaypointIndex].XZSqrMagnitude(currentPosition);
            if (num == 0)
            {
                this.lastFoundWaypointPosition = currentPosition;
                this.lastFoundWaypointTime = Time.time;
                this.currentWaypointIndex++;
                goto Label_0088;
            }
            if (num < (this.pickNextWaypointDist * this.pickNextWaypointDist))
            {
                VInt3 num2 = vectorPath[this.currentWaypointIndex - 1];
                VInt3 num3 = vectorPath[this.currentWaypointIndex];
                VInt3 num4 = num3 - num2;
                long num5 = num4.magnitude2D;
                if (VInt3.DotXZLong(currentPosition - num2, num4.NormalizeTo(0x3e8)) >= (num5 * 0x3e8L))
                {
                    this.lastFoundWaypointPosition = currentPosition;
                    this.lastFoundWaypointTime = Time.time;
                    this.currentWaypointIndex++;
                    goto Label_0088;
                }
            }
        }
        this.targetPoint = this.CalculateTargetPoint(currentPosition, vectorPath[this.currentWaypointIndex - 1], vectorPath[this.currentWaypointIndex]);
        this.targetDirection = this.targetPoint - currentPosition;
        this.targetDirection.y = 0;
        return this.targetDirection.NormalizeTo(0x3e8);
    }

    protected VInt3 CalculateTargetPoint(VInt3 p, VInt3 a, VInt3 b)
    {
        if ((a.x == b.x) && (a.z == b.z))
        {
            return a;
        }
        VFactor f = AstarMath.NearestPointFactorXZ(ref a, ref b, ref p);
        int num3 = IntMath.Sqrt(VInt3.Lerp(a, b, f).XZSqrMagnitude(ref p));
        int num5 = IntMath.Sqrt(a.XZSqrMagnitude(ref b));
        if (num5 == 0)
        {
            return b;
        }
        int num6 = Mathf.Clamp(this.forwardLook.i - num3, 0, this.forwardLook.i);
        VFactor one = new VFactor((num6 * f.den) + (f.nom * num5), num5 * f.den);
        if (one > VFactor.one)
        {
            one = VFactor.one;
        }
        else if (one < VFactor.zero)
        {
            one = VFactor.zero;
        }
        one.strip();
        return VInt3.Lerp(a, b, one);
    }

    public void EnableRVO(bool enable)
    {
        if (this.rvo != null)
        {
            this.rvo.enabled = enable;
        }
    }

    public virtual VInt3 GetFeetPosition()
    {
        return this.actor.handle.location;
    }

    public bool Init(PoolObjHandle<ActorRoot> InActor)
    {
        this.actor = InActor;
        this.seeker = this.actor.handle.GetComponent<Seeker>();
        if (this.seeker == null)
        {
            return false;
        }
        this.rvo = this.actor.handle.GetComponent<RVOController>();
        this.seeker.pathCallback = new OnPathDelegate(this.OnPathComplete);
        this.lastFoundWaypointPosition = this.GetFeetPosition();
        if ((this.actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (this.actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))
        {
            this.checkNavNode = true;
        }
        if (this.rvo != null)
        {
            this.rvo.checkNavNode = this.checkNavNode;
        }
        return true;
    }

    public void InvalidPath()
    {
        this.seeker.RecyclePath();
        this.path = null;
    }

    public void Move(out VInt3 targetDir, int dt)
    {
        targetDir = VInt3.forward;
        if (!this.targetReached && this.canMove)
        {
            ActorRoot handle = this.actor.handle;
            VInt3 location = handle.location;
            VInt3 num2 = this.CaculateDir(location);
            if ((this.targetDirection.x != 0) || (this.targetDirection.z != 0))
            {
                targetDir = this.targetDirection;
                targetDir.NormalizeTo(0x3e8);
            }
            else
            {
                targetDir = handle.forward;
            }
            VInt3 targetPos = handle.location;
            VInt3 delta = num2;
            delta *= (this.speed * dt) / 0x3e8;
            delta = (VInt3) (delta / 1000f);
            bool flag = (this.rvo != null) && this.rvo.enabled;
            bool collided = false;
            if (this.checkNavNode && !flag)
            {
                VInt num5;
                delta = PathfindingUtility.Move((ActorRoot) this.actor, delta, out num5, out collided, null);
                handle.groundY = num5;
            }
            VInt3 num7 = handle.location - this.targetPos;
            if (num7.sqrMagnitudeLong2D <= delta.sqrMagnitudeLong2D)
            {
                if (this.checkNavNode && !this.targetPosIsValid)
                {
                    targetPos = handle.location + delta;
                }
                else
                {
                    targetPos = this.targetPos;
                }
                this.targetReached = true;
                this.OnTargetReached();
            }
            else
            {
                targetPos = handle.location + delta;
            }
            if (flag)
            {
                VInt3 a = targetPos - handle.location;
                a = IntMath.Divide(a, 0x3e8L, (long) dt);
                this.rvo.Move(a);
            }
            else
            {
                handle.location = targetPos;
                handle.hasReachedNavEdge = collided;
            }
        }
    }

    public virtual void OnPathComplete(Path _p)
    {
        this.path = _p;
        this.currentWaypointIndex = 0;
        this.targetReached = false;
        this.canMove = true;
    }

    public virtual void OnTargetReached()
    {
        this.seeker.RecyclePath();
        this.path = null;
    }

    public void Reset()
    {
        this.actor.Validate();
        if (this.rvo != null)
        {
            this.rvo.enabled = false;
        }
        this.seeker.RecyclePath();
        this.path = null;
    }

    protected virtual void RotateTowards(VInt3 dir)
    {
    }

    public void SearchPath(VInt3 target)
    {
        int num2;
        if (this.path != null)
        {
            bool flag = true;
            if (this.targetSearchPos.XZSqrMagnitude(ref target) < 1L)
            {
                if (this.targetReached)
                {
                    if (this.actor.handle.location.XZSqrMagnitude(ref this.targetPos) < 100L)
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                return;
            }
        }
        this.targetPosIsValid = PathfindingUtility.ValidateTarget(this.GetFeetPosition(), target, out this.targetPos, out num2);
        VInt3 feetPosition = this.GetFeetPosition();
        int actorCamp = (int) this.actor.handle.TheActorMeta.ActorCamp;
        this.seeker.RecyclePath();
        this.path = null;
        this.path = this.seeker.StartPathEx(ref feetPosition, ref this.targetPos, actorCamp, null, -1);
        if (this.path != null)
        {
            this.targetSearchPos = target;
            this.canMove = false;
            AstarPath.WaitForPath(this.path);
        }
    }

    public void StopMove()
    {
        if (this.rvo != null)
        {
            this.rvo.Move(VInt3.zero);
        }
        this.seeker.RecyclePath();
        this.path = null;
    }

    public void UpdateLogic(int deltaTime)
    {
        if ((this.rvo != null) && this.rvo.enabled)
        {
            this.rvo.UpdateLogic(deltaTime);
            if (this.targetReached)
            {
                this.rvo.Move(VInt3.zero);
            }
        }
    }

    public bool hasCollidedWithAgents
    {
        get
        {
            return ((((this.rvo != null) && this.rvo.enabled) && (this.rvo.rvoAgent != null)) && this.rvo.rvoAgent.hasCollided);
        }
    }

    public RVOController rvo { get; protected set; }

    public bool targetReached { get; protected set; }
}

