using Assets.Scripts.GameLogic;
using System;

public class HudUT
{
    public static bool IsTower(ActorRoot actor)
    {
        if (actor == null)
        {
            return false;
        }
        return ((actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4));
    }
}

