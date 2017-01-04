namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using UnityEngine;

    [Serializable]
    public class CSkillInfo : ScriptableObject
    {
        public string AgeActionPath;
        public bool bIncludeEnemy;
        public bool bIncludeSelf;
        public int CooldownMs;
        public SkillTargetRule DetectRule;
        public int MaxAttackDistance;
        public int MaxSearchDistance;
        public CProjectileInfo ProjRef;
        public SkillRangeAppointType RangeAptType;
        public CSkillCombineInfo SelfSkillCombo;
        public string SkillDesc;
        public Texture2D SkillIcon;
        public string SkillName;
        public CSkillCombineInfo TargetSkillCombo;
        public SkillUseRule UseRule;
    }
}

