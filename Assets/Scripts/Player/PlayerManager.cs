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
    [SerializeField] private Button fullScreenButton;
    [SerializeField] private Sprite openBoxSprite;

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
        textureApplicators.Add("Monitor", sprite => ApplyTextureToTaggedObject("Monitor", sprite));
        textureApplicators.Add("Keyboard", sprite => ApplyTextureToTaggedObject("Keyboard", sprite));
        textureApplicators.Add("Mouse", sprite => ApplyTextureToTaggedObject("Mouse", sprite));
        textureApplicators.Add("Mousepad", sprite => ApplyTextureToTaggedObject("Mousepad", sprite));
        textureApplicators.Add("Cup", sprite => ApplyTextureToTaggedObject("Cup", sprite));
        textureApplicators.Add("Candle", sprite => ApplyTextureToTaggedObject("Candle", sprite));
        textureApplicators.Add("WallDecor", sprite => ApplyTextureToTaggedObject("WallDecor", sprite));
        textureApplicators.Add("Mic", sprite => ApplyTextureToTaggedObject("Mic", sprite));
        textureApplicators.Add("Headset", sprite => ApplyTextureToTaggedObject("Headset", sprite));
    }

    private void Start()
    {
        ApplyAllTextures();
        UpdateCoinText();
        SetBuyConfirmationActive(false);
        if (fullScreenButton != null)
        {
            fullScreenButton.gameObject.SetActive(false);
        }
    }

    public void VerifyItem(string itemName, Sprite sprite)
    {
        if (playerItemSO.IsSpriteInCategory(itemName, sprite))
        {
            playerItemSO.SetCurrentSprite(itemName, sprite);
            ApplyAllTextures();

            foreach (PriceManager priceManager in FindObjectsOfType<PriceManager>())
            {
                priceManager.RefreshButtonState();
            }
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
        yield return StartCoroutine(AnimateBoxImage(lastSelectedItemName));
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

        foreach (PriceManager priceManager in FindObjectsOfType<PriceManager>())
        {
            priceManager.RefreshButtonState();
        }
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
        Image boxImage = box.GetComponent<Image>();
        if (boxImage == null)
        {
            Debug.LogWarning($"Componente Image não encontrado na caixa.");
            yield break;
        }

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

        Sequence sequence = DOTween.Sequence();

        // Fase 1: Crescimento inicial (0.5s)
        sequence.Append(boxTransform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                float t = sequence.ElapsedPercentage();
                float squish = Mathf.Sin(t * Mathf.PI) * 0.05f;
                boxTransform.localScale = new Vector3(
                    boxTransform.localScale.x * (1f + squish),
                    boxTransform.localScale.y * (1f - squish),
                    1f);
                float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 3f;
                boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);
            }));

        // Fase 2: Dois ciclos de pulsação (0.4s cada)
        for (int i = 0; i < 2; i++)
        {
            float maxScale = 1.05f + (i * 0.05f);
            sequence.Append(boxTransform.DOScale(maxScale, 0.2f)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    float t = sequence.ElapsedPercentage(true);
                    float squish = Mathf.Sin(t * Mathf.PI) * 0.03f;
                    boxTransform.localScale = new Vector3(
                        boxTransform.localScale.x * (1f - squish),
                        boxTransform.localScale.y * (1f + squish),
                        1f);
                    float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 3f;
                    boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);
                }));
            sequence.Append(boxTransform.DOScale(1f, 0.2f)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    float t = sequence.ElapsedPercentage(true);
                    float squish = Mathf.Sin(t * Mathf.PI) * 0.03f;
                    boxTransform.localScale = new Vector3(
                        boxTransform.localScale.x * (1f + squish),
                        boxTransform.localScale.y * (1f - squish),
                        1f);
                    float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 3f;
                    boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);
                }));
        }

        yield return sequence.WaitForCompletion();

        // Ativa o botão de tela cheia
        if (fullScreenButton != null)
        {
            fullScreenButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Botão de tela cheia não atribuído no PlayerManager.");
            yield break;
        }

        // Animação de idle (pulsação leve e rotação)
        Sequence idleSequence = DOTween.Sequence();
        idleSequence.SetLoops(-1); // Loop infinito até o clique
        idleSequence.Append(boxTransform.DOScale(1.05f, 0.5f).SetEase(Ease.InOutSine));
        idleSequence.Append(boxTransform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine));
        idleSequence.Join(boxTransform.DORotate(new Vector3(0f, 0f, 3f), 0.5f).SetEase(Ease.InOutSine));
        idleSequence.Append(boxTransform.DORotate(new Vector3(0f, 0f, -3f), 1f).SetEase(Ease.InOutSine));
        idleSequence.Append(boxTransform.DORotate(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.InOutSine));

        // Espera o clique do jogador
        bool clicked = false;
        fullScreenButton.onClick.AddListener(() => clicked = true);
        yield return new WaitUntil(() => clicked);
        fullScreenButton.onClick.RemoveAllListeners();
        fullScreenButton.gameObject.SetActive(false);
        
        // Para a animação de idle
        idleSequence.Kill();

        // Troca para o sprite da caixa aberta
        if (openBoxSprite != null)
        {
            boxImage.sprite = openBoxSprite;
        }
        else
        {
            Debug.LogWarning("Sprite da caixa aberta não atribuído no PlayerManager.");
        }

        // Fase 3: Crescimento final (0.8s)
        sequence = DOTween.Sequence();
        sequence.Append(boxTransform.DOScale(1.3f, 0.8f)
            .SetEase(Ease.InOutCubic)
            .OnUpdate(() =>
            {
                float t = sequence.ElapsedPercentage(true);
                float squish = t * 0.05f;
                boxTransform.localScale = new Vector3(
                    boxTransform.localScale.x * (1f - squish),
                    boxTransform.localScale.y * (1f + squish),
                    1f);
                float swayAngle = Mathf.Sin(t * Mathf.PI * 2f) * 7f;
                boxTransform.localRotation = Quaternion.Euler(0f, 0f, swayAngle);
            }));

        yield return sequence.WaitForCompletion();

        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, boxTransform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab de efeito não atribuído no PlayerManager.");
        }

        gameAnimator.SetTrigger("OpenBox");
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