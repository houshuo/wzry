namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [ExecuteInEditMode]
    public class ActorConfig : MonoBehaviour, IPooledMonoBehaviour
    {
        private ObjWrapper ActorControl;
        private PlayerMovement ActorMovement;
        [NonSerialized, HideInInspector]
        private ActorRoot ActorObj;
        private PoolObjHandle<ActorRoot> ActorPtr = new PoolObjHandle<ActorRoot>();
        public ActorTypeDef ActorType;
        public int BattleOrder;
        public int[] BattleOrderDepend;
        [NonSerialized, HideInInspector]
        private bool bNeedLerp;
        [NonSerialized, HideInInspector]
        private bool bNeedReloadGizmos = true;
        public bool CanMovable = true;
        [NonSerialized, HideInInspector]
        public CActorInfo CharInfo;
        public COM_PLAYERCAMP CmpType;
        public int ConfigID;
        [NonSerialized, HideInInspector]
        private Material drawMat;
        [NonSerialized, HideInInspector]
        private Mesh drawMesh;
        [NonSerialized, HideInInspector]
        private Quaternion drawRot;
        [NonSerialized, HideInInspector]
        private Vector3 drawScale;
        [NonSerialized, HideInInspector]
        private uint FrameBlockIndex;
        [NonSerialized, HideInInspector]
        private int groundSpeed;
        public bool Invincible;
        [HideInInspector, SerializeField]
        public bool isStatic;
        [NonSerialized, HideInInspector]
        private double lastUpdateTime;
        [NonSerialized, HideInInspector]
        private float maxFrameMove;
        [NonSerialized, HideInInspector]
        public GameObject meshObject;
        [NonSerialized, HideInInspector]
        private Vector3 moveForward = Vector3.forward;
        [NonSerialized, HideInInspector]
        private Renderer myRenderer;
        public int nPreMoveSeq = -1;
        [NonSerialized, HideInInspector]
        private VInt3 oldLocation;
        [NonSerialized, HideInInspector]
        private uint RepairFramesMin = 1;
        [NonSerialized, HideInInspector]
        private string szCharInfoPath;
        [NonSerialized, HideInInspector]
        public Quaternion tarRotation;

        private event CustomMoveLerpFunc CustomMoveLerp;

        public void ActorStart()
        {
            this.bNeedLerp = Singleton<FrameSynchr>.instance.bActive && (this.ActorObj.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ);
            this.ActorControl = (this.ActorObj.TheActorMeta.ActorType >= ActorTypeDef.Actor_Type_Bullet) ? null : this.ActorObj.ActorControl;
            this.ActorMovement = (PlayerMovement) this.ActorObj.MovementComponent;
        }

        public void AddCustomMoveLerp(CustomMoveLerpFunc func)
        {
            if (this.bNeedLerp)
            {
                this.CustomMoveLerp = (CustomMoveLerpFunc) Delegate.Combine(this.CustomMoveLerp, func);
            }
        }

        public PoolObjHandle<ActorRoot> AttachActorRoot(GameObject rootObj, ref ActorMeta theActorMeta, CActorInfo actorInfo = null)
        {
            VInt num;
            DebugHelper.Assert(this.ActorObj == null);
            this.ActorObj = ClassObjPool<ActorRoot>.Get();
            this.ActorPtr = new PoolObjHandle<ActorRoot>(this.ActorObj);
            this.ActorObj.ObjLinker = this;
            this.ActorObj.location = (VInt3) rootObj.transform.position;
            this.ActorObj.forward = (VInt3) rootObj.transform.forward;
            this.ActorObj.rotation = rootObj.transform.rotation;
            this.oldLocation = this.ActorObj.location;
            this.tarRotation = this.ActorObj.rotation;
            this.ActorObj.TheActorMeta = theActorMeta;
            if (theActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                this.ActorObj.TheActorMeta.EnCId = theActorMeta.ConfigId;
            }
            this.ActorObj.CharInfo = actorInfo;
            if (((this.ActorObj.TheActorMeta.ActorType < ActorTypeDef.Actor_Type_Bullet) && (this.ActorObj.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)) && PathfindingUtility.GetGroundY(this.ActorObj, out num))
            {
                this.ActorObj.groundY = num;
                VInt3 location = this.ActorObj.location;
                location.y = num.i;
                this.ActorObj.location = location;
            }
            return this.ActorPtr;
        }

        private void Awake()
        {
            base.gameObject.layer = LayerMask.NameToLayer("Actor");
            if (this.isStatic)
            {
                MonoSingleton<GameLoader>.instance.AddStaticActor(this);
            }
        }

        public void CustumLateUpdate()
        {
            if (((this.myRenderer != null) && (this.ActorObj != null)) && (this.myRenderer.isVisible != this.ActorObj.InCamera))
            {
                this.ActorObj.InCamera = this.myRenderer.isVisible;
                if (this.ActorObj.InCamera)
                {
                    if (this.ActorObj.isMovable)
                    {
                        this.oldLocation = this.ActorObj.location;
                        base.gameObject.transform.position = (Vector3) this.ActorObj.location;
                    }
                    if (this.ActorObj.isRotatable)
                    {
                        VFactor factor = VInt3.AngleInt(this.ActorObj.forward, VInt3.forward);
                        int num = (this.ActorObj.forward.x * VInt3.forward.z) - (VInt3.forward.x * this.ActorObj.forward.z);
                        if (num < 0)
                        {
                            factor = VFactor.twoPi - factor;
                        }
                        this.tarRotation = Quaternion.AngleAxis(factor.single * 57.29578f, Vector3.up);
                        base.gameObject.transform.rotation = this.tarRotation;
                    }
                }
            }
        }

        public void DetachActorRoot()
        {
            if (this.ActorObj != null)
            {
                if (this.ActorObj.SMNode != null)
                {
                    this.ActorObj.SMNode.Detach();
                    this.ActorObj.SMNode.Release();
                    this.ActorObj.SMNode = null;
                }
                this.ActorObj.UninitActor();
                this.ActorObj.ObjLinker = null;
                this.ActorObj.Release();
                this.ActorPtr.Release();
                this.ActorObj = null;
                this.myRenderer = null;
                this.CustomMoveLerp = null;
                this.ActorControl = null;
                this.ActorMovement = null;
                if (this.meshObject != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObject);
                    this.meshObject = null;
                }
            }
        }

        public PoolObjHandle<ActorRoot> GetActorHandle()
        {
            return this.ActorPtr;
        }

        private Vector3 NormalMoveLerp(uint nDeltaTick)
        {
            float distance = this.ActorObj.MovementComponent.GetDistance(nDeltaTick);
            Vector3 moveForward = this.moveForward;
            Vector3 location = (Vector3) this.ActorObj.location;
            Vector3 position = base.gameObject.transform.position;
            Vector3 vector4 = position + ((Vector3) (moveForward * distance));
            Vector3 vector5 = location;
            if (this.ActorObj.hasReachedNavEdge || this.ActorObj.hasCollidedWithAgents)
            {
                location.y = position.y;
                Vector3 vector6 = position - location;
                float num2 = vector6.magnitude;
                vector5 = Vector3.Lerp(position, location, distance / num2);
                this.RepairFramesMin = 1;
                this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
                return vector5;
            }
            location.y = vector4.y;
            Vector3 lhs = vector4 - location;
            float magnitude = lhs.magnitude;
            float num4 = this.maxFrameMove * Singleton<FrameSynchr>.instance.PreActFrames;
            if (magnitude < (this.RepairFramesMin * this.maxFrameMove))
            {
                vector5 = vector4;
                this.RepairFramesMin = 1;
                this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
                return vector5;
            }
            if (magnitude < num4)
            {
                float num5 = Mathf.Clamp((float) (magnitude / num4), (float) 0.05f, (float) 0.3f);
                float num6 = Vector3.Dot(lhs, moveForward);
                Vector3 vector8 = location + ((Vector3) (moveForward * num4));
                Vector3 vector10 = vector8 - position;
                Vector3 normalized = vector10.normalized;
                if (num6 > (magnitude * 0.707f))
                {
                    vector5 = position + ((Vector3) ((normalized * distance) * (1f - num5)));
                }
                else if (num6 < (magnitude * -0.707f))
                {
                    vector5 = position + ((Vector3) ((normalized * distance) * (1f + num5)));
                }
                else
                {
                    vector5 = position + ((Vector3) ((normalized * distance) * (1f + num5)));
                }
                this.RepairFramesMin = 1;
                this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
                return vector5;
            }
            if (Singleton<FrameSynchr>.instance.CurFrameNum == this.FrameBlockIndex)
            {
                return position;
            }
            this.RepairFramesMin = 1;
            return vector5;
        }

        private Quaternion ObjRotationLerp()
        {
            return Quaternion.RotateTowards(base.gameObject.transform.rotation, this.tarRotation, this.ActorObj.MovementComponent.rotateSpeed * Time.deltaTime);
        }

        public void OnActorMeshChanged(GameObject newMesh)
        {
            if (this.meshObject != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObject);
            }
            this.meshObject = newMesh;
            this.myRenderer = base.gameObject.GetSkinnedMeshRendererInChildren();
            if (this.myRenderer == null)
            {
                this.myRenderer = base.gameObject.GetMeshRendererInChildren();
            }
        }

        public void OnCreate()
        {
            this.CanMovable = true;
            this.isStatic = false;
            this.CharInfo = null;
            this.ActorObj = null;
            this.ActorPtr.Release();
            this.myRenderer = null;
            this.bNeedLerp = false;
            this.GroundSpeed = 0;
            this.nPreMoveSeq = -1;
            this.RepairFramesMin = 1;
            this.FrameBlockIndex = 0;
            this.CustomMoveLerp = null;
            this.ActorControl = null;
            this.ActorMovement = null;
        }

        protected void OnDestroy()
        {
        }

        public void OnGet()
        {
            this.CanMovable = true;
            this.isStatic = false;
            this.CharInfo = null;
            this.ActorObj = null;
            this.ActorPtr.Release();
            this.myRenderer = null;
            this.bNeedLerp = false;
            this.GroundSpeed = 0;
            this.nPreMoveSeq = -1;
            this.RepairFramesMin = 1;
            this.FrameBlockIndex = 0;
            this.CustomMoveLerp = null;
            this.ActorControl = null;
            this.ActorMovement = null;
        }

        public void OnRecycle()
        {
            this.DetachActorRoot();
        }

        public void ReattachActor()
        {
            this.ActorPtr.Validate();
        }

        public void RmvCustomMoveLerp(CustomMoveLerpFunc func)
        {
            if (this.bNeedLerp)
            {
                this.CustomMoveLerp = (CustomMoveLerpFunc) Delegate.Remove(this.CustomMoveLerp, func);
                base.gameObject.transform.position = (Vector3) this.ActorObj.location;
                if (this.CustomMoveLerp != null)
                {
                    this.CustomMoveLerp(this.ActorObj, 0, true);
                }
                if (this.ActorObj.MovementComponent != null)
                {
                    this.ActorObj.MovementComponent.GravityModeLerp(0, true);
                }
            }
        }

        public void SetForward(VInt3 InDir, int nSeq)
        {
            if (this.bNeedLerp && this.ActorObj.InCamera)
            {
                bool flag = false;
                if (((this.nPreMoveSeq < 0) || (nSeq < 0)) || (nSeq == this.nPreMoveSeq))
                {
                    flag = true;
                }
                else if (nSeq > this.nPreMoveSeq)
                {
                    byte num = (byte) nSeq;
                    byte nPreMoveSeq = (byte) this.nPreMoveSeq;
                    num = (byte) (num - 0x80);
                    nPreMoveSeq = (byte) (nPreMoveSeq - 0x80);
                    flag = num < nPreMoveSeq;
                }
                if (flag)
                {
                    VInt3 moveForward;
                    Vector3 vector = (Vector3) InDir;
                    this.moveForward = vector.normalized;
                    if ((this.ActorObj.ActorControl != null) && this.ActorObj.ActorControl.CanRotate)
                    {
                        moveForward = (VInt3) this.moveForward;
                    }
                    else
                    {
                        moveForward = this.ActorObj.forward;
                    }
                    VFactor factor = VInt3.AngleInt(moveForward, VInt3.forward);
                    int num4 = (moveForward.x * VInt3.forward.z) - (VInt3.forward.x * moveForward.z);
                    if (num4 < 0)
                    {
                        factor = VFactor.twoPi - factor;
                    }
                    this.tarRotation = Quaternion.AngleAxis(factor.single * 57.29578f, Vector3.up);
                }
            }
        }

        public void Start()
        {
        }

        private void Update()
        {
            if ((Singleton<BattleLogic>.instance.isFighting && (this.ActorObj != null)) && (this.ActorObj.Visible || Singleton<WatchController>.GetInstance().IsWatching))
            {
                try
                {
                    bool flag = ((Singleton<FrameSynchr>.instance.FrameSpeed == 1) && this.bNeedLerp) && (this.ActorObj.InCamera || (this.ActorObj.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet));
                    uint nDelta = (uint) (Time.deltaTime * 1000f);
                    bool bReset = false;
                    if ((this.CustomMoveLerp != null) && flag)
                    {
                        this.CustomMoveLerp(this.ActorObj, nDelta, false);
                    }
                    else
                    {
                        VInt num2;
                        Vector3 vector;
                        if (((flag && (this.ActorControl != null)) && (this.ActorControl.CanMove && (this.ActorMovement != null))) && (this.ActorMovement.isMoving || (this.ActorMovement.nLerpStep > 0)))
                        {
                            vector = this.NormalMoveLerp(nDelta);
                            if (!this.ActorMovement.isLerpFlying && PathfindingUtility.GetGroundY((VInt3) vector, out num2))
                            {
                                vector.y = (float) num2;
                            }
                            base.gameObject.transform.position = vector;
                            this.ActorMovement.nLerpStep--;
                        }
                        else if (this.oldLocation != this.ActorObj.location)
                        {
                            this.oldLocation = this.ActorObj.location;
                            Vector3 oldLocation = (Vector3) this.oldLocation;
                            Vector3 position = base.gameObject.transform.position;
                            oldLocation.y = position.y;
                            Vector3 vector4 = oldLocation - position;
                            float num3 = 0f;
                            if (((this.groundSpeed <= 0) || !flag) || ((num3 = vector4.magnitude) > (this.maxFrameMove * Singleton<FrameSynchr>.instance.PreActFrames)))
                            {
                                base.gameObject.transform.position = (Vector3) this.ActorObj.location;
                                if (this.CustomMoveLerp != null)
                                {
                                    this.CustomMoveLerp(this.ActorObj, 0, true);
                                }
                                bReset = true;
                            }
                            else if (((num3 > 0.1f) && !ActorHelper.IsHostCtrlActor(ref this.ActorPtr)) && (this.ActorMovement != null))
                            {
                                float distance = this.ActorMovement.GetDistance(nDelta);
                                vector = Vector3.Lerp(position, oldLocation, distance / num3);
                                if (!this.ActorMovement.isLerpFlying && PathfindingUtility.GetGroundY((VInt3) vector, out num2))
                                {
                                    vector.y = (float) num2;
                                }
                                base.gameObject.transform.position = vector;
                                this.oldLocation = (VInt3) vector;
                            }
                        }
                    }
                    if (flag && (this.ActorMovement != null))
                    {
                        this.ActorMovement.GravityModeLerp(nDelta, bReset);
                    }
                    if ((flag && (this.ActorControl != null)) && this.ActorControl.CanRotate)
                    {
                        if (base.gameObject.transform.rotation != this.tarRotation)
                        {
                            base.gameObject.transform.rotation = this.ObjRotationLerp();
                        }
                    }
                    else if (base.gameObject.transform.rotation != this.ActorObj.rotation)
                    {
                        base.gameObject.transform.rotation = this.ActorObj.rotation;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public int GroundSpeed
        {
            get
            {
                return this.groundSpeed;
            }
            set
            {
                this.groundSpeed = value;
                this.maxFrameMove = ((this.groundSpeed * Singleton<FrameSynchr>.instance.FrameDelta) / ((ulong) 0x3e8L)) * 0.001f;
            }
        }
    }
}

