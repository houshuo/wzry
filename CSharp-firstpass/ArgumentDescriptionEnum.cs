using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[Argument(100)]
public class ArgumentDescriptionEnum : IArgumentDescription
{
    public bool Accept(Type InType)
    {
        return ((InType != null) && InType.IsEnum);
    }

    public bool AcceptAsMethodParameter(Type InType)
    {
        return InType.IsEnum;
    }

    public bool CheckConvert(string InArgument, Type InType, out string OutErrorMessage)
    {
        string str;
        DebugHelper.Assert((InArgument != null) && InType.IsEnum);
        OutErrorMessage = string.Empty;
        string[] names = Enum.GetNames(InType);
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Equals(InArgument, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }
        if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out str))
        {
            int num2 = System.Convert.ToInt32(InArgument);
            if (string.IsNullOrEmpty(Enum.GetName(InType, num2)))
            {
                OutErrorMessage = string.Format("不能将\"{0}\"转换到{1}的任何值.", InArgument, InType.Name);
            }
            return false;
        }
        OutErrorMessage = string.Format("不能将\"{0}\"转换为任何有效属性.", InArgument);
        return false;
    }

    public object Convert(string InArgument, Type InType)
    {
        string str;
        if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out str))
        {
            int num = System.Convert.ToInt32(InArgument);
            return Enum.ToObject(InType, num);
        }
        return Enum.Parse(InType, InArgument, true);
    }

    public List<string> FilteredCandinates(Type InType, string InArgument)
    {
        string str;
        if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out str))
        {
            int num = System.Convert.ToInt32(InArgument);
            string name = Enum.GetName(InType, num);
            return this.FilteredCandinatesInner(InType, name);
        }
        return this.FilteredCandinatesInner(InType, InArgument);
    }

    protected List<string> FilteredCandinatesInner(Type InType, string InArgument)
    {
        <FilteredCandinatesInner>c__AnonStorey40 storey = new <FilteredCandinatesInner>c__AnonStorey40 {
            InArgument = InArgument
        };
        List<string> candinates = this.GetCandinates(InType);
        if ((candinates != null) && (storey.InArgument != null))
        {
            candinates.RemoveAll(new Predicate<string>(storey.<>m__49));
        }
        return candinates;
    }

    public List<string> GetCandinates(Type InType)
    {
        string[] names = Enum.GetNames(InType);
        return ((names == null) ? null : LinqS.ToStringList(names));
    }

    public string GetValue(Type InType, string InArgument)
    {
        string str;
        DebugHelper.Assert(InArgument != null);
        string[] names = Enum.GetNames(InType);
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Equals(InArgument, StringComparison.CurrentCultureIgnoreCase))
            {
                return names[i];
            }
        }
        if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out str))
        {
            int num2 = System.Convert.ToInt32(InArgument);
            return Enum.GetName(InType, num2);
        }
        return string.Empty;
    }

    public static int StringToEnum(Type InType, string InText)
    {
        string str;
        if (ArgumentDescriptionDefault.CheckConvertUtil(InText, typeof(int), out str))
        {
            return System.Convert.ToInt32(InText);
        }
        return System.Convert.ToInt32(Enum.Parse(InType, InText, true));
    }

    [CompilerGenerated]
    private sealed class <FilteredCandinatesInner>c__AnonStorey40
    {
        internal string InArgument;

        internal bool <>m__49(string x)
        {
            return !x.StartsWith(this.InArgument, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}

