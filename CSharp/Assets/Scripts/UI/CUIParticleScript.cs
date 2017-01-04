namespace Assets.Scripts.UI
{
    using Assets.Scripts.Framework;
    using System;
    using UnityEngine;

    public class CUIParticleScript : CUIComponent
    {
        public bool m_isFixScaleToForm;
        private int m_rendererCount;
        private Renderer[] m_renderers;
        public string m_resPath = string.Empty;

        public override void Appear()
        {
            base.Appear();
            CUIUtility.SetGameObjectLayer(base.gameObject, 5);
        }

        public override void Hide()
        {
            base.Hide();
            CUIUtility.SetGameObjectLayer(base.gameObject, 0x1f);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.LoadRes();
                this.InitializeRenderers();
                base.Initialize(formScript);
                if (this.m_isFixScaleToForm)
                {
                    this.ResetScale();
                }
            }
        }

        private void InitializeRenderers()
        {
            this.m_renderers = new Renderer[100];
            this.m_rendererCount = 0;
            CUIUtility.GetComponentsInChildren<Renderer>(base.gameObject, this.m_renderers, ref this.m_rendererCount);
        }

        private void LoadRes()
        {
            if (!string.IsNullOrEmpty(this.m_resPath))
            {
                string str;
                if (GameSettings.ParticleQuality == SGameRenderQuality.Low)
                {
                    string[] textArray1 = new string[] { CUIUtility.s_Particle_Dir, this.m_resPath, "/", this.m_resPath, "_low.prefeb" };
                    str = string.Concat(textArray1);
                }
                else if (GameSettings.ParticleQuality == SGameRenderQuality.Medium)
                {
                    string[] textArray2 = new string[] { CUIUtility.s_Particle_Dir, this.m_resPath, "/", this.m_resPath, "_mid.prefeb" };
                    str = string.Concat(textArray2);
                }
                else
                {
                    string[] textArray3 = new string[] { CUIUtility.s_Particle_Dir, this.m_resPath, "/", this.m_resPath, ".prefeb" };
                    str = string.Concat(textArray3);
                }
                GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(str, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
                if (content != null)
                {
                    GameObject obj3 = UnityEngine.Object.Instantiate(content) as GameObject;
                    obj3.transform.SetParent(base.gameObject.transform);
                    obj3.transform.localPosition = Vector3.zero;
                    obj3.transform.localRotation = Quaternion.identity;
                    obj3.transform.localScale = Vector3.one;
                }
            }
        }

        private void ResetScale()
        {
            float x = 1f / base.m_belongedFormScript.gameObject.transform.localScale.x;
            base.gameObject.transform.localScale = new Vector3(x, x, 0f);
        }

        public override void SetSortingOrder(int sortingOrder)
        {
            base.SetSortingOrder(sortingOrder);
            for (int i = 0; i < this.m_rendererCount; i++)
            {
                this.m_renderers[i].sortingOrder = sortingOrder;
            }
        }
    }
}

