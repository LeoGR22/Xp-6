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

        // Obt�m a Image do objeto alvo e a Textura
        Image targetImage = targetObject.GetComponent<Image>();
        PlayerManager playerManager = targetPlayer.GetComponent<PlayerManager>();

        if (targetImage == null)
        {
            Debug.LogWarning("O objeto com tag " + tagName + " n�o possui um componente Image.");
            return;
        }

        // Obt�m a Image do bot�o que chamou a fun��o
        Button button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning("O bot�o n�o foi encontrado.");
            return;
        }

        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage == null)
        {
            Debug.LogWarning("O bot�o n�o possui um componente Image.");
            return;
        }

        playerManager.VerifyItem(tagName, buttonImage.sprite);

        //playerManager.ChangeCurrentSprite(tagName, buttonImage.sprite);
        //playerManager.ApplyMonitorTextureToMaterial();

        // Copia a imagem do bot�o para o objeto alvo
        //targetImage.sprite = buttonImage.sprite;
        //Debug.Log("Imagem do bot�o copiada para: " + tagName);
    }
}
