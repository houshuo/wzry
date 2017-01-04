namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CSingleGameSettleView
    {
        public static void SetBurnSettleData(CUIFormScript form, ref COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
            SetWin(form.gameObject, settleData.stGameInfo.bGameResult == 1);
            int num = 1;
            int num2 = 1;
            int num3 = 0;
            int num4 = 0;
            string playerName = string.Empty;
            int playerLv = 1;
            COM_PLAYERCAMP playerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            string str2 = string.Empty;
            int num6 = 1;
            COM_PLAYERCAMP com_playercamp2 = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
            while (enumerator.MoveNext())
            {
                bool flag = true;
                if (settleData.stGameInfo.bGameResult == 1)
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    if (current.Value.IsHost)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    if (pair2.Value.IsHost)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    KeyValuePair<uint, PlayerKDA> pair3 = enumerator.Current;
                    ListView<HeroKDA>.Enumerator enumerator2 = pair3.Value.GetEnumerator();
                    while (enumerator2.MoveNext() && (num <= 3))
                    {
                        GameObject gameObject = form.gameObject.transform.Find("Panel/Left_Player" + num).gameObject;
                        KeyValuePair<uint, PlayerKDA> pair4 = enumerator.Current;
                        SetPlayerStat(form, gameObject, pair4.Value, enumerator2.Current);
                        num++;
                    }
                    KeyValuePair<uint, PlayerKDA> pair5 = enumerator.Current;
                    num3 += pair5.Value.numKill;
                    KeyValuePair<uint, PlayerKDA> pair6 = enumerator.Current;
                    playerName = pair6.Value.PlayerName;
                    KeyValuePair<uint, PlayerKDA> pair7 = enumerator.Current;
                    playerCamp = pair7.Value.PlayerCamp;
                    KeyValuePair<uint, PlayerKDA> pair8 = enumerator.Current;
                    playerLv = pair8.Value.PlayerLv;
                }
                else
                {
                    KeyValuePair<uint, PlayerKDA> pair9 = enumerator.Current;
                    ListView<HeroKDA>.Enumerator enumerator3 = pair9.Value.GetEnumerator();
                    while (enumerator3.MoveNext() && (num2 <= 3))
                    {
                        GameObject item = form.gameObject.transform.Find("Panel/Right_Player" + num2).gameObject;
                        KeyValuePair<uint, PlayerKDA> pair10 = enumerator.Current;
                        SetPlayerStat(form, item, pair10.Value, enumerator3.Current);
                        num2++;
                    }
                    KeyValuePair<uint, PlayerKDA> pair11 = enumerator.Current;
                    num4 += pair11.Value.numKill;
                    KeyValuePair<uint, PlayerKDA> pair12 = enumerator.Current;
                    str2 = pair12.Value.PlayerName;
                    KeyValuePair<uint, PlayerKDA> pair13 = enumerator.Current;
                    com_playercamp2 = pair13.Value.PlayerCamp;
                    KeyValuePair<uint, PlayerKDA> pair14 = enumerator.Current;
                    num6 = pair14.Value.PlayerLv;
                }
            }
            for (int i = num; i <= 3; i++)
            {
                form.gameObject.transform.Find("Panel/Left_Player" + i).gameObject.CustomSetActive(false);
            }
            for (int j = num2; j <= 3; j++)
            {
                form.gameObject.transform.Find("Panel/Right_Player" + j).gameObject.CustomSetActive(false);
            }
            Text component = form.gameObject.transform.Find("Panel/PanelABg/Image/ImageLeft/Txt_LeftPlayerName").gameObject.GetComponent<Text>();
            component.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = string.Format("Lv.{0}", playerLv);
            component.text = playerName;
            component.color = (playerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? new Color(0.545f, 0f, 0f, 1f) : new Color(0.031f, 0.961f, 0f, 1f);
            component = form.gameObject.transform.Find("Panel/PanelABg/Image/ImageRight/Txt_RightPlayerName").gameObject.GetComponent<Text>();
            component.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = string.Format("Lv.{0}", num6);
            component.text = str2;
            component.color = (com_playercamp2 != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? new Color(0.545f, 0f, 0f, 1f) : new Color(0.031f, 0.961f, 0f, 1f);
        }

        private static void SetPlayerStat(CUIFormScript formScript, GameObject item, PlayerKDA playerKDA, HeroKDA kda)
        {
            Text componetInChild = Utility.GetComponetInChild<Text>(item, "Txt_PlayerName");
            componetInChild.text = playerKDA.PlayerName;
            componetInChild.color = (playerKDA.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? new Color(0.545f, 0f, 0f, 1f) : new Color(0.031f, 0.961f, 0f, 1f);
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) kda.HeroId);
            DebugHelper.Assert(dataByKey != null);
            item.transform.Find("Txt_HeroName").gameObject.GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
            string str = (kda.numKill >= 10) ? kda.numKill.ToString() : string.Format(" {0} ", kda.numKill.ToString());
            string str2 = (kda.numDead >= 10) ? kda.numDead.ToString() : string.Format(" {0} ", kda.numDead.ToString());
            item.transform.Find("Txt_KDA").gameObject.GetComponent<Text>().text = string.Format("{0} / {1}", str, str2);
            item.transform.Find("Txt_Hurt").gameObject.GetComponent<Text>().text = kda.hurtToEnemy.ToString();
            item.transform.Find("Txt_HurtTaken").gameObject.GetComponent<Text>().text = kda.hurtTakenByEnemy.ToString();
            item.transform.Find("Txt_Heal").gameObject.GetComponent<Text>().text = kda.hurtToHero.ToString();
            item.transform.Find("KillerImg").gameObject.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint) kda.HeroId, 0)), formScript, true, false, false);
            item.CustomSetActive(true);
        }

        private static void SetWin(GameObject root, bool bWin)
        {
            Utility.FindChild(root, "Panel/PanelABg/Image/WinOrLose/win").CustomSetActive(bWin);
            Utility.FindChild(root, "Panel/PanelABg/Image/WinOrLose/lose").CustomSetActive(!bWin);
        }

        public static void ShowBurnWinLose(CUIFormScript form, bool bWin)
        {
            if (bWin)
            {
                form.gameObject.transform.Find("Win").gameObject.CustomSetActive(true);
            }
            else
            {
                form.gameObject.transform.Find("Lose").gameObject.CustomSetActive(true);
            }
        }
    }
}

