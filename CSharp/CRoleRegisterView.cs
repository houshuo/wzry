using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CRoleRegisterView
{
    public static void CloseGameDifSelectForm()
    {
        Singleton<CUIManager>.instance.CloseForm(CRoleRegisterSys.s_gameDifficultSelectFormPath);
    }

    public static void DeactivateInputField()
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
        if (form != null)
        {
            form.transform.FindChild("NameInputText").GetComponent<InputField>().DeactivateInputField();
        }
    }

    public static void OpenGameDifSelectForm()
    {
        CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(CRoleRegisterSys.s_gameDifficultSelectFormPath, false, true);
        if (script != null)
        {
            Utility.FindChild(script.gameObject, "ToggleGroup/Toggle1").GetComponent<CUIEventScript>().m_onClickEventParams.tag = 1;
            Utility.FindChild(script.gameObject, "ToggleGroup/Toggle2").GetComponent<CUIEventScript>().m_onClickEventParams.tag = 2;
            Utility.FindChild(script.gameObject, "ToggleGroup/Toggle3").GetComponent<CUIEventScript>().m_onClickEventParams.tag = 3;
            SetGameDifficult(0);
        }
    }

    public static void SetGameDifficult(int difficult)
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_gameDifficultSelectFormPath);
        if (form != null)
        {
            GameObject gameObject = Utility.FindChild(form.gameObject, "ConfirmBtn").gameObject;
            GameObject obj3 = Utility.FindChild(form.gameObject, "Panel/LevelContent").gameObject;
            gameObject.CustomSetActive(true);
            obj3.CustomSetActive(false);
            if (form != null)
            {
                gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.tag = difficult;
                switch (difficult)
                {
                    case 1:
                        gameObject.GetComponent<CUIEventScript>().enabled = true;
                        gameObject.GetComponent<Button>().interactable = true;
                        gameObject.GetComponentInChildren<Text>().color = Color.white;
                        return;

                    case 2:
                        gameObject.GetComponent<CUIEventScript>().enabled = true;
                        gameObject.GetComponent<Button>().interactable = true;
                        gameObject.GetComponentInChildren<Text>().color = Color.white;
                        return;

                    case 3:
                        gameObject.GetComponent<CUIEventScript>().enabled = true;
                        gameObject.GetComponent<Button>().interactable = true;
                        gameObject.GetComponentInChildren<Text>().color = Color.white;
                        return;
                }
                gameObject.GetComponent<CUIEventScript>().enabled = false;
                gameObject.GetComponent<Button>().interactable = false;
                gameObject.GetComponentInChildren<Text>().color = Color.gray;
            }
        }
    }

    public static string RoleName
    {
        get
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
            if (form != null)
            {
                return CUIUtility.RemoveEmoji(form.transform.FindChild("NameInputText").GetComponent<InputField>().text);
            }
            return string.Empty;
        }
        set
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
            if (form != null)
            {
                InputField component = form.transform.FindChild("NameInputText").GetComponent<InputField>();
                component.text = value;
                component.MoveTextEnd(false);
            }
        }
    }
}

