using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItensDataSO", menuName = "ItensDataSO")]
public class ItensSO : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public Sprite itemSprite;
        public int priceSprite;
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

    private void OnEnable()
    {
        // Inicializa a lista de categorias e o cache de preços
        itemCategories.Clear();
        priceCache.Clear();

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
                    if (item != null && item.itemSprite != null && !priceCache.ContainsKey(item.itemSprite))
                    {
                        priceCache[item.itemSprite] = item.priceSprite;
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
}