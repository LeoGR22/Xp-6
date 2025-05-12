using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PriceManager : MonoBehaviour
{
    public ItensSO itensSO; // Refer�ncia ao ScriptableObject com os pre�os

    void Start()
    {
        // Procura o irm�o chamado "Icon"
        Transform iconTransform = transform.parent.Find("Icon");
        if (iconTransform == null)
        {
            Debug.LogWarning("Objeto 'Icon' n�o encontrado como irm�o.");
            return;
        }

        // Pega o sprite do Image no objeto "Icon"
        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogWarning("Componente Image n�o encontrado no 'Icon'.");
            return;
        }

        Sprite currentSprite = iconImage.sprite;
        int price = itensSO.GetPriceFromSprite(currentSprite);

        // Procura o TextMeshPro (TMP_Text) no filho deste bot�o
        TMP_Text priceTMP = GetComponentInChildren<TMP_Text>();
        if (priceTMP != null)
        {
            priceTMP.text = "R$" + price;
        }
        else
        {
            Debug.Log("Pre�o: R$" + price);
        }
    }
}
