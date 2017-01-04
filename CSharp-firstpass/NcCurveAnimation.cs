using System;
using System.Collections.Generic;
using UnityEngine;

public class NcCurveAnimation : NcEffectAniBehaviour
{
    public bool m_bAutoDestruct = true;
    protected string[] m_ChildColorNames;
    protected MeshFilter[] m_ChildMeshFilters;
    protected Renderer[] m_ChildRenderers;
    protected string m_ColorName;
    [SerializeField]
    public List<NcInfoCurve> m_CurveInfoList;
    public float m_fDelayTime;
    public float m_fDurationTime = 0.6f;
    protected float m_fElapsedRate;
    protected float m_fStartTime;
    protected Material m_MainMaterial;
    protected MeshFilter m_MainMeshFilter;
    protected NcUvAnimation m_NcUvAnimation;
    protected Transform m_Transform;

    public int AddCurveInfo()
    {
        NcInfoCurve item = new NcInfoCurve {
            m_AniCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f),
            m_ToColor = (Vector4) Color.white
        };
        item.m_ToColor.w = 0f;
        if (this.m_CurveInfoList == null)
        {
            this.m_CurveInfoList = new List<NcInfoCurve>();
        }
        this.m_CurveInfoList.Add(item);
        return (this.m_CurveInfoList.Count - 1);
    }

    public int AddCurveInfo(NcInfoCurve addCurveInfo)
    {
        if (this.m_CurveInfoList == null)
        {
            this.m_CurveInfoList = new List<NcInfoCurve>();
        }
        this.m_CurveInfoList.Add(addCurveInfo.GetClone());
        return (this.m_CurveInfoList.Count - 1);
    }

    public void AppendTo(NcCurveAnimation target, bool bCurveOnly)
    {
        if (target.m_CurveInfoList == null)
        {
            target.m_CurveInfoList = new List<NcInfoCurve>();
        }
        foreach (NcInfoCurve curve in this.m_CurveInfoList)
        {
            target.m_CurveInfoList.Add(curve.GetClone());
        }
        if (!bCurveOnly)
        {
            target.m_fDelayTime = this.m_fDelayTime;
            target.m_fDurationTime = this.m_fDurationTime;
        }
    }

    private void Awake()
    {
    }

    private void ChangeMeshColor(MeshFilter mFilter, Color tarColor)
    {
        if ((mFilter != null) && (mFilter.mesh != null))
        {
            Color[] colors = mFilter.mesh.colors;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = tarColor;
            }
            mFilter.mesh.colors = colors;
        }
    }

    public bool CheckInvalidOption()
    {
        bool flag = false;
        for (int i = 0; i < this.m_CurveInfoList.Count; i++)
        {
            if (this.CheckInvalidOption(i))
            {
                flag = true;
            }
        }
        return flag;
    }

    public bool CheckInvalidOption(int nSrcIndex)
    {
        NcInfoCurve curveInfo = this.GetCurveInfo(nSrcIndex);
        if (curveInfo == null)
        {
            return false;
        }
        return ((((curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR) && (curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.SCALE)) && (curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.TEXTUREUV)) && false);
    }

    public void ClearAllCurveInfo()
    {
        if (this.m_CurveInfoList != null)
        {
            this.m_CurveInfoList.Clear();
        }
    }

    public void CopyTo(NcCurveAnimation target, bool bCurveOnly)
    {
        target.m_CurveInfoList = new List<NcInfoCurve>();
        foreach (NcInfoCurve curve in this.m_CurveInfoList)
        {
            target.m_CurveInfoList.Add(curve.GetClone());
        }
        if (!bCurveOnly)
        {
            target.m_fDelayTime = this.m_fDelayTime;
            target.m_fDurationTime = this.m_fDurationTime;
        }
    }

    public void DeleteCurveInfo(int nIndex)
    {
        if (((this.m_CurveInfoList != null) && (nIndex >= 0)) && (this.m_CurveInfoList.Count > nIndex))
        {
            this.m_CurveInfoList.Remove(this.m_CurveInfoList[nIndex]);
        }
    }

    public override int GetAnimationState()
    {
        if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
        {
            return -1;
        }
        if ((0f >= this.m_fDurationTime) || ((this.m_fStartTime != 0f) && base.IsEndAnimation()))
        {
            return 0;
        }
        return 1;
    }

    private float GetChildNextColorValue(NcInfoCurve curveInfo, int nIndex, float fValue, int arrayIndex)
    {
        if (curveInfo.m_bApplyOption[nIndex])
        {
            float num = fValue - curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex];
            curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex] = fValue;
            return num;
        }
        return 0f;
    }

    public NcInfoCurve GetCurveInfo(int nIndex)
    {
        if (((this.m_CurveInfoList != null) && (nIndex >= 0)) && (this.m_CurveInfoList.Count > nIndex))
        {
            return this.m_CurveInfoList[nIndex];
        }
        return null;
    }

    public NcInfoCurve GetCurveInfo(string curveName)
    {
        if (this.m_CurveInfoList != null)
        {
            foreach (NcInfoCurve curve in this.m_CurveInfoList)
            {
                if (curve.m_CurveName == curveName)
                {
                    return curve;
                }
            }
        }
        return null;
    }

    public int GetCurveInfoCount()
    {
        if (this.m_CurveInfoList == null)
        {
            return 0;
        }
        return this.m_CurveInfoList.Count;
    }

    public float GetElapsedRate()
    {
        return this.m_fElapsedRate;
    }

    private float GetNextScale(NcInfoCurve curveInfo, int nIndex, float fValue)
    {
        if (curveInfo.m_bApplyOption[nIndex])
        {
            float num = curveInfo.m_OriginalValue[nIndex] * (1f + fValue);
            float num2 = num - curveInfo.m_BeforeValue[nIndex];
            curveInfo.m_BeforeValue[nIndex] = num;
            return num2;
        }
        return 0f;
    }

    private float GetNextValue(NcInfoCurve curveInfo, int nIndex, float fValue)
    {
        if (curveInfo.m_bApplyOption[nIndex])
        {
            float num = fValue - curveInfo.m_BeforeValue[nIndex];
            curveInfo.m_BeforeValue[nIndex] = fValue;
            return num;
        }
        return 0f;
    }

    public float GetRepeatedRate()
    {
        return this.m_fElapsedRate;
    }

    private void InitAnimation()
    {
        this.m_fElapsedRate = 0f;
        this.m_Transform = base.transform;
        foreach (NcInfoCurve curve in this.m_CurveInfoList)
        {
            Color color;
            int num3;
            if (curve.m_bEnabled)
            {
                switch (curve.m_ApplyType)
                {
                    case NcInfoCurve.APPLY_TYPE.POSITION:
                        curve.m_OriginalValue = Vector4.zero;
                        curve.m_BeforeValue = curve.m_OriginalValue;
                        break;

                    case NcInfoCurve.APPLY_TYPE.ROTATION:
                        curve.m_OriginalValue = Vector4.zero;
                        curve.m_BeforeValue = curve.m_OriginalValue;
                        break;

                    case NcInfoCurve.APPLY_TYPE.SCALE:
                        curve.m_OriginalValue = this.m_Transform.localScale;
                        curve.m_BeforeValue = curve.m_OriginalValue;
                        break;

                    case NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
                        if (!curve.m_bRecursively)
                        {
                            goto Label_01B0;
                        }
                        this.m_ChildRenderers = base.transform.GetComponentsInChildren<Renderer>(true);
                        this.m_ChildColorNames = new string[this.m_ChildRenderers.Length];
                        curve.m_ChildOriginalColorValues = new Vector4[this.m_ChildRenderers.Length];
                        curve.m_ChildBeforeColorValues = new Vector4[this.m_ChildRenderers.Length];
                        for (int i = 0; i < this.m_ChildRenderers.Length; i++)
                        {
                            Renderer renderer = this.m_ChildRenderers[i];
                            this.m_ChildColorNames[i] = Ng_GetMaterialColorName(renderer.sharedMaterial);
                            if (this.m_ChildColorNames[i] != null)
                            {
                                curve.m_ChildOriginalColorValues[i] = (Vector4) renderer.material.GetColor(this.m_ChildColorNames[i]);
                            }
                            curve.m_ChildBeforeColorValues[i] = Vector4.zero;
                        }
                        break;

                    case NcInfoCurve.APPLY_TYPE.TEXTUREUV:
                        if (this.m_NcUvAnimation == null)
                        {
                            this.m_NcUvAnimation = base.GetComponent<NcUvAnimation>();
                        }
                        if (this.m_NcUvAnimation != null)
                        {
                            curve.m_OriginalValue = new Vector4(this.m_NcUvAnimation.m_fScrollSpeedX, this.m_NcUvAnimation.m_fScrollSpeedY, 0f, 0f);
                        }
                        curve.m_BeforeValue = curve.m_OriginalValue;
                        break;

                    case NcInfoCurve.APPLY_TYPE.MESH_COLOR:
                    {
                        float t = curve.m_AniCurve.Evaluate(0f);
                        color = Color.Lerp(curve.m_FromColor, curve.m_ToColor, t);
                        if (!curve.m_bRecursively)
                        {
                            goto Label_031B;
                        }
                        this.m_ChildMeshFilters = base.transform.GetComponentsInChildren<MeshFilter>(true);
                        if ((this.m_ChildMeshFilters != null) && (this.m_ChildMeshFilters.Length >= 0))
                        {
                            goto Label_02E8;
                        }
                        break;
                    }
                }
            }
            continue;
        Label_01B0:
            if (base.GetRenderer() != null)
            {
                this.m_ColorName = Ng_GetMaterialColorName(base.GetRenderer().sharedMaterial);
                if (this.m_ColorName != null)
                {
                    curve.m_OriginalValue = (Vector4) base.GetRenderer().sharedMaterial.GetColor(this.m_ColorName);
                }
                curve.m_BeforeValue = Vector4.zero;
            }
            continue;
        Label_02E8:
            num3 = 0;
            while (num3 < this.m_ChildMeshFilters.Length)
            {
                this.ChangeMeshColor(this.m_ChildMeshFilters[num3], color);
                num3++;
            }
            continue;
        Label_031B:
            this.m_MainMeshFilter = base.GetComponent<MeshFilter>();
            this.ChangeMeshColor(this.m_MainMeshFilter, color);
        }
    }

    private void LateUpdate()
    {
        if (this.m_fStartTime != 0f)
        {
            if (this.m_fDelayTime != 0f)
            {
                if (NcEffectBehaviour.GetEngineTime() < (this.m_fStartTime + this.m_fDelayTime))
                {
                    return;
                }
                this.m_fDelayTime = 0f;
                base.InitAnimationTimer();
                if (base.GetRenderer() != null)
                {
                    base.GetRenderer().enabled = true;
                }
            }
            float time = base.m_Timer.GetTime();
            float fElapsedRate = time;
            if (this.m_fDurationTime != 0f)
            {
                fElapsedRate = time / this.m_fDurationTime;
            }
            this.UpdateAnimation(fElapsedRate);
        }
    }

    public static string Ng_GetMaterialColorName(Material mat)
    {
        string[] strArray = new string[] { "_Color", "_TintColor", "_EmisColor" };
        if (mat != null)
        {
            foreach (string str in strArray)
            {
                if (mat.HasProperty(str))
                {
                    return str;
                }
            }
        }
        return null;
    }

    public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
    {
        this.m_fDelayTime /= fSpeedRate;
        this.m_fDurationTime /= fSpeedRate;
    }

    public override void ResetAnimation()
    {
        base.ResetAnimation();
        this.InitAnimation();
        this.UpdateAnimation(0f);
    }

    private unsafe void SetChildMaterialColor(NcInfoCurve curveInfo, float fValue, int arrayIndex)
    {
        Color color = curveInfo.m_ToColor - curveInfo.m_ChildOriginalColorValues[arrayIndex];
        Color color2 = this.m_ChildRenderers[arrayIndex].material.GetColor(this.m_ChildColorNames[arrayIndex]);
        for (int i = 0; i < 4; i++)
        {
            ref Color colorRef;
            int num2;
            float num3 = colorRef[num2];
            (colorRef = (Color) &color2)[num2 = i] = num3 + this.GetChildNextColorValue(curveInfo, i, color[i] * fValue, arrayIndex);
        }
        this.m_ChildRenderers[arrayIndex].material.SetColor(this.m_ChildColorNames[arrayIndex], color2);
    }

    public NcInfoCurve SetCurveInfo(int nIndex, NcInfoCurve newInfo)
    {
        if (((this.m_CurveInfoList == null) || (nIndex < 0)) || (this.m_CurveInfoList.Count <= nIndex))
        {
            return null;
        }
        NcInfoCurve curve = this.m_CurveInfoList[nIndex];
        this.m_CurveInfoList[nIndex] = newInfo;
        return curve;
    }

    public void SortCurveInfo()
    {
        if (this.m_CurveInfoList != null)
        {
            this.m_CurveInfoList.Sort(new NcComparerCurve());
            foreach (NcInfoCurve curve in this.m_CurveInfoList)
            {
                curve.m_nSortGroup = NcComparerCurve.GetSortGroup(curve);
            }
        }
    }

    private void Start()
    {
        this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
        this.InitAnimation();
        if (0f < this.m_fDelayTime)
        {
            if (base.GetRenderer() != null)
            {
                base.GetRenderer().enabled = false;
            }
        }
        else
        {
            base.InitAnimationTimer();
            this.UpdateAnimation(0f);
        }
    }

    private unsafe void UpdateAnimation(float fElapsedRate)
    {
        this.m_fElapsedRate = fElapsedRate;
        foreach (NcInfoCurve curve in this.m_CurveInfoList)
        {
            float num;
            int num2;
            Color color3;
            int num4;
            if (curve.m_bEnabled)
            {
                num = curve.m_AniCurve.Evaluate(this.m_fElapsedRate);
                if ((curve.m_ApplyType != NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR) && (curve.m_ApplyType != NcInfoCurve.APPLY_TYPE.MESH_COLOR))
                {
                    num *= curve.m_fValueScale;
                }
                switch (curve.m_ApplyType)
                {
                    case NcInfoCurve.APPLY_TYPE.POSITION:
                        if (curve.m_bApplyOption[3])
                        {
                            this.m_Transform.position += new Vector3(this.GetNextValue(curve, 0, num), this.GetNextValue(curve, 1, num), this.GetNextValue(curve, 2, num));
                        }
                        else
                        {
                            this.m_Transform.localPosition += new Vector3(this.GetNextValue(curve, 0, num), this.GetNextValue(curve, 1, num), this.GetNextValue(curve, 2, num));
                        }
                        break;

                    case NcInfoCurve.APPLY_TYPE.ROTATION:
                        if (!curve.m_bApplyOption[3])
                        {
                            goto Label_0163;
                        }
                        this.m_Transform.rotation *= Quaternion.Euler(this.GetNextValue(curve, 0, num), this.GetNextValue(curve, 1, num), this.GetNextValue(curve, 2, num));
                        break;

                    case NcInfoCurve.APPLY_TYPE.SCALE:
                        this.m_Transform.localScale += new Vector3(this.GetNextScale(curve, 0, num), this.GetNextScale(curve, 1, num), this.GetNextScale(curve, 2, num));
                        break;

                    case NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
                        if (!curve.m_bRecursively)
                        {
                            goto Label_0249;
                        }
                        if ((this.m_ChildColorNames != null) && (this.m_ChildColorNames.Length >= 0))
                        {
                            goto Label_0202;
                        }
                        break;

                    case NcInfoCurve.APPLY_TYPE.TEXTUREUV:
                        if (this.m_NcUvAnimation != null)
                        {
                            this.m_NcUvAnimation.m_fScrollSpeedX += this.GetNextValue(curve, 0, num);
                            this.m_NcUvAnimation.m_fScrollSpeedY += this.GetNextValue(curve, 1, num);
                        }
                        break;

                    case NcInfoCurve.APPLY_TYPE.MESH_COLOR:
                        color3 = Color.Lerp(curve.m_FromColor, curve.m_ToColor, num);
                        if (!curve.m_bRecursively)
                        {
                            goto Label_03E2;
                        }
                        if ((this.m_ChildMeshFilters != null) && (this.m_ChildMeshFilters.Length >= 0))
                        {
                            goto Label_03AF;
                        }
                        break;
                }
            }
            continue;
        Label_0163:
            this.m_Transform.localRotation *= Quaternion.Euler(this.GetNextValue(curve, 0, num), this.GetNextValue(curve, 1, num), this.GetNextValue(curve, 2, num));
            continue;
        Label_0202:
            num2 = 0;
            while (num2 < this.m_ChildColorNames.Length)
            {
                if ((this.m_ChildColorNames[num2] != null) && (this.m_ChildRenderers[num2] != null))
                {
                    this.SetChildMaterialColor(curve, num, num2);
                }
                num2++;
            }
            continue;
        Label_0249:
            if ((base.GetRenderer() != null) && (this.m_ColorName != null))
            {
                if (this.m_MainMaterial == null)
                {
                    this.m_MainMaterial = base.GetRenderer().material;
                    base.AddRuntimeMaterial(this.m_MainMaterial);
                }
                Color color = curve.m_ToColor - curve.m_OriginalValue;
                Color color2 = this.m_MainMaterial.GetColor(this.m_ColorName);
                for (int i = 0; i < 4; i++)
                {
                    ref Color colorRef;
                    int num5;
                    float num6 = colorRef[num5];
                    (colorRef = (Color) &color2)[num5 = i] = num6 + this.GetNextValue(curve, i, color[i] * num);
                }
                this.m_MainMaterial.SetColor(this.m_ColorName, color2);
            }
            continue;
        Label_03AF:
            num4 = 0;
            while (num4 < this.m_ChildMeshFilters.Length)
            {
                this.ChangeMeshColor(this.m_ChildMeshFilters[num4], color3);
                num4++;
            }
            continue;
        Label_03E2:
            this.ChangeMeshColor(this.m_MainMeshFilter, color3);
        }
        if ((this.m_fDurationTime != 0f) && (1f < this.m_fElapsedRate))
        {
            if (!base.IsEndAnimation())
            {
                base.OnEndAnimation();
            }
            if (this.m_bAutoDestruct)
            {
                UnityEngine.Object.DestroyObject(base.gameObject);
            }
        }
    }

    private class NcComparerCurve : IComparer<NcCurveAnimation.NcInfoCurve>
    {
        protected static float m_fEqualRange = 0.03f;
        protected static float m_fHDiv = 5f;

        public int Compare(NcCurveAnimation.NcInfoCurve a, NcCurveAnimation.NcInfoCurve b)
        {
            float f = a.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv) - b.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv);
            if (Mathf.Abs(f) < m_fEqualRange)
            {
                f = b.m_AniCurve.Evaluate(1f - (m_fEqualRange / m_fHDiv)) - a.m_AniCurve.Evaluate(1f - (m_fEqualRange / m_fHDiv));
                if (Mathf.Abs(f) < m_fEqualRange)
                {
                    return 0;
                }
            }
            return (int) (f * 1000f);
        }

        public static int GetSortGroup(NcCurveAnimation.NcInfoCurve info)
        {
            float num = info.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv);
            if (num < -m_fEqualRange)
            {
                return 1;
            }
            if (m_fEqualRange < num)
            {
                return 3;
            }
            return 2;
        }
    }

    [Serializable]
    public class NcInfoCurve
    {
        public AnimationCurve m_AniCurve = new AnimationCurve();
        public APPLY_TYPE m_ApplyType = APPLY_TYPE.POSITION;
        public bool[] m_bApplyOption;
        public Vector4 m_BeforeValue;
        public bool m_bEnabled = true;
        public bool m_bRecursively;
        public Vector4[] m_ChildBeforeColorValues;
        public Vector4[] m_ChildOriginalColorValues;
        public string m_CurveName = string.Empty;
        protected const float m_fOverDraw = 0.2f;
        public Vector4 m_FromColor;
        public float m_fValueScale;
        public int m_nSortGroup;
        public int m_nTag;
        public Vector4 m_OriginalValue;
        public Vector4 m_ToColor;
        public static string[] m_TypeName = new string[] { "None", "Position", "Rotation", "Scale", "MaterialColor", "TextureUV", "MeshColor" };

        public NcInfoCurve()
        {
            bool[] flagArray1 = new bool[4];
            flagArray1[1] = true;
            this.m_bApplyOption = flagArray1;
            this.m_fValueScale = 1f;
            this.m_FromColor = (Vector4) Color.white;
            this.m_ToColor = (Vector4) Color.white;
        }

        public void CopyTo(NcCurveAnimation.NcInfoCurve target)
        {
            target.m_AniCurve = new AnimationCurve(this.m_AniCurve.keys);
            target.m_AniCurve.postWrapMode = this.m_AniCurve.postWrapMode;
            target.m_AniCurve.preWrapMode = this.m_AniCurve.preWrapMode;
            target.m_bEnabled = this.m_bEnabled;
            target.m_ApplyType = this.m_ApplyType;
            Array.Copy(this.m_bApplyOption, target.m_bApplyOption, this.m_bApplyOption.Length);
            target.m_fValueScale = this.m_fValueScale;
            target.m_bRecursively = this.m_bRecursively;
            target.m_FromColor = this.m_FromColor;
            target.m_ToColor = this.m_ToColor;
            target.m_nTag = this.m_nTag;
            target.m_nSortGroup = this.m_nSortGroup;
        }

        public NcCurveAnimation.NcInfoCurve GetClone()
        {
            NcCurveAnimation.NcInfoCurve curve = new NcCurveAnimation.NcInfoCurve {
                m_AniCurve = new AnimationCurve(this.m_AniCurve.keys)
            };
            curve.m_AniCurve.postWrapMode = this.m_AniCurve.postWrapMode;
            curve.m_AniCurve.preWrapMode = this.m_AniCurve.preWrapMode;
            curve.m_bEnabled = this.m_bEnabled;
            curve.m_CurveName = this.m_CurveName;
            curve.m_ApplyType = this.m_ApplyType;
            Array.Copy(this.m_bApplyOption, curve.m_bApplyOption, this.m_bApplyOption.Length);
            curve.m_fValueScale = this.m_fValueScale;
            curve.m_bRecursively = this.m_bRecursively;
            curve.m_FromColor = this.m_FromColor;
            curve.m_ToColor = this.m_ToColor;
            curve.m_nTag = this.m_nTag;
            curve.m_nSortGroup = this.m_nSortGroup;
            return curve;
        }

        public string GetCurveName()
        {
            return this.m_CurveName;
        }

        public Rect GetEditRange()
        {
            return new Rect(0f, -1f, 1f, 2f);
        }

        public Rect GetFixedDrawRange()
        {
            return new Rect(-0.2f, -1.2f, 1.4f, 2.4f);
        }

        public int GetValueCount()
        {
            switch (this.m_ApplyType)
            {
                case APPLY_TYPE.POSITION:
                    return 4;

                case APPLY_TYPE.ROTATION:
                    return 4;

                case APPLY_TYPE.SCALE:
                    return 3;

                case APPLY_TYPE.MATERIAL_COLOR:
                    return 4;

                case APPLY_TYPE.TEXTUREUV:
                    return 2;

                case APPLY_TYPE.MESH_COLOR:
                    return 4;
            }
            return 0;
        }

        public string GetValueName(int nIndex)
        {
            string[] strArray;
            switch (this.m_ApplyType)
            {
                case APPLY_TYPE.POSITION:
                case APPLY_TYPE.ROTATION:
                    strArray = new string[] { "X", "Y", "Z", "World" };
                    break;

                case APPLY_TYPE.SCALE:
                    strArray = new string[] { "X", "Y", "Z", string.Empty };
                    break;

                case APPLY_TYPE.MATERIAL_COLOR:
                    strArray = new string[] { "R", "G", "B", "A" };
                    break;

                case APPLY_TYPE.TEXTUREUV:
                    strArray = new string[] { "X", "Y", string.Empty, string.Empty };
                    break;

                case APPLY_TYPE.MESH_COLOR:
                    strArray = new string[] { "R", "G", "B", "A" };
                    break;

                default:
                    strArray = new string[] { string.Empty, string.Empty, string.Empty, string.Empty };
                    break;
            }
            return strArray[nIndex];
        }

        public Rect GetVariableDrawRange()
        {
            Rect rect = new Rect();
            for (int i = 0; i < this.m_AniCurve.keys.Length; i++)
            {
                Keyframe keyframe = this.m_AniCurve[i];
                rect.yMin = Mathf.Min(rect.yMin, keyframe.value);
                Keyframe keyframe2 = this.m_AniCurve[i];
                rect.yMax = Mathf.Max(rect.yMax, keyframe2.value);
            }
            int num2 = 20;
            for (int j = 0; j < num2; j++)
            {
                float b = this.m_AniCurve.Evaluate(((float) j) / ((float) num2));
                rect.yMin = Mathf.Min(rect.yMin, b);
                rect.yMax = Mathf.Max(rect.yMax, b);
            }
            rect.xMin = 0f;
            rect.xMax = 1f;
            rect.xMin -= rect.width * 0.2f;
            rect.xMax += rect.width * 0.2f;
            rect.yMin -= rect.height * 0.2f;
            rect.yMax += rect.height * 0.2f;
            return rect;
        }

        public bool IsEnabled()
        {
            return this.m_bEnabled;
        }

        public void NormalizeCurveTime()
        {
            int index = 0;
            while (index < this.m_AniCurve.keys.Length)
            {
                Keyframe keyframe = this.m_AniCurve[index];
                float a = Mathf.Max(0f, keyframe.time);
                float time = Mathf.Min(1f, Mathf.Max(a, keyframe.time));
                if (time != keyframe.time)
                {
                    Keyframe key = new Keyframe(time, keyframe.value, keyframe.inTangent, keyframe.outTangent);
                    this.m_AniCurve.RemoveKey(index);
                    index = 0;
                    this.m_AniCurve.AddKey(key);
                }
                else
                {
                    index++;
                }
            }
        }

        public void SetDefaultValueScale()
        {
            switch (this.m_ApplyType)
            {
                case APPLY_TYPE.POSITION:
                    this.m_fValueScale = 1f;
                    break;

                case APPLY_TYPE.ROTATION:
                    this.m_fValueScale = 360f;
                    break;

                case APPLY_TYPE.SCALE:
                    this.m_fValueScale = 1f;
                    break;

                case APPLY_TYPE.TEXTUREUV:
                    this.m_fValueScale = 10f;
                    break;
            }
        }

        public void SetEnabled(bool bEnable)
        {
            this.m_bEnabled = bEnable;
        }

        public enum APPLY_TYPE
        {
            NONE,
            POSITION,
            ROTATION,
            SCALE,
            MATERIAL_COLOR,
            TEXTUREUV,
            MESH_COLOR
        }
    }
}

