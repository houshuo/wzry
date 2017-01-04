namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    public interface IGameActorDataProvider
    {
        int Fast_GetActorServerDataBornIndex(ref ActorMeta actorMeta);
        bool GetActorServerCommonSkillData(ref ActorMeta actorMeta, out uint skillID);
        bool GetActorServerData(ref ActorMeta actorMeta, ref ActorServerData actorData);
        bool GetActorServerEquipData(ref ActorMeta actorMeta, ActorEquiplSlot equipSlot, ref ActorServerEquipData skillData);
        bool GetActorServerRuneData(ref ActorMeta actorMeta, ActorRunelSlot runeSlot, ref ActorServerRuneData runeData);
        bool GetActorServerSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorServerSkillData skillData);
        bool GetActorStaticData(ref ActorMeta actorMeta, ref ActorStaticData actorData);
        bool GetActorStaticPerStarLvData(ref ActorMeta actorMeta, ActorStarLv starLv, ref ActorPerStarLvData perStarLvData);
        bool GetActorStaticSkillData(ref ActorMeta actorMeta, ActorSkillSlot skillSlot, ref ActorStaticSkillData skillData);
    }
}

