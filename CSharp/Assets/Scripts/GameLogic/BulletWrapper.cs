namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class BulletWrapper : ObjWrapper
    {
        public override void Born(ActorRoot owner)
        {
            base.actor = owner;
            base.actorPtr = new PoolObjHandle<ActorRoot>(base.actor);
        }

        public override void Fight()
        {
        }

        public override void FightOver()
        {
        }

        public override string GetTypeName()
        {
            return "BulletWrapper";
        }

        public override void Init()
        {
        }

        public override void OnUse()
        {
            base.OnUse();
        }

        public override void Prepare()
        {
        }

        public override void Uninit()
        {
        }

        public override void UpdateLogic(int delta)
        {
        }
    }
}

