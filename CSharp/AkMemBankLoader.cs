using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkMemBankLoader : MonoBehaviour
{
    private const long AK_BANK_PLATFORM_DATA_ALIGNMENT = 0x10L;
    private const long AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK = 15L;
    public string bankName = string.Empty;
    public bool isLocalizedBank;
    private string m_bankPath;
    [HideInInspector]
    public uint ms_bankID;
    private IntPtr ms_pInMemoryBankPtr = IntPtr.Zero;
    private GCHandle ms_pinnedArray;
    private WWW ms_www;
    private const int WaitMs = 50;

    private void DoLoadBank(string in_bankPath)
    {
        this.m_bankPath = in_bankPath;
        base.StartCoroutine(this.LoadFile());
    }

    [DebuggerHidden]
    private IEnumerator LoadFile()
    {
        return new <LoadFile>c__Iterator2B { <>f__this = this };
    }

    public void LoadLocalizedBank(string in_bankFilename)
    {
        string str = "file://" + Path.Combine(Path.Combine(AkBankPathUtil.GetPlatformBasePath(), AkInitializer.GetCurrentLanguage()), in_bankFilename);
        this.DoLoadBank(str);
    }

    public void LoadNonLocalizedBank(string in_bankFilename)
    {
        string str = "file://" + Path.Combine(AkBankPathUtil.GetPlatformBasePath(), in_bankFilename);
        this.DoLoadBank(str);
    }

    private void OnDestroy()
    {
        if ((this.ms_pInMemoryBankPtr != IntPtr.Zero) && (AkSoundEngine.UnloadBank(this.ms_bankID, this.ms_pInMemoryBankPtr) == AKRESULT.AK_Success))
        {
            this.ms_pinnedArray.Free();
        }
    }

    private void Start()
    {
        if (this.isLocalizedBank)
        {
            this.LoadLocalizedBank(this.bankName);
        }
        else
        {
            this.LoadNonLocalizedBank(this.bankName);
        }
    }

    [CompilerGenerated]
    private sealed class <LoadFile>c__Iterator2B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal AkMemBankLoader <>f__this;
        internal byte[] <alignedBytes>__1;
        internal int <alignedOffset>__4;
        internal long <alignedPtr>__5;
        internal uint <in_uInMemoryBankSize>__0;
        internal IntPtr <new_pInMemoryBankPtr>__3;
        internal GCHandle <new_pinnedArray>__2;
        internal AKRESULT <result>__6;

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
                    this.<>f__this.ms_www = new WWW(this.<>f__this.m_bankPath);
                    this.$current = this.<>f__this.ms_www;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<in_uInMemoryBankSize>__0 = 0;
                    try
                    {
                        this.<>f__this.ms_pinnedArray = GCHandle.Alloc(this.<>f__this.ms_www.bytes, GCHandleType.Pinned);
                        this.<>f__this.ms_pInMemoryBankPtr = this.<>f__this.ms_pinnedArray.AddrOfPinnedObject();
                        this.<in_uInMemoryBankSize>__0 = (uint) this.<>f__this.ms_www.bytes.Length;
                        if ((this.<>f__this.ms_pInMemoryBankPtr.ToInt64() & 15L) != 0)
                        {
                            this.<alignedBytes>__1 = new byte[this.<>f__this.ms_www.bytes.Length + 0x10L];
                            this.<new_pinnedArray>__2 = GCHandle.Alloc(this.<alignedBytes>__1, GCHandleType.Pinned);
                            this.<new_pInMemoryBankPtr>__3 = this.<new_pinnedArray>__2.AddrOfPinnedObject();
                            this.<alignedOffset>__4 = 0;
                            if ((this.<new_pInMemoryBankPtr>__3.ToInt64() & 15L) != 0)
                            {
                                this.<alignedPtr>__5 = (this.<new_pInMemoryBankPtr>__3.ToInt64() + 15L) & -16L;
                                this.<alignedOffset>__4 = (int) (this.<alignedPtr>__5 - this.<new_pInMemoryBankPtr>__3.ToInt64());
                                this.<new_pInMemoryBankPtr>__3 = new IntPtr(this.<alignedPtr>__5);
                            }
                            Array.Copy(this.<>f__this.ms_www.bytes, 0, this.<alignedBytes>__1, this.<alignedOffset>__4, this.<>f__this.ms_www.bytes.Length);
                            this.<>f__this.ms_pInMemoryBankPtr = this.<new_pInMemoryBankPtr>__3;
                            this.<>f__this.ms_pinnedArray.Free();
                            this.<>f__this.ms_pinnedArray = this.<new_pinnedArray>__2;
                        }
                    }
                    catch
                    {
                        break;
                    }
                    this.<result>__6 = AkSoundEngine.LoadBank(this.<>f__this.ms_pInMemoryBankPtr, this.<in_uInMemoryBankSize>__0, out this.<>f__this.ms_bankID);
                    if (this.<result>__6 != AKRESULT.AK_Success)
                    {
                        UnityEngine.Debug.LogError("AkMemBankLoader: bank loading failed with result " + this.<result>__6.ToString());
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

