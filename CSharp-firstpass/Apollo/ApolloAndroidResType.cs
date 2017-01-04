namespace Apollo
{
    using System;

    internal class ApolloAndroidResType
    {
        public const string Drawable = "drawable";
        public const string ID = "id";
        public const string Layout = "layout";
        public const string Menu = "menu";
        public const string Str = "string";
        public const string Style = "style";

        public static bool Valied(string typeName)
        {
            if (((!(typeName == "id") && !(typeName == "layout")) && (!(typeName == "drawable") && !(typeName == "menu"))) && (!(typeName == "string") && !(typeName == "style")))
            {
                return false;
            }
            return true;
        }
    }
}

