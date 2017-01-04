using System;
using System.Collections.Generic;
using UnityEngine;

public class Canvas3D : MonoBehaviour
{
    private Dictionary<int, Sprite3D> m_childSprites;
    private int m_depth;

    public void LateUpdate()
    {
        this.m_depth = 0;
        this.RefreshLayout();
    }

    private void RefreshHierachy(Transform root)
    {
        if (root.gameObject.activeSelf)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                this.RefreshHierachy(root.GetChild(i));
            }
            Sprite3D component = null;
            if (this.m_childSprites.TryGetValue(root.GetInstanceID(), out component))
            {
                if (null != component)
                {
                    this.m_depth++;
                    component.depth = this.m_depth;
                }
            }
            else
            {
                component = root.GetComponent<Sprite3D>();
                this.m_childSprites.Add(root.GetInstanceID(), component);
                if (null != component)
                {
                    this.m_depth++;
                    component.depth = this.m_depth;
                }
            }
        }
    }

    public void RefreshLayout()
    {
        if (this.m_childSprites == null)
        {
            this.m_childSprites = new Dictionary<int, Sprite3D>();
        }
        this.RefreshHierachy(base.transform);
    }
}

