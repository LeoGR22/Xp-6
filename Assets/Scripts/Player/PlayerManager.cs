using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public PlayerItensSO playerItemSO;
    public PlayerMoneySO playerMoneySO;
    public ItensSO itensSO;

    public TMP_Text coinText;

    private void Start()
    {
        ApplyAllTextures();

        if (coinText != null)
            coinText.text = playerMoneySO.GetMoney().ToString();
    }

    public void VerifyItem(string name, Sprite sprite)
    {
        if (playerItemSO.IsSpriteInCategory(name, sprite))
        {
            playerItemSO.SetCurrentSprite(name, sprite);
            ApplyAllTextures();
        }
        else
        {
            int price = itensSO.GetPriceFromSprite(sprite);
            if (playerMoneySO.GetMoney() >= price)
            {
                playerMoneySO.ChangeMoney(-price);
                if (coinText != null)
                    coinText.text = playerMoneySO.GetMoney().ToString();

                playerItemSO.AddPlayerItemTexture(name, sprite);
                playerItemSO.SetCurrentSprite(name, sprite);
                ApplyAllTextures();
            }
        }
    }

    public void AddMoney(int num)
    {
        playerMoneySO.ChangeMoney(num);
        if (coinText != null)
            coinText.text = playerMoneySO.GetMoney().ToString();
    }

    public void ChangeCurrentSprite(string name, Sprite sprite)
    {
        playerItemSO.SetCurrentSprite(name, sprite);
    }

    // Aplica textura ao monitor, teclado e mouse
    public void ApplyAllTextures()
    {
        ApplyTextureToTaggedObject("Monitor", playerItemSO.ReturnMonitorTexture());
        ApplyTextureToTaggedObject("Keyboard", playerItemSO.ReturnKeyboardTexture());
        ApplyTextureToTaggedObject("Mouse", playerItemSO.ReturnMouseTexture());
    }

    // Função reutilizável para aplicar textura via Sprite
    private void ApplyTextureToTaggedObject(string tag, Sprite sprite)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);

        if (obj != null)
        {
            Image img = obj.GetComponent<Image>();
            if (img != null && sprite != null)
            {
                Material instancedMaterial = new Material(img.material);
                instancedMaterial.SetTexture("_Texture2D", sprite.texture);
                img.material = instancedMaterial;

                Debug.Log($"Textura de {tag} aplicada com sucesso ao material!");
            }
        }
    }
}
