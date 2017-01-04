using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

public class CAddSkillSys : Singleton<CAddSkillSys>
{
    public const string ADD_SKILL_FORM_PATH = "UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab";

    public static uint GetSelfSelSkill(ResDT_UnUseSkill unUseSkillInfo, uint heroId)
    {
        if (!IsSelSkillAvailable())
        {
            return 0;
        }
        if (unUseSkillInfo == null)
        {
            return 0;
        }
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
        if (masterRoleInfo == null)
        {
            return 0;
        }
        uint selSkillId = 0;
        CHeroInfo heroInfo = masterRoleInfo.GetHeroInfo(heroId, true);
        if (heroInfo != null)
        {
            selSkillId = heroInfo.skillInfo.SelSkillID;
        }
        else if (masterRoleInfo.IsFreeHero(heroId))
        {
            COMDT_FREEHERO_INFO freeHeroSymbol = masterRoleInfo.GetFreeHeroSymbol(heroId);
            if (freeHeroSymbol != null)
            {
                selSkillId = freeHeroSymbol.dwSkillID;
            }
        }
        if (!IsSelSkillAvailable(unUseSkillInfo, selSkillId))
        {
            selSkillId = GameDataMgr.addedSkiilDatabin.GetAnyData().dwUnlockSkillID;
        }
        if (!IsSelSkillAvailable(unUseSkillInfo, selSkillId))
        {
            selSkillId = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9a).dwConfValue;
        }
        return selSkillId;
    }

    public static ListView<ResSkillUnlock> GetSelSkillAvailable(ResDT_UnUseSkill unUseSkillInfo)
    {
        ListView<ResSkillUnlock> view = new ListView<ResSkillUnlock>();
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
        ResSkillUnlock item = null;
        if (unUseSkillInfo != null)
        {
            for (int i = 0; i < GameDataMgr.addedSkiilDatabin.count; i++)
            {
                item = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
                if ((masterRoleInfo != null) && (masterRoleInfo.PvpLevel >= item.wAcntLevel))
                {
                    bool flag = true;
                    if (unUseSkillInfo != null)
                    {
                        for (int j = 0; j < unUseSkillInfo.UnUseSkillList.Length; j++)
                        {
                            if (unUseSkillInfo.UnUseSkillList[j] == item.dwUnlockSkillID)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        view.Add(item);
                    }
                }
            }
        }
        return view;
    }

    public override void Init()
    {
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.AddedSkill_GetDetail, new CUIEventManager.OnUIEventHandler(this.OnGetDetail));
        base.Init();
    }

    public static bool IsSelSkillAvailable()
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
        return ((Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode() && Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL)) && (masterRoleInfo != null));
    }

    public static bool IsSelSkillAvailable(ResDT_UnUseSkill unUseSkillInfo, uint selSkillId)
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
        ResSkillUnlock unlock = null;
        ResSkillUnlock dataByIndex = null;
        if (unUseSkillInfo == null)
        {
            return false;
        }
        for (int i = 0; i < GameDataMgr.addedSkiilDatabin.count; i++)
        {
            dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
            if (dataByIndex.dwUnlockSkillID == selSkillId)
            {
                unlock = dataByIndex;
                break;
            }
        }
        if (((unlock == null) || (masterRoleInfo == null)) || (masterRoleInfo.PvpLevel < unlock.wAcntLevel))
        {
            return false;
        }
        if (unUseSkillInfo != null)
        {
            for (int j = 0; j < unUseSkillInfo.UnUseSkillList.Length; j++)
            {
                if (unUseSkillInfo.UnUseSkillList[j] == unlock.dwUnlockSkillID)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnCloseForm(CUIEvent cuiEvent)
    {
        Singleton<CUIManager>.instance.CloseForm("UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab");
        Singleton<CResourceManager>.instance.UnloadUnusedAssets();
    }

    private void OnGetDetail(CUIEvent cuiEvent)
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm("UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab");
        if ((form != null) && !form.IsHided())
        {
            CAddSkillView.OnRefresh(form.gameObject, (ushort) cuiEvent.m_eventParams.tag);
        }
    }

    private void OnOpenForm(CUIEvent cuiEvent)
    {
        if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL))
        {
            CUIFormScript script = Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/AddedSkill/Form_AddedSkill.prefab", false, true);
            if (script != null)
            {
                CAddSkillView.OpenForm(script.gameObject);
            }
        }
        else
        {
            ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x16);
            Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
        }
        CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_AddSkillBtn);
    }

    public override void UnInit()
    {
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.AddedSkill_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.AddedSkill_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.AddedSkill_GetDetail, new CUIEventManager.OnUIEventHandler(this.OnGetDetail));
        base.UnInit();
    }
}

