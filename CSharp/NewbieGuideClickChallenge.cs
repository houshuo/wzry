using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickChallenge : NewbieGuideBaseScript
{
    private CUIStepListScript m_stepList;

    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
            if (this.m_stepList != null)
            {
                int count = NewbieGuideBaseScript.ms_highlitGo.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject obj2 = NewbieGuideBaseScript.ms_highlitGo[i];
                    GameObject obj3 = NewbieGuideBaseScript.ms_originalGo[i];
                    RectTransform transform = obj2.transform as RectTransform;
                    transform.localScale = obj3.transform.localScale;
                    transform.localScale = (Vector3) (transform.localScale * 1.2f);
                }
            }
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.EXLPORE_FORM_PATH);
            if (form != null)
            {
                Transform transform2 = form.transform.FindChild("ExploreList");
                if (transform2 != null)
                {
                    this.m_stepList = transform2.gameObject.GetComponent<CUIStepListScript>();
                    int index = base.currentConf.Param[0];
                    this.m_stepList.SelectElementImmediately(index);
                    CUIListElementScript elemenet = this.m_stepList.GetElemenet(index);
                    if (elemenet != null)
                    {
                        GameObject gameObject = elemenet.gameObject;
                        if (gameObject.activeInHierarchy)
                        {
                            if (index == 1)
                            {
                                Singleton<CAdventureSys>.instance.currentDifficulty = 1;
                                Singleton<CAdventureSys>.instance.currentChapter = 1;
                                Singleton<CAdventureSys>.instance.currentLevelSeq = 1;
                            }
                            base.AddHighLightGameObject(gameObject, true, form, true);
                            base.Initialize();
                        }
                    }
                }
            }
        }
    }
}

