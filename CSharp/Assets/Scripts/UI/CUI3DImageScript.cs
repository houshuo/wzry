namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUI3DImageScript : CUIComponent
    {
        private List<st3DGameObject> m_3DGameObjects = new List<st3DGameObject>();
        public en3DImageLayer m_imageLayer;
        private Vector2 m_lastPivotScreenPosition;
        private Vector2 m_pivotScreenPosition;
        public Camera m_renderCamera;
        public Vector3 m_renderCameraDefaultScale = Vector3.one;
        public float m_renderCameraDefaultSize = 20f;
        private Light m_renderLight;
        public static int[] s_cameraDepths = new int[] { 9, 11 };
        public static int[] s_cameraLayers = new int[] { 0x10, 0x11 };

        public GameObject AddGameObject(string path, bool useGameObjectPool, bool needCached = false)
        {
            return this.AddGameObject(path, useGameObjectPool, ref this.m_pivotScreenPosition, true, needCached, null);
        }

        public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool needCached = false)
        {
            return this.AddGameObject(path, useGameObjectPool, ref screenPosition, false, needCached, null);
        }

        public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool bindPivot, bool needCached = false, string pathToAdd = null)
        {
            GameObject gameObject = null;
            if (useGameObjectPool)
            {
                gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(path, enResourceType.UI3DImage);
            }
            else
            {
                GameObject content = (GameObject) Singleton<CResourceManager>.GetInstance().GetResource(path, typeof(GameObject), enResourceType.UI3DImage, needCached, false).m_content;
                if (content != null)
                {
                    gameObject = (GameObject) UnityEngine.Object.Instantiate(content);
                }
            }
            if (gameObject == null)
            {
                return null;
            }
            Vector3 localScale = gameObject.transform.localScale;
            if (pathToAdd == null)
            {
                gameObject.transform.SetParent(base.gameObject.transform, true);
            }
            else
            {
                Transform parent = base.gameObject.transform.Find(pathToAdd);
                if (parent != null)
                {
                    gameObject.transform.SetParent(parent, true);
                }
            }
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            st3DGameObject item = new st3DGameObject {
                m_gameObject = gameObject,
                m_path = path,
                m_useGameObjectPool = useGameObjectPool,
                m_protogenic = false,
                m_bindPivot = bindPivot
            };
            this.m_3DGameObjects.Add(item);
            if (this.m_renderCamera.orthographic)
            {
                this.ChangeScreenPositionToWorld(gameObject, ref screenPosition);
                if (!this.m_renderCamera.enabled && (this.m_3DGameObjects.Count > 0))
                {
                    this.m_renderCamera.enabled = true;
                }
            }
            else
            {
                Transform transform2 = base.transform.FindChild("_root");
                if (transform2 != null)
                {
                    if (pathToAdd == null)
                    {
                        gameObject.transform.SetParent(transform2, true);
                    }
                    else
                    {
                        Transform transform3 = base.gameObject.transform.Find(pathToAdd);
                        if (transform3 != null)
                        {
                            gameObject.transform.SetParent(transform3, true);
                        }
                    }
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = localScale;
                }
            }
            CUIUtility.SetGameObjectLayer(gameObject, !this.m_renderCamera.enabled ? 0x1f : s_cameraLayers[(int) this.m_imageLayer]);
            return gameObject;
        }

        public GameObject AddGameObjectToPath(string path, bool useGameObjectPool, string pathToAdd)
        {
            return this.AddGameObject(path, useGameObjectPool, ref this.m_pivotScreenPosition, false, false, pathToAdd);
        }

        public override void Appear()
        {
            base.Appear();
            this.m_renderCamera.enabled = true;
            for (int i = 0; i < this.m_3DGameObjects.Count; i++)
            {
                st3DGameObject obj2 = this.m_3DGameObjects[i];
                CUIUtility.SetGameObjectLayer(obj2.m_gameObject, s_cameraLayers[(int) this.m_imageLayer]);
            }
        }

        public void ChangeScreenPositionToWorld(string path, ref Vector2 screenPosition)
        {
            this.ChangeScreenPositionToWorld(this.GetGameObject(path), ref screenPosition);
        }

        public void ChangeScreenPositionToWorld(GameObject gameObject, ref Vector2 screenPosition)
        {
            if (gameObject != null)
            {
                gameObject.transform.position = CUIUtility.ScreenToWorldPoint(this.m_renderCamera, screenPosition, 100f);
            }
        }

        public override void Close()
        {
            base.Close();
            int index = 0;
            while (index < this.m_3DGameObjects.Count)
            {
                st3DGameObject obj2 = this.m_3DGameObjects[index];
                if (!obj2.m_protogenic)
                {
                    st3DGameObject obj3 = this.m_3DGameObjects[index];
                    UnityEngine.Object.Destroy(obj3.m_gameObject);
                    this.m_3DGameObjects.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public GameObject GetGameObject(string path)
        {
            for (int i = 0; i < this.m_3DGameObjects.Count; i++)
            {
                st3DGameObject obj2 = this.m_3DGameObjects[i];
                if (obj2.m_path.Equals(path))
                {
                    st3DGameObject obj3 = this.m_3DGameObjects[i];
                    return obj3.m_gameObject;
                }
            }
            return null;
        }

        public Vector2 GetPivotScreenPosition()
        {
            this.m_pivotScreenPosition = CUIUtility.WorldToScreenPoint(base.m_belongedFormScript.GetCamera(), base.gameObject.transform.position);
            return this.m_pivotScreenPosition;
        }

        public override void Hide()
        {
            base.Hide();
            this.m_renderCamera.enabled = false;
            for (int i = 0; i < this.m_3DGameObjects.Count; i++)
            {
                st3DGameObject obj2 = this.m_3DGameObjects[i];
                CUIUtility.SetGameObjectLayer(obj2.m_gameObject, 0x1f);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_renderCamera = base.gameObject.GetComponent<Camera>();
                this.m_renderLight = base.gameObject.GetComponent<Light>();
                this.InitializeRender();
                this.GetPivotScreenPosition();
                this.Initialize3DGameObjects();
            }
        }

        private void Initialize3DGameObjects()
        {
            for (int i = 0; i < base.gameObject.transform.childCount; i++)
            {
                GameObject gameObject = base.gameObject.transform.GetChild(i).gameObject;
                CUIUtility.SetGameObjectLayer(gameObject, s_cameraLayers[(int) this.m_imageLayer]);
                if (this.m_renderCamera.orthographic)
                {
                    this.ChangeScreenPositionToWorld(gameObject, ref this.m_pivotScreenPosition);
                }
                st3DGameObject item = new st3DGameObject {
                    m_path = gameObject.name,
                    m_gameObject = gameObject,
                    m_useGameObjectPool = false,
                    m_protogenic = true,
                    m_bindPivot = true
                };
                this.m_3DGameObjects.Add(item);
            }
            this.m_renderCamera.enabled = this.m_3DGameObjects.Count > 0;
        }

        public void InitializeRender()
        {
            if (this.m_renderCamera != null)
            {
                this.m_renderCamera.clearFlags = CameraClearFlags.Depth;
                this.m_renderCamera.cullingMask = ((int) 1) << s_cameraLayers[(int) this.m_imageLayer];
                this.m_renderCamera.depth = s_cameraDepths[(int) this.m_imageLayer];
                if (this.m_renderCamera.orthographic)
                {
                    this.m_renderCamera.orthographicSize = this.m_renderCameraDefaultSize * ((base.m_belongedFormScript.transform as RectTransform).rect.height / base.m_belongedFormScript.GetReferenceResolution().y);
                }
                else
                {
                    this.m_renderCamera.gameObject.transform.localScale = (Vector3) (this.m_renderCameraDefaultScale * (1f / ((base.m_belongedFormScript.gameObject.transform.localScale.x != 0f) ? base.m_belongedFormScript.gameObject.transform.localScale.x : 1f)));
                }
            }
            if (this.m_renderLight != null)
            {
                this.m_renderLight.cullingMask = ((int) 1) << s_cameraLayers[(int) this.m_imageLayer];
            }
        }

        private void OnPreRender()
        {
            if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                this.GetPivotScreenPosition();
                if (this.m_lastPivotScreenPosition != this.m_pivotScreenPosition)
                {
                    if (this.m_renderCamera.orthographic)
                    {
                        for (int i = 0; i < this.m_3DGameObjects.Count; i++)
                        {
                            st3DGameObject obj2 = this.m_3DGameObjects[i];
                            if (obj2.m_bindPivot)
                            {
                                st3DGameObject obj3 = this.m_3DGameObjects[i];
                                this.ChangeScreenPositionToWorld(obj3.m_gameObject, ref this.m_pivotScreenPosition);
                            }
                        }
                    }
                    else
                    {
                        float offsetX = this.m_pivotScreenPosition.x / ((float) Mathf.Max(Screen.width, Screen.height));
                        offsetX = (offsetX * 2f) - 1f;
                        base.GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 1f);
                        base.GetComponent<Camera>().ResetAspect();
                        base.GetComponent<Camera>().SetOffsetX(offsetX);
                    }
                    this.m_lastPivotScreenPosition = this.m_pivotScreenPosition;
                }
            }
        }

        public void RemoveGameObject(string path)
        {
            int index = 0;
            while (index < this.m_3DGameObjects.Count)
            {
                st3DGameObject obj2 = this.m_3DGameObjects[index];
                if (string.Equals(obj2.m_path, path, StringComparison.OrdinalIgnoreCase))
                {
                    st3DGameObject obj3 = this.m_3DGameObjects[index];
                    if (obj3.m_useGameObjectPool)
                    {
                        st3DGameObject obj4 = this.m_3DGameObjects[index];
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(obj4.m_gameObject);
                    }
                    else
                    {
                        st3DGameObject obj5 = this.m_3DGameObjects[index];
                        UnityEngine.Object.Destroy(obj5.m_gameObject);
                    }
                    this.m_3DGameObjects.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            if (this.m_3DGameObjects.Count <= 0)
            {
                this.m_renderCamera.enabled = false;
            }
        }

        public void RemoveGameObject(GameObject removeObj)
        {
            if (removeObj != null)
            {
                int index = 0;
                while (index < this.m_3DGameObjects.Count)
                {
                    st3DGameObject obj2 = this.m_3DGameObjects[index];
                    if (obj2.m_gameObject == removeObj)
                    {
                        st3DGameObject obj3 = this.m_3DGameObjects[index];
                        if (obj3.m_useGameObjectPool)
                        {
                            st3DGameObject obj4 = this.m_3DGameObjects[index];
                            Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(obj4.m_gameObject);
                        }
                        else
                        {
                            st3DGameObject obj5 = this.m_3DGameObjects[index];
                            UnityEngine.Object.Destroy(obj5.m_gameObject);
                        }
                        this.m_3DGameObjects.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                if (this.m_3DGameObjects.Count <= 0)
                {
                    this.m_renderCamera.enabled = false;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct st3DGameObject
        {
            public string m_path;
            public GameObject m_gameObject;
            public bool m_useGameObjectPool;
            public bool m_protogenic;
            public bool m_bindPivot;
        }
    }
}

