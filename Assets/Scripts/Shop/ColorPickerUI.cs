using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ColorPickerUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private ItensSO itensSO;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 buttonSpacing = new Vector2(100f, 100f); 
    [SerializeField] private float verticalOffset = 20f; 
    [SerializeField] private float xOffset = 0f; 

    private string currentItemType;
    private PlayerManager playerManager;
    private readonly float animationDuration = 0.3f;

    private void Awake()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }
            if (canvas == null)
            {
                Debug.LogError("Canvas não encontrado para ColorPickerUI!", gameObject);
            }
        }
    }

    public void Show(string itemType, Sprite currentSprite, Vector2 touchPosition)
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas é null! Não é possível mostrar a UI.", gameObject);
            return;
        }

        currentItemType = itemType;
        playerManager = FindObjectOfType<PlayerManager>();
        if (itensSO == null)
        {
            itensSO = FindObjectOfType<PlayerManager>().GetComponentInChildren<ItensSO>();
            if (itensSO == null)
            {
                Debug.LogError("ItensSO não encontrado!", gameObject);
                return;
            }
        }

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        var variants = itensSO.GetVariantsFromSprite(currentSprite);
        Vector2 startPos = ConvertScreenToCanvasPosition(touchPosition);
        Debug.Log($"startPos: {startPos}, touchPosition: {touchPosition}, variants: {variants.Length}");

        List<Vector2> positions = CalculatePyramidPositions(variants.Length);
        if (positions.Count != variants.Length)
        {
            Debug.LogError($"CalculatePyramidPositions retornou {positions.Count} posições, esperado {variants.Length}");
            return;
        }

        for (int i = 0; i < variants.Length; i++)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
                buttonImage.sprite = variants[i].sprite;

            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.text = variants[i].colorName;

            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localScale = Vector3.zero;

            Vector2 finalPos = startPos + positions[i];
            buttonRect.anchoredPosition = startPos;

            int index = i;
            float delay = i * 0.1f;
            LeanTween.scale(buttonObj, Vector3.one, animationDuration).setDelay(delay).setEaseOutBack();
            LeanTween.move(buttonRect, finalPos, animationDuration).setDelay(delay).setEaseOutQuad().setOnComplete(() =>
            {
                Debug.Log($"Botão {index} terminou em {buttonRect.anchoredPosition} (offset: {positions[index]})");
            });

            ColorButton colorButton = buttonObj.AddComponent<ColorButton>();
            colorButton.Setup(this, variants[i].sprite, animationDuration + delay);
        }

        gameObject.SetActive(true);
        AudioManager.Instance.PlaySFX("Click2");
    }

    private List<Vector2> CalculatePyramidPositions(int count)
    {
        List<Vector2> positions = new List<Vector2>();

        if (count <= 0)
        {
            Debug.LogWarning("CalculatePyramidPositions chamado com count <= 0");
            return positions;
        }

        if (count == 1)
        {
            positions.Add(new Vector2(xOffset, verticalOffset));
            return positions;
        }

        int[] iconsPerRow;
        if (count == 2)
            iconsPerRow = new int[] { 2 };
        else if (count == 3)
            iconsPerRow = new int[] { 2, 1 };
        else if (count == 4)
            iconsPerRow = new int[] { 3, 1 };
        else if (count == 5)
            iconsPerRow = new int[] { 3, 2 };
        else 
            iconsPerRow = new int[] { 3, 2, 1 };

        int currentIcon = 0;
        float gridHeight = (iconsPerRow.Length - 1) * buttonSpacing.y;
        for (int row = 0; row < iconsPerRow.Length && currentIcon < count; row++)
        {
            int iconsInRow = Mathf.Min(iconsPerRow[row], count - currentIcon);
            float rowY = verticalOffset + gridHeight - row * buttonSpacing.y;

            // Centralizar a linha
            float rowWidth = (iconsInRow - 1) * buttonSpacing.x;
            float startX = -rowWidth / 2f;
            for (int col = 0; col < iconsInRow; col++)
            {
                float posX = startX + col * buttonSpacing.x + xOffset; 
                positions.Add(new Vector2(posX, rowY));
                currentIcon++;
            }
        }

        Debug.Log($"CalculatePyramidPositions: count={count}, positions={positions.Count}");
        return positions;
    }

    public void TrySelectColor(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            Debug.Log("TrySelectColor: Nenhum objeto sob o ponteiro.");
            AudioManager.Instance.PlaySFX("Click2");
            return;
        }

        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log($"TrySelectColor: Objeto sob o ponteiro: {target.name}");

        ColorButton colorButton = target.GetComponentInParent<ColorButton>();
        if (colorButton == null)
        {
            Debug.Log($"TrySelectColor: ColorButton não encontrado em {target.name}");
            AudioManager.Instance.PlaySFX("Click2");
            return;
        }

        SelectColor(colorButton.GetSprite());
    }

    private void SelectColor(Sprite sprite)
    {
        playerManager.ChangeCurrentSprite(currentItemType, sprite);
        playerManager.ApplyAllTextures();
        AudioManager.Instance.PlaySFX("Click2");
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void Hide()
    {
        foreach (Transform child in buttonContainer)
        {
            LeanTween.scale(child.gameObject, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => Destroy(child.gameObject));
        }
        LeanTween.delayedCall(0.2f, () => gameObject.SetActive(false));
    }

    private Vector2 ConvertScreenToCanvasPosition(Vector2 screenPos)
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas é null em ConvertScreenToCanvasPosition!", gameObject);
            return Vector2.zero;
        }

        Vector2 viewportPos = screenPos / new Vector2(Screen.width, Screen.height);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            Debug.LogError("RectTransform do Canvas não encontrado!", gameObject);
            return Vector2.zero;
        }

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera, out canvasPos);
        return canvasPos;
    }
}

public class ColorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ColorPickerUI colorPickerUI;
    private Sprite sprite;
    private RectTransform rectTransform;
    private Image buttonImage; 
    private bool isHovered;
    private int scaleTweenId = -1; 

    private readonly float hoverScale = 1.2f;
    private readonly float animationTime = 0.2f;
    private readonly float raycastEnableDelay = 0.05f; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.raycastTarget = false; 
        }
        else
        {
            Debug.LogWarning("Image component não encontrado em ColorButton!", gameObject);
        }
    }

    public void Setup(ColorPickerUI picker, Sprite sprite, float animationDuration)
    {
        this.colorPickerUI = picker;
        this.sprite = sprite;

        LeanTween.delayedCall(raycastEnableDelay, () =>
        {
            if (buttonImage != null)
            {
                buttonImage.raycastTarget = true;
            }
        });
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered || buttonImage == null || !buttonImage.raycastTarget) return;

        isHovered = true;
        if (scaleTweenId != -1) LeanTween.cancel(scaleTweenId);
        scaleTweenId = LeanTween.scale(rectTransform, Vector3.one * hoverScale, animationTime)
            .setEaseOutQuad()
            .uniqueId;
        AudioManager.Instance.PlaySFX("Hover"); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered || buttonImage == null || !buttonImage.raycastTarget) return;

        isHovered = false;
        if (scaleTweenId != -1) LeanTween.cancel(scaleTweenId);
        scaleTweenId = LeanTween.scale(rectTransform, Vector3.one, animationTime)
            .setEaseOutQuad()
            .uniqueId;
    }
}