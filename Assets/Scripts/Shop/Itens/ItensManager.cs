using UnityEngine;
using UnityEngine.UI;

public class ItensManager : MonoBehaviour
{
    public void SelectObject(string tagName)
    {
        GameObject targetObject = GameObject.FindWithTag(tagName);
        if (targetObject == null)
        {
            Debug.LogWarning($"Nenhum objeto encontrado com a tag: {tagName}");
            return;
        }

        Image targetImage = targetObject.GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogWarning($"O objeto com tag {tagName} não possui um componente Image.");
            return;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogWarning("Nenhum objeto com a tag 'Player' encontrado.");
            return;
        }

        PlayerManager playerManager = playerObject.GetComponent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogWarning("O objeto com tag 'Player' não possui um componente PlayerManager.");
            return;
        }

        Transform iconTransform = transform.Find("Icon");
        if (iconTransform == null)
        {
            Debug.LogWarning("Não foi encontrado um objeto 'Icon' como filho deste objeto.");
            return;
        }

        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogWarning("O objeto 'Icon' não possui um componente Image.");
            return;
        }

        playerManager.VerifyItem(tagName, iconImage.sprite);
    }
}