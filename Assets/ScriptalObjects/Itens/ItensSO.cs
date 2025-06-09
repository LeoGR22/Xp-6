using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItensDataSO", menuName = "ItensDataSO")]
public class ItensSO : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public Sprite sprite; 
        public int price; 
    }

    [Header("Item Categories")]
    public List<ItemData> monitors;
    public List<ItemData> keyboards;
    public List<ItemData> mouses;
    public List<ItemData> mousepads;
    public List<ItemData> cups;
    public List<ItemData> candles;
    public List<ItemData> wallDecors;
    public List<ItemData> mics;
    public List<ItemData> headsets; 

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
        itemCategories.Add(mics);
        itemCategories.Add(headsets);

        foreach (var category in itemCategories)
        {
            if (category != null)
            {
                foreach (var item in category)
                {
                    if (item != null && item.sprite != null)
                    {
                        priceCache[item.sprite] = item.price;
                        itemDataCache[item.sprite] = item;
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

    public ItemData[] GetVariantsFromSprite(Sprite targetSprite)
    {
        ItemData itemData = GetItemDataFromSprite(targetSprite);
        return itemData != null ? new ItemData[] { itemData } : new ItemData[0];
    }

    public void ResetAllItems()
    {
        monitors.Clear();
        keyboards.Clear();
        mouses.Clear();
        mousepads.Clear();
        cups.Clear();
        candles.Clear();
        wallDecors.Clear();
        mics.Clear();
        headsets.Clear();

        priceCache.Clear();
        itemDataCache.Clear();

        itemCategories.Clear();
        itemCategories.Add(monitors);
        itemCategories.Add(keyboards);
        itemCategories.Add(mouses);
        itemCategories.Add(mousepads);
        itemCategories.Add(cups);
        itemCategories.Add(candles);
        itemCategories.Add(wallDecors);
        itemCategories.Add(mics);
        itemCategories.Add(headsets);

        Debug.Log("Todas as categorias de itens foram resetadas.");
    }
}