namespace AGE
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CurvlHelper : MonoBehaviour
    {
        private static Vector3 axisWeight = new Vector3(1f, 0f, 1f);
        public bool closeLine;
        public float ctrlPointScale = 0.6f;
        private ListView<CurvlData> curvlLst;
        public int lineStep = 15;
        public DictionaryObjectView<CurvlData, Track> mCurvlTrackMap = new DictionaryObjectView<CurvlData, Track>();
        public AGE.Action mPreviewAction;
        public static float sCtrlPtScale = 0.6f;
        public static string sCurvlHelperObjName = "CurvlHelper";
        public bool showOnGame;
        public bool showOnGizmo = true;
        public bool useSameInterpMode;

        public static bool CalTransform(ModifyTransform evt, Transform dstobj, Transform fromTransform, Transform toTransform, Transform coordTransform, ref Vector3 oPos, ref Quaternion oRot, ref Vector3 oScl)
        {
            if (dstobj == null)
            {
                return false;
            }
            if ((fromTransform != null) && (toTransform != null))
            {
                Vector3 vector = toTransform.position - fromTransform.position;
                vector = new Vector3(vector.x * axisWeight.x, vector.y * axisWeight.y, vector.z * axisWeight.z);
                Vector2 vector2 = new Vector2(vector.x, vector.z);
                float magnitude = vector2.magnitude;
                Quaternion quaternion = Quaternion.Inverse(Quaternion.LookRotation(Vector3.Normalize(vector), Vector3.up));
                if (evt.normalizedRelative)
                {
                    oPos = (Vector3) (quaternion * (dstobj.position - fromTransform.position));
                    oPos = new Vector3(oPos.x / magnitude, oPos.y, oPos.z / magnitude);
                    oPos = (Vector3) (oPos - ((new Vector3(0f, 1f, 0f) * oPos.z) * (toTransform.position.y - fromTransform.position.y)));
                }
                else
                {
                    oPos = (Vector3) (quaternion * (dstobj.position - fromTransform.position));
                    oPos = (Vector3) (oPos - ((new Vector3(0f, 1f, 0f) * (oPos.z / magnitude)) * (toTransform.position.y - fromTransform.position.y)));
                }
                oRot = quaternion * dstobj.rotation;
                oScl = dstobj.localScale;
            }
            else if (coordTransform != null)
            {
                oPos = coordTransform.InverseTransformPoint(dstobj.position);
                oRot = Quaternion.Inverse(coordTransform.rotation) * dstobj.rotation;
                oScl = dstobj.localScale;
            }
            else
            {
                oPos = dstobj.position;
                oRot = dstobj.rotation;
                oScl = dstobj.localScale;
            }
            return true;
        }

        public CurvlData CreateCurvl()
        {
            CurvlData data;
            if (this.curvlLst == null)
            {
                this.curvlLst = new ListView<CurvlData>();
            }
            for (int i = 0; i < this.curvlLst.Count; i++)
            {
                data = this.curvlLst[i];
                if (data == null)
                {
                    data = new CurvlData(i);
                    data.SetCurvlID(i);
                    this.curvlLst[i] = data;
                    return data;
                }
                if (!data.isInUse)
                {
                    data.isInUse = true;
                    return data;
                }
            }
            data = new CurvlData(this.curvlLst.Count);
            data.SetCurvlID(this.curvlLst.Count);
            this.curvlLst.Add(data);
            return data;
        }

        public void Draw()
        {
            if (this.curvlLst == null)
            {
                this.HideUnactiveCurvls();
            }
            else
            {
                foreach (CurvlData data in this.curvlLst)
                {
                    if (data != null)
                    {
                        data.stepsOfEachSegment = this.lineStep;
                        data.DrawCurvel(this.useSameInterpMode);
                    }
                }
            }
        }

        public CurvlData GetCurvl(int index)
        {
            if (((this.curvlLst != null) && (index >= 0)) && (index < this.curvlLst.Count))
            {
                return this.curvlLst[index];
            }
            return null;
        }

        public int GetCurvlCount()
        {
            if (this.curvlLst == null)
            {
                return 0;
            }
            return this.curvlLst.Count;
        }

        public static int[] GetSelectedCurvl()
        {
            return null;
        }

        private void HideUnactiveCurvls()
        {
            GameObject obj2 = GameObject.Find(sCurvlHelperObjName);
            if (obj2 != null)
            {
                Transform transform = obj2.transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public CurvlData InsertCurvl(int index)
        {
            CurvlData data;
            if (this.curvlLst == null)
            {
                this.curvlLst = new ListView<CurvlData>();
            }
            if (index < 0)
            {
                return null;
            }
            if (index >= this.curvlLst.Count)
            {
                data = new CurvlData(this.curvlLst.Count);
                data.SetCurvlID(this.curvlLst.Count);
                this.curvlLst.Add(data);
                return data;
            }
            data = this.curvlLst[index];
            if (data == null)
            {
                data = new CurvlData(index);
                data.SetCurvlID(index);
                this.curvlLst[index] = data;
                return data;
            }
            if (!data.isInUse)
            {
                data.isInUse = true;
                return data;
            }
            this.curvlLst.Insert(index, null);
            for (int i = index + 1; i < this.curvlLst.Count; i++)
            {
                CurvlData data2 = this.curvlLst[i];
                if (data2 != null)
                {
                    data2.SetCurvlID(i);
                }
            }
            data = new CurvlData(index);
            data.SetCurvlID(index);
            this.curvlLst[index] = data;
            return data;
        }

        private void OnDrawGizmos()
        {
            sCtrlPtScale = this.ctrlPointScale;
            if (this.showOnGizmo)
            {
                this.Draw();
            }
        }

        public void RemoveAllCurvl(bool destroy)
        {
            if (this.curvlLst != null)
            {
                foreach (CurvlData data in this.curvlLst)
                {
                    if (data != null)
                    {
                        data.Remove(destroy);
                        if (!destroy)
                        {
                            this.curvlLst[data.GetCurvlID()] = null;
                        }
                    }
                }
                if (destroy)
                {
                    this.curvlLst.Clear();
                }
            }
            if (destroy)
            {
                GameObject obj2 = GameObject.Find(sCurvlHelperObjName);
                if (obj2 != null)
                {
                    List<GameObject> list = new List<GameObject>();
                    int childCount = obj2.transform.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        Transform child = obj2.transform.GetChild(i);
                        int num3 = child.childCount;
                        for (int j = 0; j < num3; j++)
                        {
                            Transform transform2 = child.GetChild(j);
                            list.Add(transform2.gameObject);
                        }
                        list.Add(child.gameObject);
                    }
                    foreach (GameObject obj3 in list)
                    {
                        ActionManager.DestroyGameObject(obj3);
                    }
                }
            }
        }

        public void RemoveCurvl(int index, bool destroy)
        {
            CurvlData curvl = this.GetCurvl(index);
            if (curvl != null)
            {
                curvl.Remove(destroy);
                if (destroy)
                {
                    this.curvlLst.RemoveAt(index);
                    for (int i = index; i < this.curvlLst.Count; i++)
                    {
                        curvl = this.curvlLst[i];
                        if (curvl != null)
                        {
                            curvl.SetCurvlID(i);
                        }
                    }
                }
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
            sCtrlPtScale = this.ctrlPointScale;
            if (this.showOnGame)
            {
                this.Draw();
            }
        }
    }
}

