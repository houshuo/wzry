using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ComponentMarker : TemplateMarkerBase
{
    public Component[] m_components;
    private eComponentMarkerType m_type;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        bool flag = false;
        if (this.m_type == eComponentMarkerType.And)
        {
            flag = true;
        }
        foreach (Component component in this.m_components)
        {
            if (component != null)
            {
                bool flag2 = (bool) targetObject.GetComponent(component.GetType());
                if (!flag2 && (this.m_type == eComponentMarkerType.And))
                {
                    flag = false;
                    break;
                }
                if (flag2 && (this.m_type == eComponentMarkerType.Or))
                {
                    flag = true;
                    break;
                }
            }
        }
        if (!flag)
        {
            errorInfo = "不符合组件要求规范";
        }
        return flag;
    }

    public enum eComponentMarkerType
    {
        And,
        Or
    }
}

