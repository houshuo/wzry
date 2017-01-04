using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Pathfinding/Seeker")]
public class Seeker : MonoBehaviour, IPooledMonoBehaviour
{
    public bool detailedGizmos;
    public bool drawGizmos = true;
    [NonSerialized]
    public List<GraphNode> lastCompletedNodePath;
    [NonSerialized]
    public List<VInt3> lastCompletedVectorPath;
    protected uint lastPathID;
    private ListView<IPathModifier> modifiers = new ListView<IPathModifier>();
    private OnPathDelegate onPathDelegate;
    [NonSerialized]
    protected Path path;
    public OnPathDelegate pathCallback;
    public OnPathDelegate postProcessOriginalPath;
    public OnPathDelegate postProcessPath;
    public OnPathDelegate preProcessPath;
    [HideInInspector]
    public bool saveGetNearestHints = true;
    public StartEndModifier startEndModifier = new StartEndModifier();
    [HideInInspector]
    public int[] tagPenalties = new int[0x20];
    private OnPathDelegate tmpPathCallback;
    [HideInInspector]
    public TagMask traversableTags = new TagMask(-1, -1);

    public void Awake()
    {
        this.onPathDelegate = new OnPathDelegate(this.OnPathComplete);
        this.startEndModifier.Awake(this);
    }

    [DebuggerHidden]
    public IEnumerator DelayPathStart(Path p)
    {
        return new <DelayPathStart>c__Iterator0 { p = p, <$>p = p, <>f__this = this };
    }

    public void DeregisterModifier(IPathModifier mod)
    {
        if (this.modifiers != null)
        {
            this.modifiers.Remove(mod);
        }
    }

    public ABPath GetNewPath(ref VInt3 start, ref VInt3 end)
    {
        return ABPath.Construct(ref start, ref end, null);
    }

    public void OnCreate()
    {
    }

    public void OnDestroy()
    {
        this.ReleaseClaimedPath();
        this.startEndModifier.OnDestroy(this);
    }

    public void OnDrawGizmos()
    {
        if ((this.lastCompletedNodePath != null) && this.drawGizmos)
        {
            if (this.detailedGizmos)
            {
                Gizmos.color = new Color(0.7f, 0.5f, 0.1f, 0.5f);
                if (this.lastCompletedNodePath != null)
                {
                    for (int i = 0; i < (this.lastCompletedNodePath.Count - 1); i++)
                    {
                        Gizmos.DrawLine((Vector3) this.lastCompletedNodePath[i].position, (Vector3) this.lastCompletedNodePath[i + 1].position);
                    }
                }
            }
            Gizmos.color = new Color(0f, 1f, 0f, 1f);
            if (this.lastCompletedVectorPath != null)
            {
                for (int j = 0; j < (this.lastCompletedVectorPath.Count - 1); j++)
                {
                    Gizmos.DrawLine((Vector3) this.lastCompletedVectorPath[j], (Vector3) this.lastCompletedVectorPath[j + 1]);
                }
            }
        }
    }

    public void OnGet()
    {
        this.drawGizmos = true;
        this.detailedGizmos = false;
        this.saveGetNearestHints = true;
        this.pathCallback = null;
        this.preProcessPath = null;
        this.postProcessOriginalPath = null;
        this.postProcessPath = null;
        this.lastCompletedVectorPath = null;
        this.lastCompletedNodePath = null;
        this.path = null;
        this.onPathDelegate = null;
        this.tmpPathCallback = null;
        this.lastPathID = 0;
        this.modifiers.Clear();
        this.Awake();
    }

    public void OnMultiPathComplete(Path p)
    {
        this.OnPathComplete(p, false, true);
    }

    public void OnPartialPathComplete(Path p)
    {
        this.OnPathComplete(p, true, false);
    }

    public void OnPathComplete(Path p)
    {
        this.OnPathComplete(p, true, true);
    }

    public void OnPathComplete(Path p, bool runModifiers, bool sendCallbacks)
    {
        if ((((p == null) || (p == this.path)) || !sendCallbacks) && (((this != null) && (p != null)) && (p == this.path)))
        {
            if (!this.path.error && runModifiers)
            {
                this.RunModifiers(ModifierPass.PostProcessOriginal, this.path);
                this.RunModifiers(ModifierPass.PostProcess, this.path);
            }
            if (sendCallbacks)
            {
                p.Claim(this);
                this.lastCompletedNodePath = p.path;
                this.lastCompletedVectorPath = p.vectorPath;
                if (this.tmpPathCallback != null)
                {
                    this.tmpPathCallback(p);
                }
                if (this.pathCallback != null)
                {
                    this.pathCallback(p);
                }
                if (!this.drawGizmos)
                {
                    this.ReleaseClaimedPath();
                }
            }
        }
    }

    public void OnRecycle()
    {
    }

    public void PostProcess(Path p)
    {
        this.RunModifiers(ModifierPass.PostProcess, p);
    }

    public void RecyclePath()
    {
        if (this.path != null)
        {
            this.path.Release(this);
        }
        this.path = null;
        this.lastCompletedNodePath = null;
        this.lastCompletedVectorPath = null;
    }

    public void RegisterModifier(IPathModifier mod)
    {
        if (this.modifiers == null)
        {
            this.modifiers = new ListView<IPathModifier>(1);
        }
        this.modifiers.Add(mod);
    }

    public void ReleaseClaimedPath()
    {
    }

    public void RunModifiers(ModifierPass pass, Path p)
    {
        bool flag = true;
        while (flag)
        {
            flag = false;
            for (int i = 0; i < (this.modifiers.Count - 1); i++)
            {
                if (this.modifiers[i].Priority < this.modifiers[i + 1].Priority)
                {
                    IPathModifier modifier = this.modifiers[i];
                    this.modifiers[i] = this.modifiers[i + 1];
                    this.modifiers[i + 1] = modifier;
                    flag = true;
                }
            }
        }
        switch (pass)
        {
            case ModifierPass.PreProcess:
                if (this.preProcessPath != null)
                {
                    this.preProcessPath(p);
                }
                break;

            case ModifierPass.PostProcessOriginal:
                if (this.postProcessOriginalPath != null)
                {
                    this.postProcessOriginalPath(p);
                }
                break;

            case ModifierPass.PostProcess:
                if (this.postProcessPath != null)
                {
                    this.postProcessPath(p);
                }
                break;
        }
        if (this.modifiers.Count != 0)
        {
            ModifierData input = ~ModifierData.None;
            IPathModifier modifier2 = this.modifiers[0];
            for (int j = 0; j < this.modifiers.Count; j++)
            {
                MonoModifier modifier3 = this.modifiers[j] as MonoModifier;
                if ((modifier3 != null) && !modifier3.enabled)
                {
                    continue;
                }
                switch (pass)
                {
                    case ModifierPass.PreProcess:
                        this.modifiers[j].PreProcess(p);
                        goto Label_026B;

                    case ModifierPass.PostProcessOriginal:
                        this.modifiers[j].ApplyOriginal(p);
                        goto Label_026B;

                    case ModifierPass.PostProcess:
                    {
                        ModifierData source = ModifierConverter.Convert(p, input, this.modifiers[j].input);
                        if (source == ModifierData.None)
                        {
                            break;
                        }
                        this.modifiers[j].Apply(p, source);
                        input = this.modifiers[j].output;
                        goto Label_0257;
                    }
                    default:
                        goto Label_026B;
                }
                Debug.Log("Error converting " + ((j <= 0) ? "original" : modifier2.GetType().Name) + "'s output to " + this.modifiers[j].GetType().Name + "'s input.\nTry rearranging the modifier priorities on the Seeker.");
                input = ModifierData.None;
            Label_0257:
                modifier2 = this.modifiers[j];
            Label_026B:
                if (input == ModifierData.None)
                {
                    break;
                }
            }
        }
    }

    public Path StartPath(Path p, OnPathDelegate callback = null, int graphMask = -1)
    {
        p.enabledTags = this.traversableTags.tagsChange;
        p.tagPenalties = this.tagPenalties;
        if (((this.path != null) && (this.path.GetState() <= PathState.Processing)) && (this.lastPathID == this.path.pathID))
        {
            this.path.Error();
            this.path.LogError("Canceled path because a new one was requested.\nThis happens when a new path is requested from the seeker when one was already being calculated.\nFor example if a unit got a new order, you might request a new path directly instead of waiting for the now invalid path to be calculated. Which is probably what you want.\nIf you are getting this a lot, you might want to consider how you are scheduling path requests.");
        }
        this.path = p;
        this.path.callback = (OnPathDelegate) Delegate.Combine(this.path.callback, this.onPathDelegate);
        this.path.nnConstraint.graphMask = graphMask;
        this.tmpPathCallback = callback;
        this.lastPathID = this.path.pathID;
        this.RunModifiers(ModifierPass.PreProcess, this.path);
        if (!AstarPath.StartPath(this.path, false))
        {
            this.path = null;
        }
        return this.path;
    }

    public Path StartPathEx(ref VInt3 start, ref VInt3 end, int campIndex, OnPathDelegate callback = null, int graphMask = -1)
    {
        Path newPath = this.GetNewPath(ref start, ref end);
        newPath.astarDataIndex = Mathf.Clamp(campIndex, 0, 2);
        return this.StartPath(newPath, callback, graphMask);
    }

    [CompilerGenerated]
    private sealed class <DelayPathStart>c__Iterator0 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Path <$>p;
        internal Seeker <>f__this;
        internal Path p;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.$current = null;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.RunModifiers(Seeker.ModifierPass.PreProcess, this.p);
                    AstarPath.StartPath(this.p, false);
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    public enum ModifierPass
    {
        PreProcess,
        PostProcessOriginal,
        PostProcess
    }
}

