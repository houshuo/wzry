namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CTailsman : PooledClassObject, IUpdateLogic
    {
        private int CharmId;
        private int ConfigId;
        private int EffectRadius;
        private VInt3 InitLoc;
        private STriggerCondActor[] SrcActorCond;

        public void DoClearing()
        {
            if (this.Presentation != null)
            {
                this.Presentation.CustomSetActive(false);
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.Presentation);
                this.Presentation = null;
            }
            PoolObjHandle<CTailsman> inCharm = new PoolObjHandle<CTailsman>(this);
            Singleton<ShenFuSystem>.instance.RemoveCharm(inCharm);
            base.Release();
        }

        public static int ExtractCharmIdFromLib(int inLibCfgId)
        {
            int iParam = 0;
            CharmLib dataByKey = GameDataMgr.charmLib.GetDataByKey((long) inLibCfgId);
            if (dataByKey != null)
            {
                int num2 = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (dataByKey.astCharmId[i].iParam == 0)
                    {
                        break;
                    }
                    num2++;
                }
                if (num2 > 0)
                {
                    ushort index = FrameRandom.Random((uint) num2);
                    iParam = dataByKey.astCharmId[index].iParam;
                }
            }
            return iParam;
        }

        public void Init(int inConfigId, VInt3 inWorldPos, STriggerCondActor[] inSrcActorCond)
        {
            this.ConfigId = inConfigId;
            this.InitLoc = inWorldPos;
            this.CharmId = ExtractCharmIdFromLib(this.ConfigId);
            ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey((long) this.CharmId);
            if (dataByKey != null)
            {
                this.EffectRadius = (int) dataByKey.dwGetRadius;
                string prefabName = StringHelper.UTF8BytesToString(ref dataByKey.szShenFuResPath);
                this.Presentation = MonoSingleton<SceneMgr>.instance.InstantiateLOD(prefabName, false, SceneObjType.ActionRes, (Vector3) inWorldPos);
                if (this.Presentation != null)
                {
                    this.Presentation.CustomSetActive(true);
                }
                if (inSrcActorCond == null)
                {
                    this.SrcActorCond = null;
                }
                else
                {
                    this.SrcActorCond = inSrcActorCond.Clone() as STriggerCondActor[];
                }
                Singleton<ShenFuSystem>.instance.AddCharm(new PoolObjHandle<CTailsman>(this));
                PoolObjHandle<CTailsman> inTailsman = new PoolObjHandle<CTailsman>(this);
                STailsmanEventParam prm = new STailsmanEventParam(inTailsman, new PoolObjHandle<ActorRoot>(null));
                Singleton<GameEventSys>.instance.SendEvent<STailsmanEventParam>(GameEventDef.Event_TailsmanSpawn, ref prm);
            }
        }

        public override void OnRelease()
        {
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.Presentation = null;
            this.EffectRadius = 0;
            this.ConfigId = 0;
            this.InitLoc = new VInt3();
            this.SrcActorCond = null;
            this.CharmId = 0;
        }

        private void SetMyselfOnFire(PoolObjHandle<ActorRoot> inActor)
        {
            if (inActor != 0)
            {
                if (this.CharmId > 0)
                {
                    ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey((long) this.CharmId);
                    if (dataByKey != null)
                    {
                        BufConsumer consumer = new BufConsumer(dataByKey.iBufId, inActor, inActor);
                        if (consumer.Use())
                        {
                            PoolObjHandle<CTailsman> inTailsman = new PoolObjHandle<CTailsman>(this);
                            STailsmanEventParam prm = new STailsmanEventParam(inTailsman, inActor);
                            Singleton<GameEventSys>.instance.SendEvent<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, ref prm);
                        }
                    }
                }
                this.DoClearing();
            }
        }

        public void UpdateLogic(int inDelta)
        {
            PoolObjHandle<ActorRoot> inActor = new PoolObjHandle<ActorRoot>(null);
            ulong num = (ulong) (this.EffectRadius * this.EffectRadius);
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle2 = gameActors[i];
                if ((handle2 != 0) && !handle2.handle.ActorControl.IsDeadState)
                {
                    if (this.SrcActorCond != null)
                    {
                        bool flag = true;
                        foreach (STriggerCondActor actor in this.SrcActorCond)
                        {
                            flag &= actor.FilterMatch(ref handle2);
                        }
                        if (!flag)
                        {
                            continue;
                        }
                    }
                    VInt3 num6 = handle2.handle.location - this.InitLoc;
                    if (num6.sqrMagnitudeLong2D < num)
                    {
                        inActor = handle2;
                        break;
                    }
                }
            }
            if (inActor != 0)
            {
                this.SetMyselfOnFire(inActor);
            }
        }

        public GameObject Presentation { get; private set; }
    }
}

