namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public class SpawnerTailsman : SpawnerBase
    {
        public STriggerCondActor[] SrcActorCond;
        public int TailsmanId;

        public SpawnerTailsman(SpawnerWrapper inWrapper) : base(inWrapper)
        {
        }

        public override object DoSpawn(VInt3 inWorldPos, VInt3 inDir, GameObject inSpawnPoint)
        {
            CTailsman tailsman = ClassObjPool<CTailsman>.Get();
            tailsman.Init(this.TailsmanId, inWorldPos, this.SrcActorCond);
            return tailsman;
        }
    }
}

