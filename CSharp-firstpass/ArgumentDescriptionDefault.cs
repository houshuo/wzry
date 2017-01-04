using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[Argument(-1)]
public class ArgumentDescriptionDefault : IArgumentDescription
{
    public bool Accept(Type InType)
    {
        return true;
    }

    public bool AcceptAsMethodParameter(Type InType)
    {
        return ((InType == typeof(string)) || InType.IsValueType);
    }

    public bool CheckConvert(string InArgument, Type InType, out string OutErrorMessage)
    {
        return CheckConvertUtil(InArgument, InType, out OutErrorMessage);
    }

    public static bool CheckConvertUtil(string InArgument, Type InType, out string OutErrorMessage)
    {
        try
        {
            System.Convert.ChangeType(InArgument, InType);
            OutErrorMessage = string.Empty;
            return true;
        }
        catch (Exception exception)
        {
            OutErrorMessage = exception.Message;
            return false;
        }
    }

    public object Convert(string InArgument, Type InType)
    {
        try
        {
            return System.Convert.ChangeType(InArgument, InType);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public List<string> FilteredCandinates(Type InType, string InArgument)
    {
        return null;
    }

    public List<string> GetCandinates(Type InType)
    {
        return null;
    }

    public string GetValue(Type InType, string InArgument)
    {
        DebugHelper.Assert(InArgument != null);
        return InArgument;
    }
}

