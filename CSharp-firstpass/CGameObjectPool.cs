using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class CGameObjectPool : Singleton<CGameObjectPool>
{
    private bool m_clearPooledObjects;
    private int m_clearPooledObjectsExecuteFrame;
    private LinkedList<stDelayRecycle> m_delayRecycle = new LinkedList<stDelayRecycle>();
    private DictionaryView<string, Queue<CPooledGameObjectScript>> m_pooledGameObjectMap = new DictionaryView<string, Queue<CPooledGameObjectScript>>();
    private GameObject m_poolRoot;
    private static int s_frameCounter;

    private void _RecycleGameObject(GameObject pooledGameObject, bool setIsInit)
    {
        if (pooledGameObject != null)
        {
            CPooledGameObjectScript component = pooledGameObject.GetComponent<CPooledGameObjectScript>();
            if (component != null)
            {
                Queue<CPooledGameObjectScript> queue = null;
                if (this.m_pooledGameObjectMap.TryGetValue(component.m_prefabKey, out queue))
                {
                    queue.Enqueue(component);
                    component.OnRecycle();
                    component.gameObject.transform.SetParent(this.m_poolRoot.transform, true);
                    component.m_isInit = setIsInit;
                    return;
                }
            }
            UnityEngine.Object.Destroy(pooledGameObject);
        }
    }

    public void ClearPooledObjects()
    {
        this.m_clearPooledObjects = true;
        this.m_clearPooledObjectsExecuteFrame = s_frameCounter + 1;
    }

    private CPooledGameObjectScript CreateGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, string prefabKey)
    {
        CPooledGameObjectScript component = null;
        bool needCached = resourceType == enResourceType.BattleScene;
        GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(prefabFullPath, typeof(GameObject), resourceType, needCached, false).m_content as GameObject;
        if (content == null)
        {
            return null;
        }
        GameObject obj3 = null;
        if (useRotation)
        {
            obj3 = UnityEngine.Object.Instantiate(content, pos, rot) as GameObject;
        }
        else
        {
            obj3 = UnityEngine.Object.Instantiate(content) as GameObject;
            obj3.transform.position = pos;
        }
        DebugHelper.Assert(obj3 != null);
        component = obj3.GetComponent<CPooledGameObjectScript>();
        if (component == null)
        {
            component = obj3.AddComponent<CPooledGameObjectScript>();
        }
        component.Initialize(prefabKey);
        component.OnCreate();
        return component;
    }

    public void ExecuteClearPooledObjects()
    {
        for (LinkedListNode<stDelayRecycle> node = this.m_delayRecycle.First; node != null; node = node.Next)
        {
            if (null != node.Value.recycleObj)
            {
                this.RecycleGameObject(node.Value.recycleObj);
            }
        }
        this.m_delayRecycle.Clear();
        DictionaryView<string, Queue<CPooledGameObjectScript>>.Enumerator enumerator = this.m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<CPooledGameObjectScript>> current = enumerator.Current;
            Queue<CPooledGameObjectScript> queue = current.Value;
            while (queue.Count > 0)
            {
                CPooledGameObjectScript script = queue.Dequeue();
                if ((script != null) && (script.gameObject != null))
                {
                    UnityEngine.Object.Destroy(script.gameObject);
                }
            }
        }
        this.m_pooledGameObjectMap.Clear();
    }

    public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType)
    {
        bool isInit = false;
        return this.GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType, out bool isInit)
    {
        return this.GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType)
    {
        bool isInit = false;
        return this.GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType, out bool isInit)
    {
        return this.GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType)
    {
        bool isInit = false;
        return this.GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, out bool isInit)
    {
        return this.GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit);
    }

    private GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, out bool isInit)
    {
        string key = CFileManager.EraseExtension(prefabFullPath).ToLower();
        Queue<CPooledGameObjectScript> queue = null;
        if (!this.m_pooledGameObjectMap.TryGetValue(key, out queue))
        {
            queue = new Queue<CPooledGameObjectScript>();
            this.m_pooledGameObjectMap.Add(key, queue);
        }
        CPooledGameObjectScript script = null;
        while (queue.Count > 0)
        {
            script = queue.Dequeue();
            if ((script != null) && (script.gameObject != null))
            {
                script.gameObject.transform.SetParent(null, true);
                script.gameObject.transform.position = pos;
                script.gameObject.transform.rotation = rot;
                script.gameObject.transform.localScale = script.m_defaultScale;
                break;
            }
            script = null;
        }
        if (script == null)
        {
            script = this.CreateGameObject(prefabFullPath, pos, rot, useRotation, resourceType, key);
        }
        if (script == null)
        {
            isInit = false;
            return null;
        }
        isInit = script.m_isInit;
        script.OnGet();
        return script.gameObject;
    }

    public override void Init()
    {
        this.m_poolRoot = new GameObject("CGameObjectPool");
        GameObject obj2 = GameObject.Find("BootObj");
        if (obj2 != null)
        {
            this.m_poolRoot.transform.SetParent(obj2.transform);
        }
    }

    public void PrepareGameObject(string prefabFullPath, enResourceType resourceType, int amount)
    {
        string key = CFileManager.EraseExtension(prefabFullPath).ToLower();
        Queue<CPooledGameObjectScript> queue = null;
        if (!this.m_pooledGameObjectMap.TryGetValue(key, out queue))
        {
            queue = new Queue<CPooledGameObjectScript>();
            this.m_pooledGameObjectMap.Add(key, queue);
        }
        if (queue.Count < amount)
        {
            amount -= queue.Count;
            for (int i = 0; i < amount; i++)
            {
                CPooledGameObjectScript item = this.CreateGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, key);
                object[] inParameters = new object[] { prefabFullPath };
                DebugHelper.Assert(item != null, "Failed Create Game object from \"{0}\"", inParameters);
                if (item != null)
                {
                    queue.Enqueue(item);
                    item.gameObject.transform.SetParent(this.m_poolRoot.transform, true);
                    item.OnPrepare();
                }
            }
        }
    }

    public void RecycleGameObject(GameObject pooledGameObject)
    {
        this._RecycleGameObject(pooledGameObject, false);
    }

    public void RecycleGameObjectDelay(GameObject pooledGameObject, int delayMillSeconds, OnDelayRecycleDelegate callback = null)
    {
        stDelayRecycle recycle = new stDelayRecycle {
            recycleObj = pooledGameObject,
            timeMillSecondsLeft = delayMillSeconds,
            callback = callback
        };
        this.m_delayRecycle.AddLast(recycle);
    }

    public void RecyclePreparedGameObject(GameObject pooledGameObject)
    {
        this._RecycleGameObject(pooledGameObject, true);
    }

    public override void UnInit()
    {
    }

    public void Update()
    {
        s_frameCounter++;
        this.UpdateDelayRecycle();
        if (this.m_clearPooledObjects && (this.m_clearPooledObjectsExecuteFrame == s_frameCounter))
        {
            this.ExecuteClearPooledObjects();
            this.m_clearPooledObjects = false;
        }
    }

    private void UpdateDelayRecycle()
    {
        int num = (int) (1000f * Time.deltaTime);
        LinkedListNode<stDelayRecycle> first = this.m_delayRecycle.First;
        while (first != null)
        {
            LinkedListNode<stDelayRecycle> node = first;
            first = node.Next;
            if (null == node.Value.recycleObj)
            {
                this.m_delayRecycle.Remove(node);
            }
            else
            {
                stDelayRecycle local1 = node.Value;
                local1.timeMillSecondsLeft -= num;
                if (node.Value.timeMillSecondsLeft <= 0)
                {
                    if (node.Value.callback != null)
                    {
                        node.Value.callback(node.Value.recycleObj);
                    }
                    this.RecycleGameObject(node.Value.recycleObj);
                    this.m_delayRecycle.Remove(node);
                }
            }
        }
    }

    public void UpdateParticleChecker(int maxNum)
    {
    }

    public delegate void OnDelayRecycleDelegate(GameObject recycleObj);

    private class stDelayRecycle
    {
        public CGameObjectPool.OnDelayRecycleDelegate callback;
        public GameObject recycleObj;
        public int timeMillSecondsLeft;
    }
}

