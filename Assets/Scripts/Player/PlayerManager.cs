using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public PlayerItensSO playerItemSO;
    public PlayerMoneySO playerMoneySO;

    public ItensSO itensSO;

    private void Start()
    {
        ApplyMonitorTextureToMaterial();
    }

    public void VerifyItem(string name, Sprite sprite)
    {
        if (playerItemSO.IsSpriteInCategory(name, sprite))
        {
            playerItemSO.SetCurrentSprite(name, sprite);
            ApplyMonitorTextureToMaterial();
        }
        else
        {
            if(playerMoneySO.GetMoney() >= itensSO.GetPriceFromSprite(sprite))
            {
                playerItemSO.AddPlayerItemTexture(name, sprite);
                playerItemSO.SetCurrentSprite(name, sprite);
                ApplyMonitorTextureToMaterial();
            }
        }
    }

    public void ChangeCurrentSprite(string name, Sprite sprite)
    {
        playerItemSO.SetCurrentSprite(name, sprite);
    }

    public void ApplyMonitorTextureToMaterial()
    {
        GameObject monitorObj = GameObject.FindGameObjectWithTag("Monitor");

        Image monitorImage = monitorObj.GetComponent<Image>();

        Sprite monitorSprite = playerItemSO.ReturnMonitorTexture();

        Material instancedMaterial = new Material(monitorImage.material);

        instancedMaterial.SetTexture("_Texture2D", monitorSprite.texture);

        monitorImage.material = instancedMaterial;

        Debug.Log("Textura do monitor aplicada com sucesso ao material!");
    }
}
