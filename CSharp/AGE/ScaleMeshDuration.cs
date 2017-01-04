namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class ScaleMeshDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        private int originalRadius;
        private Vector3 originalScale;
        private VInt3 originalSize;
        public int scaleRate = 0x2710;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            ScaleMeshDuration duration = ClassObjPool<ScaleMeshDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ScaleMeshDuration duration = src as ScaleMeshDuration;
            this.targetId = duration.targetId;
            this.scaleRate = duration.scaleRate;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            this.SetMeshScale(this.scaleRate);
            this.SetCollisionScale(this.scaleRate);
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            this.RecorveMeshScale();
            this.RecorveCollisionScale();
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.originalScale = Vector3.one;
            this.originalRadius = 1;
            this.originalSize = VInt3.one;
            this.scaleRate = 0x2710;
            this.actorObj.Release();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }

        private void RecorveCollisionScale()
        {
            if (this.actorObj != 0)
            {
                VCollisionShape shape = this.actorObj.handle.shape;
                if (shape != null)
                {
                    if (shape is VCollisionSphere)
                    {
                        VCollisionSphere sphere = shape as VCollisionSphere;
                        if (sphere != null)
                        {
                            sphere.Radius = this.originalRadius;
                        }
                    }
                    else if (shape is VCollisionBox)
                    {
                        VCollisionBox box = shape as VCollisionBox;
                        if (box != null)
                        {
                            box.Size = this.originalSize;
                        }
                    }
                }
            }
        }

        private void RecorveMeshScale()
        {
            if ((this.actorObj != 0) && (this.actorObj.handle.ActorMesh != null))
            {
                this.actorObj.handle.ActorMesh.transform.localScale = this.originalScale;
            }
        }

        private void SetCollisionScale(int _scaleRate)
        {
            if (this.actorObj != 0)
            {
                VFactor factor = new VFactor((long) this.scaleRate, 0x2710L);
                VCollisionShape shape = this.actorObj.handle.shape;
                if (shape != null)
                {
                    int roundInt = factor.roundInt;
                    if (shape is VCollisionSphere)
                    {
                        VCollisionSphere sphere = shape as VCollisionSphere;
                        if (sphere != null)
                        {
                            this.originalRadius = sphere.Radius;
                            sphere.Radius *= roundInt;
                        }
                    }
                    else if (shape is VCollisionBox)
                    {
                        VCollisionBox box = shape as VCollisionBox;
                        if (box != null)
                        {
                            VInt3 num2;
                            this.originalSize = box.Size;
                            num2.x = box.Size.x * roundInt;
                            num2.y = box.Size.y * roundInt;
                            num2.z = box.Size.z * roundInt;
                            box.Size = num2;
                        }
                    }
                }
            }
        }

        private void SetMeshScale(int _scaleRate)
        {
            if ((this.actorObj != 0) && (this.actorObj.handle.ActorMesh != null))
            {
                float x = ((float) _scaleRate) / 10000f;
                Vector3 vector = new Vector3(x, x, x);
                this.originalScale = this.actorObj.handle.ActorMesh.transform.localScale;
                this.actorObj.handle.ActorMesh.transform.localScale = vector;
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

