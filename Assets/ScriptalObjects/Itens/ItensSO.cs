using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItensDataSO", menuName = "ItensDataSO")]
public class ItensSO : ScriptableObject
{
    [Header("Monitores")]
    public List<ItemData> monitors;

    [Header("Teclados")]
    public List<ItemData> keyboards;

    [Header("Mouses")]
    public List<ItemData> mouses;

    public int GetPriceFromSprite(Sprite targetSprite)
    {
        // Verifica nos monitores
        foreach (ItemData item in monitors)
        {
            if (item.itemSprite == targetSprite)
                return item.priceSprite;
        }

        // Verifica nos teclados
        foreach (ItemData item in keyboards)
        {
            if (item.itemSprite == targetSprite)
                return item.priceSprite;
        }

        // Verifica nos mouses
        foreach (ItemData item in mouses)
        {
            if (item.itemSprite == targetSprite)
                return item.priceSprite;
        }

        return 0;
    }
}

[System.Serializable]
public class ItemData
{
    public Sprite itemSprite;   
    public int priceSprite; 
}