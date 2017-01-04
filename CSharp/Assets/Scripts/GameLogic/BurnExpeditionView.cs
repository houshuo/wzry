namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class BurnExpeditionView
    {
        private CUIAnimatorScript animationScript;
        private List<GameObject> boxNodeList = new List<GameObject>();
        private GameObject buff_node_0;
        private GameObject buff_node_1;
        private GameObject buff_node_2;
        private GameObject buffNode;
        private Text coinText;
        private GameObject enemy_node_0;
        private GameObject enemy_node_1;
        private GameObject enemy_node_2;
        private GameObject enemyNode;
        private CUIHttpImageScript HttpImage;
        private List<GameObject> levelNodeList = new List<GameObject>();
        private Text levelText;
        private CUIFormScript map_fromScript;
        private GameObject mapNode;
        private Text nameText;
        private Text resetNumText;
        private GameObject SymbolLevel;

        private GameObject _GetBuffNode(int index)
        {
            if (index == 0)
            {
                return this.buff_node_0;
            }
            if (index == 1)
            {
                return this.buff_node_1;
            }
            return this.buff_node_2;
        }

        private GameObject _GetEnemyNode(int index)
        {
            if (index == 0)
            {
                return this.enemy_node_0;
            }
            if (index == 1)
            {
                return this.enemy_node_1;
            }
            return this.enemy_node_2;
        }

        private void _set_buff_selected(int index, bool bSelect)
        {
            GameObject p = this._GetBuffNode(index);
            Utility.FindChild(p, "bg_frame").CustomSetActive(bSelect);
            Utility.FindChild(p, "mark").CustomSetActive(bSelect);
        }

        private void _show_BoxNode(int index, string icon, COM_LEVEL_STATUS state, bool bCheckBox = false)
        {
            if (((this.boxNodeList != null) && (index >= 0)) && (index < this.boxNodeList.Count))
            {
                GameObject obj2 = this.boxNodeList[index];
                if (obj2 != null)
                {
                    obj2.CustomSetActive(true);
                    CUIEventScript component = obj2.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        if (bCheckBox)
                        {
                            component.enabled = true;
                        }
                        else
                        {
                            component.enabled = state == COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED;
                        }
                        this._show_Node_ByState(obj2, bCheckBox, state);
                    }
                }
            }
        }

        private void _Show_Buff(GameObject node, int buffid, bool bSelected)
        {
            if (buffid == 0)
            {
                node.CustomSetActive(false);
            }
            else
            {
                GameObject obj2 = Utility.FindChild(node, "bg_frame");
                GameObject obj3 = Utility.FindChild(node, "mark");
                Text componetInChild = Utility.GetComponetInChild<Text>(node, "description");
                obj2.CustomSetActive(bSelected);
                obj3.CustomSetActive(bSelected);
                BurnExpeditionModel model = Singleton<BurnExpeditionController>.instance.model;
                componetInChild.text = model.Get_Buff_Description(buffid);
                Image image = Utility.GetComponetInChild<Image>(node, "icon");
                string str = model.Get_Buff_Icon(buffid);
                if ((image != null) && !string.IsNullOrEmpty(str))
                {
                    image.SetSprite(str, this.map_fromScript, true, false, false);
                }
            }
        }

        public void _Show_Buff_Selected_Index(int index)
        {
            this._set_buff_selected(0, false);
            this._set_buff_selected(1, false);
            this._set_buff_selected(2, false);
            this._set_buff_selected(index, true);
        }

        private void _Show_Enemy_Heros(int index, uint cfgID, string icon, int level, int startCount, uint dwBloodTTH, uint heroID)
        {
            GameObject obj2 = this._GetEnemyNode(index);
            if (obj2 != null)
            {
                obj2.CustomSetActive(true);
                float num = ((float) dwBloodTTH) / 10000f;
                num = float.Parse(num.ToString("F2"));
                Utility.GetComponetInChild<Image>(obj2, "blood_bar").fillAmount = num;
                Utility.FindChild(obj2, "bDead").CustomSetActive(num == 0f);
                this._Show_Icon(Utility.GetComponetInChild<Image>(obj2, "Hero"), heroID);
            }
        }

        private void _Show_Icon(Image img, uint configID)
        {
            if (configID != 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    img.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + masterRoleInfo.GetHeroSkinPic(configID), this.map_fromScript, true, false, false);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(UT.GetText("Burn_Error_Show_Hero"), configID), false);
                }
            }
        }

        private void _show_LevelNode(int index, string name, string icon, COM_LEVEL_STATUS state)
        {
            GameObject obj2 = this.levelNodeList[index];
            if (obj2 != null)
            {
                obj2.CustomSetActive(true);
                obj2.GetComponent<CUIEventScript>().enabled = state == COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED;
            }
            this._show_Node_ByState(obj2, Singleton<BurnExpeditionController>.instance.model.lastUnlockLevelIndex == index, state);
        }

        private void _show_Node_ByState(GameObject node, bool bCurIndex, COM_LEVEL_STATUS state)
        {
            if (node != null)
            {
                string path = string.Empty;
                if (bCurIndex)
                {
                    path = "current_node";
                }
                else
                {
                    switch (state)
                    {
                        case COM_LEVEL_STATUS.COM_LEVEL_STATUS_LOCKED:
                            path = "lock_node";
                            break;

                        case COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED:
                            path = "unlock_node";
                            break;

                        case COM_LEVEL_STATUS.COM_LEVEL_STATUS_FINISHED:
                            path = "finish_node";
                            break;
                    }
                }
                DebugHelper.Assert(path != string.Empty);
                for (int i = 0; i < node.transform.childCount; i++)
                {
                    node.transform.GetChild(i).gameObject.CustomSetActive(false);
                }
                Utility.FindChild(node, path).CustomSetActive(true);
            }
        }

        private void _Show_PlayerInfo(COMDT_PLAYERINFO info, uint force, int levelIndex, string headurl = "", COMDT_GAME_VIP_CLIENT nobeVip = null)
        {
            if ((info != null) && (info.szName != null))
            {
                this.nameText.text = UT.Bytes2String(info.szName);
                this.levelText.text = info.dwLevel.ToString();
                if (string.IsNullOrEmpty(headurl))
                {
                    this.HttpImage.GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + Singleton<BurnExpeditionController>.instance.model.GetRandomRobotIcon(levelIndex), this.map_fromScript, true, false, false);
                }
                else
                {
                    UT.SetHttpImage(this.HttpImage, headurl);
                }
                Image component = this.enemyNode.transform.FindChild("PlayerIcon/NobeIcon").GetComponent<Image>();
                Image image = this.enemyNode.transform.FindChild("PlayerIcon/pnlSnsHead/NobeImag").GetComponent<Image>();
                if (nobeVip != null)
                {
                    if (component != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component, (int) nobeVip.dwCurLevel, false);
                    }
                    if (image != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image, (int) nobeVip.dwHeadIconId);
                    }
                }
                this.enemy_node_0.CustomSetActive(false);
                this.enemy_node_1.CustomSetActive(false);
                this.enemy_node_2.CustomSetActive(false);
                for (int i = 0; i < info.astChoiceHero.Length; i++)
                {
                    COMDT_CHOICEHERO comdt_choicehero = info.astChoiceHero[i];
                    if ((comdt_choicehero != null) && (comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID != 0))
                    {
                        this._Show_Enemy_Heros(i, comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID, string.Empty, comdt_choicehero.stBaseInfo.stCommonInfo.wLevel, comdt_choicehero.stBaseInfo.stCommonInfo.wStar, comdt_choicehero.stBurningInfo.dwBloodTTH, comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID);
                    }
                }
                this.SymbolLevel.CustomSetActive(false);
                for (int j = 0; j < info.astChoiceHero.Length; j++)
                {
                    COMDT_CHOICEHERO comdt_choicehero2 = info.astChoiceHero[j];
                    if ((comdt_choicehero2 != null) && (comdt_choicehero2.stBaseInfo.stCommonInfo.dwHeroID != 0))
                    {
                        int symbolLvWithArray = CSymbolInfo.GetSymbolLvWithArray(comdt_choicehero2.SymbolID);
                        if (symbolLvWithArray > 0)
                        {
                            this.SymbolLevel.CustomSetActive(true);
                            Utility.GetComponetInChild<Text>(this.SymbolLevel, "Text").text = symbolLvWithArray.ToString();
                        }
                        break;
                    }
                }
                BurnExpeditionModel model = Singleton<BurnExpeditionController>.instance.model;
                uint[] numArray = model.Get_Buffs(levelIndex);
                this._Show_Buff(this._GetBuffNode(0), (int) numArray[0], false);
                this._Show_Buff(this._GetBuffNode(1), (int) numArray[1], false);
                this._Show_Buff(this._GetBuffNode(2), (int) numArray[2], false);
                this._Show_Buff_Selected_Index(model.curSelect_BuffIndex);
            }
        }

        public void Check_Box_Info(uint goldNum, uint burn_num)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_Award.prefab", false, true);
            Utility.GetComponetInChild<Text>(formScript.gameObject, "bg/Title").text = UT.GetText("Burn_Box_Award");
            GameObject container = Utility.FindChild(formScript.gameObject, "IconContainer");
            CUIListScript component = container.GetComponent<CUIListScript>();
            if ((goldNum > 0) && (burn_num > 0))
            {
                component.SetElementAmount(2);
            }
            else
            {
                component.SetElementAmount(1);
            }
            int index = 0;
            if (goldNum > 0)
            {
                this.Set_Award(container, index, CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90001", goldNum.ToString(), UT.GetText("Burn_Info_Coin"), formScript);
                index++;
            }
            if (burn_num > 0)
            {
                this.Set_Award(container, index, CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90008", burn_num.ToString(), UT.GetText("Burn_Info_yuanzheng"), formScript);
                index++;
            }
        }

        public void Clear()
        {
            this.levelNodeList.Clear();
            this.boxNodeList.Clear();
            this.mapNode = null;
            this.resetNumText = null;
            this.animationScript = null;
            this.enemyNode = null;
            this.enemy_node_0 = null;
            this.enemy_node_1 = null;
            this.enemy_node_2 = null;
            this.buffNode = null;
            this.buff_node_0 = null;
            this.buff_node_1 = null;
            this.buff_node_2 = null;
            this.nameText = null;
            this.levelText = null;
            this.HttpImage = null;
            this.coinText = null;
            this.map_fromScript = null;
        }

        public void Init()
        {
            this.map_fromScript = Singleton<CUIManager>.GetInstance().OpenForm(BurnExpeditionController.Map_FormPath, false, true);
            this.mapNode = Utility.FindChild(this.map_fromScript.gameObject, "mapNode/map");
            this.animationScript = Utility.GetComponetInChild<CUIAnimatorScript>(this.mapNode, "Panel_Pointer");
            for (int i = 0; i < this.mapNode.transform.childCount; i++)
            {
                if (this.mapNode.transform.GetChild(i).name.IndexOf("level") != -1)
                {
                    this.levelNodeList.Add(null);
                }
                if (this.mapNode.transform.GetChild(i).name.IndexOf("box") != -1)
                {
                    this.boxNodeList.Add(null);
                }
            }
            for (int j = 0; j < this.mapNode.transform.childCount; j++)
            {
                GameObject gameObject = this.mapNode.transform.GetChild(j).gameObject;
                if (gameObject.name.IndexOf("Panel") == -1)
                {
                    gameObject.CustomSetActive(false);
                    int index = BurnExpeditionUT.GetIndex(gameObject.name);
                    if (gameObject.name.IndexOf("level") != -1)
                    {
                        this.levelNodeList[index - 1] = gameObject;
                    }
                    else if (gameObject.name.IndexOf("box") != -1)
                    {
                        this.boxNodeList[index - 1] = gameObject;
                    }
                }
            }
            this.resetNumText = Utility.GetComponetInChild<Text>(this.map_fromScript.gameObject, "mapNode/toolbar/Info");
            this.coinText = Utility.GetComponetInChild<Text>(this.map_fromScript.gameObject, "mapNode/toolbar/Coin/num");
            this.enemyNode = Utility.FindChild(this.map_fromScript.gameObject, "enemyNode");
            this.enemy_node_0 = Utility.FindChild(this.enemyNode, "Heros/hero_0");
            this.enemy_node_1 = Utility.FindChild(this.enemyNode, "Heros/hero_1");
            this.enemy_node_2 = Utility.FindChild(this.enemyNode, "Heros/hero_2");
            this.buffNode = Utility.FindChild(this.map_fromScript.gameObject, "enemyNode/Buffs");
            this.buff_node_0 = Utility.FindChild(this.buffNode, "buff_0");
            this.buff_node_1 = Utility.FindChild(this.buffNode, "buff_1");
            this.buff_node_2 = Utility.FindChild(this.buffNode, "buff_2");
            this.nameText = Utility.GetComponetInChild<Text>(this.enemyNode, "PlayerIcon/Name");
            this.levelText = Utility.GetComponetInChild<Text>(this.enemyNode, "PlayerIcon/level");
            this.HttpImage = Utility.GetComponetInChild<CUIHttpImageScript>(this.enemyNode, "PlayerIcon/pnlSnsHead/HttpImage");
            this.SymbolLevel = Utility.FindChild(this.enemyNode, "PlayerIcon/SymbolLevel");
            this.mapNode.transform.parent.gameObject.CustomSetActive(true);
            this.SetEnemyNodeShow(false);
            this.Show_Line(0);
        }

        public bool IsShow()
        {
            return ((this.map_fromScript != null) && this.map_fromScript.gameObject.activeInHierarchy);
        }

        public void OpenForm()
        {
            if (this.map_fromScript == null)
            {
                this.Init();
            }
            this.Show();
        }

        public void Refresh_Map_Node()
        {
            BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
            if (model._data != null)
            {
                int num = Math.Min(model.Get_LevelNum(model.curDifficultyType), this.levelNodeList.Count);
                for (int i = 0; i < this.levelNodeList.Count; i++)
                {
                    this.levelNodeList[i].CustomSetActive(false);
                }
                for (int j = 0; j < this.boxNodeList.Count; j++)
                {
                    this.boxNodeList[j].CustomSetActive(false);
                }
                for (int k = 0; k < num; k++)
                {
                    this._show_LevelNode(k, "0", "0", model.Get_LevelStatus(k));
                    this._show_BoxNode(k, "0", model.Get_ChestRewardStatus(k), false);
                }
                bool flag = false;
                if ((model.lastUnlockLevelIndex >= 0) && (model.lastUnlockLevelIndex <= BurnExpeditionController.Max_Level_Index))
                {
                    flag = model.Get_ChestRewardStatus(model.lastUnlockLevelIndex) == COM_LEVEL_STATUS.COM_LEVEL_STATUS_LOCKED;
                }
                if (flag)
                {
                    this._show_BoxNode(model.lastUnlockLevelIndex, "0", COM_LEVEL_STATUS.COM_LEVEL_STATUS_LOCKED, true);
                }
            }
        }

        private void Set_Award(GameObject container, int index, string icon, string count, string desc, CUIFormScript formScript)
        {
            if ((container != null) && (formScript != null))
            {
                GameObject gameObject = container.GetComponent<CUIListScript>().GetElemenet(index).gameObject;
                gameObject.CustomSetActive(true);
                Utility.GetComponetInChild<Image>(gameObject, "imgIcon").SetSprite(icon, formScript, true, false, false);
                Utility.GetComponetInChild<Text>(gameObject, "lblIconCount").gameObject.CustomSetActive(!string.IsNullOrEmpty(count));
                Utility.GetComponetInChild<Text>(gameObject, "lblIconCount").text = count;
                if (!string.IsNullOrEmpty(desc))
                {
                    Utility.GetComponetInChild<Text>(gameObject, "ItemName").text = desc;
                }
            }
        }

        public void SetEnemyNodeShow(bool b)
        {
            if (this.enemyNode != null)
            {
                this.enemyNode.CustomSetActive(b);
            }
        }

        public void Show()
        {
            this.Show_Map();
        }

        public void Show_BurnCoin(int num)
        {
            this.coinText.text = num.ToString();
        }

        private void Show_Enemy(COMDT_BURNING_ENEMY_TEAM_INFO info, int levelIndex)
        {
            string headurl = string.Empty;
            COMDT_PLAYERINFO stEnemyDetail = null;
            uint force = 0;
            COMDT_GAME_VIP_CLIENT nobeVip = null;
            if (info.bType == 1)
            {
                stEnemyDetail = info.stDetail.stRealMan.stEnemyDetail;
                force = info.dwEnemyTeamForce;
                headurl = UT.Bytes2String(info.stDetail.stRealMan.szHeadUrl);
                nobeVip = info.stDetail.stRealMan.stVip;
            }
            else
            {
                stEnemyDetail = info.stDetail.stRobot.stEnemyDetail;
                force = info.dwEnemyTeamForce;
                nobeVip = null;
            }
            this._Show_PlayerInfo(stEnemyDetail, force, levelIndex, headurl, nobeVip);
        }

        public void Show_ENEMY(int levelIndex)
        {
            BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
            if (model._data != null)
            {
                this.SetEnemyNodeShow(true);
                this.Show_Enemy(model.Get_ENEMY_TEAM_INFO(model.curDifficultyType, levelIndex), levelIndex);
            }
        }

        public void Show_Line(int index)
        {
            string stateName = string.Empty;
            if (index == 0)
            {
                stateName = "State_0";
            }
            else if (index == 1)
            {
                stateName = "State_2";
            }
            else if (index == 2)
            {
                stateName = "State_4";
            }
            else if (index == 3)
            {
                stateName = "State_6";
            }
            else if (index == 4)
            {
                stateName = "State_8";
            }
            else if (index == 5)
            {
                stateName = "State_10";
            }
            if (this.animationScript != null)
            {
                this.animationScript.PlayAnimator(stateName);
            }
        }

        public void Show_Map()
        {
            BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
            if (model._data != null)
            {
                this.Refresh_Map_Node();
                this.SetEnemyNodeShow(false);
                this.Show_ResetNum(model.Get_ResetNum(model.curDifficultyType));
                this.Show_Line(model.lastUnlockLevelIndex);
                if (this.coinText != null)
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        this.coinText.text = masterRoleInfo.BurningCoin.ToString();
                    }
                }
            }
        }

        public void Show_ResetNum(int num)
        {
            if (this.resetNumText != null)
            {
                this.resetNumText.gameObject.CustomSetActive(true);
                string str = string.Format("<color=#be7d15ff>{0}</color>", num);
                this.resetNumText.text = string.Format(UT.GetText("Burn_Valid_Count"), str);
            }
        }
    }
}

