namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class ApolloRouteInfoBase : ApolloBufferBase
    {
        protected ApolloRouteInfoBase(ApolloRouteType routeType)
        {
            this.RouteType = routeType;
        }

        public ApolloRouteInfoBase CopyInstance()
        {
            ApolloRouteInfoBase base2 = this.onCopyInstance();
            if (base2 != null)
            {
                base2.RouteType = this.RouteType;
            }
            return base2;
        }

        protected abstract ApolloRouteInfoBase onCopyInstance();
        public override void ReadFrom(ApolloBufferReader reader)
        {
            int v = 0;
            reader.Read(ref v);
            this.RouteType = (ApolloRouteType) v;
        }

        public override void WriteTo(ApolloBufferWriter writer)
        {
            writer.Write((int) this.RouteType);
        }

        public ApolloRouteType RouteType { get; protected set; }
    }
}

