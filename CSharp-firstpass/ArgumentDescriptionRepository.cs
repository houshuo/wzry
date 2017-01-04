using System;
using System.Collections.Generic;

public class ArgumentDescriptionRepository : Singleton<ArgumentDescriptionRepository>
{
    public SortedList<int, IArgumentDescription> Descriptions = new SortedList<int, IArgumentDescription>(new Greater());

    public ArgumentDescriptionRepository()
    {
        ClassEnumerator enumerator = new ClassEnumerator(typeof(ArgumentAttribute), typeof(IArgumentDescription), typeof(ArgumentAttribute).Assembly, true, false, false);
        ListView<Type>.Enumerator enumerator2 = enumerator.results.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Type current = enumerator2.Current;
            ArgumentAttribute attribute = current.GetCustomAttributes(typeof(ArgumentAttribute), false)[0] as ArgumentAttribute;
            IArgumentDescription description = Activator.CreateInstance(current) as IArgumentDescription;
            this.Descriptions.Add(attribute.order, description);
        }
    }

    public IArgumentDescription GetDescription(Type InType)
    {
        IEnumerator<KeyValuePair<int, IArgumentDescription>> enumerator = this.Descriptions.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<int, IArgumentDescription> current = enumerator.Current;
            if (current.Value.Accept(InType))
            {
                KeyValuePair<int, IArgumentDescription> pair2 = enumerator.Current;
                return pair2.Value;
            }
        }
        object[] inParameters = new object[] { InType.Name };
        DebugHelper.Assert(false, "can't find valid description for {0}, internal error!", inParameters);
        return null;
    }

    private class Greater : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return ((x <= y) ? ((x != y) ? 1 : 0) : -1);
        }
    }
}

