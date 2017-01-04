namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUIUtility
    {
        public const int c_defaultLayer = 0;
        public const int c_formBaseHeight = 640;
        public const int c_formBaseWidth = 960;
        public const int c_hideLayer = 0x1f;
        public const int c_UIBottomBg = 0x12;
        public const int c_uiLayer = 5;
        public static Color Intimacy_Freeze = new Color(0.8078431f, 0.8117647f, 0.8823529f);
        public static Color Intimacy_Full = new Color(1f, 0.09411765f, 0f);
        public static Color Intimacy_High = new Color(1f, 0.09411765f, 1f);
        public static Color Intimacy_Low = new Color(1f, 0.8745098f, 0.1803922f);
        public static Color Intimacy_Mid = new Color(1f, 0.5215687f, 0.1333333f);
        public static string s_Animation3D_Dir = "UGUI/Animation/";
        public static string s_battleResultBgPath = "UIScene_BattleResult";
        public static string s_battleSignalPrefabDir = "UGUI/Sprite/Battle/Signal/";
        public static Color s_Color_Button_Disable = new Color(0.3843137f, 0.3843137f, 0.3843137f, 0.9019608f);
        public static Color s_Color_DisableGray = new Color(0.3921569f, 0.3921569f, 0.3921569f, 1f);
        public static Color s_Color_Full = new Color(1f, 1f, 1f, 1f);
        public static Color s_Color_GrayShader = new Color(0f, 1f, 1f);
        public static Color s_Color_Grey = new Color(0.3137255f, 0.3137255f, 0.3137255f);
        public static Color s_Color_White = new Color(1f, 1f, 1f);
        public static Color s_Color_White_FullAlpha = new Color(1f, 1f, 1f, 0f);
        public static Color s_Color_White_HalfAlpha = new Color(1f, 1f, 1f, 0.4901961f);
        public static string s_Form_Activity_Dir = "UGUI/Form/System/OpActivity/";
        public const string s_Form_Battle_Dir = "UGUI/Form/Battle/";
        public const string s_Form_Common_Dir = "UGUI/Form/Common/";
        public const string s_Form_System_Dir = "UGUI/Form/System/";
        public static string s_heroSceneBgPath = "UIScene_HeroInfo";
        public static string s_heroSelectBgPath = "UIScene_HeroSelect";
        public static string s_IDIP_Form_Dir = "UGUI/Form/System/IDIPNotice/";
        public static string s_lotterySceneBgPath = "UIScene_Lottery";
        public static string s_Particle_Dir = "UGUI/Particle/";
        public static string s_recommendHeroInfoBgPath = "UIScene_Recommend_HeroInfo";
        private static readonly Regex s_regexEmoji = new Regex(@"\ud83c[\udf00-\udfff]|\ud83d[\udc00-\udeff]|\ud83d[\ude80-\udeff]");
        public static string s_Sprite_Activity_Dir = "UGUI/Sprite/System/OpActivity/";
        public const string s_Sprite_Battle_Dir = "UGUI/Sprite/Battle/";
        public const string s_Sprite_Common_Dir = "UGUI/Sprite/Common/";
        public static string s_Sprite_Dynamic_Achieve_Dir = "UGUI/Sprite/Dynamic/Achieve/";
        public static string s_Sprite_Dynamic_ActivityPve_Dir = "UGUI/Sprite/Dynamic/ActivityPve/";
        public static string s_Sprite_Dynamic_AddedSkill_Dir = "UGUI/Sprite/Dynamic/AddedSkill/";
        public static string s_Sprite_Dynamic_Adventure_Dir = "UGUI/Sprite/Dynamic/Adventure/";
        public static string s_Sprite_Dynamic_BustCircle_Dir = "UGUI/Sprite/Dynamic/BustCircle/";
        public static string s_Sprite_Dynamic_BustCircleSmall_Dir = "UGUI/Sprite/Dynamic/BustCircleSmall/";
        public static string s_Sprite_Dynamic_BustHero_Dir = "UGUI/Sprite/Dynamic/BustHero/";
        public static string s_Sprite_Dynamic_BustPlayer_Dir = "UGUI/Sprite/Dynamic/BustPlayer/";
        public static string s_Sprite_Dynamic_Dialog_Dir = "UGUI/Sprite/Dynamic/Dialog/";
        public static string s_Sprite_Dynamic_Dialog_Dir_Head = (s_Sprite_Dynamic_Dialog_Dir + "Heads/");
        public static string s_Sprite_Dynamic_Dialog_Dir_Portrait = (s_Sprite_Dynamic_Dialog_Dir + "Portraits/");
        public const string s_Sprite_Dynamic_Dir = "UGUI/Sprite/Dynamic/";
        public static string s_Sprite_Dynamic_ExperienceCard_Dir = "UGUI/Sprite/Dynamic/ExperienceCard/";
        public static string s_Sprite_Dynamic_FucUnlock_Dir = "UGUI/Sprite/Dynamic/FunctionUnlock/";
        public static string s_Sprite_Dynamic_Guild_Dir = "UGUI/Sprite/Dynamic/Guild/";
        public static string s_Sprite_Dynamic_GuildHead_Dir = "UGUI/Sprite/Dynamic/GuildHead/";
        public static string s_Sprite_Dynamic_Icon_Dir = "UGUI/Sprite/Dynamic/Icon/";
        public static string s_Sprite_Dynamic_Map_Dir = "UGUI/Sprite/Dynamic/Map/";
        public static string s_Sprite_Dynamic_Newbie_Dir = "UGUI/Sprite/Dynamic/Newbie/";
        public static string s_Sprite_Dynamic_Nobe_Dir = "UGUI/Sprite/Dynamic/Nobe/";
        public static string s_Sprite_Dynamic_Profession_Dir = "UGUI/Sprite/Dynamic/Profession/";
        public static string s_Sprite_Dynamic_Proficiency_Dir = "UGUI/Sprite/Dynamic/HeroProficiency/";
        public static string s_Sprite_Dynamic_Purchase_Dir = "UGUI/Sprite/Dynamic/Purchase/";
        public static string s_Sprite_Dynamic_Pvp_Settle_Dir = "UGUI/Sprite/System/PvpIcon/";
        public static string s_Sprite_Dynamic_Pvp_Settle_Large_Dir = "UGUI/Sprite/System/PvpIconLarge/";
        public static string s_Sprite_Dynamic_PvpAchievementShare_Dir = "UGUI/Sprite/Dynamic/PvpShare/";
        public static string s_Sprite_Dynamic_PvpEntry_Dir = "UGUI/Sprite/Dynamic/PvpEntry/";
        public static string s_Sprite_Dynamic_PvPTitle_Dir = "UGUI/Sprite/Dynamic/PvPTitle/";
        public const string s_Sprite_Dynamic_Quality_Dir = "UGUI/Sprite/Dynamic/Quality/";
        public static string s_Sprite_Dynamic_Skill_Dir = "UGUI/Sprite/Dynamic/Skill/";
        public static string s_Sprite_Dynamic_SkinFeature_Dir = "UGUI/Sprite/Dynamic/SkinFeature/";
        public static string s_Sprite_Dynamic_SkinQuality_Dir = "UGUI/Sprite/Dynamic/SkinQuality/";
        public static string s_Sprite_Dynamic_Talent_Dir = "UGUI/Sprite/Dynamic/Skill/";
        public static string s_Sprite_Dynamic_Task_Dir = "UGUI/Sprite/Dynamic/Task/";
        public static string s_Sprite_Dynamic_UnionBattleBaner_Dir = "UGUI/Sprite/Dynamic/UnionBattleBaner/";
        public static string s_Sprite_HeroInfo_Dir = "UGUI/Sprite/Dynamic/HeroInfo/";
        public static string s_Sprite_System_BattleEquip_Dir = "UGUI/Sprite/System/BattleEquip/";
        public static string s_Sprite_System_Burn_Dir = "UGUI/Sprite/System/BurnExpedition/";
        public const string s_Sprite_System_Dir = "UGUI/Sprite/System/";
        public static string s_Sprite_System_Equip_Dir = "UGUI/Sprite/System/Equip/";
        public static string s_Sprite_System_HeroSelect_Dir = "UGUI/Sprite/System/HeroSelect/";
        public static string s_Sprite_System_Honor_Dir = "UGUI/Sprite/System/Honor/";
        public static string s_Sprite_System_Ladder_Dir = "UGUI/Sprite/System/Ladder/";
        public static string s_Sprite_System_Lobby_Dir = "UGUI/Sprite/System/LobbyDynamic/";
        public static string s_Sprite_System_Mall_Dir = "UGUI/Sprite/System/Mall/";
        public static string s_Sprite_System_Qualifying_Dir = "UGUI/Sprite/System/Qualifying/";
        public static string s_Sprite_System_ShareUI_Dir = "UGUI/Sprite/System/ShareUI/";
        public static string s_Sprite_System_Wifi_Dir = "UGUI/Sprite/System/Wifi/";
        public static Color s_Text_Color_Camp_1 = new Color(0.4039216f, 0.6039216f, 0.9686275f);
        public static Color s_Text_Color_Camp_2 = new Color(0.8588235f, 0.1803922f, 0.282353f);
        public static Color s_Text_Color_Camp_Allies = new Color(0.3529412f, 0.5490196f, 0.8352941f);
        public static Color s_Text_Color_Camp_Enemy = new Color(0.6862745f, 0.1607843f, 0.2352941f);
        public static Color s_Text_Color_Can_Afford = Color.white;
        public static Color s_Text_Color_Can_Not_Afford = new Color(0.6784314f, 0.227451f, 0.2039216f);
        public static Color s_Text_Color_CommonGray = new Color(0.7019608f, 0.7058824f, 0.7137255f);
        public static Color s_Text_Color_Disable = new Color(0.6039216f, 0.6f, 0.6f);
        public static Color[] s_Text_Color_Hero_Advance = new Color[] { new Color(1f, 1f, 1f), new Color(0.3882353f, 0.9019608f, 0.2392157f), new Color(0.1176471f, 0.6431373f, 0.9137255f), new Color(0.7647059f, 0.3372549f, 0.8235294f), new Color(0.9490196f, 0.4666667f, 0.07058824f) };
        public static Color s_Text_Color_Hero_Name_Active = new Color(0.9254902f, 0.8509804f, 0.6431373f);
        public static Color s_Text_Color_Hero_Name_DeActive = new Color(0.4f, 0.3647059f, 0.2705882f);
        public static Color s_Text_Color_ListElement_Normal = new Color(0.4941176f, 0.5333334f, 0.6352941f);
        public static Color s_Text_Color_ListElement_Select = new Color(1f, 1f, 1f);
        public static Color s_Text_Color_Self = new Color(0.9490196f, 0.7882353f, 0.3019608f);
        public static Color s_Text_Color_Vip_Chat_Other = new Color(0.7764706f, 0.6509804f, 0.3137255f);
        public static Color s_Text_Color_Vip_Chat_Self = new Color(1f, 0.8941177f, 0f);
        public static Color s_Text_Color_White = new Color(1f, 1f, 1f);

        public static T GetComponentInChildren<T>(GameObject go) where T: Component
        {
            if (go != null)
            {
                T component = go.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
                Transform transform = go.transform;
                int childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    component = GetComponentInChildren<T>(transform.GetChild(i).gameObject);
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return null;
        }

        public static void GetComponentsInChildren<T>(GameObject go, T[] components, ref int count) where T: Component
        {
            T component = go.GetComponent<T>();
            if (component != null)
            {
                components[count] = component;
                count++;
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                GetComponentsInChildren<T>(go.transform.GetChild(i).gameObject, components, ref count);
            }
        }

        public static Vector2 GetFixedTextSize(Text text, string content, float fixedWidth)
        {
            return Vector2.zero;
        }

        public static GameObject GetSpritePrefeb(string prefebPath, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
        {
            GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(prefebPath, typeof(GameObject), enResourceType.UISprite, needCached, unloadBelongedAssetBundleAfterLoaded).m_content as GameObject;
            if (content == null)
            {
                content = Singleton<CResourceManager>.GetInstance().GetResource(s_Sprite_Dynamic_Icon_Dir + "0", typeof(GameObject), enResourceType.UISprite, true, true).m_content as GameObject;
            }
            return content;
        }

        public static string RemoveEmoji(string str)
        {
            return s_regexEmoji.Replace(str, string.Empty);
        }

        public static void ResetUIScale(GameObject target)
        {
            Vector3 localScale = target.transform.localScale;
            Transform parent = target.transform.parent;
            target.transform.SetParent(null);
            target.transform.localScale = localScale;
            target.transform.SetParent(parent);
        }

        public static Vector3 ScreenToWorldPoint(Camera camera, Vector2 screenPoint, float z)
        {
            return ((camera != null) ? camera.ViewportToWorldPoint(new Vector3(screenPoint.x / ((float) Screen.width), screenPoint.y / ((float) Screen.height), z)) : new Vector3(screenPoint.x, screenPoint.y, z));
        }

        public static void SetGameObjectLayer(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                SetGameObjectLayer(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }

        public static void SetImageGrey(Graphic graphic, bool isSetGrey)
        {
            SetImageGrey(graphic, isSetGrey, Color.white);
        }

        private static void SetImageGrey(Graphic graphic, bool isSetGrey, Color defaultColor)
        {
            if (graphic != null)
            {
                graphic.color = !isSetGrey ? defaultColor : s_Color_Grey;
            }
        }

        public static void SetImageSprite(Image image, GameObject prefab)
        {
            if (image != null)
            {
                if (prefab == null)
                {
                    image.sprite = null;
                }
                else
                {
                    SpriteRenderer component = prefab.GetComponent<SpriteRenderer>();
                    if (component != null)
                    {
                        image.sprite = component.sprite;
                    }
                    if (image is Image2)
                    {
                        SGameSpriteSettings settings = prefab.GetComponent<SGameSpriteSettings>();
                        Image2 image2 = image as Image2;
                        image2.alphaTexLayout = (settings == null) ? ImageAlphaTexLayout.None : settings.layout;
                    }
                }
            }
        }

        public static void SetImageSprite(Image image, Image targetImage)
        {
            if (image != null)
            {
                if (targetImage == null)
                {
                    image.sprite = null;
                }
                else
                {
                    image.sprite = targetImage.sprite;
                    if (image is Image2)
                    {
                        Image2 image2 = image as Image2;
                        image2.alphaTexLayout = ImageAlphaTexLayout.None;
                        if (targetImage is Image2)
                        {
                            Image2 image3 = targetImage as Image2;
                            image2.alphaTexLayout = image3.alphaTexLayout;
                        }
                    }
                }
            }
        }

        public static void SetImageSprite(Image image, string prefabPath, CUIFormScript formScript, bool loadSync = true, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
        {
            if (image != null)
            {
                if (loadSync)
                {
                    SetImageSprite(image, GetSpritePrefeb(prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded));
                }
                else
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
                    formScript.AddASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded);
                }
            }
        }

        public static string StringReplace(string scrStr, params string[] values)
        {
            return string.Format(scrStr, (object[]) values);
        }

        public static float ValueInRange(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        public static Vector2 WorldToScreenPoint(Camera camera, Vector3 worldPoint)
        {
            return ((camera != null) ? camera.WorldToScreenPoint(worldPoint) : new Vector2(worldPoint.x, worldPoint.y));
        }
    }
}

