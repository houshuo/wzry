using Assets.Scripts.UI;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class NewbieBannerIntroDialog
{
    public static string BANNER_INTRO_DIALOG_PATH = "UGUI/Form/System/Newbie/Form_BannerIntroDialog";
    private bool m_bAutoMove;
    private GameObject m_BottomBtn;
    private GameObject m_BtnLeft;
    private string m_btnName;
    private GameObject m_BtnRight;
    private int m_curImgIndex;
    private CUIFormScript m_form;
    private string[] m_imgPath;
    private Vector2 m_lastPos = Vector2.zero;
    private int[] m_PickIdxList;
    private GameObject m_PickObject;
    private CUIStepListScript m_stepList;
    private CUITimerScript m_timer;
    private string m_title;
    private int m_totalImgNum;
    private CUIEvent m_uiEventParam;

    public void AutoMove()
    {
        if (this.m_bAutoMove)
        {
            this.MoveToNextPage();
        }
    }

    public void Clear()
    {
        this.m_imgPath = null;
        this.m_title = null;
        this.m_btnName = null;
        this.m_PickIdxList = null;
        this.m_BottomBtn = null;
        this.m_stepList = null;
        this.m_form = null;
        this.m_timer = null;
        this.m_PickObject = null;
        this.m_BtnLeft = null;
        this.m_BtnRight = null;
    }

    public void Confirm()
    {
        if (this.m_uiEventParam != null)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(this.m_uiEventParam);
            this.m_uiEventParam = null;
        }
    }

    public void DragEnd(Vector2 pos)
    {
        Vector2 vector = pos;
        if (vector.x > this.m_lastPos.x)
        {
            this.MoveToPrePage();
        }
        else if (vector.x < this.m_lastPos.x)
        {
            this.MoveToNextPage();
        }
    }

    public void DragStart(Vector2 pos)
    {
        this.m_lastPos = pos;
        if (this.m_timer != null)
        {
            this.m_timer.ResetTime();
        }
    }

    private void InitForm()
    {
        if (this.m_form != null)
        {
            Text component = this.m_form.GetWidget(0).GetComponent<Text>();
            Text text2 = this.m_form.GetWidget(1).transform.FindChild("Text").GetComponent<Text>();
            this.m_stepList = this.m_form.GetWidget(2).GetComponent<CUIStepListScript>();
            this.m_BottomBtn = this.m_form.GetWidget(1).gameObject;
            this.m_PickObject = this.m_form.GetWidget(3).gameObject;
            this.m_BtnLeft = this.m_form.GetWidget(4).gameObject;
            this.m_BtnRight = this.m_form.GetWidget(5).gameObject;
            this.m_timer = this.m_form.GetWidget(6).GetComponent<CUITimerScript>();
            if (this.m_title != null)
            {
                component.text = this.m_title;
            }
            if (this.m_btnName != null)
            {
                text2.text = this.m_btnName;
            }
            this.m_stepList.SetElementAmount(this.m_totalImgNum);
            for (int i = 0; i < this.m_totalImgNum; i++)
            {
                if (this.m_stepList.GetElemenet(i) != null)
                {
                    this.m_stepList.GetElemenet(i).transform.FindChild("Image").GetComponent<Image>().SetSprite(this.m_imgPath[i], this.m_form, true, false, false);
                }
            }
            this.m_BottomBtn.CustomSetActive(false);
            this.m_stepList.SetDontUpdate(true);
        }
    }

    private void InitPickObjElement(int nImageCount)
    {
        GameObject pickObject = this.m_PickObject;
        if (pickObject != null)
        {
            CUIContainerScript component = pickObject.GetComponent<CUIContainerScript>();
            if (component != null)
            {
                component.RecycleAllElement();
                for (int i = 0; i < nImageCount; i++)
                {
                    this.m_PickIdxList[i] = component.GetElement();
                }
            }
        }
    }

    public void MoveToNextPage()
    {
        if (this.m_curImgIndex < (this.m_totalImgNum - 1))
        {
            this.m_curImgIndex++;
            if (this.m_stepList != null)
            {
                this.m_stepList.MoveElementInScrollArea(this.m_curImgIndex, false);
            }
            this.RefreshUI(this.m_curImgIndex);
        }
        if (this.m_timer != null)
        {
            this.m_timer.ReStartTimer();
        }
    }

    public void MoveToPrePage()
    {
        if (this.m_curImgIndex > 0)
        {
            this.m_curImgIndex--;
            if (this.m_stepList != null)
            {
                this.m_stepList.MoveElementInScrollArea(this.m_curImgIndex, false);
            }
            this.RefreshUI(this.m_curImgIndex);
        }
        if (this.m_timer != null)
        {
            this.m_timer.ReStartTimer();
        }
    }

    public void OpenForm(string[] imgPath, int imgNum, CUIEvent uiEventParam = null, string title = null, string btnName = null, bool bAutoMove = true)
    {
        this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(BANNER_INTRO_DIALOG_PATH, false, true);
        if (this.m_form != null)
        {
            CUIContainerScript component = this.m_form.GetWidget(2).transform.FindChild("pickObj").GetComponent<CUIContainerScript>();
            this.m_totalImgNum = Math.Min(Math.Min(imgPath.Length, imgNum), component.m_prepareElementAmount);
            this.m_imgPath = imgPath;
            this.m_uiEventParam = uiEventParam;
            this.m_title = title;
            this.m_btnName = btnName;
            this.m_PickIdxList = new int[this.m_totalImgNum];
            this.m_bAutoMove = bAutoMove;
            this.m_curImgIndex = 0;
            this.InitForm();
            this.InitPickObjElement(this.m_totalImgNum);
            this.RefreshUI(0);
        }
    }

    private void RefreshUI(int idx)
    {
        if (this.m_form != null)
        {
            GameObject pickObject = this.m_PickObject;
            if (pickObject != null)
            {
                CUIContainerScript component = pickObject.GetComponent<CUIContainerScript>();
                if (component != null)
                {
                    for (int i = 0; i < this.m_PickIdxList.Length; i++)
                    {
                        if (i == idx)
                        {
                            GameObject element = component.GetElement(this.m_PickIdxList[i]);
                            if (element != null)
                            {
                                Transform transform = element.transform.FindChild("Image_Pointer");
                                if (transform != null)
                                {
                                    transform.gameObject.CustomSetActive(true);
                                }
                            }
                        }
                        else
                        {
                            GameObject obj4 = component.GetElement(this.m_PickIdxList[i]);
                            if (obj4 != null)
                            {
                                Transform transform2 = obj4.transform.FindChild("Image_Pointer");
                                if (transform2 != null)
                                {
                                    transform2.gameObject.CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }
            this.m_BottomBtn.CustomSetActive((idx == (this.m_totalImgNum - 1)) || (this.m_totalImgNum == 0));
            this.m_PickObject.CustomSetActive(idx != (this.m_totalImgNum - 1));
            this.m_BtnLeft.CustomSetActive((idx != 0) && (this.m_totalImgNum > 1));
            this.m_BtnRight.CustomSetActive((idx != (this.m_totalImgNum - 1)) && (this.m_totalImgNum > 1));
        }
    }

    public enum enIntroDlgWidget
    {
        enTitleTxt,
        enBottomBtn,
        enStepList,
        enPickObject,
        enBtnLeft,
        enBtnRight,
        enTimer
    }
}

