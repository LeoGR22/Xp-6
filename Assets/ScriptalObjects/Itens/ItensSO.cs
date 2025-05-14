using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItensDataSO", menuName = "ItensDataSO")]
public class ItensSO : ScriptableObject
{
    [System.Serializable]
    public class ItemVariant
    {
        public Sprite sprite; // Sprite da variação de cor
        public string colorName; // Nome da cor (ex.: "Red", "Blue")
    }

    [System.Serializable]
    public class ItemData
    {
        public ItemVariant[] variants; // Variações de cor do item
        public int price; // Preço para comprar o item (inclui todas as variações)
    }

    [Header("Item Categories")]
    public List<ItemData> monitors;
    public List<ItemData> keyboards;
    public List<ItemData> mouses;
    public List<ItemData> mousepads;
    public List<ItemData> cups;
    public List<ItemData> candles;
    public List<ItemData> wallDecors;

    private readonly List<List<ItemData>> itemCategories = new List<List<ItemData>>();
    private readonly Dictionary<Sprite, int> priceCache = new Dictionary<Sprite, int>();
    private readonly Dictionary<Sprite, ItemData> itemDataCache = new Dictionary<Sprite, ItemData>();

    private void OnEnable()
    {
        itemCategories.Clear();
        priceCache.Clear();
        itemDataCache.Clear();

        itemCategories.Add(monitors);
        itemCategories.Add(keyboards);
        itemCategories.Add(mouses);
        itemCategories.Add(mousepads);
        itemCategories.Add(cups);
        itemCategories.Add(candles);
        itemCategories.Add(wallDecors);

        foreach (var category in itemCategories)
        {
            if (category != null)
            {
                foreach (var item in category)
                {
                    if (item != null && item.variants != null)
                    {
                        foreach (var variant in item.variants)
                        {
                            if (variant.sprite != null)
                            {
                                priceCache[variant.sprite] = item.price;
                                itemDataCache[variant.sprite] = item;
                            }
                        }
                    }
                }
            }
        }
    }

    public int GetPriceFromSprite(Sprite targetSprite)
    {
        if (targetSprite == null) return 0;

        if (priceCache.TryGetValue(targetSprite, out int price))
        {
            return price;
        }

        Debug.LogWarning($"Sprite {targetSprite.name} não encontrado em nenhuma categoria.");
        return 0;
    }

    public ItemData GetItemDataFromSprite(Sprite targetSprite)
    {
        if (targetSprite == null) return null;

        if (itemDataCache.TryGetValue(targetSprite, out ItemData itemData))
        {
            return itemData;
        }

        Debug.LogWarning($"Sprite {targetSprite.name} não encontrado em nenhuma categoria.");
        return null;
    }

    public ItemVariant[] GetVariantsFromSprite(Sprite targetSprite)
    {
        ItemData itemData = GetItemDataFromSprite(targetSprite);
        return itemData?.variants ?? new ItemVariant[0];
    }
}