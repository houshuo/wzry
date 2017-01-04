using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class ClassEnumerator
{
    private System.Type AttributeType;
    private System.Type InterfaceType;
    protected ListView<System.Type> Results = new ListView<System.Type>();

    public ClassEnumerator(System.Type InAttributeType, System.Type InInterfaceType, Assembly InAssembly, bool bIgnoreAbstract = true, bool bInheritAttribute = false, bool bShouldCrossAssembly = false)
    {
        this.AttributeType = InAttributeType;
        this.InterfaceType = InInterfaceType;
        try
        {
            if (bShouldCrossAssembly)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                if (assemblies != null)
                {
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        Assembly inAssembly = assemblies[i];
                        this.CheckInAssembly(inAssembly, bIgnoreAbstract, bInheritAttribute);
                    }
                }
            }
            else
            {
                this.CheckInAssembly(InAssembly, bIgnoreAbstract, bInheritAttribute);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Error in enumerate classes :" + exception.Message);
        }
    }

    protected void CheckInAssembly(Assembly InAssembly, bool bInIgnoreAbstract, bool bInInheritAttribute)
    {
        System.Type[] types = InAssembly.GetTypes();
        if (types != null)
        {
            for (int i = 0; i < types.Length; i++)
            {
                System.Type c = types[i];
                if ((((this.InterfaceType == null) || this.InterfaceType.IsAssignableFrom(c)) && (!bInIgnoreAbstract || (bInIgnoreAbstract && !c.IsAbstract))) && (c.GetCustomAttributes(this.AttributeType, bInInheritAttribute).Length > 0))
                {
                    this.Results.Add(c);
                }
            }
        }
    }

    public ListView<System.Type> results
    {
        get
        {
            return this.Results;
        }
    }
}

