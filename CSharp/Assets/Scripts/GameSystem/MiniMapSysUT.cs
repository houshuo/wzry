namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MiniMapSysUT
    {
        private const int ATK = 2;
        private const int DEF = 3;
        private const int PROTECT = 5;

        public static void RefreshMapPointerBig(GameObject go)
        {
            CUIFormScript formScript = Singleton<CBattleSystem>.GetInstance().FormScript;
            if (((formScript != null) && (formScript.m_sgameGraphicRaycaster != null)) && (go != null))
            {
                formScript.m_sgameGraphicRaycaster.RefreshGameObject(go);
            }
        }

        public static void SetMapElement_EventParam(GameObject obj, bool bAlien, MinimapSys.ElementType type, uint objID = 0, uint targetHeroID = 0)
        {
            if ((type != MinimapSys.ElementType.None) && (obj != null))
            {
                CUIEventScript component = obj.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    stUIEventParams @params = new stUIEventParams {
                        tag = !bAlien ? 0 : 1,
                        tag2 = (int) type,
                        tagUInt = objID,
                        heroId = targetHeroID
                    };
                    if (((type == MinimapSys.ElementType.Dragon_3) || (type == MinimapSys.ElementType.Dragon_5_big)) || (type == MinimapSys.ElementType.Dragon_5_small))
                    {
                        @params.tag3 = 2;
                    }
                    else if ((type == MinimapSys.ElementType.Tower) || (type == MinimapSys.ElementType.Base))
                    {
                        @params.tag3 = !bAlien ? 2 : 3;
                    }
                    else if (type == MinimapSys.ElementType.Hero)
                    {
                        @params.tag3 = !bAlien ? 2 : 5;
                    }
                    component.m_onClickEventParams = @params;
                }
            }
        }
    }
}

