using UnityEngine;

public class SCollisionComponent : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public VInt3 Pos = VInt3.zero;
    public CollisionShapeType shapeType = CollisionShapeType.Sphere;
    [SerializeField, HideInInspector]
    public VInt3 Size = new VInt3(500, 500, 500);

    public VCollisionShape CreateShape()
    {
        DebugHelper.Assert((!Singleton<BattleLogic>.instance.isFighting || Singleton<GameLogic>.instance.bInLogicTick) || Singleton<FrameSynchr>.instance.isCmdExecuting);
        VCollisionShape shape = null;
        CollisionShapeType shapeType = this.shapeType;
        if (shapeType != CollisionShapeType.Box)
        {
            if (shapeType != CollisionShapeType.Sphere)
            {
                return shape;
            }
        }
        else
        {
            return new VCollisionBox { Size = this.Size, Pos = this.Pos };
        }
        return new VCollisionSphere { Pos = this.Pos, Radius = this.Size.x };
    }
}

