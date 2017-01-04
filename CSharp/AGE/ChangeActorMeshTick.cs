namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/System")]
    public class ChangeActorMeshTick : TickEvent
    {
        [AssetReference(AssetRefType.Prefab)]
        public string prefabName = string.Empty;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            ChangeActorMeshTick tick = ClassObjPool<ChangeActorMeshTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ChangeActorMeshTick tick = src as ChangeActorMeshTick;
            this.targetId = tick.targetId;
            this.prefabName = tick.prefabName;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.prefabName = string.Empty;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle != 0)
            {
                GameObject inActorMesh = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.prefabName, false, SceneObjType.ActionRes, Vector3.zero);
                if (inActorMesh != null)
                {
                    Transform transform = inActorMesh.transform;
                    transform.SetParent(actorHandle.handle.gameObject.transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    actorHandle.handle.SetActorMesh(inActorMesh);
                }
            }
        }
    }
}

