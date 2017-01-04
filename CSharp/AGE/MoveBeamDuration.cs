namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class MoveBeamDuration : DurationCondition
    {
        private bool bDone;
        private GameObject beamObject;
        public float beamWidth = 1f;
        public VInt3 bindDestOffet = new VInt3(0, 0, 0);
        [SubObject]
        public string bindPointName = string.Empty;
        public VInt3 bindPosOffset = new VInt3(0, 0, 0);
        private bool bInit;
        private LineRenderer lineRenderer;
        [AssetReference(AssetRefType.Particle)]
        public string resourceName = string.Empty;
        [ObjectTemplate(new System.Type[] {  })]
        public int sourceId;
        private PoolObjHandle<ActorRoot> srcActor;
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;
        public int textureScale = 5;

        public override BaseEvent Clone()
        {
            MoveBeamDuration duration = ClassObjPool<MoveBeamDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MoveBeamDuration duration = src as MoveBeamDuration;
            this.sourceId = duration.sourceId;
            this.targetId = duration.targetId;
            this.bindPosOffset = duration.bindPosOffset;
            this.bindDestOffet = duration.bindDestOffet;
            this.resourceName = duration.resourceName;
            this.beamWidth = duration.beamWidth;
            this.textureScale = duration.textureScale;
            this.bindPointName = duration.bindPointName;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.bInit = this.Init(_action);
            base.Enter(_action, _track);
        }

        private Vector3 GetSrcPosition()
        {
            Vector3 zero = Vector3.zero;
            if (this.bindPointName.Length != 0)
            {
                GameObject obj2 = null;
                obj2 = SubObject.FindSubObject(this.srcActor.handle.gameObject, this.bindPointName);
                if (obj2 != null)
                {
                    return obj2.transform.position;
                }
            }
            return (Vector3) IntMath.Transform(this.bindPosOffset, this.srcActor.handle.forward, (VInt3) this.srcActor.handle.gameObject.transform.position);
        }

        private void HideBeam()
        {
            if (this.lineRenderer != null)
            {
                this.lineRenderer.SetVertexCount(0);
            }
        }

        private bool Init(AGE.Action _action)
        {
            this.srcActor = _action.GetActorHandle(this.sourceId);
            this.targetActor = _action.GetActorHandle(this.targetId);
            if ((this.srcActor == 0) || (this.targetActor == 0))
            {
                return false;
            }
            Vector3 srcPosition = this.GetSrcPosition();
            Quaternion rot = Quaternion.LookRotation((Vector3) this.targetActor.handle.forward);
            bool isInit = false;
            this.beamObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.resourceName, true, SceneObjType.ActionRes, srcPosition, rot, out isInit);
            if (this.beamObject == null)
            {
                return false;
            }
            this.lineRenderer = this.beamObject.gameObject.GetComponentInChildren<LineRenderer>();
            if (this.lineRenderer == null)
            {
                return false;
            }
            this.lineRenderer.SetWidth(this.beamWidth, this.beamWidth);
            return true;
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            this.UnInit();
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.srcActor.Release();
            this.targetActor.Release();
            this.bInit = false;
            this.bDone = false;
            this.beamObject = null;
            this.lineRenderer = null;
            this.bindPointName = string.Empty;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
            if (this.bInit || !this.bDone)
            {
                if ((this.srcActor == 0) || (this.targetActor == 0))
                {
                    this.bDone = true;
                    this.HideBeam();
                }
                else
                {
                    VInt3 location = this.srcActor.handle.location;
                    VInt3 num4 = this.targetActor.handle.location - location;
                    int num3 = num4.magnitude2D;
                    if (num3 > 0)
                    {
                        this.SetBeamLength(num3);
                        this.RenderBeam();
                    }
                    if (this.srcActor.handle.ActorControl.IsDeadState || this.targetActor.handle.ActorControl.IsDeadState)
                    {
                        this.bDone = true;
                        this.HideBeam();
                    }
                }
            }
        }

        private void RenderBeam()
        {
            Vector3 srcPosition = this.GetSrcPosition();
            Vector3 position = (Vector3) IntMath.Transform(this.bindDestOffet, this.targetActor.handle.forward, (VInt3) this.targetActor.handle.gameObject.transform.position);
            this.lineRenderer.SetVertexCount(2);
            this.lineRenderer.SetPosition(0, srcPosition);
            this.lineRenderer.SetPosition(1, position);
        }

        private void SetBeamLength(int _length)
        {
            this.SetTextureScale((_length * this.textureScale) / 0x3e8, 1);
        }

        private void SetTextureScale(int _tileX, int _tileY)
        {
            Renderer component = this.beamObject.GetComponent<Renderer>();
            if ((component != null) && (component.material != null))
            {
                component.material.SetTextureScale("_MainTex", new Vector2((float) _tileX, (float) _tileY));
            }
        }

        private void UnInit()
        {
            this.HideBeam();
            if (this.beamObject != null)
            {
                this.beamObject.transform.position = new Vector3(10000f, 10000f, 10000f);
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.beamObject);
            }
        }
    }
}

