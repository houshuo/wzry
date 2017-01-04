namespace Apollo
{
    using System;

    internal class ApolloHttpMethod
    {
        public const string GET = "GET";
        public const string HEAD = "HEAD";
        public const string POST = "POST";

        public static bool Valied(string method)
        {
            if ((!(method == "GET") && !(method == "POST")) && !(method == "HEAD"))
            {
                return false;
            }
            return true;
        }
    }
}

