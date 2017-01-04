namespace Assets.Scripts.Framework
{
    using AGE;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class EditorFramework : GameFramework
    {
        protected override void Init()
        {
            base.Init();
            base.StartPrepareBaseSystem(new GameFramework.DelegateOnBaseSystemPrepareComplete(this.OnPrepareBaseSystemComplete));
        }

        private void OnPrepareBaseSystemComplete()
        {
            base.StartCoroutine(this.StartPrepareGameSystem());
        }

        public override void Start()
        {
        }

        [DebuggerHidden]
        private IEnumerator StartPrepareGameSystem()
        {
            return new <StartPrepareGameSystem>c__Iterator7 { <>f__this = this };
        }

        [CompilerGenerated]
        private sealed class <StartPrepareGameSystem>c__Iterator7 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal EditorFramework <>f__this;
            internal SCPKG_STARTSINGLEGAMERSP <simuData>__0;

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
                        this.$current = this.<>f__this.StartCoroutine(this.<>f__this.PrepareGameSystem());
                        this.$PC = 1;
                        return true;

                    case 1:
                        ActionManager.Instance.frameMode = true;
                        this.<simuData>__0 = new SCPKG_STARTSINGLEGAMERSP();
                        Singleton<GameBuilder>.instance.StartGame(new TestGameContext(ref this.<simuData>__0));
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
    }
}

