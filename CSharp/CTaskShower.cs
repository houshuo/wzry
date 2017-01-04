using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CTaskShower : MonoBehaviour
{
    public GameObject award_obj;
    public GameObject goto_obj;
    public GameObject has_finish;
    public GameObject m_awardContainer;
    public Text progress;
    public GameObject progressObj;
    public Text taskDesc;
    public GameObject taskIcon_back;
    public GameObject taskIcon_front;
    public uint taskID;
    public Text taskTitle;

    public void ShowTask(CTask task, CUIFormScript fromScript)
    {
        if ((task != null) && (fromScript != null))
        {
            this.taskID = task.m_baseId;
            if (this.taskTitle != null)
            {
                this.taskTitle.text = task.m_taskTitle;
            }
            if (this.taskDesc != null)
            {
                this.taskDesc.text = task.m_taskDesc;
            }
            this.taskIcon_back.CustomSetActive(true);
            this.taskIcon_front.CustomSetActive(true);
            if (task.m_resTask.bTaskIconShowType == 0)
            {
                this.taskIcon_front.GetComponent<Image>().enabled = false;
                this.taskIcon_back.GetComponent<Image>().enabled = true;
                this.taskIcon_back.GetComponent<Image>().SetSprite(task.m_resTask.szTaskBgIcon, fromScript, true, false, false);
            }
            else if (task.m_resTask.bTaskIconShowType == 1)
            {
                this.taskIcon_back.GetComponent<Image>().enabled = false;
                this.taskIcon_front.GetComponent<Image>().enabled = true;
                this.taskIcon_front.GetComponent<Image>().SetSprite(task.m_resTask.szTaskIcon, fromScript, true, false, false);
            }
            else
            {
                this.taskIcon_back.GetComponent<Image>().enabled = true;
                this.taskIcon_front.GetComponent<Image>().enabled = true;
                this.taskIcon_back.GetComponent<Image>().SetSprite(task.m_resTask.szTaskBgIcon, fromScript, true, false, false);
                this.taskIcon_front.GetComponent<Image>().SetSprite(task.m_resTask.szTaskIcon, fromScript, true, false, false);
            }
            this.award_obj.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = this.taskID;
            this.award_obj.CustomSetActive(false);
            bool flag = (this.taskID == Singleton<CTaskSys>.instance.monthCardTaskID) || (this.taskID == Singleton<CTaskSys>.instance.weekCardTaskID);
            if ((task.m_taskState == 0) || (task.m_taskState == 4))
            {
                CUIEventScript component = this.goto_obj.GetComponent<CUIEventScript>();
                if (flag)
                {
                    this.goto_obj.CustomSetActive(true);
                    component.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
                }
                else
                {
                    string str = string.Empty;
                    for (int i = 0; i < task.m_prerequisiteInfo.Length; i++)
                    {
                        RES_PERREQUISITE_TYPE conditionType = (RES_PERREQUISITE_TYPE) task.m_prerequisiteInfo[i].m_conditionType;
                        if (task.m_prerequisiteInfo[i].m_valueTarget > 0)
                        {
                            if ((conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ACNTLVL) && (conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL))
                            {
                                string str2 = str;
                                object[] objArray1 = new object[] { str2, task.m_prerequisiteInfo[i].m_value, "/", task.m_prerequisiteInfo[i].m_valueTarget, " " };
                                str = string.Concat(objArray1);
                                this.progressObj.CustomSetActive(true);
                            }
                            else
                            {
                                this.progressObj.CustomSetActive(false);
                            }
                            if (!task.m_prerequisiteInfo[i].m_isReach)
                            {
                                switch (conditionType)
                                {
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVP:
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPKILLCNT:
                                    {
                                        this.goto_obj.CustomSetActive(true);
                                        component.m_onClickEventID = enUIEventID.Matching_OpenEntry;
                                        stUIEventParams @params = new stUIEventParams {
                                            tag = 0
                                        };
                                        component.m_onClickEventParams = @params;
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVE:
                                    {
                                        int iParam = task.m_resTask.astPrerequisiteArray[i].astPrerequisiteParam[3].iParam;
                                        if (iParam == Mathf.Pow(2f, 0f))
                                        {
                                            this.goto_obj.CustomSetActive(true);
                                            component.m_onClickEventParams.taskId = task.m_baseId;
                                            component.m_onClickEventParams.tag = task.m_resTask.astPrerequisiteArray[i].astPrerequisiteParam[4].iParam;
                                            component.m_onClickEventID = enUIEventID.Task_LinkPve;
                                        }
                                        else if (iParam == Mathf.Pow(2f, 7f))
                                        {
                                            this.goto_obj.CustomSetActive(true);
                                            component.m_onClickEventID = enUIEventID.Burn_OpenForm;
                                        }
                                        else if (iParam == Mathf.Pow(2f, 8f))
                                        {
                                            this.goto_obj.CustomSetActive(true);
                                            component.m_onClickEventID = enUIEventID.Arena_OpenForm;
                                        }
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_FRIENDCNT:
                                    {
                                        this.goto_obj.CustomSetActive(false);
                                        component.m_onClickEventID = enUIEventID.Friend_OpenForm;
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL:
                                    {
                                        this.goto_obj.CustomSetActive(true);
                                        component.m_onClickEventID = enUIEventID.Matching_OpenEntry;
                                        stUIEventParams params2 = new stUIEventParams {
                                            tag = 0
                                        };
                                        component.m_onClickEventParams = params2;
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_GUILDOPT:
                                    {
                                        if (task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
                                        {
                                            this.goto_obj.CustomSetActive(true);
                                            component.m_onClickEventID = enUIEventID.Matching_OpenEntry;
                                            stUIEventParams params3 = new stUIEventParams {
                                                tag = 3
                                            };
                                            component.m_onClickEventParams = params3;
                                        }
                                        else
                                        {
                                            this.goto_obj.CustomSetActive(false);
                                        }
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ARENAOPT:
                                    {
                                        this.goto_obj.CustomSetActive(true);
                                        component.m_onClickEventID = enUIEventID.Arena_OpenForm;
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_SYMBOLCOMP:
                                    {
                                        this.goto_obj.CustomSetActive(true);
                                        component.m_onClickEventID = enUIEventID.Symbol_OpenForm;
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_BUYOPT:
                                    {
                                        this.goto_obj.CustomSetActive(true);
                                        component.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_OPENBOXCNT:
                                    {
                                        this.goto_obj.CustomSetActive(true);
                                        component.m_onClickEventID = enUIEventID.Mall_GoToSymbolTab;
                                        continue;
                                    }
                                    case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_DUOBAO:
                                    {
                                        this.goto_obj.CustomSetActive(true);
                                        if (task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 1L)
                                        {
                                            component.m_onClickEventID = enUIEventID.Mall_GotoDianmondTreasureTab;
                                        }
                                        else if (task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
                                        {
                                            component.m_onClickEventID = enUIEventID.Mall_GotoCouponsTreasureTab;
                                        }
                                        continue;
                                    }
                                }
                                if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_RECALLFRIEND)
                                {
                                    this.goto_obj.CustomSetActive(true);
                                    component.m_onClickEventID = enUIEventID.Friend_OpenForm;
                                }
                                else
                                {
                                    this.goto_obj.CustomSetActive(false);
                                }
                            }
                            else
                            {
                                this.goto_obj.CustomSetActive(false);
                            }
                        }
                    }
                    if (this.progress != null)
                    {
                        this.progress.text = str;
                    }
                }
            }
            else if (task.m_taskState == 1)
            {
                this.award_obj.CustomSetActive(true);
                this.goto_obj.CustomSetActive(false);
            }
            if ((task.m_taskState == 1) || (task.m_taskState == 3))
            {
                this.goto_obj.CustomSetActive(false);
                this.progressObj.CustomSetActive(false);
            }
            if (flag)
            {
                this.progressObj.CustomSetActive(true);
                this.progressObj.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("RemainDays");
                uint num3 = (this.taskID != Singleton<CTaskSys>.instance.monthCardTaskID) ? Singleton<CTaskSys>.instance.weekCardExpireTime : Singleton<CTaskSys>.instance.monthCardExpireTime;
                uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
                int num5 = Mathf.CeilToInt(((num3 <= currentUTCTime) ? 0f : ((float) (num3 - currentUTCTime))) / 86400f);
                if ((task.m_taskState == 3) && (num5 > 0))
                {
                    num5--;
                }
                this.progress.text = num5.ToString();
            }
            else
            {
                this.progressObj.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Progress");
            }
            this.has_finish.CustomSetActive(task.m_taskState == 3);
            CTaskView.CTaskUT.ShowTaskAward(fromScript, task, this.m_awardContainer);
        }
    }

    public void ShowTask(ResTask task, CUIFormScript fromScript)
    {
        if ((task != null) && (fromScript != null))
        {
            this.taskID = task.dwTaskID;
            if (this.taskTitle != null)
            {
                this.taskTitle.text = task.szTaskName;
            }
            if (this.taskDesc != null)
            {
                this.taskDesc.text = task.szTaskDesc;
            }
            this.taskIcon_back.CustomSetActive(true);
            this.taskIcon_front.CustomSetActive(true);
            if (task.bTaskIconShowType == 0)
            {
                this.taskIcon_front.GetComponent<Image>().enabled = false;
                this.taskIcon_back.GetComponent<Image>().enabled = true;
                this.taskIcon_back.GetComponent<Image>().SetSprite(task.szTaskBgIcon, fromScript, true, false, false);
            }
            else if (task.bTaskIconShowType == 1)
            {
                this.taskIcon_back.GetComponent<Image>().enabled = false;
                this.taskIcon_front.GetComponent<Image>().enabled = true;
                this.taskIcon_front.GetComponent<Image>().SetSprite(task.szTaskIcon, fromScript, true, false, false);
            }
            else
            {
                this.taskIcon_back.GetComponent<Image>().enabled = true;
                this.taskIcon_front.GetComponent<Image>().enabled = true;
                this.taskIcon_back.GetComponent<Image>().SetSprite(task.szTaskBgIcon, fromScript, true, false, false);
                this.taskIcon_front.GetComponent<Image>().SetSprite(task.szTaskIcon, fromScript, true, false, false);
            }
            this.progressObj.CustomSetActive(false);
            this.award_obj.CustomSetActive(false);
            this.goto_obj.CustomSetActive(false);
            this.has_finish.CustomSetActive(true);
            CTaskView.CTaskUT.ShowTaskAward(fromScript, task, this.m_awardContainer);
        }
    }
}

