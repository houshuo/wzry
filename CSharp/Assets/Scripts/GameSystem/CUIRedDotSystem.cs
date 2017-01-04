namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUIRedDotSystem : Singleton<CUIRedDotSystem>
    {
        public static string s_redDotName = "redDot";
        public static string s_redVersionKey = "RedVer_";

        public static void AddRedDot(GameObject target, enRedDotPos dotPos = 2, int alertNum = 0)
        {
            if ((target != null) && (target.transform != null))
            {
                DelRedDot(target);
                Transform transform = null;
                GameObject obj2 = null;
                if (alertNum == 0)
                {
                    obj2 = UnityEngine.Object.Instantiate(CUIUtility.GetSpritePrefeb("UGUI/Form/Common/redDot", false, false)) as GameObject;
                }
                else
                {
                    obj2 = UnityEngine.Object.Instantiate(CUIUtility.GetSpritePrefeb("UGUI/Form/Common/redDotBig", false, false)) as GameObject;
                }
                transform = obj2.transform;
                transform.gameObject.name = s_redDotName;
                transform.GetComponent<CUIMiniEventScript>().m_onDownEventParams.tag = 0;
                if ((alertNum != 0) && (transform.Find("Text") != null))
                {
                    transform.Find("Text").GetComponent<Text>().text = alertNum.ToString();
                }
                transform.SetParent(target.transform, false);
                transform.SetAsLastSibling();
                RectTransform transform2 = transform as RectTransform;
                Vector2 vector = new Vector2();
                Vector2 vector2 = new Vector2();
                Vector2 vector3 = new Vector2();
                switch (dotPos)
                {
                    case enRedDotPos.enTopLeft:
                        vector.x = 0f;
                        vector.y = 1f;
                        vector2.x = 0f;
                        vector2.y = 1f;
                        vector3.x = 0f;
                        vector3.y = 1f;
                        break;

                    case enRedDotPos.enTopCenter:
                        vector.x = 0.5f;
                        vector.y = 1f;
                        vector2.x = 0.5f;
                        vector2.y = 1f;
                        vector3.x = 0.5f;
                        vector3.y = 1f;
                        break;

                    case enRedDotPos.enTopRight:
                        vector.x = 1f;
                        vector.y = 1f;
                        vector2.x = 1f;
                        vector2.y = 1f;
                        vector3.x = 1f;
                        vector3.y = 1f;
                        break;

                    case enRedDotPos.enMiddleLeft:
                        vector.x = 0f;
                        vector.y = 0.5f;
                        vector2.x = 0f;
                        vector2.y = 0.5f;
                        vector3.x = 0f;
                        vector3.y = 0.5f;
                        break;

                    case enRedDotPos.enMiddleCenter:
                        vector.x = 0.5f;
                        vector.y = 0.5f;
                        vector2.x = 0.5f;
                        vector2.y = 0.5f;
                        vector3.x = 0.5f;
                        vector3.y = 0.5f;
                        break;

                    case enRedDotPos.enMiddleRight:
                        vector.x = 1f;
                        vector.y = 0.5f;
                        vector2.x = 1f;
                        vector2.y = 0.5f;
                        vector3.x = 1f;
                        vector3.y = 0.5f;
                        break;

                    case enRedDotPos.enBottomLeft:
                        vector.x = 0f;
                        vector.y = 0f;
                        vector2.x = 0f;
                        vector2.y = 0f;
                        vector3.x = 0f;
                        vector3.y = 0f;
                        break;

                    case enRedDotPos.enBottomCenter:
                        vector.x = 0.5f;
                        vector.y = 0f;
                        vector2.x = 0.5f;
                        vector2.y = 0f;
                        vector3.x = 0.5f;
                        vector3.y = 0f;
                        break;

                    case enRedDotPos.enBottomRight:
                        vector.x = 1f;
                        vector.y = 0f;
                        vector2.x = 1f;
                        vector2.y = 0f;
                        vector3.x = 1f;
                        vector3.y = 0f;
                        break;
                }
                transform2.pivot = vector3;
                transform2.anchorMin = vector;
                transform2.anchorMax = vector2;
                transform2.anchoredPosition = new Vector2();
            }
        }

        public static void DelRedDot(GameObject target)
        {
            if ((target != null) && (target.transform != null))
            {
                CUIMiniEventScript[] componentsInChildren = target.transform.GetComponentsInChildren<CUIMiniEventScript>(true);
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    if ((componentsInChildren[i].m_onDownEventID == enUIEventID.Common_RedDotParsEvent) && (componentsInChildren[i].m_onDownEventParams.tag == 0))
                    {
                        componentsInChildren[i].m_onDownEventParams.tag = 1;
                        UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
                    }
                }
            }
        }

        public static uint GetRedIDVersionByServerData(enRedID redID, out bool isHaveData)
        {
            uint dwVerNum = 0;
            isHaveData = false;
            ResRedDotInfo info = new ResRedDotInfo();
            if (GameDataMgr.redDotInfoDict.TryGetValue((uint) redID, out info))
            {
                dwVerNum = info.dwVerNum;
                isHaveData = true;
            }
            return dwVerNum;
        }

        public static bool IsHaveRedDot(GameObject target)
        {
            if ((target == null) || (target.transform == null))
            {
                return false;
            }
            CUIMiniEventScript[] componentsInChildren = target.transform.GetComponentsInChildren<CUIMiniEventScript>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if ((componentsInChildren[i].m_onDownEventID == enUIEventID.Common_RedDotParsEvent) && (componentsInChildren[i].m_onDownEventParams.tag == 0))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsShowRedDotByLogic(enRedID redID)
        {
            bool flag = false;
            enRedID did = redID;
            if (did != enRedID.Mall_SymbolTab)
            {
                if (did != enRedID.Mall_MysteryTab)
                {
                    return flag;
                }
            }
            else
            {
                return Singleton<CMallSystem>.GetInstance().HasFreeDrawCnt(redID);
            }
            return Singleton<CMallMysteryShop>.GetInstance().IsShopAvailable();
        }

        public static bool IsShowRedDotByVersion(enRedID redID)
        {
            bool flag = false;
            bool isHaveData = false;
            uint redIDVersionByServerData = GetRedIDVersionByServerData(redID, out isHaveData);
            if (!isHaveData)
            {
                return flag;
            }
            string key = s_redVersionKey + ((int) redID);
            if (!PlayerPrefs.HasKey(key))
            {
                SetRedDotViewByVersion(redID);
                return flag;
            }
            string str2 = PlayerPrefs.GetString(key);
            if (str2 == null)
            {
                return flag;
            }
            char[] separator = new char[] { '_' };
            string[] strArray = str2.Split(separator);
            if (strArray.Length <= 1)
            {
                return flag;
            }
            uint result = 0;
            int num3 = 0;
            uint.TryParse(strArray[0], out result);
            int.TryParse(strArray[1], out num3);
            if ((num3 != 0) && (result == redIDVersionByServerData))
            {
                return flag;
            }
            return true;
        }

        public static void SetRedDotViewByVersion(enRedID redID)
        {
            bool isHaveData = false;
            uint redIDVersionByServerData = GetRedIDVersionByServerData(redID, out isHaveData);
            if (isHaveData)
            {
                string key = s_redVersionKey + ((int) redID);
                string str2 = redIDVersionByServerData + "_1";
                PlayerPrefs.SetString(key, str2);
                PlayerPrefs.Save();
            }
        }
    }
}

