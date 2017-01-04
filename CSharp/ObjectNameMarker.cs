using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ObjectNameMarker : TemplateMarkerBase
{
    public string m_namePattern;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        string name = targetObject.name;
        bool flag = base.isWildCardMatch(name, this.m_namePattern);
        if (!flag)
        {
            errorInfo = "命名规范错误，要求：" + this.m_namePattern + "， 实际：" + name;
        }
        return flag;
    }
}

