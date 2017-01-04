namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct MonsterDropItemCreator
    {
        private MonsterWrapper MonsterRef;
        public void MakeDropItemIfNeed(MonsterWrapper InMonster, ObjWrapper InTarget)
        {
            DebugHelper.Assert(InMonster != null);
            ResMonsterCfgInfo cfgInfo = InMonster.cfgInfo;
            if ((cfgInfo != null) && (cfgInfo.iBufDropID != 0))
            {
                this.MonsterRef = InMonster;
                if (FrameRandom.Random(0x2710) < cfgInfo.iBufDropRate)
                {
                    this.SpawnBuf(cfgInfo.iBufDropID);
                }
            }
        }

        private void SpawnBuf(int BufID)
        {
            ResBufDropInfo dataByKey = GameDataMgr.bufDropInfoDatabin.GetDataByKey((uint) BufID);
            object[] inParameters = new object[] { BufID };
            DebugHelper.Assert(dataByKey != null, "找不到Buf，id={0}", inParameters);
            if (dataByKey != null)
            {
                int num = 0;
                uint nMax = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (dataByKey.astBufs[i].dwBufID == 0)
                    {
                        break;
                    }
                    num++;
                    nMax += dataByKey.astBufs[i].dwProbability;
                }
                if (num > 0)
                {
                    int num4 = FrameRandom.Random(nMax);
                    ResBufConfigInfo inBufDropInfo = null;
                    for (int j = 0; j < num; j++)
                    {
                        if (num4 < dataByKey.astBufs[j].dwProbability)
                        {
                            inBufDropInfo = dataByKey.astBufs[j];
                            break;
                        }
                        num4 -= (int) dataByKey.astBufs[j].dwProbability;
                    }
                    DebugHelper.Assert(inBufDropInfo != null);
                    SimpleParabolaEffect inDropdownEffect = new SimpleParabolaEffect(this.MonsterRef.actor.location, this.TraceOnTerrain(this.MonsterRef.actor.location));
                    PickupBufEffect inPickupEffect = new PickupBufEffect(inBufDropInfo);
                    Singleton<DropItemMgr>.instance.CreateItem(Utility.UTF8Convert(inBufDropInfo.szPrefab), inDropdownEffect, inPickupEffect);
                }
            }
        }

        private VInt3 TraceOnTerrain(VInt3 InLocation)
        {
            RaycastHit hit;
            Ray ray = new Ray((Vector3) InLocation, new Vector3(0f, -1f, 0f));
            if (!Physics.Raycast(ray, out hit, float.PositiveInfinity, ((int) 1) << LayerMask.NameToLayer("Scene")))
            {
                return InLocation;
            }
            return new VInt3(hit.point);
        }
    }
}

