using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MeshMarker : TemplateMarkerBase
{
    public string m_namePattern;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        MeshFilter component = targetObject.GetComponent<MeshFilter>();
        SkinnedMeshRenderer renderer = targetObject.GetComponent<SkinnedMeshRenderer>();
        if ((null == component) && (renderer == null))
        {
            errorInfo = "没有MeshFilter组件或者SkinnedMeshRender组件";
            return false;
        }
        Mesh sharedMesh = null;
        if (null != component)
        {
            sharedMesh = component.sharedMesh;
        }
        else
        {
            sharedMesh = renderer.sharedMesh;
        }
        if (null == sharedMesh)
        {
            errorInfo = "没有Mesh";
            return false;
        }
        if (!base.isWildCardMatch(sharedMesh.name, this.m_namePattern))
        {
            errorInfo = "Mesh不符合规范";
            return false;
        }
        return true;
    }
}

