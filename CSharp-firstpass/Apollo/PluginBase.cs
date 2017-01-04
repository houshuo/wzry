namespace Apollo
{
    using System;

    public abstract class PluginBase : ApolloObject
    {
        protected PluginBase()
        {
            PluginManager.Instance.Add(this);
        }

        public virtual ApolloActionBufferBase CreatePayResponseAction(int action)
        {
            return null;
        }

        public virtual ApolloBufferBase CreatePayResponseInfo(int action)
        {
            return null;
        }

        public virtual IApolloExtendPayServiceBase GetPayExtendService()
        {
            return null;
        }

        public abstract string GetPluginName();
        public virtual IApolloServiceBase GetService(int serviceType)
        {
            return null;
        }

        public abstract bool Install();
    }
}

