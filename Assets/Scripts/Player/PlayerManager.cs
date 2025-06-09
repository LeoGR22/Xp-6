using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private PlayerItensSO playerItemSO;
    [SerializeField] private PlayerMoneySO playerMoneySO;
    [SerializeField] private ItensSO itensSO;

    [Header("UI Elements")]
    [SerializeField] private GameObject buyConfirmationUI;
    [SerializeField] private TMP_Text coinText;
    //[SerializeField] private ColorPickerUI colorPickerUI;

    [Header("Animation Settings")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Animator gameAnimator;

    [Header("Frame Rate Settings")]
    [SerializeField] private float targetFrameRate = 60f;
    private const int MaxFrameRate = 9999;

    private string lastSelectedItemName;
    private Sprite lastSelectedSprite;

    private readonly Dictionary<string, System.Action<Sprite>> textureApplicators = new Dictionary<string, System.Action<Sprite>>();

    private void Awake()
    {
        // Add applicators for existing and new categories
        textureApplicators.Add("Monitor", sprite => ApplyTextureToTaggedObject("Monitor", sprite));
        textureApplicators.Add("Keyboard", sprite => ApplyTextureToTaggedObject("Keyboard", sprite));
        textureApplicators.Add("Mouse", sprite => ApplyTextureToTaggedObject("Mouse", sprite));
        textureApplicators.Add("Mousepad", sprite => ApplyTextureToTaggedObject("Mousepad", sprite));
        textureApplicators.Add("Cup", sprite => ApplyTextureToTaggedObject("Cup", sprite));
        textureApplicators.Add("Candle", sprite => ApplyTextureToTaggedObject("Candle", sprite));
        textureApplicators.Add("WallDecor", sprite => ApplyTextureToTaggedObject("WallDecor", sprite));
        textureApplicators.Add("Mic", sprite => ApplyTextureToTaggedObject("Mic", sprite)); // New Mic category
        textureApplicators.Add("Headset", sprite => ApplyTextureToTaggedObject("Headset", sprite)); // New Headset category
    }

    private void Start()
    {
        ApplyAllTextures();
        UpdateCoinText();
        SetBuyConfirmationActive(false);
    }

    public void VerifyItem(string itemName, Sprite sprite)
    {
        if (playerItemSO.IsSpriteInCategory(itemName, sprite))
        {
            playerItemSO.SetCurrentSprite(itemName, sprite);
            ApplyAllTextures();
        }
        else
        {
            int price = itensSO.GetPriceFromSprite(sprite);
            if (playerMoneySO.GetMoney() >= price)
            {
                lastSelectedItemName = itemName;
                lastSelectedSprite = sprite;

                if (buyConfirmationUI != null)
                {
                    buyConfirmationUI.GetComponent<BuyConfirmation>().SetSprite(sprite);
                    SetBuyConfirmationActive(true);
                    AudioManager.Instance.PlaySFX("Click2");
                }
            }
        }
    }

    public void ChooseOption(string option)
    {
        if (option == "Yes")
        {
            StartCoroutine(OpenBoxCoroutine());
        }
        SetBuyConfirmationActive(false);
    }

    private IEnumerator OpenBoxCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AnimateBoxImage(lastSelectedItemName));
        yield return new WaitForSeconds(2f);
        BuyItem(lastSelectedItemName, lastSelectedSprite);
    }

    public void BuyItem(string itemName, Sprite sprite)
    {
        int price = itensSO.GetPriceFromSprite(sprite);
        playerMoneySO.ChangeMoney(-price);
        UpdateCoinText();

        playerItemSO.AddPlayerItemTexture(itemName, sprite);
        playerItemSO.SetCurrentSprite(itemName, sprite);
        ApplyAllTextures();
    }

    public void AddMoney(int amount)
    {
        playerMoneySO.ChangeMoney(amount);
        UpdateCoinText();
    }

    public void ChangeCurrentSprite(string itemName, Sprite sprite)
    {
        playerItemSO.SetCurrentSprite(itemName, sprite);
    }

    public void ApplyAllTextures()
    {
        textureApplicators["Monitor"](playerItemSO.ReturnMonitorTexture());
        textureApplicators["Keyboard"](playerItemSO.ReturnKeyboardTexture());
        textureApplicators["Mouse"](playerItemSO.ReturnMouseTexture());
        textureApplicators["Mousepad"](playerItemSO.ReturnMousepadTexture());
        textureApplicators["Cup"](playerItemSO.ReturnCupTexture());
        textureApplicators["Candle"](playerItemSO.ReturnCandleTexture());
        textureApplicators["WallDecor"](playerItemSO.ReturnWallDecorTexture());
        textureApplicators["Mic"](playerItemSO.ReturnMicTexture()); 
        textureApplicators["Headset"](playerItemSO.ReturnHeadsetTexture());
    }

    private void ApplyTextureToTaggedObject(string tag, Sprite sprite)
    {
        if (sprite == null) return;

        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj == null)
        {
            Debug.LogWarning($"Objeto com tag {tag} não encontrado.");
            return;
        }

        Image img = obj.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogWarning($"Componente Image não encontrado para o objeto com tag {tag}.");
            return;
        }

        img.sprite = sprite;
        Debug.Log($"Sprite de {tag} aplicado com sucesso!");
    }

    private IEnumerator AnimateBoxImage(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj == null)
        {
            Debug.LogWarning($"Objeto com tag {tag} não encontrado para animar a imagem da caixa.");
            yield break;
        }

        if (obj.transform.childCount != 1)
        {
            Debug.LogWarning($"O objeto com tag {tag} não tem exatamente um filho (esperado: 1, encontrado: {obj.transform.childCount}).");
            yield break;
        }

        Transform boxTransform = obj.transform.GetChild(0);
        if (boxTransform == null)
        {
            Debug.LogWarning($"Filho do objeto com tag {tag} não encontrado.");
            yield break;
        }

        GameObject box = boxTransform.gameObject;
        box.SetActive(true);

        Canvas boxCanvas = box.GetComponent<Canvas>();
        if (boxCanvas == null)
        {
            boxCanvas = box.AddComponent<Canvas>();
            boxCanvas.overrideSorting = true;
        }
        int originalSortingOrder = boxCanvas.sortingOrder;
        boxCanvas.sortingOrder = 100;

        boxTransform.localScale = new Vector3(0.2f, 0.2f, 1f);
        boxTransform.localRotation = Quaternion.identity;

        float initialGrowTime = 0.5f;
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one;
        while (elapsed < initialGrowTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / initialGrowTime;

            float baseScale = Mathf.Lerp(0.2f, 1f, t);

            float squish = Mathf.Sin(t * Mathf.PI) * 0.05f;
            float scaleX = baseScale * (1f + squish);
            float scaleY = baseScale * (1f - squish);
            boxTransform.localScale = new Vector3(scaleX, scaleY, 1f);

            float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 3f;
            boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);

            yield return null;
        }
        boxTransform.localScale = Vector3.one;
        boxTransform.localRotation = Quaternion.identity;

        for (int i = 0; i < 2; i++)
        {
            float cycleTime = 0.4f;
            float maxScale = 1.05f + (i * 0.05f);

            elapsed = 0f;
            while (elapsed < cycleTime / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (cycleTime / 2);
                float baseScale = Mathf.Lerp(1f, maxScale, t);

                float squish = Mathf.Sin(t * Mathf.PI) * 0.03f;
                float scaleX = baseScale * (1f - squish);
                float scaleY = baseScale * (1f + squish);
                boxTransform.localScale = new Vector3(scaleX, scaleY, 1f);

                float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 3f;
                boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);

                yield return null;
            }

            elapsed = 0f;
            while (elapsed < cycleTime / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (cycleTime / 2);
                float baseScale = Mathf.Lerp(maxScale, 1f, t);

                float squish = Mathf.Sin(t * Mathf.PI) * 0.03f;
                float scaleX = baseScale * (1f + squish);
                float scaleY = baseScale * (1f - squish);
                boxTransform.localScale = new Vector3(scaleX, scaleY, 1f);

                float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 3f;
                boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);

                yield return null;
            }
            boxTransform.localScale = Vector3.one;
            boxTransform.localRotation = Quaternion.identity;
        }

        float finalGrowTime = 0.8f;
        elapsed = 0f;
        float finalMaxScale = 1.3f;
        while (elapsed < finalGrowTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / finalGrowTime;
            float easedT = t * t * t;
            float baseScale = Mathf.Lerp(1f, finalMaxScale, easedT);

            float squish = easedT * 0.05f;
            float scaleX = baseScale * (1f - squish);
            float scaleY = baseScale * (1f + squish);
            boxTransform.localScale = new Vector3(scaleX, scaleY, 1f);

            float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 7f;
            boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);

            yield return null;
        }

        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, boxTransform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab de efeito não atribuído no PlayerManager.");
        }

        boxCanvas.sortingOrder = originalSortingOrder;
        box.SetActive(false);
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = playerMoneySO.GetMoney().ToString();
        }
    }

    private void SetBuyConfirmationActive(bool isActive)
    {
        if (buyConfirmationUI != null)
        {
            if (isActive)
            {
                buyConfirmationUI.SetActive(true);
                buyConfirmationUI.transform.localScale = Vector3.zero;
                buyConfirmationUI.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                buyConfirmationUI.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    buyConfirmationUI.SetActive(false);
                });
            }
        }
    }

    public void ResetAllItems()
    {
        playerItemSO.ResetAllItems();
        itensSO.ResetAllItems();

        ApplyAllTextures();

        UpdateCoinText();

        Debug.Log("All items reset in PlayerManager.");
    }
}