using System.Collections;
using TMPro;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System.Diagnostics;

public class PlayerManager : MonoBehaviour
{
    public PlayerItensSO playerItemSO;
    public PlayerMoneySO playerMoneySO;
    public ItensSO itensSO;

    public GameObject buyConfirmationUI;

    public TMP_Text coinText;

    int MaxRate = 9999;
    public float TargetFrameRate = 60.0f;
    float currentFrameTime;

    private void Start()
    {
        ApplyAllTextures();

        if (coinText != null)
            coinText.text = playerMoneySO.GetMoney().ToString();

        if (buyConfirmationUI != null)
            buyConfirmationUI.SetActive(false);
    }

    private string lastName;
    private Sprite lastSprite;

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
                lastName = name;
                lastSprite = sprite;

                buyConfirmationUI.GetComponent<BuyConfirmation>().SetSprite(sprite);
                buyConfirmationUI.SetActive(true);
                AudioManager.Instance.PlaySFX("Click2");
            }
        }
    }

    public void ChooseOption(string option)
    {
        if (option == "Yes")
        {
            BuyItem(lastName, lastSprite);
            buyConfirmationUI.SetActive(false);
        }
        else
        {
            buyConfirmationUI.SetActive(false);
        }
    }

    public void BuyItem(string name, Sprite sprite)
    {
        int price = itensSO.GetPriceFromSprite(sprite);

        playerMoneySO.ChangeMoney(-price);
        if (coinText != null)
            coinText.text = playerMoneySO.GetMoney().ToString();

        playerItemSO.AddPlayerItemTexture(name, sprite);
        playerItemSO.SetCurrentSprite(name, sprite);
        ApplyAllTextures();
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
        ApplyTextureToTaggedObject("Mousepad", playerItemSO.ReturnMousepadTexture());
        ApplyTextureToTaggedObject("Cup", playerItemSO.ReturnCupTexture());
        ApplyTextureToTaggedObject("Candle", playerItemSO.ReturnCandleTexture());
        ApplyTextureToTaggedObject("WallDecor", playerItemSO.ReturnWallDecorTexture());
    }

    // Função reutilizável para aplicar sprite diretamente no componente Image
    private void ApplyTextureToTaggedObject(string tag, Sprite sprite)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);

        if (obj != null)
        {
            Image img = obj.GetComponent<Image>();
            if (img != null && sprite != null)
            {
                img.sprite = sprite; // Atualiza apenas o sprite
                UnityEngine.Debug.Log($"Sprite de {tag} aplicado com sucesso!");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Image ou sprite não encontrado para o objeto com tag {tag}.");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Objeto com tag {tag} não encontrado.");
        }
    }

    // FrameRate 
    /*
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MaxRate;
        currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine(WaitForNextFrame());
    }
    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / TargetFrameRate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));
            while (t < currentFrameTime)
                t = Time.realtimeSinceStartup;
        }
    }
    */
}