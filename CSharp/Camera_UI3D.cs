using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

public class Camera_UI3D : Singleton<Camera_UI3D>
{
    private Camera m_camera;

    public Camera GetCurrentCamera()
    {
        return this.m_camera;
    }

    public override void Init()
    {
        Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
    }

    public void OnFightOver(ref DefaultGameEventParam prm)
    {
        if (this.m_camera != null)
        {
            UnityEngine.Object.Destroy(this.m_camera.gameObject);
            this.m_camera = null;
        }
    }

    public void Reset()
    {
        GameObject obj2 = new GameObject("Camera_UI3D");
        MonoSingleton<SceneMgr>.GetInstance().AddToRoot(obj2, SceneObjType.Temp);
        obj2.transform.position = new Vector3(100f, 100f, 100f);
        this.m_camera = obj2.AddComponent<Camera>();
        this.m_camera.CopyFrom(Moba_Camera.currentMobaCamera);
        this.m_camera.orthographic = true;
        this.m_camera.orthographicSize = 8f;
        string[] layerNames = new string[] { "3DUI" };
        this.m_camera.cullingMask = LayerMask.GetMask(layerNames);
        this.m_camera.depth++;
        this.m_camera.clearFlags = CameraClearFlags.Nothing;
        obj2.tag = "Untagged";
    }
}

