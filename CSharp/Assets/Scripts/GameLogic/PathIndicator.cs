namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections;
    using UnityEngine;

    public class PathIndicator : MonoBehaviour
    {
        private PoolObjHandle<ActorRoot> DestActorHandle;
        public GameObject DestObj;
        public Vector3 DestPosition = new Vector3(0f, 0f, 0f);
        private bool m_bWorking;
        private int m_parCacheIndex = -1;
        private ArrayList m_parCachePool = new ArrayList();
        private const int ParNumMax = 0x19;
        private const float ParSpacing = 1.15f;
        private const string ResourceName = "Prefab_Skill_Effects/tongyong_effects/UI_fx/yidong_ui_blue01.prefab";
        private PoolObjHandle<ActorRoot> SrcActorHandle;
        public GameObject SrcObj;

        private void Clear()
        {
            for (int i = 0; i < this.m_parCachePool.Count; i++)
            {
                GameObject pooledGameObject = this.m_parCachePool[i] as GameObject;
                if (pooledGameObject != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(pooledGameObject);
                }
            }
            this.m_parCachePool.Clear();
            this.m_parCacheIndex = -1;
        }

        private void GetParFromCache(int inNeedNum)
        {
            int num = this.m_parCacheIndex + inNeedNum;
            if (num < this.m_parCachePool.Count)
            {
                for (int i = this.m_parCacheIndex + 1; i <= num; i++)
                {
                    GameObject obj2 = this.m_parCachePool[i] as GameObject;
                    if (obj2 != null)
                    {
                        obj2.CustomSetActive(true);
                    }
                }
            }
            else
            {
                while (num >= this.m_parCachePool.Count)
                {
                    for (int j = 0; j < 0x19; j++)
                    {
                        bool isInit = false;
                        GameObject obj3 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/UI_fx/yidong_ui_blue01.prefab", true, SceneObjType.Temp, Vector3.zero, Quaternion.identity, out isInit);
                        if (obj3 != null)
                        {
                            int num4 = this.m_parCachePool.Add(obj3);
                            if ((num4 >= (this.m_parCacheIndex + 1)) && (num4 <= num))
                            {
                                obj3.CustomSetActive(true);
                            }
                            else
                            {
                                obj3.CustomSetActive(false);
                            }
                        }
                    }
                }
            }
            this.m_parCacheIndex = num;
        }

        public void Play(GameObject inSrc, GameObject inDest, ref Vector3 inDestPos)
        {
            this.SrcObj = inSrc;
            this.DestObj = inDest;
            this.DestPosition = inDestPos;
            this.m_bWorking = true;
            if (inSrc != null)
            {
                ActorConfig component = inSrc.GetComponent<ActorConfig>();
                if (component != null)
                {
                    this.SrcActorHandle = component.GetActorHandle();
                }
            }
            if (inDest != null)
            {
                ActorConfig config2 = inDest.GetComponent<ActorConfig>();
                if (config2 != null)
                {
                    this.DestActorHandle = config2.GetActorHandle();
                }
            }
        }

        private void ReturnToParCache(int inReturnNum)
        {
            int num = this.m_parCacheIndex - inReturnNum;
            num = Math.Max(num, -1);
            for (int i = num + 1; i <= this.m_parCacheIndex; i++)
            {
                GameObject obj2 = this.m_parCachePool[i] as GameObject;
                if (obj2 != null)
                {
                    obj2.CustomSetActive(false);
                }
            }
            this.m_parCacheIndex = num;
            int num3 = this.m_parCachePool.Count - this.m_parCacheIndex;
            if (num3 >= 50)
            {
                int num4 = this.m_parCachePool.Count - 1;
                int num5 = this.m_parCachePool.Count - 50;
                for (int j = num4; j > num5; j--)
                {
                    GameObject pooledGameObject = this.m_parCachePool[j] as GameObject;
                    if (pooledGameObject != null)
                    {
                        DebugHelper.Assert(!pooledGameObject.activeInHierarchy);
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(pooledGameObject);
                    }
                    this.m_parCachePool.RemoveAt(j);
                }
            }
        }

        public void Stop()
        {
            this.Clear();
            this.m_bWorking = false;
        }

        private void Update()
        {
            if (this.m_bWorking && (this.SrcObj != null))
            {
                Vector3 zero = Vector3.zero;
                if (this.SrcActorHandle != 0)
                {
                    zero = (Vector3) this.SrcActorHandle.handle.location;
                }
                else if (this.SrcObj != null)
                {
                    zero = this.SrcObj.transform.position;
                }
                Vector3 destPosition = this.DestPosition;
                if (this.DestActorHandle != 0)
                {
                    destPosition = (Vector3) this.DestActorHandle.handle.location;
                }
                else if (this.DestObj != null)
                {
                    destPosition = this.DestObj.transform.position;
                }
                Vector3 vector5 = destPosition - zero;
                int num = (int) (Mathf.Sqrt(vector5.sqrMagnitude) / 1.15f);
                int num2 = this.m_parCacheIndex + 1;
                if (num > num2)
                {
                    this.GetParFromCache(num - num2);
                }
                else if (num < num2)
                {
                    this.ReturnToParCache(num2 - num);
                }
                DebugHelper.Assert(num == (this.m_parCacheIndex + 1));
                if (num > 0)
                {
                    Vector3 vector6 = destPosition - zero;
                    Vector3 normalized = vector6.normalized;
                    bool flag = true;
                    for (int i = 0; i <= this.m_parCacheIndex; i++)
                    {
                        GameObject obj2 = this.m_parCachePool[i] as GameObject;
                        if (obj2 != null)
                        {
                            obj2.transform.forward = normalized;
                            Vector3 one = Vector3.one;
                            if (flag)
                            {
                                one = destPosition + ((Vector3) (((i + 1) * 1.15f) * -normalized));
                            }
                            else
                            {
                                one = zero + ((Vector3) (((i + 1) * 1.15f) * normalized));
                            }
                            one.y = 0.1f;
                            obj2.transform.position = one;
                        }
                    }
                }
            }
        }
    }
}

