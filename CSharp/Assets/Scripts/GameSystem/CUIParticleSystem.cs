namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUIParticleSystem : Singleton<CUIParticleSystem>
    {
        private CUI3DImageScript m_particleContainerScript;
        private ListView<UIParticleInfo> m_particleList = new ListView<UIParticleInfo>();
        public static string s_particleSkillBtnEffect_Path = (CUIUtility.s_Animation3D_Dir + "UI_Effect_02");
        public static string s_particleTest_Path = (CUIUtility.s_Animation3D_Dir + "Test");
        public static string s_qualifyingFormPath = "UGUI/Form/Common/Form_Particle.prefab";

        public UIParticleInfo AddParticle(string parPath, float playTime, Vector2 sreenLoc)
        {
            UIParticleInfo info;
            if (this.m_particleContainerScript == null)
            {
                return null;
            }
            info = new UIParticleInfo {
                path = parPath,
                currentTime = 0f,
                totlalTime = playTime,
                parObj = this.m_particleContainerScript.AddGameObject(info.path, false, ref sreenLoc, false)
            };
            this.m_particleList.Add(info);
            return info;
        }

        public UIParticleInfo AddParticle(string parPath, float playTime, GameObject targetLoc, CUIFormScript targetFormScript)
        {
            if ((targetLoc == null) || (targetFormScript == null))
            {
                return null;
            }
            Vector2 sreenLoc = CUIUtility.WorldToScreenPoint(targetFormScript.GetCamera(), targetLoc.transform.position);
            return this.AddParticle(parPath, playTime, sreenLoc);
        }

        public void ClearAll(bool isClearCycle = false)
        {
            for (int i = 0; i < this.m_particleList.Count; i++)
            {
                bool flag = true;
                if ((this.m_particleList[i].totlalTime < 0f) && !isClearCycle)
                {
                    flag = false;
                }
                if (flag)
                {
                    if (this.m_particleContainerScript != null)
                    {
                        this.m_particleContainerScript.RemoveGameObject(this.m_particleList[i].parObj);
                    }
                    this.m_particleList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Hide(CUIEvent evt = null)
        {
            if (this.m_particleContainerScript != null)
            {
                Camera component = this.m_particleContainerScript.GetComponent<Camera>();
                if (component != null)
                {
                    component.cullingMask = 0;
                }
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ParticlTimer, new CUIEventManager.OnUIEventHandler(this.OnTimer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ParticlShow, new CUIEventManager.OnUIEventHandler(this.Show));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ParticlHide, new CUIEventManager.OnUIEventHandler(this.Hide));
            this.Open();
        }

        private void OnTimer(CUIEvent uiEvent)
        {
            for (int i = 0; i < this.m_particleList.Count; i++)
            {
                if (this.m_particleList[i].totlalTime >= 0f)
                {
                    UIParticleInfo local1 = this.m_particleList[i];
                    local1.currentTime += 0.1f;
                    if (this.m_particleList[i].currentTime >= this.m_particleList[i].totlalTime)
                    {
                        if (this.m_particleContainerScript != null)
                        {
                            this.m_particleContainerScript.RemoveGameObject(this.m_particleList[i].parObj);
                        }
                        this.m_particleList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void Open()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(s_qualifyingFormPath) == null)
            {
                this.m_particleList = new ListView<UIParticleInfo>();
                this.m_particleContainerScript = null;
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_qualifyingFormPath, false, false);
                if (script != null)
                {
                    Transform transform = script.transform.Find("3DImage");
                    if (transform != null)
                    {
                        this.m_particleContainerScript = transform.GetComponent<CUI3DImageScript>();
                    }
                    Transform transform2 = script.transform.Find("txtInfo");
                    if (transform2 != null)
                    {
                        transform2.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void RemoveParticle(UIParticleInfo pInfo)
        {
            if (pInfo != null)
            {
                if (this.m_particleContainerScript != null)
                {
                    this.m_particleContainerScript.RemoveGameObject(pInfo.parObj);
                }
                pInfo.parObj = null;
                this.m_particleList.Remove(pInfo);
            }
        }

        public void SetParticleScreenPosition(UIParticleInfo uiParticleInfo, ref Vector2 screenPosition)
        {
            if (((this.m_particleContainerScript != null) && (uiParticleInfo != null)) && (uiParticleInfo.parObj != null))
            {
                this.m_particleContainerScript.ChangeScreenPositionToWorld(uiParticleInfo.parObj, ref screenPosition);
            }
        }

        public void Show(CUIEvent evt = null)
        {
            if (this.m_particleContainerScript != null)
            {
                Camera component = this.m_particleContainerScript.GetComponent<Camera>();
                if (component != null)
                {
                    component.cullingMask = ((int) 1) << CUI3DImageScript.s_cameraLayers[1];
                }
            }
        }
    }
}

