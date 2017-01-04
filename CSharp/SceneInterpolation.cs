using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

internal class SceneInterpolation : MonoBehaviour
{
    [CompilerGenerated]
    private static Comparison<Camera> <>f__am$cacheB;
    private int activedCamera = -1;
    private Camera camera_pp;
    private Camera camera_scene0;
    private Camera camera_scene1;
    private List<RestoreCameraClearFlags> cameraClearFlagsList = new List<RestoreCameraClearFlags>();
    private List<Camera> cameraList = new List<Camera>();
    private float factor;
    public float FadeTime = 2f;
    private SceneInterpolationRT interpolationRT;
    private RenderTexture rt0;
    private RenderTexture rt1;

    private void DuplicateCamera(Camera src, Camera dest)
    {
        dest.transform.parent = src.transform;
        dest.transform.localPosition = Vector3.zero;
        dest.transform.localRotation = Quaternion.identity;
        dest.transform.localScale = Vector3.one;
        dest.aspect = src.aspect;
        dest.backgroundColor = src.backgroundColor;
        dest.nearClipPlane = src.nearClipPlane;
        dest.farClipPlane = src.farClipPlane;
        dest.fieldOfView = src.fieldOfView;
        dest.orthographic = src.orthographic;
        dest.orthographicSize = src.orthographicSize;
        dest.pixelRect = src.pixelRect;
        dest.rect = src.rect;
    }

    public void Play()
    {
        this.factor = 0f;
        this.cameraClearFlagsList.Clear();
        this.cameraList.Clear();
        string[] layerNames = new string[] { "Scene", "Scene2" };
        int mask = LayerMask.GetMask(layerNames);
        UnityEngine.Object[] objArray = UnityEngine.Object.FindObjectsOfType(typeof(Camera));
        if (objArray != null)
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                Camera item = objArray[i] as Camera;
                if ((item.cullingMask & mask) != 0)
                {
                    item.cullingMask &= ~mask;
                    this.cameraList.Add(item);
                }
            }
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = delegate (Camera a, Camera b) {
                    if (a.depth < b.depth)
                    {
                        return -1;
                    }
                    if (a.depth > b.depth)
                    {
                        return 1;
                    }
                    return 0;
                };
            }
            this.cameraList.Sort(<>f__am$cacheB);
        }
        GameObject obj2 = new GameObject();
        GameObject obj3 = new GameObject();
        GameObject obj4 = new GameObject();
        int num3 = -500;
        this.camera_scene0 = obj2.AddComponent<Camera>();
        string[] textArray2 = new string[] { "Scene" };
        this.camera_scene0.cullingMask = LayerMask.GetMask(textArray2);
        this.camera_scene0.depth = num3--;
        this.camera_scene1 = obj3.AddComponent<Camera>();
        string[] textArray3 = new string[] { "Scene2" };
        this.camera_scene1.cullingMask = LayerMask.GetMask(textArray3);
        this.camera_scene1.depth = num3--;
        this.camera_pp = obj4.AddComponent<Camera>();
        this.camera_pp.cullingMask = 0;
        this.camera_pp.depth = num3--;
        if (this.UpdateCamera())
        {
            int width = Mathf.RoundToInt(this.camera_scene0.get_pixelWidth());
            int height = Mathf.RoundToInt(this.camera_scene0.get_pixelHeight());
            this.rt0 = new RenderTexture(width, height, 0x18);
            this.rt1 = new RenderTexture(width, height, 0x18);
            this.camera_scene0.targetTexture = this.rt0;
            this.camera_scene1.targetTexture = this.rt1;
            this.interpolationRT = this.camera_pp.gameObject.AddComponent<SceneInterpolationRT>();
            this.interpolationRT.rt0 = this.rt0;
            this.interpolationRT.rt1 = this.rt1;
            this.interpolationRT.factor = 0f;
            Camera camera2 = this.cameraList[this.activedCamera];
            this.camera_scene0.clearFlags = camera2.clearFlags;
            this.camera_scene1.clearFlags = camera2.clearFlags;
            this.camera_pp.clearFlags = camera2.clearFlags;
            for (int j = 0; j < this.cameraList.Count; j++)
            {
                Camera camera3 = this.cameraList[j];
                if ((camera3.clearFlags == CameraClearFlags.Skybox) || (camera3.clearFlags == CameraClearFlags.Color))
                {
                    RestoreCameraClearFlags flags = new RestoreCameraClearFlags {
                        camera = camera3,
                        flags = camera3.clearFlags
                    };
                    this.cameraClearFlagsList.Add(flags);
                    camera3.clearFlags = CameraClearFlags.Depth;
                }
            }
        }
    }

    public void Restore()
    {
        string[] layerNames = new string[] { "Scene" };
        int mask = LayerMask.GetMask(layerNames);
        string[] textArray2 = new string[] { "Scene2" };
        int num2 = LayerMask.GetMask(textArray2);
        UnityEngine.Object[] objArray = UnityEngine.Object.FindObjectsOfType(typeof(Camera));
        if (objArray != null)
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                Camera camera = objArray[i] as Camera;
                if ((camera.cullingMask & num2) != 0)
                {
                    camera.cullingMask &= ~num2;
                    camera.cullingMask |= mask;
                }
            }
        }
    }

    public void Stop()
    {
        if (this.interpolationRT != null)
        {
            UnityEngine.Object.Destroy(this.interpolationRT);
            this.interpolationRT = null;
        }
        if (this.camera_scene0 != null)
        {
            UnityEngine.Object.Destroy(this.camera_scene0.gameObject);
            this.camera_scene0 = null;
        }
        if (this.camera_scene1 != null)
        {
            UnityEngine.Object.Destroy(this.camera_scene1.gameObject);
            this.camera_scene1 = null;
        }
        if (this.camera_pp != null)
        {
            UnityEngine.Object.Destroy(this.camera_pp.gameObject);
            this.camera_pp = null;
        }
        if (this.rt0 != null)
        {
            this.rt0.Release();
            UnityEngine.Object.Destroy(this.rt0);
            this.rt0 = null;
        }
        if (this.rt1 != null)
        {
            this.rt1.Release();
            UnityEngine.Object.Destroy(this.rt0);
            this.rt1 = null;
        }
        string[] layerNames = new string[] { "Scene2" };
        int mask = LayerMask.GetMask(layerNames);
        for (int i = 0; i < this.cameraClearFlagsList.Count; i++)
        {
            RestoreCameraClearFlags flags = this.cameraClearFlagsList[i];
            if (flags.camera != null)
            {
                flags.camera.clearFlags = flags.flags;
            }
        }
        for (int j = 0; j < this.cameraList.Count; j++)
        {
            Camera camera = this.cameraList[j];
            if (camera != null)
            {
                camera.cullingMask |= mask;
            }
        }
        this.cameraClearFlagsList.Clear();
        this.cameraList.Clear();
    }

    private void Update()
    {
        this.UpdateCamera();
        if (this.interpolationRT != null)
        {
            float num = Mathf.Max(0.01f, this.FadeTime);
            this.factor += Time.deltaTime / num;
            this.interpolationRT.factor = Mathf.Clamp01(this.factor);
            if (this.factor > 1f)
            {
                this.Stop();
            }
        }
    }

    private bool UpdateCamera()
    {
        int num = -1;
        for (int i = 0; i < this.cameraList.Count; i++)
        {
            Camera camera = this.cameraList[i];
            if ((camera.enabled && camera.gameObject.activeSelf) && camera.gameObject.activeInHierarchy)
            {
                num = i;
                break;
            }
        }
        if (num == -1)
        {
            return false;
        }
        this.activedCamera = num;
        Camera src = this.cameraList[this.activedCamera];
        this.DuplicateCamera(src, this.camera_scene0);
        this.DuplicateCamera(src, this.camera_scene1);
        this.DuplicateCamera(src, this.camera_pp);
        return true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RestoreCameraClearFlags
    {
        public Camera camera;
        public CameraClearFlags flags;
    }
}

