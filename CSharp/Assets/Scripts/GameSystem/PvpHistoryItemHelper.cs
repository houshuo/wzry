namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class PvpHistoryItemHelper : MonoBehaviour
    {
        public GameObject equipObj;
        public GameObject FriendItem;
        public GameObject headObj;
        public GameObject KDAText;
        public GameObject MatchTypeText;
        public GameObject Mvp;
        public GameObject reSesultText;
        public GameObject ShowDetailBtn;
        public GameObject time;

        public void SetEuipItems(ref COMDT_PLAYER_FIGHT_DATA playerData, CUIFormScript form)
        {
            if ((playerData != null) && (form != null))
            {
                for (int i = 0; i < 6; i++)
                {
                    COMDT_INGAME_EQUIP_INFO comdt_ingame_equip_info = playerData.astEquipDetail[i];
                    int num2 = i + 1;
                    Image component = this.equipObj.transform.FindChild(string.Format("TianFuIcon{0}", num2.ToString())).GetComponent<Image>();
                    if ((comdt_ingame_equip_info.dwEquipID == 0) || (comdt_ingame_equip_info == null))
                    {
                        component.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        component.gameObject.CustomSetActive(true);
                        CUICommonSystem.SetEquipIcon((ushort) comdt_ingame_equip_info.dwEquipID, component.gameObject, form);
                    }
                }
            }
        }
    }
}

