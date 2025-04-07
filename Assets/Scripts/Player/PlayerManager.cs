using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public PlayerItensSO playerItemSO;
    public PlayerMoneySO playerMoneySO;

    public ItensSO itensSO;

    public TMP_Text coinText;

    private void Start()
    {
        ApplyMonitorTextureToMaterial();
        coinText.text = playerMoneySO.GetMoney().ToString(); 
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
                playerMoneySO.ChangeMoney(-itensSO.GetPriceFromSprite(sprite));
                coinText.text = playerMoneySO.GetMoney().ToString();

                playerItemSO.AddPlayerItemTexture(name, sprite);
                playerItemSO.SetCurrentSprite(name, sprite);
                ApplyMonitorTextureToMaterial();
            }
        }
    }

    public void AddMoney(int num)
    {
        playerMoneySO.ChangeMoney(num);
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
