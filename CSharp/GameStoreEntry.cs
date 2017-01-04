using System;

public class GameStoreEntry
{
    private static GameStoreEntry _instance;

    private GameStoreEntry(object notUse)
    {
    }

    public void Store(string savePath)
    {
        GameStore.Instance.StartStore(savePath);
    }

    public static GameStoreEntry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStoreEntry(GameStore.Instance);
            }
            return _instance;
        }
    }
}

