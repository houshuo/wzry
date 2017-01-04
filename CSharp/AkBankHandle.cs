using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkBankHandle
{
    public AkCallbackManager.BankCallback bankCallback;
    public string bankName;
    private uint m_BankID;
    private int m_RefCount;

    public AkBankHandle(string name)
    {
        this.bankName = name;
        this.bankCallback = null;
    }

    public void DecRef()
    {
        this.m_RefCount--;
        if ((this.m_RefCount == 0) && (this.m_BankID > 0))
        {
            AKRESULT akresult = AkSoundEngine.UnloadBank(this.m_BankID, IntPtr.Zero);
            if (akresult != AKRESULT.AK_Success)
            {
                Debug.LogWarning("Wwise: Bank " + this.bankName + " failed to unload (" + akresult.ToString() + ")");
            }
            this.m_BankID = 0;
        }
    }

    public void IncRef()
    {
        this.m_RefCount++;
    }

    public void LoadBank()
    {
        if (this.m_RefCount == 0)
        {
            AKRESULT akresult = AkSoundEngine.LoadBank(this.bankName, -1, out this.m_BankID);
            if (akresult != AKRESULT.AK_Success)
            {
                Debug.LogWarning("Wwise: Bank " + this.bankName + " failed to load (" + akresult.ToString() + ")");
            }
        }
        this.IncRef();
    }

    public void LoadBank(byte[] data)
    {
        if (this.m_RefCount == 0)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr zero = handle.AddrOfPinnedObject();
            if (zero != IntPtr.Zero)
            {
                AKRESULT akresult = AkSoundEngine.LoadBank(zero, (uint) data.Length, -1, out this.m_BankID);
                if (akresult != AKRESULT.AK_Success)
                {
                    this.m_BankID = 0;
                    Debug.LogWarning("Wwise: Bank " + this.bankName + " failed to load (" + akresult.ToString() + ")");
                }
                handle.Free();
                zero = IntPtr.Zero;
            }
            else
            {
                Debug.LogWarning("Wwise: Bank " + this.bankName + " failed to Alloc Memory");
            }
        }
        this.IncRef();
    }

    public int RefCount
    {
        get
        {
            return this.m_RefCount;
        }
    }
}

