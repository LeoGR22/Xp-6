using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItensManager : MonoBehaviour
{
    public void SelectObject(string tagName)
    {
        // Encontra o objeto alvo pela tag
        GameObject targetObject = GameObject.FindWithTag(tagName);
        GameObject targetPlayer = GameObject.FindWithTag("Player");

        if (targetObject == null)
        {
            Debug.LogWarning("Nenhum objeto encontrado com a tag: " + tagName);
            return;
        }

        Image targetImage = targetObject.GetComponent<Image>();
        PlayerManager playerManager = targetPlayer.GetComponent<PlayerManager>();

        if (targetImage == null)
        {
            Debug.LogWarning("O objeto com tag " + tagName + " não possui um componente Image.");
            return;
        }

        // Busca o objeto "icon" que é filho deste objeto (pai do botão e do icon)
        Transform iconTransform = transform.Find("Icon");

        if (iconTransform == null)
        {
            Debug.LogWarning("Não foi encontrado um objeto 'icon' como filho deste objeto.");
            return;
        }

        Image iconImage = iconTransform.GetComponent<Image>();

        if (iconImage == null)
        {
            Debug.LogWarning("O objeto 'icon' não possui um componente Image.");
            return;
        }

        // Usa a imagem do ícone para chamar o método
        playerManager.VerifyItem(tagName, iconImage.sprite);
    }
}
