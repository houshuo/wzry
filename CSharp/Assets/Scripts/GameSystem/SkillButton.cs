namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class SkillButton
    {
        public bool bDisableFlag;
        public bool bLimitedFlag;
        public Image effectTimeImage;
        public int effectTimeLeft;
        public int effectTimeTotal;
        public GameObject m_button;
        public GameObject m_cdText;
        public Vector3 m_skillIndicatorFixedPosition = Vector3.zero;
        private static string[] skillLvlImgName = new string[] { "SkillLvlImg_1", "SkillLvlImg_2", "SkillLvlImg_3", "SkillLvlImg_4", "SkillLvlImg_5", "SkillLvlImg_6" };

        public void ChangeSkillIcon(int skillID)
        {
            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long) skillID);
            if (dataByKey != null)
            {
                GameObject skillImg = this.GetSkillImg();
                if (skillImg != null)
                {
                    Image component = skillImg.GetComponent<Image>();
                    if (component != null)
                    {
                        component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szIconPath), Singleton<CBattleSystem>.GetInstance().FightFormScript, true, false, false);
                    }
                }
            }
        }

        public void Clear()
        {
            this.m_button = null;
            this.m_cdText = null;
            this.effectTimeImage = null;
        }

        public GameObject GetAnimationCD()
        {
            if (this.m_button == null)
            {
                return null;
            }
            Transform transform = this.m_button.transform.Find("Present/Panel_CD");
            DebugHelper.Assert(transform != null, "failed GetAnimationCD");
            return ((transform == null) ? null : transform.gameObject);
        }

        public GameObject GetAnimationPresent()
        {
            if (this.m_button == null)
            {
                return null;
            }
            Transform transform = this.m_button.transform.Find("Present");
            DebugHelper.Assert(transform != null, "failed GetAnimationPresent");
            return ((transform == null) ? null : transform.gameObject);
        }

        public GameObject GetDisableButton()
        {
            if (this.m_button == null)
            {
                return null;
            }
            Transform transform = this.m_button.transform.Find("disable");
            DebugHelper.Assert(transform != null, "failed GetDisableButton");
            return ((transform == null) ? null : transform.gameObject);
        }

        public GameObject GetLearnSkillButton()
        {
            if (this.m_button == null)
            {
                return null;
            }
            Transform transform = this.m_button.transform.Find("LearnBtn");
            DebugHelper.Assert(transform != null, "GetLearnSkillButton failed GetDisableButton");
            return ((transform == null) ? null : transform.gameObject);
        }

        public GameObject GetSkillFrameImg()
        {
            if (this.m_button == null)
            {
                return null;
            }
            string name = "Present/SkillFrame";
            Transform transform = this.m_button.transform.Find(name);
            DebugHelper.Assert(transform != null, "GetSkillFrameImg failed");
            return ((transform == null) ? null : transform.gameObject);
        }

        public GameObject GetSkillImg()
        {
            if (this.m_button == null)
            {
                return null;
            }
            string name = "Present/SkillImg";
            Transform transform = this.m_button.transform.Find(name);
            DebugHelper.Assert(transform != null, "GetSkillImg failed");
            return ((transform == null) ? null : transform.gameObject);
        }

        public GameObject GetSkillLvlFrameImg(bool bIsRight)
        {
            string str;
            if (this.m_button == null)
            {
                return null;
            }
            if (bIsRight)
            {
                str = "Present/SkillLvlFrame_right";
            }
            else
            {
                str = "Present/SkillLvlFrame_left";
            }
            Transform transform = this.m_button.transform.Find(str);
            DebugHelper.Assert(transform != null, "GetSkillLvlImg failed GetDisableButton");
            return ((transform == null) ? null : transform.gameObject);
        }

        public GameObject GetSkillLvlImg(int iSkillLvl)
        {
            if (this.m_button == null)
            {
                return null;
            }
            GameObject animationPresent = this.GetAnimationPresent();
            if (animationPresent == null)
            {
                return null;
            }
            Transform transform = animationPresent.transform.Find(skillLvlImgName[iSkillLvl - 1]);
            DebugHelper.Assert(transform != null, "GetSkillLvlImg failed GetDisableButton");
            return ((transform == null) ? null : transform.gameObject);
        }
    }
}

