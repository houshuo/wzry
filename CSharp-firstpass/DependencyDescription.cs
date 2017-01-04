using System;
using System.Runtime.CompilerServices;

public class DependencyDescription
{
    [CompilerGenerated]
    private static Func<string, bool> <>f__am$cache2;
    protected string[] Dpendencies;

    public DependencyDescription(int InIndex, string InValue)
    {
        this.dependsIndex = InIndex;
        char[] separator = new char[] { '|' };
        if (<>f__am$cache2 == null)
        {
            <>f__am$cache2 = new Func<string, bool>(DependencyDescription.<DependencyDescription>m__4A);
        }
        this.Dpendencies = LinqS.Where(InValue.Split(separator), <>f__am$cache2);
    }

    [CompilerGenerated]
    private static bool <DependencyDescription>m__4A(string x)
    {
        return !string.IsNullOrEmpty(x.Trim());
    }

    public bool ShouldBackOff(string InTest)
    {
        if (this.Dpendencies != null)
        {
            for (int i = 0; i < this.Dpendencies.Length; i++)
            {
                if (this.Dpendencies[i].Equals(InTest, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int dependsIndex { get; protected set; }
}

