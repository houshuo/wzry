namespace AGE
{
    using Assets.Scripts.GameLogic;
    using System;

    public class AGE_Helper
    {
        public static VCollisionShape GetCollisionShape(ActorRoot actorRoot)
        {
            if (actorRoot == null)
            {
                return null;
            }
            VCollisionShape shape = actorRoot.shape;
            if (shape == null)
            {
                shape = VCollisionShape.InitActorCollision(actorRoot);
            }
            if (shape != null)
            {
                shape.ConditionalUpdateShape();
            }
            return shape;
        }
    }
}

