namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AnimPlayComponent : LogicComponent
    {
        [CompilerGenerated]
        private static Comparison<PlayAnimParam> <>f__am$cache5;
        private List<PlayAnimParam> anims = new List<PlayAnimParam>(3);
        public bool bPausePlay;
        private List<Assets.Scripts.GameLogic.ChangeAnimParam> changeList = new List<Assets.Scripts.GameLogic.ChangeAnimParam>(2);
        private string curAnimName;
        private ulong curAnimPlayFrameTick;

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            int childCount = base.gameObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject gameObject = base.gameObject.transform.GetChild(i).gameObject;
                if (gameObject.GetComponent<Animation>() != null)
                {
                    base.actor.SetActorMesh(gameObject);
                    base.actor.RecordOriginalActorMesh();
                    break;
                }
            }
        }

        private void ChangeAnimName(ref PlayAnimParam param)
        {
            for (int i = 0; i < this.changeList.Count; i++)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param2 = this.changeList[i];
                if ((param2.originalAnimName == param.animName) && (base.actor.ActorMeshAnimation.GetClip(param2.changedAnimName) != null))
                {
                    param.animName = param2.changedAnimName;
                    return;
                }
            }
        }

        public void ChangeAnimParam(string _oldAnimName, string _newAnimName)
        {
            Assets.Scripts.GameLogic.ChangeAnimParam item = new Assets.Scripts.GameLogic.ChangeAnimParam {
                originalAnimName = _oldAnimName,
                changedAnimName = _newAnimName
            };
            this.changeList.Add(item);
            this.ChangeCurAnimParam(item, false);
        }

        private void ChangeCurAnimParam(Assets.Scripts.GameLogic.ChangeAnimParam _param, bool bRecover)
        {
            string str2 = !bRecover ? _param.originalAnimName : _param.changedAnimName;
            string str3 = !bRecover ? _param.changedAnimName : _param.originalAnimName;
            for (int i = 0; i < this.anims.Count; i++)
            {
                PlayAnimParam param = this.anims[i];
                if (param.animName == str2)
                {
                    string animName = param.animName;
                    param.animName = str3;
                    if (animName == this.curAnimName)
                    {
                        this.Play(param);
                    }
                }
            }
        }

        private void ClearVariables()
        {
            this.anims.Clear();
            this.changeList.Clear();
            this.curAnimName = null;
            this.curAnimPlayFrameTick = 0L;
            this.bPausePlay = false;
        }

        public override void Deactive()
        {
            this.ClearVariables();
            base.Deactive();
        }

        private void DoPlay(PlayAnimParam param)
        {
            if (!this.bPausePlay)
            {
                if (param.blendTime > 0f)
                {
                    if (!param.loop)
                    {
                        AnimationState state = base.actor.ActorMeshAnimation.CrossFadeQueued(param.animName, param.blendTime, QueueMode.PlayNow);
                        if (state != null)
                        {
                            state.speed = param.speed;
                            state.wrapMode = !param.loop ? WrapMode.ClampForever : WrapMode.Loop;
                        }
                    }
                    else
                    {
                        base.actor.ActorMeshAnimation.CrossFade(param.animName, param.blendTime);
                    }
                }
                else
                {
                    base.actor.ActorMeshAnimation.Stop();
                    base.actor.ActorMeshAnimation.Play(param.animName);
                }
                this.curAnimName = param.animName;
                this.curAnimPlayFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                AnimationState state2 = base.actor.ActorMeshAnimation[param.animName];
                if (state2 != null)
                {
                    state2.wrapMode = !param.loop ? WrapMode.ClampForever : WrapMode.Loop;
                }
            }
        }

        private string GetChangeAnimName(string changeName)
        {
            for (int i = 0; i < this.changeList.Count; i++)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param = this.changeList[i];
                if ((param.originalAnimName == changeName) && (base.actor.ActorMeshAnimation.GetClip(param.changedAnimName) != null))
                {
                    return param.changedAnimName;
                }
            }
            return changeName;
        }

        public string GetCurAnimName()
        {
            return this.curAnimName;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.ClearVariables();
        }

        public void Play(PlayAnimParam param)
        {
            if (((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null)) && (base.actor.ActorMeshAnimation.GetClip(param.animName) != null))
            {
                if (this.changeList.Count > 0)
                {
                    this.ChangeAnimName(ref param);
                }
                if (param.cancelAll)
                {
                    this.anims.Clear();
                }
                if (param.cancelCurrent && (this.anims.Count > 0))
                {
                    this.anims.RemoveAt(this.anims.Count - 1);
                }
                for (int i = 0; i < this.anims.Count; i++)
                {
                    PlayAnimParam param2 = this.anims[i];
                    if (param2.layer == param.layer)
                    {
                        this.anims.RemoveAt(i);
                        i--;
                    }
                }
                this.anims.Add(param);
                bool flag = true;
                if (this.anims.Count > 1)
                {
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = delegate (PlayAnimParam a, PlayAnimParam b) {
                            if (a.layer == b.layer)
                            {
                                return 0;
                            }
                            if (a.layer < b.layer)
                            {
                                return -1;
                            }
                            return 1;
                        };
                    }
                    this.anims.Sort(<>f__am$cache5);
                    PlayAnimParam param3 = this.anims[this.anims.Count - 1];
                    flag = param3.animName == param.animName;
                }
                if (flag)
                {
                    this.DoPlay(param);
                }
            }
        }

        public void RecoverAnimParam(string _changeAnimName)
        {
            int index = -1;
            for (int i = 0; i < this.changeList.Count; i++)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param2 = this.changeList[i];
                if (param2.originalAnimName == _changeAnimName)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param = this.changeList[index];
                this.changeList.RemoveAt(index);
                this.ChangeCurAnimParam(param, true);
            }
        }

        public void SetAnimPlaySpeed(string clipName, float speed)
        {
            if ((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null))
            {
                AnimationState state = base.actor.ActorMeshAnimation[clipName];
                if (state != null)
                {
                    state.speed = speed;
                }
            }
        }

        public void Stop(string origAnimName, bool bFlag = false)
        {
            if ((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null))
            {
                string changeAnimName = this.GetChangeAnimName(origAnimName);
                if (base.actor.ActorMeshAnimation.GetClip(changeAnimName) != null)
                {
                    bool flag = false;
                    int count = this.anims.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        PlayAnimParam param = this.anims[i];
                        if (param.animName == changeAnimName)
                        {
                            flag = i == (count - 1);
                            this.anims.RemoveAt(i);
                        }
                    }
                    if (flag && bFlag)
                    {
                        if (this.anims.Count > 0)
                        {
                            this.DoPlay(this.anims[this.anims.Count - 1]);
                        }
                        else
                        {
                            base.actor.ActorMeshAnimation[changeAnimName].enabled = false;
                        }
                    }
                }
            }
        }

        public void UpdateCurAnimState()
        {
            if (((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null)) && ((this.curAnimName != null) && (base.actor.ActorMeshAnimation.GetClip(this.curAnimName) != null)))
            {
                Animation actorMeshAnimation = base.actor.ActorMeshAnimation;
                FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
                AnimationState state = actorMeshAnimation[this.curAnimName];
                float length = (float) (((double) (instance.LogicFrameTick - this.curAnimPlayFrameTick)) / 1000.0);
                if (state.wrapMode == WrapMode.Loop)
                {
                    if (state.length == 0f)
                    {
                        length = 0f;
                    }
                    else
                    {
                        int num2 = (int) (length / state.length);
                        length -= num2 * state.length;
                    }
                    actorMeshAnimation.Play(this.curAnimName);
                    state.time = length;
                }
                else
                {
                    if (length >= state.length)
                    {
                        length = state.length;
                    }
                    state.time = length;
                }
            }
        }

        public override void UpdateLogic(int delta)
        {
            if (((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null)) && !base.actor.ActorMeshAnimation.isPlaying)
            {
                if (this.anims.Count > 0)
                {
                    this.anims.RemoveAt(this.anims.Count - 1);
                }
                if (this.anims.Count > 0)
                {
                    this.DoPlay(this.anims[this.anims.Count - 1]);
                }
            }
        }

        public void UpdatePlay()
        {
            if (this.anims.Count > 0)
            {
                this.DoPlay(this.anims[this.anims.Count - 1]);
            }
        }
    }
}

