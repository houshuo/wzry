namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUIRawImageScript : CUIComponent
    {
        private const int c_uiRawLayer = 15;
        private GameObject m_rawRootObject;
        private Camera m_renderTextureCamera;

        public void AddGameObject(string name, GameObject rawObject, Vector3 position, Quaternion rotation, Vector3 scaler)
        {
            if (this.m_rawRootObject != null)
            {
                this.SetRawObjectLayer(rawObject, LayerMask.NameToLayer("UIRaw"));
                rawObject.name = name;
                rawObject.transform.SetParent(this.m_rawRootObject.transform);
                rawObject.transform.localPosition = position;
                rawObject.transform.localRotation = rotation;
                rawObject.transform.localScale = scaler;
            }
        }

        public override void Appear()
        {
            base.Appear();
            CUIUtility.SetGameObjectLayer(this.m_rawRootObject, 15);
        }

        public GameObject GetGameObject(string name)
        {
            if (this.m_rawRootObject == null)
            {
                return null;
            }
            for (int i = 0; i < this.m_rawRootObject.transform.childCount; i++)
            {
                GameObject gameObject = this.m_rawRootObject.transform.GetChild(i).gameObject;
                if (gameObject.name.Equals(name))
                {
                    return gameObject;
                }
            }
            return null;
        }

        public override void Hide()
        {
            base.Hide();
            CUIUtility.SetGameObjectLayer(this.m_rawRootObject, 0x1f);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_renderTextureCamera = base.GetComponentInChildren<Camera>(base.gameObject);
                if (this.m_renderTextureCamera != null)
                {
                    Transform transform = this.m_renderTextureCamera.gameObject.transform.FindChild("RawRoot");
                    if (transform != null)
                    {
                        this.m_rawRootObject = transform.gameObject;
                    }
                }
            }
        }

        public GameObject RemoveGameObject(string name)
        {
            if (this.m_rawRootObject != null)
            {
                for (int i = 0; i < this.m_rawRootObject.transform.childCount; i++)
                {
                    GameObject gameObject = this.m_rawRootObject.transform.GetChild(i).gameObject;
                    if (gameObject.name.Equals(name))
                    {
                        gameObject.transform.SetParent(null);
                        return gameObject;
                    }
                }
            }
            return null;
        }

        public void SetRawObjectLayer(GameObject rawObject, int layer)
        {
            rawObject.layer = layer;
            for (int i = 0; i < rawObject.transform.childCount; i++)
            {
                this.SetRawObjectLayer(rawObject.transform.GetChild(i).gameObject, layer);
            }
        }
    }
}

