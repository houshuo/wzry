namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CBattleHeroInfoPanel : Singleton<CBattleHeroInfoPanel>
    {
        public CUIFormScript m_FormScript;
        private static uint PropertyMaxAmount = 0x12;
        private static string s_battleHeroInfoForm = "UGUI/Form/Battle/Form_Battle_HeroInfo.prefab";
        private static string s_propPanel = "Panel_Prop";
        public static readonly string valForm1 = "<color=#60bd67>{0}</color>({1}+<color=#60bd67>{2}</color>)";
        public static readonly string valForm2 = "<color=#60bd67>{0}</color>";

        private static string GetFormPercentStr(int percent, bool isExtra)
        {
            if (isExtra)
            {
                return string.Format(valForm2, CUICommonSystem.GetValuePercent(percent));
            }
            return CUICommonSystem.GetValuePercent(percent);
        }

        private static string GetFormStr(float baseValue, float growValue)
        {
            if (growValue > 0f)
            {
                return string.Format(valForm1, baseValue + growValue, baseValue, growValue);
            }
            return baseValue.ToString();
        }

        public void Hide()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleHeroInfoForm);
            this.m_FormScript = null;
            Singleton<CUIParticleSystem>.instance.Show(null);
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleHeroInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleHeroInfoTabChange));
        }

        public bool IsFormOpened()
        {
            return (this.m_FormScript != null);
        }

        protected void OnBttleHeroInfoTabChange(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            if (component != null)
            {
                int selectedIndex = component.GetSelectedIndex();
                if (this.m_FormScript != null)
                {
                    if (selectedIndex == 0)
                    {
                        this.m_FormScript.m_formWidgets[2].CustomSetActive(true);
                        this.m_FormScript.m_formWidgets[1].CustomSetActive(false);
                    }
                    else
                    {
                        this.m_FormScript.m_formWidgets[2].CustomSetActive(false);
                        this.m_FormScript.m_formWidgets[1].CustomSetActive(true);
                    }
                }
            }
        }

        public void OnSkillTipsShow()
        {
            if (this.m_FormScript != null)
            {
                GameObject gameObject = this.m_FormScript.transform.Find("SkillTipsBg").gameObject;
                if (!gameObject.activeSelf)
                {
                    gameObject.CustomSetActive(true);
                }
                SkillSlotType[] typeArray1 = new SkillSlotType[4];
                typeArray1[1] = SkillSlotType.SLOT_SKILL_1;
                typeArray1[2] = SkillSlotType.SLOT_SKILL_2;
                typeArray1[3] = SkillSlotType.SLOT_SKILL_3;
                SkillSlotType[] typeArray = typeArray1;
                GameObject[] objArray = new GameObject[typeArray.Length];
                objArray[0] = gameObject.transform.Find("Panel0").gameObject;
                objArray[1] = gameObject.transform.Find("Panel1").gameObject;
                objArray[2] = gameObject.transform.Find("Panel2").gameObject;
                objArray[3] = gameObject.transform.Find("Panel3").gameObject;
                Skill skillObj = null;
                if (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)
                {
                    PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                    if (captain != 0)
                    {
                        IHeroData data = CHeroDataFactory.CreateHeroData((uint) captain.handle.TheActorMeta.ConfigId);
                        SkillSlot[] skillSlotArray = captain.handle.SkillControl.SkillSlotArray;
                        for (int i = 0; i < typeArray.Length; i++)
                        {
                            SkillSlot slot = skillSlotArray[(int) typeArray[i]];
                            objArray[i].CustomSetActive(true);
                            if (slot != null)
                            {
                                skillObj = slot.SkillObj;
                            }
                            else if ((i < (typeArray.Length - 1)) && (i > 0))
                            {
                                skillObj = new Skill((captain.handle.TheActorMeta.ConfigId * 100) + (i * 10));
                            }
                            else
                            {
                                skillObj = null;
                            }
                            if (skillObj != null)
                            {
                                Image component = objArray[i].transform.Find("SkillMask/SkillImg").GetComponent<Image>();
                                if ((component != null) && !string.IsNullOrEmpty(skillObj.IconName))
                                {
                                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + skillObj.IconName, Singleton<CUIManager>.GetInstance().GetForm(s_battleHeroInfoForm), true, false, false);
                                }
                                Text text = objArray[i].transform.Find("Text_Tittle").GetComponent<Text>();
                                if ((text != null) && (skillObj.cfgData.szSkillName.Length > 0))
                                {
                                    text.text = StringHelper.UTF8BytesToString(ref skillObj.cfgData.szSkillName);
                                }
                                Text text2 = objArray[i].transform.Find("Text_CD").GetComponent<Text>();
                                int skillCDMax = 0;
                                if (slot != null)
                                {
                                    skillCDMax = slot.GetSkillCDMax();
                                }
                                text2.text = (i != 0) ? Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[1]) : Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_5");
                                if ((i < typeArray.Length) && (i > 0))
                                {
                                    Text text3 = objArray[i].transform.Find("Text_EnergyCost").GetComponent<Text>();
                                    if (slot == null)
                                    {
                                        string[] args = new string[] { skillObj.cfgData.iEnergyCost.ToString() };
                                        text3.text = Singleton<CTextManager>.instance.GetText(CUICommonSystem.GetEnergyMaxOrCostText(skillObj.cfgData.dwEnergyCostType, EnergyShowType.CostValue), args);
                                    }
                                    else
                                    {
                                        string[] textArray3 = new string[] { slot.NextSkillEnergyCostTotal().ToString() };
                                        text3.text = Singleton<CTextManager>.instance.GetText(CUICommonSystem.GetEnergyMaxOrCostText(skillObj.cfgData.dwEnergyCostType, EnergyShowType.CostValue), textArray3);
                                    }
                                }
                                uint[] skillEffectType = skillObj.cfgData.SkillEffectType;
                                GameObject obj3 = null;
                                for (int j = 1; j <= 2; j++)
                                {
                                    obj3 = objArray[i].transform.Find(string.Format("EffectNode{0}", j)).gameObject;
                                    if ((j <= skillEffectType.Length) && (skillEffectType[j - 1] != 0))
                                    {
                                        obj3.CustomSetActive(true);
                                        obj3.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType) skillEffectType[j - 1]), this.m_FormScript, true, false, false);
                                        obj3.transform.Find("Text").GetComponent<Text>().text = CSkillData.GetEffectDesc((SkillEffectType) skillEffectType[j - 1]);
                                    }
                                    else
                                    {
                                        obj3.CustomSetActive(false);
                                    }
                                }
                                Text text4 = objArray[i].transform.Find("Text_Detail").GetComponent<Text>();
                                ValueDataInfo[] actorValue = captain.handle.ValueComponent.mActorValue.GetActorValue();
                                if ((text4 != null) && (skillObj.cfgData.szSkillDesc.Length > 0))
                                {
                                    text4.text = CUICommonSystem.GetSkillDesc(skillObj.cfgData.szSkillDesc, actorValue, slot.GetSkillLevel(), captain.handle.ValueComponent.actorSoulLevel);
                                }
                            }
                            else if (i == (typeArray.Length - 1))
                            {
                                Text text5 = objArray[i].transform.Find("Text_Detail").GetComponent<Text>();
                                if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
                                {
                                    text5.text = Singleton<CTextManager>.GetInstance().GetText("Skill_Text_Lock_PVP");
                                }
                                else
                                {
                                    text5.text = Singleton<CTextManager>.GetInstance().GetText("Skill_Text_Lock_PVE");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshHeroPropPanel(GameObject root, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((actor.handle != null) && (actor.handle.ValueComponent != null))
            {
                ValueDataInfo[] actorValue = actor.handle.ValueComponent.mActorValue.GetActorValue();
                int soulLevel = actor.handle.ValueComponent.mActorValue.SoulLevel;
                uint configId = (uint) actor.handle.TheActorMeta.ConfigId;
                int actorMoveSpeed = actor.handle.ValueComponent.actorMoveSpeed;
                uint energyType = (uint) actor.handle.ValueComponent.mActorValue.EnergyType;
                Transform transform = root.transform;
                int totalValue = 0;
                int percent = 0;
                Text[] textArray = new Text[PropertyMaxAmount + 1];
                Text[] textArray2 = new Text[PropertyMaxAmount + 1];
                for (int i = 1; i <= PropertyMaxAmount; i++)
                {
                    textArray[i] = transform.Find(string.Format("TextL{0}", i)).GetComponent<Text>();
                    textArray2[i] = transform.Find(string.Format("TextR{0}", i)).GetComponent<Text>();
                }
                ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
                textArray[1].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyAtkPt");
                textArray2[1].text = GetFormStr((float) actorValue[1].basePropertyValue, (float) actorValue[1].extraPropertyValue);
                textArray[2].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcAtkPt");
                textArray2[2].text = GetFormStr((float) actorValue[2].basePropertyValue, (float) actorValue[2].extraPropertyValue);
                textArray[3].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MaxHp");
                textArray2[3].text = GetFormStr((float) actorValue[5].basePropertyValue, (float) actorValue[5].extraPropertyValue);
                textArray[4].text = Singleton<CTextManager>.GetInstance().GetText(CUICommonSystem.GetEnergyMaxOrCostText(energyType, EnergyShowType.MaxValue));
                textArray2[4].text = GetFormStr((float) actorValue[0x20].basePropertyValue, (float) actorValue[0x20].extraPropertyValue);
                totalValue = actorValue[3].totalValue;
                percent = (int) ((totalValue * 0x2710) / ((totalValue + (soulLevel * anyData.dwM_PhysicsDefend)) + anyData.dwN_PhysicsDefend));
                textArray[5].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyDefPt");
                textArray2[5].text = string.Format("{0}|{1}", GetFormStr((float) actorValue[3].basePropertyValue, (float) actorValue[3].extraPropertyValue), GetFormPercentStr(percent, actorValue[3].extraPropertyValue > 0));
                totalValue = actorValue[4].totalValue;
                percent = (int) ((totalValue * 0x2710) / ((totalValue + (soulLevel * anyData.dwM_MagicDefend)) + anyData.dwN_MagicDefend));
                textArray[6].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcDefPt");
                textArray2[6].text = string.Format("{0}|{1}", GetFormStr((float) actorValue[4].basePropertyValue, (float) actorValue[4].extraPropertyValue), GetFormPercentStr(percent, actorValue[4].extraPropertyValue > 0));
                totalValue = actorValue[0x1c].totalValue;
                percent = ((int) ((0x2710 * totalValue) / ((totalValue + (soulLevel * anyData.dwM_AttackSpeed)) + anyData.dwN_AttackSpeed))) + actorValue[0x12].totalValue;
                textArray[7].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_AtkSpdLvl");
                textArray2[7].text = GetFormPercentStr(percent, actorValue[0x12].extraPropertyValue > 0);
                percent = actorValue[20].totalValue;
                textArray[8].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CdReduce");
                textArray2[8].text = GetFormPercentStr(percent, actorValue[20].extraPropertyValue > 0);
                totalValue = actorValue[0x18].totalValue;
                percent = ((int) ((0x2710 * totalValue) / ((totalValue + (soulLevel * anyData.dwM_Critical)) + anyData.dwN_Critical))) + actorValue[6].totalValue;
                textArray[9].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CritLvl");
                textArray2[9].text = GetFormPercentStr(percent, actorValue[6].extraPropertyValue > 0);
                textArray[10].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MoveSpd");
                textArray2[10].text = GetFormStr((float) (actorValue[15].basePropertyValue / 10), (float) ((actorMoveSpeed - actorValue[15].basePropertyValue) / 10));
                textArray[11].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover");
                totalValue = actorValue[0x10].totalValue;
                string str = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover_Desc"), totalValue);
                textArray2[11].text = GetFormStr((float) actorValue[0x10].basePropertyValue, (float) actorValue[0x10].extraPropertyValue);
                textArray[12].text = Singleton<CTextManager>.GetInstance().GetText(CUICommonSystem.GetEnergyMaxOrCostText(energyType, EnergyShowType.RecoverValue));
                totalValue = actorValue[0x21].totalValue;
                string str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_EpRecover_Desc"), totalValue);
                textArray2[12].text = GetFormStr((float) actorValue[0x21].basePropertyValue, (float) actorValue[0x21].extraPropertyValue);
                textArray[13].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyArmorHurt");
                textArray2[13].text = string.Format("{0}|{1}", GetFormStr((float) actorValue[7].baseValue, (float) actorValue[7].extraPropertyValue), GetFormPercentStr(actorValue[0x22].totalValue, actorValue[0x22].extraPropertyValue > 0));
                textArray[14].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcArmorHurt");
                textArray2[14].text = string.Format("{0}|{1}", GetFormStr((float) actorValue[8].baseValue, (float) actorValue[8].extraPropertyValue), GetFormPercentStr(actorValue[0x23].totalValue, actorValue[0x23].extraPropertyValue > 0));
                totalValue = actorValue[0x1a].totalValue;
                percent = ((int) ((0x2710 * totalValue) / ((totalValue + (soulLevel * anyData.dwM_PhysicsHemophagia)) + anyData.dwN_PhysicsHemophagia))) + actorValue[9].totalValue;
                textArray[15].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyVampLvl");
                textArray2[15].text = GetFormPercentStr(percent, actorValue[9].extraPropertyValue > 0);
                totalValue = actorValue[0x1b].totalValue;
                percent = ((int) ((0x2710 * totalValue) / ((totalValue + (soulLevel * anyData.dwM_MagicHemophagia)) + anyData.dwN_MagicHemophagia))) + actorValue[10].totalValue;
                textArray[0x10].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcVampLvl");
                textArray2[0x10].text = GetFormPercentStr(percent, actorValue[10].extraPropertyValue > 0);
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(configId);
                if (dataByKey != null)
                {
                    textArray[0x11].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_BaseAtkRange");
                    textArray2[0x11].text = Utility.UTF8Convert(dataByKey.szAttackRangeDesc);
                }
                else
                {
                    textArray[0x11].text = string.Empty;
                    textArray2[0x11].text = string.Empty;
                }
                totalValue = actorValue[0x1d].totalValue;
                percent = ((int) ((0x2710 * totalValue) / ((totalValue + (soulLevel * anyData.dwM_Tenacity)) + anyData.dwN_Tenacity))) + actorValue[0x11].totalValue;
                textArray[0x12].text = Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CtrlReduceLvl");
                textArray2[0x12].text = GetFormPercentStr(percent, actorValue[0x11].extraPropertyValue > 0);
            }
        }

        private void RefreshPropPanel(GameObject root, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((actor != 0) && (root != null))
            {
                GameObject gameObject = root.transform.Find(s_propPanel).gameObject.transform.Find("Panel_HeroProp").gameObject;
                this.RefreshHeroPropPanel(gameObject, ref actor);
            }
        }

        public void Show()
        {
            this.m_FormScript = Singleton<CUIManager>.GetInstance().OpenForm(s_battleHeroInfoForm, false, true);
            if ((this.m_FormScript != null) && (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer() != null))
            {
                PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain;
                if (captain != 0)
                {
                    GameObject listObj = this.m_FormScript.m_formWidgets[3];
                    if (listObj == null)
                    {
                        return;
                    }
                    string[] titleList = new string[] { Singleton<CTextManager>.GetInstance().GetText("BattleHeroInfo_Property"), Singleton<CTextManager>.GetInstance().GetText("BattleHeroInfo_Skill") };
                    CUICommonSystem.InitMenuPanel(listObj, titleList, 0);
                    this.ShowHero(captain);
                    this.RefreshPropPanel(this.m_FormScript.gameObject, ref captain);
                    this.OnSkillTipsShow();
                    this.m_FormScript.m_formWidgets[2].CustomSetActive(true);
                    this.m_FormScript.m_formWidgets[1].CustomSetActive(false);
                }
                Singleton<CUIParticleSystem>.instance.Hide(null);
            }
        }

        private void ShowHero(PoolObjHandle<ActorRoot> actor)
        {
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long) actor.handle.TheActorMeta.ConfigId);
            GameObject obj2 = this.m_FormScript.m_formWidgets[4];
            if (obj2 != null)
            {
                string str = CUIUtility.s_Sprite_Dynamic_BustHero_Dir;
                string heroSkinPic = CSkinInfo.GetHeroSkinPic((uint) actor.handle.TheActorMeta.ConfigId, 0);
                Image component = obj2.transform.GetComponent<Image>();
                if (component != null)
                {
                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + heroSkinPic, Singleton<CUIManager>.GetInstance().GetForm(s_battleHeroInfoForm), true, false, false);
                }
            }
            GameObject obj3 = this.m_FormScript.m_formWidgets[5];
            if (obj3 != null)
            {
                Text text = obj3.transform.GetComponent<Text>();
                if (text != null)
                {
                    text.text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
                }
            }
            GameObject obj4 = this.m_FormScript.m_formWidgets[6];
            if (obj4 != null)
            {
                Text text2 = obj4.transform.GetComponent<Text>();
                if (text2 != null)
                {
                    text2.text = StringHelper.UTF8BytesToString(ref dataByKey.szHeroTips);
                }
            }
        }

        public void SwitchPanel()
        {
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleHeroInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleHeroInfoTabChange));
        }
    }
}

