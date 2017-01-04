using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CSignalTipShower : MonoBehaviour
{
    public GameObject bg_icon;
    public GameObject inbattlemsg_node;
    public Text inbattlemsg_txt;
    public GameObject leftIcon;
    public static string res_dir = "UGUI/Sprite/Dynamic/Signal/";
    public GameObject rightIcon;
    public static string S_Base_blue_Icon = "UGUI/Sprite/Dynamic/Signal/Base_blue";
    public static string S_Bg_Blue = (CUIUtility.s_battleSignalPrefabDir + "Signal_Tips_Blue.prefab");
    public static string S_Bg_Green = (CUIUtility.s_battleSignalPrefabDir + "Signal_Tips_Green.prefab");
    public static string S_Dragon_big_Icon = "UGUI/Sprite/Dynamic/Signal/Dragon_big";
    public static string S_Dragon_small_Icon = "UGUI/Sprite/Dynamic/Signal/Dragon_small";
    public static string S_kn_dragon_Icon = "UGUI/Sprite/Dynamic/Signal/kn_dragon";
    public static string S_kn_Tower_Icon = "UGUI/Sprite/Dynamic/Signal/kn_Tower";
    public GameObject signal_icon;
    public GameObject signal_node;
    public Text signal_txt;

    public static void Preload(ref ActorPreloadTab preloadTab)
    {
        preloadTab.AddSprite(S_kn_Tower_Icon);
        preloadTab.AddSprite(S_Base_blue_Icon);
        preloadTab.AddSprite(S_Dragon_big_Icon);
        preloadTab.AddSprite(S_Dragon_small_Icon);
        preloadTab.AddSprite(S_kn_dragon_Icon);
    }

    public void Set(CSignalTipsElement data, CUIFormScript formScript)
    {
        if ((data != null) && (formScript != null))
        {
            if (data.type == CSignalTipsElement.EType.Signal)
            {
                this.Show(data as CSignalTips, formScript);
            }
            else if (data.type == CSignalTipsElement.EType.InBattleMsg)
            {
                this.Show(data as CSignalTips_InBatMsg, formScript);
            }
        }
    }

    private void SetHeroHeadIcon(Image img, CUIFormScript formScript, ResHeroCfgInfo heroCfgInfo)
    {
        if (((img != null) && (formScript != null)) && (heroCfgInfo != null))
        {
            string prefabPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + heroCfgInfo.szImagePath;
            img.SetSprite(prefabPath, formScript, true, false, false);
        }
    }

    private void Show(CSignalTips data, CUIFormScript formScript)
    {
        if (data != null)
        {
            this.signal_node.CustomSetActive(true);
            this.inbattlemsg_node.CustomSetActive(false);
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.m_heroID);
            if (dataByKey == null)
            {
                return;
            }
            this.SetHeroHeadIcon(this.leftIcon.gameObject.GetComponent<Image>(), formScript, dataByKey);
            string str = !data.m_isHostPlayer ? S_Bg_Blue : S_Bg_Green;
            if (this.bg_icon == null)
            {
                return;
            }
            Image component = this.bg_icon.GetComponent<Image>();
            if ((component != null) && !string.IsNullOrEmpty(str))
            {
                component.SetSprite(str, formScript, true, false, false);
            }
            ResSignalInfo info2 = GameDataMgr.signalDatabin.GetDataByKey((long) data.m_signalID);
            if (info2 == null)
            {
                return;
            }
            this.signal_icon.CustomSetActive(true);
            if (this.signal_icon == null)
            {
                return;
            }
            Image image = this.signal_icon.GetComponent<Image>();
            if (image != null)
            {
                image.SetSprite(info2.szUIIcon, formScript, true, false, false);
            }
            if (this.signal_txt != null)
            {
                this.signal_txt.text = info2.szText;
            }
            if (data.m_elementType < 1)
            {
                this.rightIcon.CustomSetActive(false);
                return;
            }
            Image image3 = this.rightIcon.GetComponent<Image>();
            if (image3 == null)
            {
                return;
            }
            switch (data.m_elementType)
            {
                case 1:
                    image3.SetSprite(S_kn_Tower_Icon, formScript, true, false, false);
                    goto Label_021C;

                case 2:
                    image3.SetSprite(S_Base_blue_Icon, formScript, true, false, false);
                    goto Label_021C;

                case 3:
                {
                    ResHeroCfgInfo heroCfgInfo = GameDataMgr.heroDatabin.GetDataByKey(data.m_targetHeroID);
                    if (heroCfgInfo != null)
                    {
                        this.SetHeroHeadIcon(image3, formScript, heroCfgInfo);
                        goto Label_021C;
                    }
                    return;
                }
                case 4:
                    image3.SetSprite(S_Dragon_big_Icon, formScript, true, false, false);
                    goto Label_021C;

                case 5:
                    image3.SetSprite(S_Dragon_small_Icon, formScript, true, false, false);
                    goto Label_021C;

                case 6:
                    image3.SetSprite(S_kn_dragon_Icon, formScript, true, false, false);
                    goto Label_021C;
            }
        }
        return;
    Label_021C:
        this.rightIcon.CustomSetActive(true);
    }

    private void Show(CSignalTips_InBatMsg data, CUIFormScript formScript)
    {
        if ((data != null) && (formScript != null))
        {
            if (this.signal_node != null)
            {
                this.signal_node.CustomSetActive(false);
            }
            if (this.inbattlemsg_node != null)
            {
                this.inbattlemsg_node.CustomSetActive(true);
            }
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.heroID);
            if ((dataByKey != null) && (this.leftIcon != null))
            {
                this.SetHeroHeadIcon(this.leftIcon.gameObject.GetComponent<Image>(), formScript, dataByKey);
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(data.playerID);
                if ((hostPlayer != null) && (playerByUid != null))
                {
                    string str = (playerByUid != hostPlayer) ? S_Bg_Blue : S_Bg_Green;
                    if (this.bg_icon != null)
                    {
                        Image component = this.bg_icon.GetComponent<Image>();
                        if ((component != null) && !string.IsNullOrEmpty(str))
                        {
                            component.SetSprite(str, formScript, true, false, false);
                        }
                        if (this.inbattlemsg_txt != null)
                        {
                            this.inbattlemsg_txt.text = data.content;
                            this.inbattlemsg_txt.gameObject.CustomSetActive(true);
                        }
                        if (this.rightIcon != null)
                        {
                            this.rightIcon.CustomSetActive(false);
                        }
                        if (this.signal_icon != null)
                        {
                            this.signal_icon.CustomSetActive(false);
                        }
                    }
                }
            }
        }
    }
}

