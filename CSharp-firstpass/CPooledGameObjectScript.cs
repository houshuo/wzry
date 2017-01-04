using System;
using UnityEngine;

public class CPooledGameObjectScript : MonoBehaviour
{
    private IPooledMonoBehaviour[] m_cachedIPooledMonos;
    public Vector3 m_defaultScale;
    private bool m_inUse;
    public bool m_isInit;
    public string m_prefabKey;

    public void AddCachedMono(MonoBehaviour mono, bool defaultEnabled)
    {
        if ((mono != null) && (mono is IPooledMonoBehaviour))
        {
            IPooledMonoBehaviour[] behaviourArray = new IPooledMonoBehaviour[this.m_cachedIPooledMonos.Length + 1];
            for (int i = 0; i < this.m_cachedIPooledMonos.Length; i++)
            {
                behaviourArray[i] = this.m_cachedIPooledMonos[i];
            }
            behaviourArray[this.m_cachedIPooledMonos.Length] = mono as IPooledMonoBehaviour;
            this.m_cachedIPooledMonos = behaviourArray;
        }
    }

    public void Initialize(string prefabKey)
    {
        MonoBehaviour[] componentsInChildren = base.gameObject.GetComponentsInChildren<MonoBehaviour>(true);
        if ((componentsInChildren != null) && (componentsInChildren.Length > 0))
        {
            int num = 0;
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i] is IPooledMonoBehaviour)
                {
                    num++;
                }
            }
            this.m_cachedIPooledMonos = new IPooledMonoBehaviour[num];
            int index = 0;
            for (int j = 0; j < componentsInChildren.Length; j++)
            {
                if (componentsInChildren[j] is IPooledMonoBehaviour)
                {
                    this.m_cachedIPooledMonos[index] = componentsInChildren[j] as IPooledMonoBehaviour;
                    index++;
                }
            }
        }
        else
        {
            this.m_cachedIPooledMonos = new IPooledMonoBehaviour[0];
        }
        this.m_prefabKey = prefabKey;
        this.m_defaultScale = base.gameObject.transform.localScale;
        this.m_isInit = true;
        this.m_inUse = false;
    }

    public void OnCreate()
    {
        if ((this.m_cachedIPooledMonos != null) && (this.m_cachedIPooledMonos.Length > 0))
        {
            for (int i = 0; i < this.m_cachedIPooledMonos.Length; i++)
            {
                if (this.m_cachedIPooledMonos[i] != null)
                {
                    this.m_cachedIPooledMonos[i].OnCreate();
                }
            }
        }
    }

    public void OnGet()
    {
        if (!base.gameObject.activeSelf)
        {
            base.gameObject.SetActive(true);
        }
        if ((this.m_cachedIPooledMonos != null) && (this.m_cachedIPooledMonos.Length > 0))
        {
            for (int i = 0; i < this.m_cachedIPooledMonos.Length; i++)
            {
                if (this.m_cachedIPooledMonos[i] != null)
                {
                    this.m_cachedIPooledMonos[i].OnGet();
                }
            }
        }
        this.m_inUse = true;
    }

    public void OnPrepare()
    {
        base.gameObject.SetActive(false);
    }

    public void OnRecycle()
    {
        if ((this.m_cachedIPooledMonos != null) && (this.m_cachedIPooledMonos.Length > 0))
        {
            for (int i = 0; i < this.m_cachedIPooledMonos.Length; i++)
            {
                if (this.m_cachedIPooledMonos[i] != null)
                {
                    this.m_cachedIPooledMonos[i].OnRecycle();
                }
            }
        }
        base.gameObject.SetActive(false);
        this.m_inUse = false;
    }
}

