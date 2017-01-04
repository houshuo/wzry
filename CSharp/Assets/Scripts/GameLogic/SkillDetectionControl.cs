namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public class SkillDetectionControl : Singleton<SkillDetectionControl>
    {
        private DictionaryView<uint, SkillBaseDetection> _registedRule = new DictionaryView<uint, SkillBaseDetection>();

        public bool Detection(SkillUseRule ruleType, SkillSlot slot)
        {
            SkillBaseDetection detection;
            if (this._registedRule.TryGetValue((uint) ruleType, out detection))
            {
                return detection.Detection(slot);
            }
            return true;
        }

        public override void Init()
        {
            ClassEnumerator enumerator = new ClassEnumerator(typeof(SkillBaseDetectionAttribute), typeof(SkillBaseDetection), typeof(SkillBaseDetectionAttribute).Assembly, true, false, false);
            foreach (System.Type type in enumerator.results)
            {
                SkillBaseDetection detection = (SkillBaseDetection) Activator.CreateInstance(type);
                Attribute customAttribute = Attribute.GetCustomAttribute(type, typeof(SkillBaseDetectionAttribute));
                this._registedRule.Add((uint) (customAttribute as SkillBaseDetectionAttribute).UseRule, detection);
            }
        }
    }
}

