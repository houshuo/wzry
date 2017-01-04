namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [Serializable]
    public class CProjectileInfo : ScriptableObject
    {
        public string AgeActionPath;
        public string ProjDesc;
        public string ProjName;
        public CSkillCombineInfo SelfSkillCombo;
        public CSkillCombineInfo TargetSkillCombo;
    }
}

