using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollShop : MonoBehaviour
{
    public RectTransform content;
    public List<GameObject> itemPrefabs;
    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        HorizontalLayoutGroup layoutGroup = content.GetComponent<HorizontalLayoutGroup>();

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            GameObject selectedPrefab = itemPrefabs[i];
            GameObject newItem = Instantiate(selectedPrefab, content);
            spawnedItems.Add(newItem);

            // For�a o material padr�o compat�vel com UI
            Image image = newItem.GetComponent<Image>();
            if (image != null)
            {
                image.material = new Material(Shader.Find("UI/Default"));
            }
        }
    }
}
