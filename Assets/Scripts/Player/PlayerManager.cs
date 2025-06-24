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
    [SerializeField] private FloatSO tutorialPart;
    [SerializeField] private ItensSO itensSO;
    [SerializeField] private LevelData levelData;
    [SerializeField] private BooleanSO tutorialSO;

    [Header("UI Elements")]
    [SerializeField] private GameObject buyConfirmationUI;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Button fullScreenButton;
    [SerializeField] private Sprite openBoxSprite;
    [SerializeField] private TMP_Text levelText;

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
        // Corrigido para incluir apenas as categorias válidas
        textureApplicators.Add("Monitor", sprite => ApplyTextureToTaggedObject("Monitor", sprite));
        textureApplicators.Add("Keyboard", sprite => ApplyTextureToTaggedObject("Keyboard", sprite));
        textureApplicators.Add("Mouse", sprite => ApplyTextureToTaggedObject("Mouse", sprite));
        textureApplicators.Add("Mousepad", sprite => ApplyTextureToTaggedObject("Mousepad", sprite));
        textureApplicators.Add("Cup", sprite => ApplyTextureToTaggedObject("Cup", sprite));
        textureApplicators.Add("Candle", sprite => ApplyTextureToTaggedObject("Candle", sprite));
        textureApplicators.Add("WallDecor", sprite => ApplyTextureToTaggedObject("WallDecor", sprite));
        textureApplicators.Add("Mic", sprite => ApplyTextureToTaggedObject("Mic", sprite));
        textureApplicators.Add("Headset", sprite => ApplyTextureToTaggedObject("Headset", sprite));

        if (PlayerPrefs.GetInt("FirstRun", 1) == 1)
        {
            Debug.Log("Nova instalação detectada. Limpando save e inicializando valores padrão.");
            SaveSystem.ClearSave(); // Exclui qualquer save residual
            PlayerPrefs.SetInt("FirstRun", 0);
            PlayerPrefs.Save();

            // Reseta ScriptableObjects para valores padrão
            playerMoneySO.SetMoney(0);
            levelData.ChangeLevel(0);
            tutorialSO.ChangeBool(true);
            tutorialPart.SetFloat(1f);
            playerItemSO.ResetAllItems();
            //itensSO.ResetAllItems();
        }
    }

    private void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;

        LoadPlayer();

        ApplyAllTextures();
        UpdateCoinText();
        SetBuyConfirmationActive(false);
        if (fullScreenButton != null)
        {
            fullScreenButton.gameObject.SetActive(false);
        }
        foreach (PriceManager priceManager in FindObjectsOfType<PriceManager>())
        {
            priceManager.RefreshButtonState();
            Debug.Log("Atualizando PriceManager no Start do PlayerManager.");
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log($"Jogador salvo em: {Application.persistentDataPath}/player.json");
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer(this);
        if (data != null)
        {
            playerMoneySO.SetMoney(data.money);
            Debug.Log($"Dinheiro carregado: {data.money}");

            if(levelText != null)
            {
                levelText.text = data.level.ToString();
            }

            if (levelData != null)
            {
                levelData.ChangeLevel(data.level);
                Debug.Log($"Nível carregado: {data.level}");
            }

            if (tutorialSO != null)
            {
                tutorialSO.ChangeBool(data.isTutorial);
                Debug.Log($"Tutorial carregado: {data.isTutorial}");
            }

            if (data.ownedItems != null)
            {
                playerItemSO.ResetAllItems();
                foreach (var category in data.ownedItems)
                {
                    Debug.Log($"Carregando categoria {category.category}: {string.Join(", ", category.sprites)}");
                    foreach (var spriteName in category.sprites)
                    {
                        if (!string.IsNullOrEmpty(spriteName))
                        {
                            ItensSO.ItemData itemData = itensSO.GetItemDataFromSpriteName(spriteName);
                            if (itemData != null)
                            {
                                playerItemSO.AddPlayerItemTexture(category.category, itemData.sprite);
                                Debug.Log($"Item adicionado: {spriteName} em {category.category}");
                            }
                            else
                            {
                                Debug.LogWarning($"Item {spriteName} não encontrado no ItensSO.");
                            }
                        }
                    }
                }
            }

            if (data.currentItems != null)
            {
                foreach (var category in data.currentItems)
                {
                    if (!string.IsNullOrEmpty(category.sprite))
                    {
                        ItensSO.ItemData itemData = itensSO.GetItemDataFromSpriteName(category.sprite);
                        if (itemData != null)
                        {
                            playerItemSO.SetCurrentSprite(category.category, itemData.sprite);
                            Debug.Log($"Sprite atual definido: {category.sprite} em {category.category}");
                        }
                        else
                        {
                            Debug.LogWarning($"Sprite atual {category.sprite} não encontrado no ItensSO.");
                        }
                    }
                }
            }

            ApplyAllTextures();
            UpdateCoinText();

            foreach (PriceManager priceManager in FindObjectsOfType<PriceManager>())
            {
                priceManager.RefreshButtonState();
                Debug.Log("Atualizando PriceManager após LoadPlayer.");
            }

            Debug.Log($"Estado final (Monitor): Possuídos={string.Join(", ", GetSpriteNames(playerItemSO.playerMonitorSprites))}, Atual={playerItemSO.currentMonitor?.name}");
        }
    }

    public int GetMoney()
    {
        return playerMoneySO.GetMoney();
    }

    public int GetLevel()
    {
        return levelData != null ? (int)levelData.GetLevel() : 0;
    }

    public bool GetTutorialState()
    {
        return tutorialSO != null ? tutorialSO.value : false;
    }

    public float GetTutorialPart()
    {
        return tutorialPart.GetFloat();
    }

    public Dictionary<string, string[]> GetOwnedItems()
    {
        var ownedItems = new Dictionary<string, string[]>();
        ownedItems.Add("Monitor", GetSpriteNames(playerItemSO.playerMonitorSprites));
        Debug.Log($"Itens possuídos (Monitor): {string.Join(", ", GetSpriteNames(playerItemSO.playerMonitorSprites))}");
        ownedItems.Add("Keyboard", GetSpriteNames(playerItemSO.playerKeyboardSprites));
        Debug.Log($"Itens possuídos (Keyboard): {string.Join(", ", GetSpriteNames(playerItemSO.playerKeyboardSprites))}");
        ownedItems.Add("Mouse", GetSpriteNames(playerItemSO.playerMouseSprites));
        ownedItems.Add("Mousepad", GetSpriteNames(playerItemSO.playerMousepadSprites));
        ownedItems.Add("Cup", GetSpriteNames(playerItemSO.playerCupSprites));
        ownedItems.Add("Candle", GetSpriteNames(playerItemSO.playerCandleSprites));
        ownedItems.Add("WallDecor", GetSpriteNames(playerItemSO.playerWallDecorSprites));
        ownedItems.Add("Mic", GetSpriteNames(playerItemSO.playerMicSprites));
        ownedItems.Add("Headset", GetSpriteNames(playerItemSO.playerHeadsetSprites));
        return ownedItems;
    }

    public Dictionary<string, string> GetCurrentItems()
    {
        var currentItems = new Dictionary<string, string>();
        currentItems.Add("Monitor", playerItemSO.currentMonitor?.name);
        currentItems.Add("Keyboard", playerItemSO.currentKeyboard?.name);
        currentItems.Add("Mouse", playerItemSO.currentMouse?.name);
        currentItems.Add("Mousepad", playerItemSO.currentMousepad?.name);
        currentItems.Add("Cup", playerItemSO.currentCup?.name);
        currentItems.Add("Candle", playerItemSO.currentCandle?.name);
        currentItems.Add("WallDecor", playerItemSO.currentWallDecor?.name);
        currentItems.Add("Mic", playerItemSO.currentMic?.name);
        currentItems.Add("Headset", playerItemSO.currentHeadset?.name);
        return currentItems;
    }

    private string[] GetSpriteNames(Sprite[] sprites)
    {
        if (sprites == null)
            return new string[0];
        string[] names = new string[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            names[i] = sprites[i]?.name;
        }
        return names;
    }

    // Métodos existentes do PlayerManager
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
            Debug.Log($"Atualizando PriceManager para {itemName}, Sprite={sprite.name}");
        }

        SavePlayer();

        LoadPlayer();
        Debug.Log($"Item salvo: Categoria={itemName}, Sprite={sprite.name}, Possuídos (Monitor): {string.Join(", ", GetSpriteNames(playerItemSO.playerMonitorSprites))}");
    }


    public void AddMoney(int amount)
    {
        playerMoneySO.ChangeMoney(amount);
        UpdateCoinText();
        //SavePlayer();
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

        Sprite initialBoxSprite = boxImage.sprite;

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

        if (fullScreenButton != null)
        {
            fullScreenButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Botão de tela cheia não atribuído no PlayerManager.");
            yield break;
        }

        Sequence idleSequence = DOTween.Sequence();
        idleSequence.SetLoops(-1);
        idleSequence.Append(boxTransform.DOScale(1.05f, 0.5f).SetEase(Ease.InOutSine));
        idleSequence.Append(boxTransform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine));
        idleSequence.Join(boxTransform.DORotate(new Vector3(0f, 0f, 3f), 0.5f).SetEase(Ease.InOutSine));
        idleSequence.Append(boxTransform.DORotate(new Vector3(0f, 0f, -3f), 1f).SetEase(Ease.InOutSine));
        idleSequence.Append(boxTransform.DORotate(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.InOutSine));

        bool clicked = false;
        fullScreenButton.onClick.AddListener(() => clicked = true);
        yield return new WaitUntil(() => clicked);
        fullScreenButton.onClick.RemoveAllListeners();
        fullScreenButton.gameObject.SetActive(false);

        idleSequence.Kill();
        AudioManager.Instance.PlaySFX("OpenBox");

        if (openBoxSprite != null)
        {
            boxImage.sprite = openBoxSprite;
        }
        else
        {
            Debug.LogWarning("Sprite da caixa aberta não atribuído no PlayerManager.");
        }

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

        boxImage.sprite = initialBoxSprite;

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
        //itensSO.ResetAllItems();
        ApplyAllTextures();
        UpdateCoinText();
        Debug.Log("All items reset in PlayerManager.");
    }
}