namespace PigeonCoopToolkit.Effects.Trails
{
    using PigeonCoopToolkit.Utillities;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class TrailRenderer_Base : MonoBehaviour
    {
        private PCTrail _activeTrail;
        protected bool _emit;
        private ListView<PCTrail> _fadingTrails;
        private static bool _hasRenderer;
        private static DictionaryView<Material, ListView<PCTrail>> _matToTrailList;
        protected bool _noDecay;
        protected Transform _t;
        private static ListView<Mesh> _toClean;
        public bool Emit;
        private static int GlobalTrailRendererCount;
        public PCTrailRendererData TrailData;

        protected TrailRenderer_Base()
        {
        }

        protected void AddPoint(PCTrailPoint newPoint, Vector3 pos)
        {
            if (this._activeTrail != null)
            {
                newPoint.Position = pos;
                newPoint.PointNumber = (this._activeTrail.Points.Count != 0) ? (this._activeTrail.Points[this._activeTrail.Points.Count - 1].PointNumber + 1) : 0;
                this.InitialiseNewPoint(newPoint);
                newPoint.SetDistanceFromStart((this._activeTrail.Points.Count != 0) ? (this._activeTrail.Points[this._activeTrail.Points.Count - 1].GetDistanceFromStart() + Vector3.Distance(this._activeTrail.Points[this._activeTrail.Points.Count - 1].Position, pos)) : 0f);
                if (this.TrailData.UseForwardOverride)
                {
                    newPoint.Forward = !this.TrailData.ForwardOverrideRelative ? this.TrailData.ForwardOverride.normalized : this._t.TransformDirection(this.TrailData.ForwardOverride.normalized);
                }
                this._activeTrail.Points.Add(newPoint);
            }
        }

        protected bool AnyElement(CircularBuffer<PCTrailPoint> InPoints)
        {
            for (int i = 0; i < InPoints.Count; i++)
            {
                PCTrailPoint point = InPoints[i];
                if (point.TimeActive() < this.TrailData.Lifetime)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void Awake()
        {
            GlobalTrailRendererCount++;
            if (GlobalTrailRendererCount == 1)
            {
                _matToTrailList = new DictionaryView<Material, ListView<PCTrail>>();
                _toClean = new ListView<Mesh>();
            }
            this._fadingTrails = new ListView<PCTrail>();
            this._t = base.transform;
            this._emit = this.Emit;
            if (this._emit)
            {
                this._activeTrail = new PCTrail(this.GetMaxNumberOfPoints());
                this._activeTrail.IsActiveTrail = true;
                this.OnStartEmit();
            }
        }

        private void CheckEmitChange()
        {
            if (this._emit != this.Emit)
            {
                this._emit = this.Emit;
                if (this._emit)
                {
                    this._activeTrail = new PCTrail(this.GetMaxNumberOfPoints());
                    this._activeTrail.IsActiveTrail = true;
                    this.OnStartEmit();
                }
                else
                {
                    this.OnStopEmit();
                    this._activeTrail.IsActiveTrail = false;
                    this._fadingTrails.Add(this._activeTrail);
                    this._activeTrail = null;
                }
            }
        }

        public void ClearSystem(bool emitState)
        {
            if (this._activeTrail != null)
            {
                this._activeTrail.Dispose();
                this._activeTrail = null;
            }
            if (this._fadingTrails != null)
            {
                for (int i = 0; i < this._fadingTrails.Count; i++)
                {
                    PCTrail trail = this._fadingTrails[i];
                    if (trail != null)
                    {
                        trail.Dispose();
                    }
                }
                this._fadingTrails.Clear();
            }
            this.Emit = emitState;
            this._emit = !emitState;
            this.CheckEmitChange();
        }

        public void CreateTrail(Vector3 from, Vector3 to, float distanceBetweenPoints)
        {
            float num = Vector3.Distance(from, to);
            Vector3 normalized = to - from;
            normalized = normalized.normalized;
            float num2 = 0f;
            CircularBuffer<PCTrailPoint> buffer = new CircularBuffer<PCTrailPoint>(this.GetMaxNumberOfPoints());
            int num3 = 0;
            while (num2 < num)
            {
                PCTrailPoint point = new PCTrailPoint {
                    PointNumber = num3,
                    Position = from + ((Vector3) (normalized * num2))
                };
                buffer.Add(point);
                this.InitialiseNewPoint(point);
                num3++;
                if (distanceBetweenPoints <= 0f)
                {
                    break;
                }
                num2 += distanceBetweenPoints;
            }
            PCTrailPoint item = new PCTrailPoint {
                PointNumber = num3,
                Position = to
            };
            buffer.Add(item);
            this.InitialiseNewPoint(item);
            PCTrail trail = new PCTrail(this.GetMaxNumberOfPoints()) {
                Points = buffer
            };
            this._fadingTrails.Add(trail);
        }

        private void DrawMesh(Mesh trailMesh, Material trailMaterial)
        {
            Graphics.DrawMesh(trailMesh, Matrix4x4.identity, trailMaterial, base.gameObject.layer);
        }

        private void GenerateMesh(PCTrail trail)
        {
            trail.Mesh.Clear(false);
            Vector3 rhs = (Camera.main == null) ? Vector3.forward : Camera.main.transform.forward;
            if (this.TrailData.UseForwardOverride)
            {
                rhs = this.TrailData.ForwardOverride.normalized;
            }
            trail.activePointCount = this.NumberOfActivePoints(trail);
            if (trail.activePointCount >= 2)
            {
                int index = 0;
                for (int i = 0; i < trail.Points.Count; i++)
                {
                    PCTrailPoint point = trail.Points[i];
                    float time = point.TimeActive() / this.TrailData.Lifetime;
                    if (point.TimeActive() <= this.TrailData.Lifetime)
                    {
                        if (this.TrailData.UseForwardOverride && this.TrailData.ForwardOverrideRelative)
                        {
                            rhs = point.Forward;
                        }
                        Vector3 zero = Vector3.zero;
                        if (i < (trail.Points.Count - 1))
                        {
                            Vector3 vector4 = trail.Points[i + 1].Position - point.Position;
                            zero = Vector3.Cross(vector4.normalized, rhs).normalized;
                        }
                        else
                        {
                            Vector3 vector6 = point.Position - trail.Points[i - 1].Position;
                            zero = Vector3.Cross(vector6.normalized, rhs).normalized;
                        }
                        Color color = !this.TrailData.StretchColorToFit ? (!this.TrailData.UsingSimpleColor ? this.TrailData.ColorOverLife.Evaluate(time) : Color.Lerp(this.TrailData.SimpleColorOverLifeStart, this.TrailData.SimpleColorOverLifeEnd, time)) : (!this.TrailData.UsingSimpleColor ? this.TrailData.ColorOverLife.Evaluate(1f - ((((float) index) / ((float) trail.activePointCount)) / 2f)) : Color.Lerp(this.TrailData.SimpleColorOverLifeStart, this.TrailData.SimpleColorOverLifeEnd, 1f - ((((float) index) / ((float) trail.activePointCount)) / 2f)));
                        float num4 = !this.TrailData.StretchSizeToFit ? (!this.TrailData.UsingSimpleSize ? this.TrailData.SizeOverLife.Evaluate(time) : Mathf.Lerp(this.TrailData.SimpleSizeOverLifeStart, this.TrailData.SimpleSizeOverLifeEnd, time)) : (!this.TrailData.UsingSimpleSize ? this.TrailData.SizeOverLife.Evaluate(1f - ((((float) index) / ((float) trail.activePointCount)) / 2f)) : Mathf.Lerp(this.TrailData.SimpleSizeOverLifeStart, this.TrailData.SimpleSizeOverLifeEnd, 1f - ((((float) index) / ((float) trail.activePointCount)) / 2f)));
                        trail.verticies[index] = point.Position + ((Vector3) (zero * num4));
                        if (this.TrailData.MaterialTileLength <= 0f)
                        {
                            trail.uvs[index] = new Vector2((((float) index) / ((float) trail.activePointCount)) / 2f, 0f);
                        }
                        else
                        {
                            trail.uvs[index] = new Vector2(point.GetDistanceFromStart() / this.TrailData.MaterialTileLength, 0f);
                        }
                        trail.normals[index] = rhs;
                        trail.colors[index] = color;
                        index++;
                        trail.verticies[index] = point.Position - ((Vector3) (zero * num4));
                        if (this.TrailData.MaterialTileLength <= 0f)
                        {
                            trail.uvs[index] = new Vector2((((float) index) / ((float) trail.activePointCount)) / 2f, 1f);
                        }
                        else
                        {
                            trail.uvs[index] = new Vector2(point.GetDistanceFromStart() / this.TrailData.MaterialTileLength, 1f);
                        }
                        trail.normals[index] = rhs;
                        trail.colors[index] = color;
                        index++;
                    }
                }
                Vector2 vector3 = trail.verticies[index - 1];
                for (int j = index; j < trail.verticies.Length; j++)
                {
                    trail.verticies[j] = (Vector3) vector3;
                }
                int num6 = 0;
                for (int k = 0; k < (2 * (trail.activePointCount - 1)); k++)
                {
                    if ((k % 2) == 0)
                    {
                        trail.indicies[num6] = k;
                        num6++;
                        trail.indicies[num6] = k + 1;
                        num6++;
                        trail.indicies[num6] = k + 2;
                    }
                    else
                    {
                        trail.indicies[num6] = k + 2;
                        num6++;
                        trail.indicies[num6] = k + 1;
                        num6++;
                        trail.indicies[num6] = k;
                    }
                    num6++;
                }
                int num8 = trail.indicies[num6 - 1];
                for (int m = num6; m < trail.indicies.Length; m++)
                {
                    trail.indicies[m] = num8;
                }
                trail.Mesh.vertices = trail.verticies;
                trail.Mesh.SetIndices(trail.indicies, MeshTopology.Triangles, 0);
                trail.Mesh.uv = trail.uvs;
                trail.Mesh.normals = trail.normals;
                trail.Mesh.colors = trail.colors;
            }
        }

        protected abstract int GetMaxNumberOfPoints();
        protected virtual void InitialiseNewPoint(PCTrailPoint newPoint)
        {
        }

        protected virtual void LateUpdate()
        {
            if (!_hasRenderer)
            {
                _hasRenderer = true;
                foreach (KeyValuePair<Material, ListView<PCTrail>> pair in _matToTrailList)
                {
                    CombineInstance[] combine = new CombineInstance[pair.Value.Count];
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        combine[i] = new CombineInstance { mesh = pair.Value[i].Mesh, subMeshIndex = 0, transform = Matrix4x4.identity };
                    }
                    Mesh item = new Mesh();
                    item.CombineMeshes(combine, true, false);
                    _toClean.Add(item);
                    this.DrawMesh(item, pair.Key);
                    pair.Value.Clear();
                }
            }
        }

        public void LifeDecayEnabled(bool enabled)
        {
            this._noDecay = !enabled;
        }

        private int NumberOfActivePoints(PCTrail line)
        {
            int num = 0;
            for (int i = 0; i < line.Points.Count; i++)
            {
                if (line.Points[i].TimeActive() < this.TrailData.Lifetime)
                {
                    num++;
                }
            }
            return num;
        }

        public int NumSegments()
        {
            int num = 0;
            if ((this._activeTrail != null) && (this.NumberOfActivePoints(this._activeTrail) != 0))
            {
                num++;
            }
            return (num + this._fadingTrails.Count);
        }

        protected virtual void OnDestroy()
        {
            GlobalTrailRendererCount--;
            if (GlobalTrailRendererCount == 0)
            {
                if ((_toClean != null) && (_toClean.Count > 0))
                {
                    for (int i = 0; i < _toClean.Count; i++)
                    {
                        Mesh mesh = _toClean[i];
                        if (mesh != null)
                        {
                            if (Application.isEditor)
                            {
                                UnityEngine.Object.DestroyImmediate(mesh, true);
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(mesh);
                            }
                        }
                    }
                }
                _toClean = null;
                _matToTrailList.Clear();
                _matToTrailList = null;
            }
            if (this._activeTrail != null)
            {
                this._activeTrail.Dispose();
                this._activeTrail = null;
            }
            if (this._fadingTrails != null)
            {
                for (int j = 0; j < this._fadingTrails.Count; j++)
                {
                    PCTrail trail = this._fadingTrails[j];
                    if (trail != null)
                    {
                        trail.Dispose();
                    }
                }
                this._fadingTrails.Clear();
            }
        }

        protected virtual void OnStartEmit()
        {
        }

        protected virtual void OnStopEmit()
        {
        }

        protected virtual void OnTranslate(Vector3 t)
        {
        }

        protected virtual void Reset()
        {
            if (this.TrailData == null)
            {
                this.TrailData = new PCTrailRendererData();
            }
            this.TrailData.Lifetime = 1f;
            this.TrailData.UsingSimpleColor = false;
            this.TrailData.UsingSimpleSize = false;
            this.TrailData.ColorOverLife = new Gradient();
            this.TrailData.SimpleColorOverLifeStart = Color.white;
            this.TrailData.SimpleColorOverLifeEnd = new Color(1f, 1f, 1f, 0f);
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(1f, 0f) };
            this.TrailData.SizeOverLife = new AnimationCurve(keys);
            this.TrailData.SimpleSizeOverLifeStart = 1f;
            this.TrailData.SimpleSizeOverLifeEnd = 0f;
        }

        protected virtual void Start()
        {
        }

        [ContextMenu("Toggle inspector color input method")]
        protected void ToggleColorInputStyle()
        {
            this.TrailData.UsingSimpleColor = !this.TrailData.UsingSimpleColor;
        }

        [ContextMenu("Toggle inspector size input method")]
        protected void ToggleSizeInputStyle()
        {
            this.TrailData.UsingSimpleSize = !this.TrailData.UsingSimpleSize;
        }

        public void Translate(Vector3 t)
        {
            if (this._activeTrail != null)
            {
                for (int i = 0; i < this._activeTrail.Points.Count; i++)
                {
                    PCTrailPoint local1 = this._activeTrail.Points[i];
                    local1.Position += t;
                }
            }
            if (this._fadingTrails != null)
            {
                for (int j = 0; j < this._fadingTrails.Count; j++)
                {
                    PCTrail trail = this._fadingTrails[j];
                    if (trail != null)
                    {
                        for (int k = 0; k < trail.Points.Count; k++)
                        {
                            PCTrailPoint local2 = trail.Points[k];
                            local2.Position += t;
                        }
                    }
                }
            }
            this.OnTranslate(t);
        }

        protected virtual void Update()
        {
            if (_hasRenderer)
            {
                _hasRenderer = false;
                if (_toClean.Count > 0)
                {
                    for (int j = 0; j < _toClean.Count; j++)
                    {
                        Mesh mesh = _toClean[j];
                        if (mesh != null)
                        {
                            if (Application.isEditor)
                            {
                                UnityEngine.Object.DestroyImmediate(mesh, true);
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(mesh);
                            }
                        }
                    }
                }
                _toClean.Clear();
            }
            if (!_matToTrailList.ContainsKey(this.TrailData.TrailMaterial))
            {
                _matToTrailList.Add(this.TrailData.TrailMaterial, new ListView<PCTrail>());
            }
            if (this._activeTrail != null)
            {
                this.UpdatePoints(this._activeTrail, Time.deltaTime);
                this.UpdateTrail(this._activeTrail, Time.deltaTime);
                this.GenerateMesh(this._activeTrail);
                _matToTrailList[this.TrailData.TrailMaterial].Add(this._activeTrail);
            }
            for (int i = this._fadingTrails.Count - 1; i >= 0; i--)
            {
                if ((this._fadingTrails[i] == null) || !this.AnyElement(this._fadingTrails[i].Points))
                {
                    if (this._fadingTrails[i] != null)
                    {
                        this._fadingTrails[i].Dispose();
                    }
                    this._fadingTrails.RemoveAt(i);
                }
                else
                {
                    this.UpdatePoints(this._fadingTrails[i], Time.deltaTime);
                    this.UpdateTrail(this._fadingTrails[i], Time.deltaTime);
                    this.GenerateMesh(this._fadingTrails[i]);
                    _matToTrailList[this.TrailData.TrailMaterial].Add(this._fadingTrails[i]);
                }
            }
            this.CheckEmitChange();
        }

        [Obsolete("UpdatePoint is deprecated, you should instead override UpdateTrail and loop through the individual points yourself (See Smoke or Smoke Plume scripts for how to do this).", true)]
        protected virtual void UpdatePoint(PCTrailPoint pCTrailPoint, float deltaTime)
        {
        }

        private void UpdatePoints(PCTrail line, float deltaTime)
        {
            for (int i = 0; i < line.Points.Count; i++)
            {
                line.Points[i].Update(!this._noDecay ? deltaTime : 0f);
            }
        }

        protected virtual void UpdateTrail(PCTrail trail, float deltaTime)
        {
        }
    }
}

