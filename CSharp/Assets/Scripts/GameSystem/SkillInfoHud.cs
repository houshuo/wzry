namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class SkillInfoHud
    {
        private PoolObjHandle<ActorRoot> _curActor;
        private Image[] _skillCdBgs = new Image[5];
        private Image[] _skillIcons = new Image[5];
        private GameObject[] _skillItems = new GameObject[5];
        private GameObject[] _skillLevelRoots = new GameObject[5];
        private Text[] _skillLevels = new Text[5];
        public const int MAX_SKILL_HUD_COUNT = 5;

        public SkillInfoHud(GameObject root)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject p = Utility.FindChild(root, "Skill_" + (i + 1));
                this._skillItems[i] = p;
                this._skillIcons[i] = Utility.GetComponetInChild<Image>(p, "Icon");
                this._skillLevelRoots[i] = Utility.FindChild(p, "Level");
                this._skillLevels[i] = Utility.GetComponetInChild<Text>(p, "Level/Text");
                this._skillCdBgs[i] = Utility.GetComponetInChild<Image>(p, "CdBg");
            }
            this._curActor = new PoolObjHandle<ActorRoot>(null);
        }

        public void SwitchActor(ref PoolObjHandle<ActorRoot> actor)
        {
            if (actor != this._curActor)
            {
                this._curActor = actor;
                SkillSlot[] skillSlotArray = actor.handle.SkillControl.SkillSlotArray;
                for (int i = 0; (i < this._skillItems.Length) && ((i + 1) < skillSlotArray.Length); i++)
                {
                    SkillSlotType type = (SkillSlotType) (i + 1);
                    SkillSlot slot = skillSlotArray[i + 1];
                    GameObject obj2 = this._skillItems[i];
                    if (obj2 != null)
                    {
                        if (slot != null)
                        {
                            obj2.CustomSetActive(true);
                            this._skillIcons[i].SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + slot.SkillObj.IconName, Singleton<CBattleSystem>.GetInstance().WatchFormScript, true, false, false);
                            this._skillLevelRoots[i].CustomSetActive((type >= SkillSlotType.SLOT_SKILL_1) && (type <= SkillSlotType.SLOT_SKILL_3));
                            this.ValidateLevel(type);
                            this.ValidateCD(type);
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void ValidateCD(SkillSlotType slot)
        {
            if (this._curActor != 0)
            {
                int index = ((int) slot) - 1;
                if ((index >= 0) && (index < this._skillCdBgs.Length))
                {
                    SkillSlot slot2 = this._curActor.handle.SkillControl.SkillSlotArray[(int) slot];
                    this._skillCdBgs[index].fillAmount = ((float) slot2.CurSkillCD) / ((float) slot2.GetSkillCDMax());
                }
            }
        }

        public void ValidateLevel(SkillSlotType slot)
        {
            if (this._curActor != 0)
            {
                int index = ((int) slot) - 1;
                if ((index >= 0) && (index < this._skillLevels.Length))
                {
                    SkillSlot slot2 = this._curActor.handle.SkillControl.SkillSlotArray[(int) slot];
                    this._skillLevels[index].text = slot2.GetSkillLevel().ToString();
                }
            }
        }
    }
}

