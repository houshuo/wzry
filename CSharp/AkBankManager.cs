using System;

public static class AkBankManager
{
    private static DictionaryView<string, AkBankHandle> m_BankHandles = new DictionaryView<string, AkBankHandle>();

    public static void LoadBank(string name)
    {
        AkBankHandle handle = null;
        if (!m_BankHandles.TryGetValue(name, out handle))
        {
            handle = new AkBankHandle(name);
            m_BankHandles.Add(name, handle);
            handle.LoadBank();
        }
    }

    public static void LoadBank(string name, byte[] data)
    {
        AkBankHandle handle = null;
        if (!m_BankHandles.TryGetValue(name, out handle))
        {
            handle = new AkBankHandle(name);
            m_BankHandles.Add(name, handle);
            handle.LoadBank(data);
        }
    }

    public static void UnloadBank(string name)
    {
        AkBankHandle handle = null;
        if (m_BankHandles.TryGetValue(name, out handle))
        {
            handle.DecRef();
            if (handle.RefCount == 0)
            {
                m_BankHandles.Remove(name);
            }
        }
    }
}

