namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("Movement")]
    public class ModifyTransform : TickEvent
    {
        public static Vector3 axisWeight = new Vector3(1f, 0f, 1f);
        public bool cubic;
        private bool currentInitialized;
        public bool currentRotation;
        public bool currentScaling;
        public bool currentTranslation;
        public bool enableRotation = true;
        public bool enableScaling;
        public bool enableTranslation = true;
        [ObjectTemplate(new System.Type[] {  })]
        public int fromId = -1;
        public bool normalizedRelative;
        [ObjectTemplate(new System.Type[] {  })]
        public int objectSpaceId = -1;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scaling = Vector3.one;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;
        [ObjectTemplate(new System.Type[] {  })]
        public int toId = -1;
        public Vector3 translation = Vector3.zero;

        public override BaseEvent Clone()
        {
            ModifyTransform transform = ClassObjPool<ModifyTransform>.Get();
            transform.CopyData(this);
            return transform;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ModifyTransform transform = src as ModifyTransform;
            this.enableTranslation = transform.enableTranslation;
            this.currentTranslation = transform.currentTranslation;
            this.translation = transform.translation;
            this.enableRotation = transform.enableRotation;
            this.currentRotation = transform.currentRotation;
            this.rotation = transform.rotation;
            this.enableScaling = transform.enableScaling;
            this.currentScaling = transform.currentScaling;
            this.scaling = transform.scaling;
            this.targetId = transform.targetId;
            this.objectSpaceId = transform.objectSpaceId;
            this.fromId = transform.fromId;
            this.toId = transform.toId;
            this.normalizedRelative = transform.normalizedRelative;
            this.cubic = transform.cubic;
            this.currentInitialized = transform.currentInitialized;
        }

        public void CubicVectorBlend(AGE.Action _action, Track _track, TickEvent _prevEvent, float _blendWeight, bool isPos)
        {
            Vector3 vector5;
            Vector3 vector6;
            int indexOfEvent = _track.GetIndexOfEvent(_prevEvent);
            int num2 = _track.GetIndexOfEvent(this);
            int eventsCount = _track.GetEventsCount();
            int index = indexOfEvent - 1;
            if (index < 0)
            {
                if (_action.loop)
                {
                    index = eventsCount - 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                }
                else
                {
                    index = 0;
                }
            }
            int num5 = num2 + 1;
            if (num5 >= eventsCount)
            {
                if (_action.loop)
                {
                    num5 = 0;
                }
                else
                {
                    num5 = num2;
                }
            }
            ModifyTransform transform = _prevEvent as ModifyTransform;
            ModifyTransform transform2 = _track.GetEvent(index) as ModifyTransform;
            ModifyTransform transform3 = _track.GetEvent(num5) as ModifyTransform;
            DebugHelper.Assert(((transform != null) && (transform2 != null)) && (transform3 != null), "��һ�Ѷ���Ӧ��Ϊ��");
            Vector3 prevPoint = !isPos ? transform.scaling : transform.GetTranslation(_action);
            Vector3 curnPoint = !isPos ? this.scaling : this.GetTranslation(_action);
            Vector3 formPoint = !isPos ? transform2.scaling : transform2.GetTranslation(_action);
            Vector3 lattPoint = !isPos ? transform3.scaling : transform3.GetTranslation(_action);
            CurvlData.CalculateCtrlPoint(formPoint, prevPoint, curnPoint, lattPoint, out vector5, out vector6);
            float num6 = 1f - _blendWeight;
            float num7 = _blendWeight;
            Vector3 vector7 = (Vector3) ((((((prevPoint * num6) * num6) * num6) + ((((vector5 * 3f) * num6) * num6) * num7)) + ((((vector6 * 3f) * num6) * num7) * num7)) + (((curnPoint * num7) * num7) * num7));
            if (isPos)
            {
                _action.GetGameObject(this.targetId).transform.position = vector7;
            }
            else
            {
                _action.GetGameObject(this.targetId).transform.localScale = vector7;
            }
        }

        public Quaternion GetRotation(AGE.Action _action)
        {
            if (_action.GetGameObject(this.targetId) != null)
            {
                this.SetCurrentTransform(_action.GetGameObject(this.targetId).transform);
            }
            GameObject gameObject = _action.GetGameObject(this.fromId);
            GameObject obj3 = _action.GetGameObject(this.toId);
            if ((gameObject != null) && (obj3 != null))
            {
                Vector3 vector = obj3.transform.position - gameObject.transform.position;
                return (Quaternion.LookRotation(Vector3.Normalize(new Vector3(vector.x * axisWeight.x, vector.y * axisWeight.y, vector.z * axisWeight.z)), Vector3.up) * this.rotation);
            }
            GameObject obj4 = _action.GetGameObject(this.objectSpaceId);
            if (obj4 != null)
            {
                return (obj4.transform.rotation * this.rotation);
            }
            GameObject obj5 = _action.GetGameObject(this.targetId);
            if ((obj5 != null) && (obj5.transform.parent != null))
            {
                return (obj5.transform.parent.rotation * this.rotation);
            }
            return this.rotation;
        }

        public Vector3 GetTranslation(AGE.Action _action)
        {
            if (_action.GetGameObject(this.targetId) != null)
            {
                this.SetCurrentTransform(_action.GetGameObject(this.targetId).transform);
            }
            GameObject gameObject = _action.GetGameObject(this.fromId);
            GameObject obj3 = _action.GetGameObject(this.toId);
            if ((gameObject != null) && (obj3 != null))
            {
                Vector3 vector = new Vector3();
                Vector3 vector2 = obj3.transform.position - gameObject.transform.position;
                Vector2 vector3 = new Vector2(vector2.x, vector2.z);
                float magnitude = vector3.magnitude;
                Quaternion quaternion = Quaternion.LookRotation(Vector3.Normalize(new Vector3(vector2.x * axisWeight.x, vector2.y * axisWeight.y, vector2.z * axisWeight.z)), Vector3.up);
                if (this.normalizedRelative)
                {
                    vector = (Vector3) (quaternion * this.translation);
                    vector = gameObject.transform.position + new Vector3(vector.x * magnitude, vector.y, vector.z * magnitude);
                    return (vector + new Vector3(0f, this.translation.z * (obj3.transform.position.y - gameObject.transform.position.y), 0f));
                }
                vector = gameObject.transform.position + (quaternion * this.translation);
                return (vector + new Vector3(0f, (this.translation.z / magnitude) * (obj3.transform.position.y - gameObject.transform.position.y), 0f));
            }
            GameObject obj4 = _action.GetGameObject(this.objectSpaceId);
            if (obj4 != null)
            {
                return obj4.transform.localToWorldMatrix.MultiplyPoint(this.translation);
            }
            GameObject obj5 = _action.GetGameObject(this.targetId);
            if ((obj5 != null) && (obj5.transform.parent != null))
            {
                return obj5.transform.parent.localToWorldMatrix.MultiplyPoint(this.translation);
            }
            return this.translation;
        }

        public int HasDependObject(AGE.Action _action)
        {
            if ((this.currentTranslation || this.currentRotation) || this.currentScaling)
            {
                return 1;
            }
            if (this.fromId >= 0)
            {
                if (_action.GetGameObject(this.fromId) != null)
                {
                    return 1;
                }
                return -1;
            }
            if (this.toId >= 0)
            {
                if (_action.GetGameObject(this.toId) != null)
                {
                    return 1;
                }
                return -1;
            }
            if (this.objectSpaceId < 0)
            {
                return 0;
            }
            if (_action.GetGameObject(this.objectSpaceId) != null)
            {
                return 1;
            }
            return -1;
        }

        public bool HasTempObj(AGE.Action _action)
        {
            return (((this.fromId >= 0) && (_action.GetGameObject(this.fromId) == null)) || (((this.toId >= 0) && (_action.GetGameObject(this.toId) == null)) || ((this.objectSpaceId >= 0) && (_action.GetGameObject(this.objectSpaceId) == null))));
        }

        public override void OnUse()
        {
            base.OnUse();
            this.enableTranslation = true;
            this.currentTranslation = false;
            this.translation = Vector3.zero;
            this.enableRotation = true;
            this.currentRotation = false;
            this.rotation = Quaternion.identity;
            this.enableScaling = false;
            this.currentScaling = false;
            this.scaling = Vector3.one;
            this.targetId = 0;
            this.objectSpaceId = -1;
            this.fromId = -1;
            this.toId = -1;
            this.normalizedRelative = false;
            this.cubic = false;
            this.currentInitialized = false;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (_action.GetGameObject(this.targetId) != null)
            {
                this.currentInitialized = false;
                this.SetCurrentTransform(_action.GetGameObject(this.targetId).transform);
                if (this.enableTranslation)
                {
                    _action.GetGameObject(this.targetId).transform.position = this.GetTranslation(_action);
                }
                if (this.enableRotation)
                {
                    _action.GetGameObject(this.targetId).transform.rotation = this.GetRotation(_action);
                }
                if (this.enableScaling)
                {
                    _action.GetGameObject(this.targetId).transform.localScale = this.scaling;
                }
            }
        }

        public override void ProcessBlend(AGE.Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
        {
            if ((_action.GetGameObject(this.targetId) != null) && (_prevEvent != null))
            {
                if (this.enableTranslation)
                {
                    if (this.cubic)
                    {
                        this.CubicVectorBlend(_action, _track, _prevEvent, _blendWeight, true);
                    }
                    else
                    {
                        _action.GetGameObject(this.targetId).transform.position = (Vector3) ((this.GetTranslation(_action) * _blendWeight) + ((_prevEvent as ModifyTransform).GetTranslation(_action) * (1f - _blendWeight)));
                    }
                }
                if (this.enableRotation)
                {
                    _action.GetGameObject(this.targetId).transform.rotation = Quaternion.Slerp((_prevEvent as ModifyTransform).GetRotation(_action), this.GetRotation(_action), _blendWeight);
                }
                if (this.enableScaling)
                {
                    if (this.cubic)
                    {
                        this.CubicVectorBlend(_action, _track, _prevEvent, _blendWeight, false);
                    }
                    else
                    {
                        _action.GetGameObject(this.targetId).transform.localScale = (Vector3) ((this.scaling * _blendWeight) + ((_prevEvent as ModifyTransform).scaling * (1f - _blendWeight)));
                    }
                }
            }
        }

        private void SetCurrentTransform(Transform _transform)
        {
            if (!this.currentInitialized)
            {
                if (this.currentTranslation)
                {
                    this.objectSpaceId = this.fromId = this.toId = -1;
                    this.translation = _transform.localPosition;
                }
                if (this.currentRotation)
                {
                    this.objectSpaceId = this.fromId = this.toId = -1;
                    this.rotation = _transform.localRotation;
                }
                if (this.currentScaling)
                {
                    this.scaling = _transform.localScale;
                }
                this.currentInitialized = true;
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

