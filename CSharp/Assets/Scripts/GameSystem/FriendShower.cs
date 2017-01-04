namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class FriendShower : MonoBehaviour
    {
        public GameObject add_node;
        public GameObject black_node;
        public GameObject del_node;
        public Text distanceTxt;
        public uint dwLogicWorldID;
        public CUIFormScript formScript;
        public GameObject freeze;
        public GameObject full;
        public GameObject genderImage;
        public GameObject Giftutton;
        public Image headIcon;
        public GameObject HeadIconBack;
        public GameObject high;
        public CUIHttpImageScript HttpImage;
        public GameObject intimacyNode;
        public Text intimacyNum;
        private static uint IntimacyNumMax;
        public CUIEventScript inviteGuildBtn_eventScript;
        public Text inviteGuildBtnText;
        public Button inviteGuildButton;
        public GameObject lbs_node;
        public Button lbsAddFriendBtn;
        public GameObject lbsBodyNode;
        public Text LevelText;
        public GameObject low;
        public GameObject mid;
        public Text NameText;
        public GameObject nobeIcon;
        public GameObject normal_node;
        public Button OBButton;
        public Button PKButton;
        public GameObject platChannelIcon;
        public Image pvpIcon;
        public Text pvpText;
        public GameObject QQVipImage;
        public CUIEventScript reCallBtn_eventScript;
        public Button reCallButton;
        public Text reCallText;
        public GameObject request_node;
        public Text SendBtnText;
        public CUIEventScript sendHeartBtn_eventScript;
        public Button sendHeartButton;
        public Image sendHeartIcon;
        public Text time;
        public ulong ullUid;
        public Text VerifyText;
        public Text VipLevel;

        public void HideSendButton()
        {
            if (this.sendHeartButton != null)
            {
                this.sendHeartButton.gameObject.CustomSetActive(false);
            }
        }

        public void SetBGray(bool bGray)
        {
            UT.SetImage(this.headIcon, bGray);
        }

        public void SetFriendItemType(ItemType type, bool bShowDelete = true)
        {
            if (this.inviteGuildButton != null)
            {
                this.inviteGuildButton.gameObject.CustomSetActive(false);
            }
            if (type == ItemType.Add)
            {
                this.add_node.CustomSetActive(true);
                this.normal_node.CustomSetActive(false);
                this.request_node.CustomSetActive(false);
                this.black_node.CustomSetActive(false);
                if (this.lbs_node != null)
                {
                    this.lbs_node.CustomSetActive(false);
                }
            }
            else if (type == ItemType.Normal)
            {
                this.add_node.CustomSetActive(false);
                this.request_node.CustomSetActive(false);
                this.normal_node.CustomSetActive(true);
                this.black_node.CustomSetActive(false);
                if (this.del_node != null)
                {
                    this.del_node.CustomSetActive(bShowDelete);
                }
                if (this.lbs_node != null)
                {
                    this.lbs_node.CustomSetActive(false);
                }
            }
            else if (type == ItemType.Request)
            {
                this.add_node.CustomSetActive(false);
                this.normal_node.CustomSetActive(false);
                this.request_node.CustomSetActive(true);
                this.black_node.CustomSetActive(false);
                if (this.lbs_node != null)
                {
                    this.lbs_node.CustomSetActive(false);
                }
            }
            else if (type == ItemType.BlackList)
            {
                this.add_node.CustomSetActive(false);
                this.normal_node.CustomSetActive(false);
                this.request_node.CustomSetActive(false);
                this.black_node.CustomSetActive(true);
                if (this.lbs_node != null)
                {
                    this.lbs_node.CustomSetActive(false);
                }
            }
            else if (type == ItemType.LBS)
            {
                this.add_node.CustomSetActive(false);
                this.normal_node.CustomSetActive(false);
                this.request_node.CustomSetActive(false);
                this.black_node.CustomSetActive(false);
                if (this.lbs_node != null)
                {
                    this.lbs_node.CustomSetActive(true);
                }
            }
            if (this.VerifyText != null)
            {
                this.VerifyText.transform.parent.gameObject.CustomSetActive(type == ItemType.Request);
            }
            if (this.lbsBodyNode != null)
            {
                this.lbsBodyNode.gameObject.CustomSetActive(type == ItemType.LBS);
            }
        }

        public void ShowDistance(string txt)
        {
            if (this.distanceTxt != null)
            {
                this.distanceTxt.text = txt;
            }
        }

        public void ShowGameState(COM_ACNT_GAME_STATE state, bool bOnline)
        {
            if ((state != COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE) && bOnline)
            {
                this.ShowLastTime(true, "游戏中");
            }
        }

        public void ShowGenderType(COM_SNSGENDER genderType)
        {
            if (this.genderImage != null)
            {
                this.genderImage.gameObject.CustomSetActive(genderType != COM_SNSGENDER.COM_SNSGENDER_NONE);
                if (genderType == COM_SNSGENDER.COM_SNSGENDER_MALE)
                {
                    CUIUtility.SetImageSprite(this.genderImage.GetComponent<Image>(), string.Format("{0}icon/Ico_boy", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                }
                else if (genderType == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
                {
                    CUIUtility.SetImageSprite(this.genderImage.GetComponent<Image>(), string.Format("{0}icon/Ico_girl", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                }
            }
        }

        public void ShowIntimacyNum(int value, CFriendModel.EIntimacyType type, bool bFreeze)
        {
            if (IntimacyNumMax == 0)
            {
                IntimacyNumMax = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xc3).dwConfValue;
            }
            if (this.intimacyNum != null)
            {
                if (value >= IntimacyNumMax)
                {
                    this.intimacyNum.text = "Max";
                }
                else
                {
                    this.intimacyNum.text = value.ToString();
                }
            }
            if (bFreeze)
            {
                if (this.freeze != null)
                {
                    this.freeze.CustomSetActive(true);
                }
                this.intimacyNum.color = CUIUtility.Intimacy_Freeze;
            }
            else
            {
                if (this.freeze != null)
                {
                    this.freeze.CustomSetActive(false);
                }
                if (type == CFriendModel.EIntimacyType.Low)
                {
                    if (this.full != null)
                    {
                        this.full.CustomSetActive(false);
                    }
                    if (this.high != null)
                    {
                        this.high.CustomSetActive(false);
                    }
                    if (this.mid != null)
                    {
                        this.mid.CustomSetActive(false);
                    }
                    if (this.low != null)
                    {
                        this.low.CustomSetActive(true);
                    }
                    this.intimacyNum.color = CUIUtility.Intimacy_Low;
                }
                else if (type == CFriendModel.EIntimacyType.Middle)
                {
                    if (this.full != null)
                    {
                        this.full.CustomSetActive(false);
                    }
                    if (this.high != null)
                    {
                        this.high.CustomSetActive(false);
                    }
                    if (this.mid != null)
                    {
                        this.mid.CustomSetActive(true);
                    }
                    if (this.low != null)
                    {
                        this.low.CustomSetActive(false);
                    }
                    this.intimacyNum.color = CUIUtility.Intimacy_Mid;
                }
                else if (type == CFriendModel.EIntimacyType.High)
                {
                    if (this.full != null)
                    {
                        this.full.CustomSetActive(false);
                    }
                    if (this.high != null)
                    {
                        this.high.CustomSetActive(true);
                    }
                    if (this.mid != null)
                    {
                        this.mid.CustomSetActive(false);
                    }
                    if (this.low != null)
                    {
                        this.low.CustomSetActive(false);
                    }
                    this.intimacyNum.color = CUIUtility.Intimacy_High;
                }
                else if (type == CFriendModel.EIntimacyType.full)
                {
                    if (this.full != null)
                    {
                        this.full.CustomSetActive(true);
                    }
                    if (this.high != null)
                    {
                        this.high.CustomSetActive(false);
                    }
                    if (this.mid != null)
                    {
                        this.mid.CustomSetActive(false);
                    }
                    if (this.low != null)
                    {
                        this.low.CustomSetActive(false);
                    }
                    this.intimacyNum.color = CUIUtility.Intimacy_Full;
                }
            }
        }

        public void ShowInviteButton(bool isShow, bool isEnable)
        {
            if (this.reCallButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked)
                {
                    this.reCallButton.gameObject.CustomSetActive(false);
                }
                else if (!isShow)
                {
                    this.reCallButton.gameObject.CustomSetActive(false);
                }
                else
                {
                    if (this.reCallText != null)
                    {
                        if (isEnable)
                        {
                            this.reCallText.text = Singleton<CTextManager>.instance.GetText("Friend_ReCall_Tips_1");
                        }
                        else
                        {
                            this.reCallText.text = Singleton<CTextManager>.instance.GetText("Friend_ReCall_Tips_2");
                        }
                    }
                    this.reCallButton.gameObject.CustomSetActive(true);
                    if (this.reCallBtn_eventScript != null)
                    {
                        this.reCallBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_SNS_ReCall);
                    }
                    CUICommonSystem.SetButtonEnableWithShader(this.reCallButton, isEnable, true);
                }
            }
        }

        public void ShowinviteGuild(bool isShow, bool isEnable)
        {
            if (this.inviteGuildButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked && this.inviteGuildButton.gameObject.activeSelf)
                {
                    this.inviteGuildButton.gameObject.SetActive(false);
                }
                else if (!isShow)
                {
                    this.inviteGuildButton.gameObject.CustomSetActive(false);
                }
                else
                {
                    this.inviteGuildButton.gameObject.CustomSetActive(true);
                    CUICommonSystem.SetButtonEnableWithShader(this.inviteGuildButton, isEnable, true);
                    if (this.inviteGuildBtn_eventScript != null)
                    {
                        this.inviteGuildBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_InviteGuild);
                    }
                }
            }
        }

        public void ShowLastTime(bool bShow, string text)
        {
            if (this.time != null)
            {
                this.time.gameObject.CustomSetActive(bShow);
                this.time.text = text;
            }
        }

        public void ShowLevel(uint level)
        {
            if (this.LevelText != null)
            {
                this.LevelText.text = "LV." + level.ToString();
            }
        }

        public void ShowName(string name)
        {
            if (this.NameText != null)
            {
                this.NameText.text = name;
            }
            if (this.pvpText != null)
            {
                this.pvpText.gameObject.CustomSetActive(false);
            }
            if (this.pvpIcon != null)
            {
                this.pvpIcon.gameObject.CustomSetActive(false);
            }
            if (this.sendHeartIcon != null)
            {
                this.sendHeartIcon.gameObject.CustomSetActive(false);
            }
        }

        public void ShowOBButton(bool isShow)
        {
            if ((this.OBButton != null) && (this.OBButton.gameObject != null))
            {
                this.OBButton.gameObject.CustomSetActive(isShow);
            }
        }

        public void ShowPKButton(bool b)
        {
            if (this.PKButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked && this.PKButton.gameObject.activeSelf)
                {
                    this.PKButton.gameObject.SetActive(false);
                }
                else if (!b)
                {
                    if (this.PKButton.gameObject.activeSelf)
                    {
                        this.PKButton.gameObject.SetActive(false);
                    }
                }
                else if (!this.PKButton.gameObject.activeSelf)
                {
                    this.PKButton.gameObject.SetActive(true);
                }
            }
        }

        public void ShowPlatChannelIcon(COMDT_FRIEND_INFO info)
        {
            if (this.platChannelIcon != null)
            {
                this.platChannelIcon.CustomSetActive(!Utility.IsSamePlatform(info.stUin.dwLogicWorldId));
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    this.platChannelIcon.CustomSetActive(false);
                }
            }
        }

        public void ShowPVP_Level(string text, string icon)
        {
        }

        public void ShowRank(byte RankGrade, uint RankClass)
        {
        }

        public void ShowRecommendGuild(bool isShow, bool isEnabled)
        {
            if (this.inviteGuildButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked && this.inviteGuildButton.gameObject.activeSelf)
                {
                    this.inviteGuildButton.gameObject.SetActive(false);
                }
                else if (!isShow)
                {
                    if (this.inviteGuildButton.gameObject.activeSelf)
                    {
                        this.inviteGuildButton.gameObject.CustomSetActive(false);
                    }
                }
                else
                {
                    if (!this.inviteGuildButton.gameObject.activeSelf)
                    {
                        this.inviteGuildButton.gameObject.CustomSetActive(true);
                    }
                    if (this.inviteGuildBtn_eventScript != null)
                    {
                        this.inviteGuildBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_RecommendGuild);
                    }
                    if (isEnabled)
                    {
                        CUICommonSystem.SetButtonEnable(this.inviteGuildButton, true, true, true);
                        if (this.inviteGuildBtnText != null)
                        {
                            this.inviteGuildBtnText.text = Singleton<CFriendContoller>.instance.model.Guild_Recommend_txt;
                        }
                    }
                    else
                    {
                        CUICommonSystem.SetButtonEnable(this.inviteGuildButton, false, false, true);
                        if (this.inviteGuildBtnText != null)
                        {
                            this.inviteGuildBtnText.text = Singleton<CFriendContoller>.instance.model.Guild_Has_Recommended_txt;
                        }
                    }
                }
            }
        }

        public void ShowSendButton(bool bEnable)
        {
            if ((this.sendHeartButton != null) && (this.sendHeartButton.gameObject != null))
            {
                CUICommonSystem.SetButtonEnableWithShader(this.sendHeartButton, bEnable, true);
                this.sendHeartButton.gameObject.CustomSetActive(true);
            }
        }

        public void ShowSendGiftBtn(bool bShow)
        {
            if (this.Giftutton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked)
                {
                    this.Giftutton.CustomSetActive(false);
                }
                else
                {
                    this.Giftutton.CustomSetActive(bShow);
                }
            }
        }

        public void ShowVerify(string text)
        {
            if (this.VerifyText != null)
            {
                this.VerifyText.text = Singleton<CFriendContoller>.instance.model.friend_static_text + text;
            }
        }

        public void ShowVipLevel(uint level)
        {
            if (this.VipLevel != null)
            {
                this.VipLevel.text = "VIP." + level.ToString();
            }
        }

        public enum ItemType
        {
            Add,
            Normal,
            Request,
            BlackList,
            LBS
        }
    }
}

