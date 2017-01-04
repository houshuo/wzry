namespace Apollo
{
    using System;

    public class BasicClassTypeUtil
    {
        public static object CreateListItem(Type typeList)
        {
            Type[] genericArguments = typeList.GetGenericArguments();
            if ((genericArguments != null) && (genericArguments.Length != 0))
            {
                return CreateObject(genericArguments[0]);
            }
            return null;
        }

        public static object CreateObject<T>()
        {
            return CreateObject(typeof(T));
        }

        public static object CreateObject(Type type)
        {
            object obj2;
            try
            {
                if (type.ToString() == "System.String")
                {
                    return string.Empty;
                }
                obj2 = Activator.CreateInstance(type);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return obj2;
        }
    }
}

