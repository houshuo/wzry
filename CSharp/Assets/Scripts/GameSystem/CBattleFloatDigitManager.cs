namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using TMPro;
    using UnityEngine;

    public class CBattleFloatDigitManager
    {
        private const uint DEFAULT_FONT_SIZE = 6;
        private const string FLOAT_TEXT_PREFAB = "Text/FloatText/FloatText.prefab";
        private const uint HP_RECOVER_SHOW_THRESHOLD = 50;
        private List<FloatDigitInfo> m_floatDigitInfoList;
        private Queue<GameObject> m_floatTexts = new Queue<GameObject>();
        private static string[][] s_battleFloatDigitAnimatorStates;
        private static string[] s_otherFloatTextAnimatorStates;
        private static string[] s_otherFloatTextKeys;
        private static string s_restrictTextAnimatorState;
        private static string[] s_restrictTextKeys;

        static CBattleFloatDigitManager()
        {
            string[][] textArrayArray1 = new string[14][];
            textArrayArray1[0] = new string[] { string.Empty };
            textArrayArray1[1] = new string[] { "Physics_Right", "Physics_Left" };
            textArrayArray1[2] = new string[] { "Physics_RightCrit", "Physics_LeftCrit" };
            textArrayArray1[3] = new string[] { "Magic_Right", "Magic_Left" };
            textArrayArray1[4] = new string[] { "Magic_RightCrit", "Magic_LeftCrit" };
            textArrayArray1[5] = new string[] { "ZhenShi_Right", "ZhenShi_Left" };
            textArrayArray1[6] = new string[] { "ZhenShi_RightCrit", "ZhenShi_LeftCrit" };
            textArrayArray1[7] = new string[] { "SufferPhysicalDamage" };
            textArrayArray1[8] = new string[] { "SufferMagicDamage" };
            textArrayArray1[9] = new string[] { "SufferRealDamage" };
            textArrayArray1[10] = new string[] { "ReviveHp" };
            textArrayArray1[11] = new string[] { "Exp" };
            textArrayArray1[12] = new string[] { "Gold" };
            textArrayArray1[13] = new string[] { "LastHitGold" };
            s_battleFloatDigitAnimatorStates = textArrayArray1;
            s_restrictTextKeys = new string[] { 
                "Restrict_None", "Restrict_Dizzy", "Restrict_SlowDown", "Restrict_Taunt", "Restrict_Fear", "Restrict_Frozen", "Restrict_Floating", "Restrict_Slient", "Restrict_Stone", "SkillBuff_Custom_Type_1", "SkillBuff_Custom_Type_2", "SkillBuff_Custom_Type_3", "SkillBuff_Custom_Type_4", "SkillBuff_Custom_Type_5", "SkillBuff_Custom_Type_6", "SkillBuff_Custom_Type_7", 
                "SkillBuff_Custom_Type_8", "SkillBuff_Custom_Type_9", "SkillBuff_Custom_Type_10", "SkillBuff_Custom_Type_11", "SkillBuff_Custom_Type_12", "SkillBuff_Custom_Type_13", "SkillBuff_Custom_Type_14", "SkillBuff_Custom_Type_15", "SkillBuff_Custom_Type_16", "SkillBuff_Custom_Type_17", "SkillBuff_Custom_Type_18", "SkillBuff_Custom_Type_19", "SkillBuff_Custom_Type_20", "SkillBuff_Custom_Type_21", "SkillBuff_Custom_Type_22", "SkillBuff_Custom_Type_23", 
                "SkillBuff_Custom_Type_24", "SkillBuff_Custom_Type_25", "SkillBuff_Custom_Type_26", "SkillBuff_Custom_Type_27", "SkillBuff_Custom_Type_28", "SkillBuff_Custom_Type_29", "SkillBuff_Custom_Type_30", "SkillBuff_Custom_Type_31", "SkillBuff_Custom_Type_32", "SkillBuff_Custom_Type_33", "SkillBuff_Custom_Type_34", "SkillBuff_Custom_Type_35", "SkillBuff_Custom_Type_36", "SkillBuff_Custom_Type_37", "SkillBuff_Custom_Type_38", "SkillBuff_Custom_Type_39", 
                "SkillBuff_Custom_Type_40"
             };
            s_restrictTextAnimatorState = "RestrictText_Anim";
            s_otherFloatTextKeys = new string[] { "Accept_Task", "Complete_Task", "Level_Up", "Talent_Open", "Talent_Learn", "DragonBuff_Get1", "DragonBuff_Get2", "DragonBuff_Get3", "Battle_Absorb", "Battle_ShieldDisappear", "Battle_Immunity", "Battle_InCooldown", "Battle_NoTarget", "Battle_EnergyShortage", "Battle_Blindess", "Battle_MadnessShortage" };
            s_otherFloatTextAnimatorStates = new string[] { "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim" };
        }

        private bool CanMergeToCritText(ref DIGIT_TYPE type1, DIGIT_TYPE type2)
        {
            if (((((type1 != DIGIT_TYPE.PhysicalAttackNormal) || (type2 != DIGIT_TYPE.PhysicalAttackCrit)) && ((type1 != DIGIT_TYPE.PhysicalAttackCrit) || (type2 != DIGIT_TYPE.PhysicalAttackNormal))) && (((type1 != DIGIT_TYPE.MagicAttackNormal) || (type2 != DIGIT_TYPE.MagicAttackCrit)) && ((type1 != DIGIT_TYPE.MagicAttackCrit) || (type2 != DIGIT_TYPE.MagicAttackNormal)))) && (((type1 != DIGIT_TYPE.RealAttackNormal) || (type2 != DIGIT_TYPE.RealAttackCrit)) && ((type1 != DIGIT_TYPE.RealAttackCrit) || (type2 != DIGIT_TYPE.RealAttackNormal))))
            {
                return false;
            }
            if (type1 < type2)
            {
                type1 = type2;
            }
            return true;
        }

        public void ClearAllBattleFloatText()
        {
            while (this.m_floatTexts.Count > 0)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_floatTexts.Dequeue());
            }
            if (this.m_floatDigitInfoList != null)
            {
                this.m_floatDigitInfoList.Clear();
                this.m_floatDigitInfoList = null;
            }
        }

        public void ClearBattleFloatText(CUIAnimatorScript animatorScript)
        {
        }

        public void CollectFloatDigitInSingleFrame(PoolObjHandle<ActorRoot> attacker, PoolObjHandle<ActorRoot> target, DIGIT_TYPE digitType, int value)
        {
            if (!MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
            {
                FloatDigitInfo info;
                if (this.m_floatDigitInfoList == null)
                {
                    this.m_floatDigitInfoList = new List<FloatDigitInfo>();
                }
                for (int i = 0; i < this.m_floatDigitInfoList.Count; i++)
                {
                    info = this.m_floatDigitInfoList[i];
                    if (((info.m_attacker == attacker) && (info.m_target == target)) && ((info.m_digitType == digitType) || this.CanMergeToCritText(ref info.m_digitType, digitType)))
                    {
                        info.m_value += value;
                        this.m_floatDigitInfoList[i] = info;
                        return;
                    }
                }
                info = new FloatDigitInfo(attacker, target, digitType, value);
                this.m_floatDigitInfoList.Add(info);
            }
        }

        public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, ref Vector3 worldPosition)
        {
            if (((((GameSettings.RenderQuality != SGameRenderQuality.Low) || (digitType == DIGIT_TYPE.MagicAttackCrit)) || ((digitType == DIGIT_TYPE.PhysicalAttackCrit) || (digitType == DIGIT_TYPE.RealAttackCrit))) || ((digitType == DIGIT_TYPE.ReceiveGoldCoinInBattle) || (digitType == DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle))) && ((digitType != DIGIT_TYPE.ReviveHp) || (digitValue >= 50L)))
            {
                string[] strArray = s_battleFloatDigitAnimatorStates[(int) digitType];
                if (strArray.Length > 0)
                {
                    string content = (((((digitType != DIGIT_TYPE.ReviveHp) && (digitType != DIGIT_TYPE.ReceiveSpirit)) && (digitType != DIGIT_TYPE.ReceiveGoldCoinInBattle)) || (digitValue <= 0)) ? string.Empty : "+") + Mathf.Abs(digitValue).ToString();
                    if (digitType == DIGIT_TYPE.ReceiveSpirit)
                    {
                        content = content + "xp";
                    }
                    else
                    {
                        if ((digitType == DIGIT_TYPE.ReceiveGoldCoinInBattle) || (digitType == DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle))
                        {
                            content = content + "g";
                        }
                        this.CreateBattleFloatText(content, ref worldPosition, strArray[UnityEngine.Random.Range(0, strArray.Length)], 0);
                    }
                }
            }
        }

        public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, ref Vector3 worldPosition, int animatIndex)
        {
            if ((((GameSettings.RenderQuality != SGameRenderQuality.Low) || (digitType == DIGIT_TYPE.MagicAttackCrit)) || ((digitType == DIGIT_TYPE.PhysicalAttackCrit) || (digitType == DIGIT_TYPE.RealAttackCrit))) || (digitType == DIGIT_TYPE.ReceiveGoldCoinInBattle))
            {
                string[] strArray = s_battleFloatDigitAnimatorStates[(int) digitType];
                if (((strArray.Length > 0) && (animatIndex >= 0)) && (animatIndex < strArray.Length))
                {
                    string content = (((digitType != DIGIT_TYPE.ReceiveSpirit) || (digitValue <= 0)) ? string.Empty : "+") + SimpleNumericString.GetNumeric(Mathf.Abs(digitValue));
                    this.CreateBattleFloatText(content, ref worldPosition, strArray[animatIndex], 0);
                }
            }
        }

        private void CreateBattleFloatText(string content, ref Vector3 worldPosition, string animatorState, uint fontSize = 0)
        {
            if (!MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover && (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(animatorState)))
            {
                GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject("Text/FloatText/FloatText.prefab", enResourceType.BattleScene);
                if ((gameObject != null) && (null != Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera()))
                {
                    int num;
                    gameObject.transform.parent = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera().transform;
                    gameObject.transform.localRotation = Quaternion.identity;
                    TextMeshPro component = gameObject.transform.GetChild(0).GetComponent<TextMeshPro>();
                    Animator animator = gameObject.GetComponent<Animator>();
                    Vector3 position = Camera.main.WorldToScreenPoint(worldPosition);
                    position.Set(position.x, position.y, 30f);
                    gameObject.transform.position = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera().ScreenToWorldPoint(position);
                    if (animatorState.IndexOf("Crit") != -1)
                    {
                        GameObject obj3 = gameObject.transform.Find("Text/icon").gameObject;
                        if (obj3 != null)
                        {
                            Vector3 localPosition = obj3.transform.localPosition;
                            if (animatorState.IndexOf("Left") != -1)
                            {
                                localPosition.x = -0.3f * (content.Length + 1);
                            }
                            else
                            {
                                localPosition.x = -0.3f * (content.Length + 1);
                            }
                            obj3.transform.localPosition = localPosition;
                        }
                    }
                    if (animatorState.IndexOf("LastHit") != -1)
                    {
                        GameObject obj4 = gameObject.transform.Find("Text/icon").gameObject;
                        if (obj4 != null)
                        {
                            Vector3 vector3 = obj4.transform.localPosition;
                            vector3.x = -0.24f * (content.Length + 1);
                            obj4.transform.localPosition = vector3;
                        }
                    }
                    if (int.TryParse(content, out num))
                    {
                        Vector3 localScale = gameObject.transform.localScale;
                        if (num > 0x5dc)
                        {
                            localScale.x = 1.2f;
                            localScale.y = 1.2f;
                        }
                        else if (num > 600)
                        {
                            localScale.x = 1.1f;
                            localScale.y = 1.1f;
                        }
                        else if (num > 300)
                        {
                            localScale.x = 1f;
                            localScale.y = 1f;
                        }
                        else if (num > 100)
                        {
                            localScale.x = 0.8f;
                            localScale.y = 0.8f;
                        }
                        else
                        {
                            localScale.x = 0.7f;
                            localScale.y = 0.7f;
                        }
                        gameObject.transform.localScale = localScale;
                    }
                    if (animator != null)
                    {
                        animator.Play(animatorState);
                    }
                    if (component != null)
                    {
                        component.text = content;
                        component.fontSize = (fontSize <= 0) ? ((float) 6) : ((float) fontSize);
                    }
                    this.m_floatTexts.Enqueue(gameObject);
                    Singleton<CTimerManager>.GetInstance().AddTimer(0x7d0, 1, new CTimer.OnTimeUpHandler(this.OnRecycle));
                }
            }
        }

        public void CreateOtherFloatText(enOtherFloatTextContent otherFloatTextContent, ref Vector3 worldPosition, params string[] args)
        {
            if (GameSettings.RenderQuality != SGameRenderQuality.Low)
            {
                string text = Singleton<CTextManager>.GetInstance().GetText(s_otherFloatTextKeys[(int) otherFloatTextContent], args);
                this.CreateBattleFloatText(text, ref worldPosition, s_otherFloatTextAnimatorStates[(int) otherFloatTextContent], 0);
            }
        }

        public void CreateRestrictFloatText(RESTRICT_TYPE restrictType, ref Vector3 worldPosition)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText(s_restrictTextKeys[(int) restrictType]);
            this.CreateBattleFloatText(text, ref worldPosition, s_restrictTextAnimatorState, 0);
        }

        public void CreateSpecifiedFloatText(uint floatTextID, ref Vector3 worldPosition)
        {
            ResBattleFloatText dataByKey = GameDataMgr.floatTextDatabin.GetDataByKey(floatTextID);
            if (dataByKey != null)
            {
                string animatorState = (dataByKey.szAnimation.Length <= 0) ? s_restrictTextAnimatorState : dataByKey.szAnimation;
                this.CreateBattleFloatText(dataByKey.szText, ref worldPosition, animatorState, dataByKey.dwFontsize);
            }
        }

        public void LateUpdate()
        {
            this.updateFloatDigitInLastFrame();
        }

        private void OnRecycle(int timerSequence)
        {
            if (this.m_floatTexts.Count != 0)
            {
                GameObject pooledGameObject = this.m_floatTexts.Dequeue();
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(pooledGameObject);
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddParticle("Text/FloatText/FloatText.prefab");
        }

        private void updateFloatDigitInLastFrame()
        {
            if (!MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover && ((this.m_floatDigitInfoList != null) && (this.m_floatDigitInfoList.Count != 0)))
            {
                for (int i = 0; i < this.m_floatDigitInfoList.Count; i++)
                {
                    FloatDigitInfo info = this.m_floatDigitInfoList[i];
                    if ((info.m_attacker != 0) && (info.m_target != 0))
                    {
                        Vector3 position;
                        if (info.m_digitType == DIGIT_TYPE.ReviveHp)
                        {
                            position = info.m_target.handle.gameObject.transform.position;
                            this.CreateBattleFloatDigit(info.m_value, info.m_digitType, ref position);
                        }
                        else
                        {
                            Vector3 vector2;
                            Vector3 vector3;
                            float num2;
                            float num3;
                            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                            if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
                            {
                                vector2 = info.m_target.handle.gameObject.transform.position;
                                vector3 = info.m_attacker.handle.gameObject.transform.position;
                                num2 = UnityEngine.Random.Range((float) 0.5f, (float) 1f);
                                num3 = UnityEngine.Random.Range((float) -1f, (float) -0.5f);
                            }
                            else
                            {
                                vector2 = info.m_attacker.handle.gameObject.transform.position;
                                vector3 = info.m_target.handle.gameObject.transform.position;
                                num2 = UnityEngine.Random.Range((float) -1f, (float) -0.5f);
                                num3 = UnityEngine.Random.Range((float) 0.5f, (float) 1f);
                            }
                            Vector3 vector4 = vector2 - vector3;
                            if (vector4.x > 0f)
                            {
                                position = new Vector3(info.m_target.handle.gameObject.transform.position.x + num2, info.m_target.handle.gameObject.transform.position.y + Math.Abs(num2), info.m_target.handle.gameObject.transform.position.z);
                                this.CreateBattleFloatDigit(info.m_value, info.m_digitType, ref position, 1);
                            }
                            else
                            {
                                position = new Vector3(info.m_target.handle.gameObject.transform.position.x + num3, info.m_target.handle.gameObject.transform.position.y + Math.Abs(num3), info.m_target.handle.gameObject.transform.position.z);
                                this.CreateBattleFloatDigit(info.m_value, info.m_digitType, ref position, 0);
                            }
                        }
                    }
                }
                this.m_floatDigitInfoList.Clear();
            }
        }
    }
}

