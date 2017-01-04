namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CHeadImg : CUseable
    {
        public ResHeadImage m_headImgData;

        public CHeadImg(ulong objID, uint baseID, int addTime = 0)
        {
            GameDataMgr.headImageDict.TryGetValue(baseID, out this.m_headImgData);
            if (this.m_headImgData == null)
            {
                Debug.Log("not HeadImg id" + baseID);
            }
            else
            {
                base.m_type = COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG;
                base.m_objID = objID;
                base.m_baseID = baseID;
                base.m_name = StringHelper.UTF8BytesToString(ref this.m_headImgData.szHeadDesc);
                base.m_description = StringHelper.UTF8BytesToString(ref this.m_headImgData.szHeadDesc);
                base.m_goldCoinBuy = this.m_headImgData.dwPVPCoinBuy;
                base.m_dianQuanBuy = this.m_headImgData.dwCouponsBuy;
                base.m_diamondBuy = this.m_headImgData.dwDiamondBuy;
                base.m_guildCoinBuy = this.m_headImgData.dwGuildCoinBuy;
                base.m_dianQuanDirectBuy = 0;
                base.m_stackMax = 1;
                base.m_addTime = addTime;
                base.ResetTime();
            }
        }

        public override string GetIconPath()
        {
            return string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Nobe_Dir, this.m_headImgData.szHeadIcon);
        }
    }
}

