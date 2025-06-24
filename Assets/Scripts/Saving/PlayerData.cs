[System.Serializable]
public class PlayerData
{
    public int money;
    public int level;
    public float tutorialPart;
    public bool isTutorial;
    public OwnedItemCategory[] ownedItems;
    public CurrentItem[] currentItems;

    [System.Serializable]
    public class OwnedItemCategory
    {
        public string category;
        public string[] sprites;
    }

    [System.Serializable]
    public class CurrentItem
    {
        public string category;
        public string sprite;
    }

    public PlayerData(PlayerManager manager)
    {
        money = manager.GetMoney();
        level = manager.GetLevel();
        tutorialPart = manager.GetTutorialPart();
        isTutorial = manager.GetTutorialState();

        var ownedItemsDict = manager.GetOwnedItems();
        ownedItems = new OwnedItemCategory[ownedItemsDict.Count];
        int i = 0;
        foreach (var pair in ownedItemsDict)
        {
            ownedItems[i] = new OwnedItemCategory
            {
                category = pair.Key,
                sprites = pair.Value
            };
            i++;
        }

        var currentItemsDict = manager.GetCurrentItems();
        currentItems = new CurrentItem[currentItemsDict.Count];
        i = 0;
        foreach (var pair in currentItemsDict)
        {
            currentItems[i] = new CurrentItem
            {
                category = pair.Key,
                sprite = pair.Value
            };
            i++;
        }
    }

    public PlayerData()
    {
        // Construtor padrão para novos saves
        money = 0;
        level = 0;
        isTutorial = true;
        tutorialPart = 1f;
        ownedItems = new OwnedItemCategory[]
        {
            new OwnedItemCategory { category = "Monitor", sprites = new string[0] },
            new OwnedItemCategory { category = "Keyboard", sprites = new string[0] },
            new OwnedItemCategory { category = "Mouse", sprites = new string[0] },
            new OwnedItemCategory { category = "Mousepad", sprites = new string[0] },
            new OwnedItemCategory { category = "Cup", sprites = new string[0] },
            new OwnedItemCategory { category = "Candle", sprites = new string[0] },
            new OwnedItemCategory { category = "WallDecor", sprites = new string[0] },
            new OwnedItemCategory { category = "Mic", sprites = new string[0] },
            new OwnedItemCategory { category = "Headset", sprites = new string[0] }
        };
        currentItems = new CurrentItem[]
        {
            new CurrentItem { category = "Monitor", sprite = null },
            new CurrentItem { category = "Keyboard", sprite = null },
            new CurrentItem { category = "Mouse", sprite = null },
            new CurrentItem { category = "Mousepad", sprite = null },
            new CurrentItem { category = "Cup", sprite = null },
            new CurrentItem { category = "Candle", sprite = null },
            new CurrentItem { category = "WallDecor", sprite = null },
            new CurrentItem { category = "Mic", sprite = null },
            new CurrentItem { category = "Headset", sprite = null }
        };
    }
}