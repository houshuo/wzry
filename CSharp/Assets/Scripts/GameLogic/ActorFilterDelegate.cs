namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.CompilerServices;

    public delegate bool ActorFilterDelegate(ref PoolObjHandle<ActorRoot> actor);
}

