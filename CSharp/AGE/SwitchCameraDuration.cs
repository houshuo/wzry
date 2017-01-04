namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/System")]
    public class SwitchCameraDuration : DurationCondition
    {
        [ObjectTemplate(new System.Type[] {  })]
        public int cameraId = -1;
        [ObjectTemplate(new System.Type[] {  })]
        public int cameraId2 = -1;
        private Camera curCamera;
        public bool cutBackOnExit;
        private Vector3 destPos = Vector3.zero;
        private Quaternion destRot = Quaternion.identity;
        private bool isMoba_camera;
        private Camera oldCamera;
        public int slerpTick = 0xbb8;
        private Vector3 startPos = Vector3.zero;
        private Quaternion startRot = Quaternion.identity;
        private bool switchFinished;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.switchFinished;
        }

        public override BaseEvent Clone()
        {
            SwitchCameraDuration duration = ClassObjPool<SwitchCameraDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SwitchCameraDuration duration = src as SwitchCameraDuration;
            this.cameraId = duration.cameraId;
            this.cameraId2 = duration.cameraId2;
            this.slerpTick = duration.slerpTick;
            this.cutBackOnExit = duration.cutBackOnExit;
            this.startPos = duration.startPos;
            this.startRot = duration.startRot;
            this.destPos = duration.destPos;
            this.destRot = duration.destRot;
            this.curCamera = duration.curCamera;
            this.oldCamera = duration.oldCamera;
            this.switchFinished = duration.switchFinished;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.switchFinished = false;
            GameObject destObj = this.GetDestObj(_action);
            if (destObj != null)
            {
                if (((destObj.transform.parent != null) && (destObj.transform.parent.parent != null)) && (destObj.transform.parent.parent.GetComponent<Moba_Camera>() != null))
                {
                    this.isMoba_camera = true;
                }
                else
                {
                    this.isMoba_camera = false;
                }
                this.curCamera = destObj.GetComponent<Camera>();
                DebugHelper.Assert(this.curCamera != null, "switch camera but dest camera not exist");
                if (this.curCamera != null)
                {
                    string[] layerNames = new string[] { "Hide" };
                    this.curCamera.cullingMask &= ~LayerMask.GetMask(layerNames);
                    string[] textArray2 = new string[] { "UIRaw" };
                    this.curCamera.cullingMask &= ~LayerMask.GetMask(textArray2);
                    string[] textArray3 = new string[] { "UI_Background" };
                    this.curCamera.cullingMask &= ~LayerMask.GetMask(textArray3);
                    string[] textArray4 = new string[] { "UI_Foreground" };
                    this.curCamera.cullingMask &= ~LayerMask.GetMask(textArray4);
                    string[] textArray5 = new string[] { "UI_BottomBG" };
                    this.curCamera.cullingMask &= ~LayerMask.GetMask(textArray5);
                    string[] textArray6 = new string[] { "3DUI" };
                    this.curCamera.cullingMask &= ~LayerMask.GetMask(textArray6);
                    this.destPos = this.curCamera.transform.position;
                    this.destRot = this.curCamera.transform.rotation;
                    this.oldCamera = Camera.main;
                    if (this.oldCamera != null)
                    {
                        this.startPos = this.oldCamera.transform.position;
                        this.startRot = this.oldCamera.transform.rotation;
                        this.curCamera.transform.position = this.startPos;
                        this.curCamera.transform.rotation = this.startRot;
                        destObj.SetActive(true);
                        SwitchCamera(this.oldCamera, this.curCamera);
                    }
                }
            }
            base.Enter(_action, _track);
        }

        private GameObject GetDestObj(AGE.Action _action)
        {
            GameObject gameObject = _action.GetGameObject(this.cameraId);
            if (this.cameraId2 != -1)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
                {
                    gameObject = _action.GetGameObject(this.cameraId2);
                }
            }
            return gameObject;
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if ((this.curCamera != null) && !this.switchFinished)
            {
                if (this.isMoba_camera)
                {
                    this.curCamera.transform.position = this.curCamera.transform.parent.position;
                    this.curCamera.transform.rotation = this.curCamera.transform.parent.rotation;
                }
                else
                {
                    this.curCamera.transform.position = this.destPos;
                    this.curCamera.transform.rotation = this.destRot;
                }
            }
            this.switchFinished = true;
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.cameraId = -1;
            this.cameraId2 = -1;
            this.slerpTick = 0xbb8;
            this.cutBackOnExit = false;
            this.startPos = Vector3.zero;
            this.startRot = Quaternion.identity;
            this.destPos = Vector3.zero;
            this.destRot = Quaternion.identity;
            this.curCamera = null;
            this.oldCamera = null;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if (!this.switchFinished && (this.curCamera != null))
            {
                if (_localTime >= this.slerpTick)
                {
                    if (this.isMoba_camera)
                    {
                        this.curCamera.transform.position = this.curCamera.transform.parent.position;
                        this.curCamera.transform.rotation = this.curCamera.transform.parent.rotation;
                    }
                    else
                    {
                        this.curCamera.transform.position = this.destPos;
                        this.curCamera.transform.rotation = this.destRot;
                    }
                    this.switchFinished = true;
                }
                else if (this.isMoba_camera)
                {
                    this.curCamera.transform.position = Vector3.Lerp(this.startPos, this.curCamera.transform.parent.position, ((float) _localTime) / ((float) this.slerpTick));
                    this.curCamera.transform.rotation = Quaternion.Slerp(this.startRot, this.curCamera.transform.parent.rotation, ((float) _localTime) / ((float) this.slerpTick));
                }
                else
                {
                    this.curCamera.transform.position = Vector3.Lerp(this.startPos, this.destPos, ((float) _localTime) / ((float) this.slerpTick));
                    this.curCamera.transform.rotation = Quaternion.Slerp(this.startRot, this.destRot, ((float) _localTime) / ((float) this.slerpTick));
                }
                base.Process(_action, _track, _localTime);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }

        private static void SwitchCamera(Camera camera1, Camera camera2)
        {
            if (camera1 != null)
            {
                camera1.enabled = false;
            }
            if (camera2 != null)
            {
                camera2.tag = "MainCamera";
                camera2.enabled = true;
            }
        }
    }
}

