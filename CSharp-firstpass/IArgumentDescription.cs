using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public interface IArgumentDescription
{
    bool Accept(Type InType);
    bool AcceptAsMethodParameter(Type InType);
    bool CheckConvert(string InArgument, Type InType, out string OutErrorMessage);
    object Convert(string InArgument, Type InType);
    List<string> FilteredCandinates(Type InType, string InArgument);
    List<string> GetCandinates(Type InType);
    string GetValue(Type InType, string InArgument);
}

