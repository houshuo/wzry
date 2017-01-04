namespace AGE
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CurvlData
    {
        public bool closeCurvl;
        private Vector3[] controlPoints;
        private int curvePointCount;
        private List<Vector3> curvePoints;
        private int curvlID;
        private GameObject curvlRootObj;
        public string displayName = string.Empty;
        private List<Vector3> extrapoints;
        public bool isCubic;
        public bool isHide = false;
        public bool isInUse = true;
        public Color lineColor = Color.red;
        private List<Vector3> midpoints;
        public ListView<TransNode> nodeLst;
        public string prefabFile = "Assets/AGE/Action/Prefab/Resources/Cube.prefab";
        public int stepsOfEachSegment;
        private ListView<GameObject> transNodeObjs;
        public GameObject transObjPrefb;
        public bool useCamera;

        public CurvlData(int id)
        {
            this.curvlID = id;
            this.stepsOfEachSegment = 10;
            this.isCubic = true;
            this.closeCurvl = false;
            this.InitData();
        }

        public TransNode AddNode(bool needRefreshGfxData)
        {
            if (this.nodeLst == null)
            {
                this.nodeLst = new ListView<TransNode>();
            }
            TransNode item = new TransNode();
            this.nodeLst.Add(item);
            if (needRefreshGfxData)
            {
                this.RefreshDataToGfx();
            }
            return item;
        }

        public static void CalculateCtrlPoint(Vector3 formPoint, Vector3 prevPoint, Vector3 curnPoint, Vector3 lattPoint, out Vector3 ctrlPoint1, out Vector3 ctrlPoint2)
        {
            Vector3 vector = (Vector3) ((formPoint + prevPoint) * 0.5f);
            Vector3 vector2 = (Vector3) ((curnPoint + prevPoint) * 0.5f);
            Vector3 vector3 = (Vector3) ((curnPoint + lattPoint) * 0.5f);
            Vector3 vector4 = (Vector3) ((vector + vector2) * 0.5f);
            Vector3 vector5 = (Vector3) ((vector3 + vector2) * 0.5f);
            Vector3 vector6 = vector2 - vector4;
            Vector3 vector7 = vector2 - vector5;
            float sCtrlPtScale = CurvlHelper.sCtrlPtScale;
            float num2 = CurvlHelper.sCtrlPtScale;
            float magnitude = vector6.magnitude;
            float num4 = vector7.magnitude;
            Vector3 vector8 = curnPoint - prevPoint;
            float num5 = vector8.magnitude * 0.5f;
            if (num5 < magnitude)
            {
                sCtrlPtScale = num5 / magnitude;
            }
            if (num5 < num4)
            {
                num2 = num5 / num4;
            }
            ctrlPoint1 = prevPoint + ((Vector3) (vector6 * sCtrlPtScale));
            ctrlPoint2 = curnPoint + ((Vector3) (vector7 * num2));
        }

        private void CalculateCurvlPoints(Vector3 prevPoint, Vector3 curnPoint, Vector3 ctrlPoint1, Vector3 ctrlPoint2, float stepLen)
        {
            bool flag = false;
            float num = 0f;
            while (num <= 1f)
            {
                float num2 = 1f - num;
                float num3 = num;
                Vector3 item = (Vector3) ((((((prevPoint * num2) * num2) * num2) + ((((ctrlPoint1 * 3f) * num2) * num2) * num3)) + ((((ctrlPoint2 * 3f) * num2) * num3) * num3)) + (((curnPoint * num3) * num3) * num3));
                if (this.curvePoints.Count <= this.curvePointCount)
                {
                    this.curvePoints.Add(item);
                }
                else
                {
                    this.curvePoints[this.curvePointCount] = item;
                }
                this.curvePointCount++;
                num += stepLen;
                if (flag)
                {
                    break;
                }
                if (num >= 1f)
                {
                    num = 1f;
                    flag = true;
                }
            }
        }

        private void CreateCurvlPoints(bool useUnifyInterp)
        {
            if (this.curvePoints == null)
            {
                this.curvePoints = new List<Vector3>();
            }
            if (this.controlPoints == null)
            {
                this.controlPoints = new Vector3[4];
            }
            int count = this.nodeLst.Count;
            this.curvePointCount = 0;
            float stepLen = 1f / ((float) this.stepsOfEachSegment);
            for (int i = 0; i < count; i++)
            {
                if (this.closeCurvl || (i != 0))
                {
                    int num4 = i - 1;
                    if (num4 < 0)
                    {
                        if (this.closeCurvl)
                        {
                            num4 = count - 1;
                        }
                        else
                        {
                            num4 = 0;
                        }
                    }
                    int num5 = i - 2;
                    if (num5 < 0)
                    {
                        if (this.closeCurvl)
                        {
                            num5 = count - 1;
                        }
                        else
                        {
                            num5 = 0;
                        }
                    }
                    int num6 = i + 1;
                    if (num6 >= count)
                    {
                        if (this.closeCurvl)
                        {
                            num6 = 0;
                        }
                        else
                        {
                            num6 = i;
                        }
                    }
                    Vector3 pos = this.nodeLst[num4].pos;
                    Vector3 item = this.nodeLst[i].pos;
                    Vector3 formPoint = this.nodeLst[num5].pos;
                    Vector3 lattPoint = this.nodeLst[num6].pos;
                    if ((useUnifyInterp && !this.isCubic) || (!useUnifyInterp && !this.nodeLst[i].isCubic))
                    {
                        if (this.curvePoints.Count <= this.curvePointCount)
                        {
                            this.curvePoints.Add(pos);
                        }
                        else
                        {
                            this.curvePoints[this.curvePointCount] = pos;
                        }
                        this.curvePointCount++;
                        if (this.curvePoints.Count <= this.curvePointCount)
                        {
                            this.curvePoints.Add(item);
                        }
                        else
                        {
                            this.curvePoints[this.curvePointCount] = item;
                        }
                        this.curvePointCount++;
                    }
                    else
                    {
                        Vector3 vector5;
                        Vector3 vector6;
                        CalculateCtrlPoint(formPoint, pos, item, lattPoint, out vector5, out vector6);
                        this.CalculateCurvlPoints(pos, item, vector5, vector6, stepLen);
                    }
                }
            }
        }

        public void DrawCurvel(bool useUnifyInterp)
        {
            this.UpdateNodeShowState(this.isInUse && !this.isHide);
            if (this.isInUse && !this.isHide)
            {
                this.curvlRootObj.SetActive(true);
                this.SyncTransformDataToTransNode();
                this.CreateCurvlPoints(useUnifyInterp);
                for (int i = 0; i < (this.curvePointCount - 1); i++)
                {
                    Vector3 start = this.curvePoints[i];
                    Vector3 end = this.curvePoints[i + 1];
                    Debug.DrawLine(start, end, this.lineColor);
                }
            }
        }

        public int GetCurvlID()
        {
            return this.curvlID;
        }

        public GameObject GetCurvlRootObject()
        {
            return this.curvlRootObj;
        }

        public TransNode GetNode(int index)
        {
            if (((this.nodeLst != null) && (index >= 0)) && (index < this.nodeLst.Count))
            {
                return this.nodeLst[index];
            }
            return null;
        }

        public GameObject GetNodeObject(int index)
        {
            if (((this.transNodeObjs != null) && (index >= 0)) && (index < this.transNodeObjs.Count))
            {
                return this.transNodeObjs[index];
            }
            return null;
        }

        private void InitData()
        {
            if (this.nodeLst == null)
            {
                this.nodeLst = new ListView<TransNode>();
            }
            if (this.transNodeObjs == null)
            {
                this.transNodeObjs = new ListView<GameObject>();
            }
            if (this.curvlRootObj == null)
            {
                string name = "CurvlRootObject_" + this.curvlID;
                this.curvlRootObj = GameObject.Find(name);
                if (this.curvlRootObj == null)
                {
                    GameObject obj2 = GameObject.Find(CurvlHelper.sCurvlHelperObjName);
                    if (obj2 == null)
                    {
                        obj2 = new GameObject {
                            name = CurvlHelper.sCurvlHelperObjName
                        };
                    }
                    this.curvlRootObj = new GameObject();
                    this.curvlRootObj.name = name;
                    this.curvlRootObj.transform.parent = obj2.transform;
                }
            }
        }

        public TransNode InsertNode(int index, bool needRefreshGfxData)
        {
            if (this.nodeLst == null)
            {
                this.nodeLst = new ListView<TransNode>();
            }
            if (index < 0)
            {
                return null;
            }
            TransNode item = new TransNode();
            if (index >= this.nodeLst.Count)
            {
                if (this.nodeLst.Count > 0)
                {
                    TransNode node2 = this.nodeLst[this.nodeLst.Count - 1];
                    item.pos = node2.pos;
                    item.isCubic = node2.isCubic;
                }
                this.nodeLst.Add(item);
            }
            else
            {
                TransNode node3 = this.nodeLst[index];
                item.pos = node3.pos;
                item.isCubic = node3.isCubic;
                this.nodeLst.Insert(index, item);
            }
            if (needRefreshGfxData)
            {
                this.RefreshDataToGfx();
            }
            return item;
        }

        public void RefreshDataToGfx()
        {
            this.InitData();
            int count = this.transNodeObjs.Count;
            int num2 = 0;
            while (num2 < this.nodeLst.Count)
            {
                TransNode node = this.nodeLst[num2];
                GameObject item = null;
                string str = "CurvlTransNode_" + num2;
                if (num2 >= count)
                {
                    GameObject curvlRootObj = this.curvlRootObj;
                    if (curvlRootObj != null)
                    {
                        for (int j = 0; j < curvlRootObj.transform.childCount; j++)
                        {
                            item = curvlRootObj.transform.GetChild(j).gameObject;
                            char[] separator = new char[] { '_' };
                            string[] strArray = item.name.Split(separator);
                            int result = -1;
                            int.TryParse(strArray[1], out result);
                            if (result == num2)
                            {
                                break;
                            }
                            item = null;
                        }
                    }
                    if (item == null)
                    {
                        item = ActionManager.InstantiateObject(this.transObjPrefb) as GameObject;
                        item.name = str;
                        item.transform.parent = this.curvlRootObj.transform;
                        Camera component = item.GetComponent<Camera>();
                        if ((component == null) && this.useCamera)
                        {
                            component = item.AddComponent<Camera>();
                        }
                        else if ((component != null) && !this.useCamera)
                        {
                            UnityEngine.Object.DestroyImmediate(component);
                        }
                    }
                    this.transNodeObjs.Add(item);
                }
                else
                {
                    item = this.transNodeObjs[num2];
                    if (item == null)
                    {
                        item = ActionManager.InstantiateObject(this.transObjPrefb) as GameObject;
                        item.name = str;
                        item.transform.parent = this.curvlRootObj.transform;
                        Camera camera2 = item.GetComponent<Camera>();
                        if ((camera2 == null) && this.useCamera)
                        {
                            camera2 = item.AddComponent<Camera>();
                        }
                        else if ((camera2 != null) && !this.useCamera)
                        {
                            UnityEngine.Object.DestroyImmediate(camera2);
                        }
                        this.transNodeObjs[num2] = item;
                    }
                }
                Transform transform = item.transform;
                item.SetActive(true);
                transform.position = node.pos;
                transform.rotation = node.rot;
                num2++;
            }
            for (int i = num2; i < count; i++)
            {
                this.transNodeObjs[i].SetActive(false);
            }
        }

        public void Remove(bool destroy)
        {
            this.isHide = true;
            this.isInUse = false;
            if (this.transNodeObjs != null)
            {
                foreach (GameObject obj2 in this.transNodeObjs)
                {
                    if (obj2 != null)
                    {
                        if (destroy)
                        {
                            ActionManager.DestroyGameObject(obj2);
                        }
                        else
                        {
                            obj2.SetActive(false);
                        }
                    }
                }
            }
            if (this.curvlRootObj != null)
            {
                if (destroy)
                {
                    ActionManager.DestroyGameObject(this.curvlRootObj);
                }
                else
                {
                    this.curvlRootObj.SetActive(false);
                }
            }
            if (destroy)
            {
                if (this.transNodeObjs != null)
                {
                    this.transNodeObjs.Clear();
                    this.transNodeObjs = null;
                }
                if (this.nodeLst != null)
                {
                    this.nodeLst.Clear();
                    this.nodeLst = null;
                }
                if (this.midpoints != null)
                {
                    this.midpoints.Clear();
                    this.midpoints = null;
                }
                if (this.curvePoints != null)
                {
                    this.curvePoints.Clear();
                    this.curvePoints = null;
                }
                if (this.extrapoints != null)
                {
                    this.extrapoints.Clear();
                    this.extrapoints = null;
                }
            }
        }

        public void RemoveAllNodes(bool needRefreshGfxData)
        {
            if (this.nodeLst != null)
            {
                this.nodeLst.Clear();
                if (needRefreshGfxData)
                {
                    this.RefreshDataToGfx();
                }
            }
        }

        public void RemoveNode(int index, bool needRefreshGfxData)
        {
            if (((this.nodeLst != null) && (index >= 0)) && (index < this.nodeLst.Count))
            {
                this.nodeLst.RemoveAt(index);
                if (needRefreshGfxData)
                {
                    this.RefreshDataToGfx();
                }
            }
        }

        public void ReplaceNodeObjectPrefab(GameObject obj)
        {
            this.transObjPrefb = obj;
            for (int i = 0; i < this.transNodeObjs.Count; i++)
            {
                ActionManager.DestroyGameObject(this.transNodeObjs[i]);
            }
            this.transNodeObjs.Clear();
            this.RefreshDataToGfx();
        }

        public void ReverseNodes()
        {
            if ((this.nodeLst != null) && (this.nodeLst.Count > 1))
            {
                this.nodeLst.Reverse();
                for (int i = this.transNodeObjs.Count - 1; i >= 0; i--)
                {
                    if (!this.transNodeObjs[i].activeInHierarchy)
                    {
                        ActionManager.DestroyGameObject(this.transNodeObjs[i]);
                        this.transNodeObjs.RemoveAt(i);
                    }
                }
                this.transNodeObjs.Reverse();
            }
        }

        public void SetCurvlID(int id)
        {
            this.curvlID = id;
            if (this.curvlRootObj != null)
            {
                this.curvlRootObj.name = "CurvlRootObject_" + this.curvlID;
            }
        }

        public void SetUseCameraComp(bool bEnable)
        {
            this.useCamera = bEnable;
            foreach (GameObject obj2 in this.transNodeObjs)
            {
                if (obj2 != null)
                {
                    Camera component = obj2.GetComponent<Camera>();
                    if ((component == null) && this.useCamera)
                    {
                        component = obj2.AddComponent<Camera>();
                    }
                    else if ((component != null) && !this.useCamera)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                    }
                }
            }
        }

        private void SyncTransformDataToTransNode()
        {
            if ((this.nodeLst != null) && (this.nodeLst.Count != 0))
            {
                if (((this.transNodeObjs == null) || (this.transNodeObjs.Count < this.nodeLst.Count)) || (this.transNodeObjs[0] == null))
                {
                    this.RefreshDataToGfx();
                }
                if (this.transNodeObjs != null)
                {
                    for (int i = 0; i < this.nodeLst.Count; i++)
                    {
                        TransNode node = this.nodeLst[i];
                        Transform transform = this.transNodeObjs[i].transform;
                        node.pos = transform.position;
                        node.rot = transform.rotation;
                        node.scl = transform.localScale;
                    }
                }
            }
        }

        public void UnifyCubicPropertyToAllNodes(bool cubic)
        {
            this.isCubic = cubic;
            foreach (TransNode node in this.nodeLst)
            {
                node.isCubic = cubic;
            }
        }

        private void UpdateNodeShowState(bool show)
        {
            int count = this.nodeLst.Count;
            for (int i = 0; i < this.transNodeObjs.Count; i++)
            {
                if (i < count)
                {
                    this.transNodeObjs[i].SetActive(show);
                }
                else
                {
                    this.transNodeObjs[i].SetActive(false);
                }
            }
        }
    }
}

