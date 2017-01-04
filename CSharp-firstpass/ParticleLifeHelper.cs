using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ParticleLifeHelper : MonoBehaviour, IPooledMonoBehaviour
{
    public DelegateOnPlayEnd m_delegatePlayEnd;
    public bool m_isValid;
    public ParticleSystem m_particleSys;
    public float m_yOffset;

    public static T GetComponentInChildren<T>(GameObject go) where T: Component
    {
        if (go != null)
        {
            T component = go.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            Transform transform = go.transform;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                component = GetComponentInChildren<T>(transform.GetChild(i).gameObject);
                if (component != null)
                {
                    return component;
                }
            }
        }
        return null;
    }

    public void OnCreate()
    {
        this.m_particleSys = base.GetComponentInChildren<ParticleSystem>();
    }

    public void OnGet()
    {
        this.m_isValid = true;
    }

    public void OnRecycle()
    {
        this.m_isValid = false;
        this.m_delegatePlayEnd = null;
    }

    public void Stop()
    {
        this.m_particleSys.Stop(true);
    }

    private void Update()
    {
        if (this.m_isValid)
        {
            bool isValid = this.m_isValid;
            this.m_isValid = ((this.m_particleSys != null) && !this.m_particleSys.isStopped) && this.m_particleSys.IsAlive();
            if ((!this.m_isValid && isValid) && (this.m_delegatePlayEnd != null))
            {
                this.m_delegatePlayEnd(base.gameObject);
                this.m_delegatePlayEnd = null;
            }
        }
    }

    public delegate void DelegateOnPlayEnd(GameObject go);
}

