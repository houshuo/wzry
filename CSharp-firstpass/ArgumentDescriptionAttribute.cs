using System;
using System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
public class ArgumentDescriptionAttribute : Attribute
{
    public ArgumentDescriptionAttribute(Type InArgumentType, string InName, params object[] InDependencies) : this(0, InArgumentType, InName, InDependencies)
    {
        this.defaultValue = string.Empty;
    }

    public ArgumentDescriptionAttribute(int Index, Type InArgumentType, string InName, params object[] InDependencies)
    {
        this.index = Index;
        this.argumentType = InArgumentType;
        this.name = InName;
        this.defaultValue = string.Empty;
        this.commandLineType = !this.argumentType.IsEnum ? this.argumentType : typeof(string);
        if ((InDependencies != null) && (InDependencies.Length >= 2))
        {
            DebugHelper.Assert((InDependencies.Length % 2) == 0);
            int num = InDependencies.Length >> 1;
            this.depends = new DependencyDescription[num];
            for (int i = 0; i < num; i++)
            {
                this.depends[i] = new DependencyDescription(Convert.ToInt32(InDependencies[i << 1]), Convert.ToString(InDependencies[(i << 1) + 1]));
            }
        }
    }

    public ArgumentDescriptionAttribute(EDefaultValueTag InTag, int Index, Type InArgumentType, string InName, string InDefaultValue, params object[] InDependencies) : this(Index, InArgumentType, InName, InDependencies)
    {
        this.defaultValue = InDefaultValue;
    }

    public void ValidateDependencies(int MaxIndex)
    {
        if (this.depends != null)
        {
            for (int i = 0; i < this.depends.Length; i++)
            {
                DebugHelper.Assert(this.depends[i].dependsIndex <= MaxIndex, "Invalid Dependencies!");
            }
        }
    }

    public Type argumentType { get; protected set; }

    public Type commandLineType { get; protected set; }

    public string defaultValue { get; protected set; }

    public DependencyDescription[] depends { get; protected set; }

    public int index { get; protected set; }

    public bool isEnum
    {
        get
        {
            return this.argumentType.IsEnum;
        }
    }

    public bool isOptional
    {
        get
        {
            return ((this.depends != null) && (this.depends.Length > 0));
        }
    }

    public string name { get; protected set; }

    public enum EDefaultValueTag
    {
        Tag
    }
}

