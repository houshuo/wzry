using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

public abstract class CheatCommandBase : ICheatCommand
{
    private string[] Arguments;
    public static readonly string Done = "done";

    public virtual bool CheckArguments(string[] InArguments, out string OutMessage)
    {
        if ((this.argumentsTypes != null) && (this.argumentsTypes.Length > 0))
        {
            for (int i = 0; i < this.argumentsTypes.Length; i++)
            {
                ArgumentDescriptionAttribute inArugmentDescription = this.argumentsTypes[i];
                if ((InArguments == null) || (i >= InArguments.Length))
                {
                    if (!inArugmentDescription.isOptional)
                    {
                        OutMessage = string.Format("无法执行命令，因为缺少参数<{0}>, 类型为:<{1}>", inArugmentDescription.name, inArugmentDescription.argumentType.Name);
                        return false;
                    }
                    DependencyDescription[] depends = inArugmentDescription.depends;
                    if ((depends != null) && !this.CheckDependencies(inArugmentDescription, depends, InArguments, out OutMessage))
                    {
                        return false;
                    }
                }
                else if (!inArugmentDescription.isOptional)
                {
                    string str;
                    if (!TypeCastCheck(InArguments[i], this.argumentsTypes[i], out str))
                    {
                        object[] args = new object[] { InArguments[i], this.argumentsTypes[i].argumentType.Name, this.GetArgumentNameAt(i), str };
                        OutMessage = string.Format("无法执行命令，因为参数[{2}]=\"{0}\"无法转换到{1}, 错误信息:{3}", args);
                        return false;
                    }
                }
                else
                {
                    string str2;
                    DependencyDescription[] inDependencies = inArugmentDescription.depends;
                    if (((inDependencies != null) && !this.CheckDependencies(inArugmentDescription, inDependencies, InArguments, out OutMessage)) && !TypeCastCheck(InArguments[i], this.argumentsTypes[i], out str2))
                    {
                        object[] objArray2 = new object[] { InArguments[i], this.argumentsTypes[i].argumentType.Name, this.GetArgumentNameAt(i), str2 };
                        OutMessage = string.Format("无法执行命令，因为参数[{2}]=\"{0}\"无法转换到{1}, 错误信息:{3}", objArray2);
                        return false;
                    }
                }
            }
        }
        OutMessage = string.Empty;
        return true;
    }

    protected virtual bool CheckDependencies(ArgumentDescriptionAttribute InArugmentDescription, DependencyDescription[] InDependencies, string[] InArguments, out string OutMessage)
    {
        OutMessage = string.Empty;
        if (InArguments == null)
        {
            OutMessage = "缺少所有必要参数";
            return false;
        }
        for (int i = 0; i < InDependencies.Length; i++)
        {
            DependencyDescription description = InDependencies[i];
            DebugHelper.Assert((description.dependsIndex >= 0) && (description.dependsIndex < this.argumentsTypes.Length), "maybe internal error, can't find depend argument description.");
            if ((description.dependsIndex < 0) || (description.dependsIndex >= this.argumentsTypes.Length))
            {
                OutMessage = "maybe internal error, can't find depend argument description.";
                return false;
            }
            DebugHelper.Assert(description.dependsIndex < InArguments.Length);
            string inArgument = InArguments[description.dependsIndex];
            Type argumentType = this.argumentsTypes[description.dependsIndex].argumentType;
            IArgumentDescription description2 = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(argumentType);
            DebugHelper.Assert(description2 != null);
            inArgument = description2.GetValue(argumentType, inArgument);
            if (description.ShouldBackOff(inArgument))
            {
                OutMessage = string.Format("您必须提供参数<{2}>, 因为参数<{0}>=\"{1}\"", this.argumentsTypes[description.dependsIndex].name, inArgument, InArugmentDescription.name);
                return false;
            }
        }
        return true;
    }

    protected abstract string Execute(string[] InArguments);
    private string GetArgumentNameAt(int Index)
    {
        DebugHelper.Assert((Index >= 0) && (Index < this.argumentsTypes.Length));
        return this.argumentsTypes[Index].name;
    }

    public static T SmartConvert<T>(string InArgument)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null)
        {
            DebugHelper.Assert(converter.CanConvertFrom(typeof(string)));
            return (T) converter.ConvertFrom(InArgument);
        }
        return default(T);
    }

    public virtual string StartProcess(string[] InArguments)
    {
        string str;
        if (!this.CheckArguments(InArguments, out str))
        {
            return str;
        }
        return this.Execute(InArguments);
    }

    public static int StringToEnum(string InTest, Type InEnumType)
    {
        return ArgumentDescriptionEnum.StringToEnum(InEnumType, InTest);
    }

    public static bool TypeCastCheck(string InArgument, ArgumentDescriptionAttribute InArgDescription, out string OutErrorMessage)
    {
        DebugHelper.Assert(InArgDescription != null);
        return TypeCastCheck(InArgument, InArgDescription.argumentType, out OutErrorMessage);
    }

    public static bool TypeCastCheck(string InArgument, Type InType, out string OutErrorMessage)
    {
        IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(InType);
        DebugHelper.Assert(description != null);
        return description.CheckConvert(InArgument, InType, out OutErrorMessage);
    }

    protected void ValidateArgumentsBuffer()
    {
        if ((this.argumentsTypes != null) && (this.argumentsTypes.Length > 0))
        {
            this.Arguments = new string[this.argumentsTypes.Length];
            for (int i = 0; i < this.Arguments.Length; i++)
            {
                this.Arguments[i] = this.argumentsTypes[i].defaultValue;
            }
        }
    }

    public virtual string[] arguments
    {
        get
        {
            return this.Arguments;
        }
    }

    public abstract ArgumentDescriptionAttribute[] argumentsTypes { get; }

    public abstract CheatCommandName command { get; }

    public abstract string comment { get; }

    public virtual string description
    {
        get
        {
            string str = string.Empty;
            if ((this.argumentsTypes != null) && (this.argumentsTypes.Length > 0))
            {
                for (int i = 0; i < this.argumentsTypes.Length; i++)
                {
                    Type argumentType = this.argumentsTypes[i].argumentType;
                    str = str + string.Format(" <{0}{2}|{1}>", this.argumentsTypes[i].name, argumentType.Name, !this.argumentsTypes[i].isOptional ? string.Empty : "(Optional)");
                }
            }
            return string.Format("{0}{1}", this.command.baseName, str);
        }
    }

    public virtual string fullyHelper
    {
        get
        {
            return string.Format("{0} 描述: {1}", this.description, this.comment);
        }
    }

    public virtual bool isHiddenInMobile
    {
        get
        {
            return false;
        }
    }

    public virtual bool isSupportInEditor
    {
        get
        {
            return true;
        }
    }

    public abstract int messageID { get; }
}

