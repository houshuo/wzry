namespace Assets.Scripts.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUIHttpTextScript : CUIComponent
    {
        private enHttpTextState m_httpTextState;
        public GameObject m_loadingCover;
        private ScrollRect m_scrollRectScript;
        private Text m_textScript;
        public string m_textUrl;
        private Text m_titleTextScript;

        [DebuggerHidden]
        private IEnumerator DownloadText(string url)
        {
            return new <DownloadText>c__Iterator29 { url = url, <$>url = url, <>f__this = this };
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_scrollRectScript = CUIUtility.GetComponentInChildren<ScrollRect>(base.gameObject);
                this.m_textScript = (this.m_scrollRectScript == null) ? null : CUIUtility.GetComponentInChildren<Text>(this.m_scrollRectScript.gameObject);
                Transform transform = base.gameObject.transform.FindChild("Title");
                this.m_titleTextScript = (transform == null) ? null : CUIUtility.GetComponentInChildren<Text>(transform.gameObject);
                this.m_httpTextState = enHttpTextState.Unload;
                if (this.m_loadingCover != null)
                {
                    this.m_loadingCover.CustomSetActive(true);
                }
                if (base.gameObject.activeInHierarchy && !string.IsNullOrEmpty(this.m_textUrl))
                {
                    this.LoadText(this.m_textUrl);
                }
            }
        }

        private void LoadText(string url)
        {
            if (this.m_httpTextState != enHttpTextState.Loaded)
            {
                base.StartCoroutine(this.DownloadText(url));
            }
        }

        private void OnDisable()
        {
            if (base.m_isInitialized && (this.m_httpTextState == enHttpTextState.Loading))
            {
                base.StopAllCoroutines();
                this.m_httpTextState = enHttpTextState.Unload;
                if (this.m_loadingCover != null)
                {
                    this.m_loadingCover.CustomSetActive(true);
                }
            }
        }

        private void OnEnable()
        {
            if ((base.m_isInitialized && (this.m_httpTextState == enHttpTextState.Unload)) && !string.IsNullOrEmpty(this.m_textUrl))
            {
                this.LoadText(this.m_textUrl);
            }
        }

        public void SetTextUrl(string url, bool forceReset = false)
        {
            if (!string.IsNullOrEmpty(url) && (!string.Equals(url, this.m_textUrl) || forceReset))
            {
                this.m_textUrl = url;
                if (this.m_titleTextScript != null)
                {
                    this.m_titleTextScript.text = string.Empty;
                }
                if (this.m_textScript != null)
                {
                    this.m_textScript.text = string.Empty;
                }
                if (base.gameObject.activeInHierarchy && (this.m_httpTextState == enHttpTextState.Loading))
                {
                    base.StopAllCoroutines();
                }
                this.m_httpTextState = enHttpTextState.Unload;
                if (this.m_loadingCover != null)
                {
                    this.m_loadingCover.CustomSetActive(true);
                }
                if (base.gameObject.activeInHierarchy)
                {
                    this.LoadText(this.m_textUrl);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadText>c__Iterator29 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>url;
            internal string <_title>__6;
            internal CUIHttpTextScript <>f__this;
            internal string <content>__3;
            internal bool <hasTitle>__4;
            internal int <lineBreakPosition>__5;
            internal string <text>__1;
            internal string <title>__2;
            internal WWW <www>__0;
            internal string url;

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
                        this.<>f__this.m_httpTextState = enHttpTextState.Loading;
                        this.<www>__0 = new WWW(this.url);
                        this.$current = this.<www>__0;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.m_httpTextState = enHttpTextState.Loaded;
                        if (string.IsNullOrEmpty(this.<www>__0.error))
                        {
                            if (this.<>f__this.m_loadingCover != null)
                            {
                                this.<>f__this.m_loadingCover.CustomSetActive(false);
                            }
                            this.<text>__1 = this.<www>__0.text;
                            this.<title>__2 = string.Empty;
                            this.<content>__3 = string.Empty;
                            this.<hasTitle>__4 = false;
                            this.<lineBreakPosition>__5 = this.<text>__1.IndexOf('\n');
                            if (this.<lineBreakPosition>__5 >= 0)
                            {
                                this.<_title>__6 = this.<text>__1.Substring(0, this.<lineBreakPosition>__5).Trim();
                                if (((this.<_title>__6 != null) && (this.<_title>__6.Length >= 2)) && ((this.<_title>__6[0] == '[') && (this.<_title>__6[this.<_title>__6.Length - 1] == ']')))
                                {
                                    this.<hasTitle>__4 = true;
                                    this.<title>__2 = this.<_title>__6.Substring(1, this.<_title>__6.Length - 2).Trim();
                                    this.<content>__3 = this.<text>__1.Substring(this.<lineBreakPosition>__5).Trim();
                                }
                            }
                            if (!this.<hasTitle>__4)
                            {
                                this.<title>__2 = string.Empty;
                                this.<content>__3 = this.<text>__1;
                            }
                            if (this.<>f__this.m_titleTextScript != null)
                            {
                                this.<>f__this.m_titleTextScript.text = this.<title>__2;
                            }
                            if (this.<>f__this.m_textScript != null)
                            {
                                this.<>f__this.m_textScript.text = this.<content>__3;
                                this.<>f__this.m_textScript.rectTransform.anchoredPosition = new Vector2(this.<>f__this.m_textScript.rectTransform.anchoredPosition.x, 0f);
                            }
                        }
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

