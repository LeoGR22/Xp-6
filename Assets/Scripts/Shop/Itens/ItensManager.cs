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

        if (targetObject == null)
        {
            Debug.LogWarning("Nenhum objeto encontrado com a tag: " + tagName);
            return;
        }

        // Obtém a Image do objeto alvo
        Image targetImage = targetObject.GetComponent<Image>();

        if (targetImage == null)
        {
            Debug.LogWarning("O objeto com tag " + tagName + " não possui um componente Image.");
            return;
        }

        // Obtém a Image do botão que chamou a função
        Button button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning("O botão não foi encontrado.");
            return;
        }

        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage == null)
        {
            Debug.LogWarning("O botão não possui um componente Image.");
            return;
        }

        // Copia a imagem do botão para o objeto alvo
        targetImage.sprite = buttonImage.sprite;
        Debug.Log("Imagem do botão copiada para: " + tagName);
    }
}
