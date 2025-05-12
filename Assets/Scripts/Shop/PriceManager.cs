using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PriceManager : MonoBehaviour
{
    public ItensSO itensSO; // Referência ao ScriptableObject com os preços

    void Start()
    {
        // Procura o irmão chamado "Icon"
        Transform iconTransform = transform.parent.Find("Icon");
        if (iconTransform == null)
        {
            Debug.LogWarning("Objeto 'Icon' não encontrado como irmão.");
            return;
        }

        // Pega o sprite do Image no objeto "Icon"
        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogWarning("Componente Image não encontrado no 'Icon'.");
            return;
        }

        Sprite currentSprite = iconImage.sprite;
        int price = itensSO.GetPriceFromSprite(currentSprite);

        // Procura o TextMeshPro (TMP_Text) no filho deste botão
        TMP_Text priceTMP = GetComponentInChildren<TMP_Text>();
        if (priceTMP != null)
        {
            priceTMP.text = "R$" + price;
        }
        else
        {
            Debug.Log("Preço: R$" + price);
        }
    }
}
