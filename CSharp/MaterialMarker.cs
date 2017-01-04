using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MaterialMarker : TemplateMarkerBase
{
    public string m_namePattern;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        Renderer component = targetObject.GetComponent<Renderer>();
        if (null == component)
        {
            errorInfo = "没有Render组件";
            return false;
        }
        if (null == component.sharedMaterial)
        {
            errorInfo = "没有Material";
            return false;
        }
        if (!base.isWildCardMatch(component.sharedMaterial.shader.name, this.m_namePattern))
        {
            errorInfo = "Shader名称不符合规范";
            return false;
        }
        return true;
    }
}

