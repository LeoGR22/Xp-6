using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;

public class LevelConfirmationTarget : MonoBehaviour
{
    [Header("UI Prefabs (TextMeshProUGUI)")]
    [SerializeField] private GameObject violetCountUIPrefab;
    [SerializeField] private GameObject greenCountUIPrefab;
    [SerializeField] private GameObject redCountUIPrefab;
    [SerializeField] private GameObject orangeCountUIPrefab;
    [SerializeField] private GameObject blueCountUIPrefab;

    [Header("Objective Data")]
    [SerializeField] private ObjectiveBoardData violetPotionCount;
    [SerializeField] private ObjectiveBoardData greenPotionCount;
    [SerializeField] private ObjectiveBoardData redPotionCount;
    [SerializeField] private ObjectiveBoardData orangePotionCount;
    [SerializeField] private ObjectiveBoardData bluePotionCount;

    [Header("Alignment Settings")]
    [SerializeField] private GameObject centerObject;
    [SerializeField] private float uiSpacing = 50f;
    [SerializeField] private float verticalSpacing = 50f;
    [SerializeField] private GameObject canvasParent;

    [Header("Level")]
    [SerializeField] private LevelData level;
    [SerializeField] private TMP_Text levelText;

    public LoadLevelData loadLevel;

    private List<GameObject> instantiatedCountUIs = new List<GameObject>();

    void Start()
    {
        loadLevel.LoadData();
        AlignUIElements();

       gameObject.SetActive(false);
    }

    private void AlignUIElements()
    {
        ClearInstantiatedElements();

        if (levelText != null)
        {
            levelText.text = $"Level {level.level}";
        }

        var countUIPrefabs = new Dictionary<ItemType, GameObject>
        {
            { ItemType.Violet, violetCountUIPrefab },
            { ItemType.Green, greenCountUIPrefab },
            { ItemType.Red, redCountUIPrefab },
            { ItemType.Orange, orangeCountUIPrefab },
            { ItemType.Blue, blueCountUIPrefab }
        };

        var potionCounts = new Dictionary<ItemType, ObjectiveBoardData>
        {
            { ItemType.Violet, violetPotionCount },
            { ItemType.Green, greenPotionCount },
            { ItemType.Red, redPotionCount },
            { ItemType.Orange, orangePotionCount },
            { ItemType.Blue, bluePotionCount }
        };

        var activeTypes = potionCounts.Where(kvp => kvp.Value != null && kvp.Value.count > 0)
                                      .Select(kvp => kvp.Key)
                                      .ToList();

        if (activeTypes.Count == 0)
        {
            Debug.LogWarning("Nenhum objetivo ativo encontrado para alinhar.");
            return;
        }

        // Verifica se os componentes necess�rios est�o configurados
        if (canvasParent == null || canvasParent.GetComponent<RectTransform>() == null)
        {
            Debug.LogError("canvasParent n�o est� atribu�do ou n�o tem RectTransform!");
            return;
        }

        if (centerObject == null)
        {
            Debug.LogWarning("centerObject n�o atribu�do. Usando centro do canvas como fallback.");
            centerObject = canvasParent; // Fallback para o pr�prio canvas
        }

        RectTransform canvasRect = canvasParent.GetComponent<RectTransform>();
        Vector2 localCenterPos = Vector2.zero;

        // Converte a posi��o do centerObject para o espa�o local do canvas
        if (Camera.main != null)
        {
            Vector3 centerWorldPos = centerObject.transform.position;
            Debug.Log($"centerObject posi��o no mundo: {centerWorldPos}");
            Vector2 centerCanvasPos = RectTransformUtility.WorldToScreenPoint(Camera.main, centerWorldPos);
            Debug.Log($"centerObject posi��o na tela: {centerCanvasPos}");
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, centerCanvasPos, Camera.main, out localCenterPos);
            Debug.Log($"centerObject posi��o local no canvas: {localCenterPos}");
        }
        else
        {
            Debug.LogWarning("Camera.main n�o encontrada. Usando centro do canvas (0,0).");
            localCenterPos = Vector2.zero; // Fallback para o centro do canvas
        }

        if (activeTypes.Count <= 2)
        {
            float uiTotalWidth = (activeTypes.Count - 1) * uiSpacing;
            float uiStartX = localCenterPos.x - (uiTotalWidth / 2f);

            for (int i = 0; i < activeTypes.Count; i++)
            {
                ItemType type = activeTypes[i];
                if (countUIPrefabs[type] != null)
                {
                    GameObject countUI = Instantiate(countUIPrefabs[type], canvasParent.transform);
                    RectTransform uiRect = countUI.GetComponent<RectTransform>();
                    if (uiRect != null)
                    {
                        float newX = uiStartX + (i * uiSpacing);
                        Vector2 newCanvasPos = new Vector2(newX, localCenterPos.y);
                        uiRect.anchoredPosition = newCanvasPos;
                        Debug.Log($"Instanciando {countUI.name} na posi��o local: {newCanvasPos}");

                        TextMeshProUGUI textComponent = countUI.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.text = potionCounts[type].count.ToString();
                        }
                        else
                        {
                            Debug.LogWarning($"TextMeshProUGUI n�o encontrado em {countUI.name}.");
                        }

                        uiRect.localScale = Vector3.zero;
                        uiRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                        instantiatedCountUIs.Add(countUI);
                    }
                    else
                    {
                        Debug.LogWarning($"RectTransform n�o encontrado em {countUI.name}.");
                        Destroy(countUI);
                    }
                }
            }
        }
        else
        {
            List<ItemType> topRow = activeTypes.Take(2).ToList();
            List<ItemType> bottomRow = activeTypes.Skip(2).ToList();

            float topRowWidth = (topRow.Count - 1) * uiSpacing;
            float topRowStartX = localCenterPos.x - (topRowWidth / 2f);
            float topRowY = localCenterPos.y + (verticalSpacing / 2f);

            for (int i = 0; i < topRow.Count; i++)
            {
                ItemType type = topRow[i];
                if (countUIPrefabs[type] != null)
                {
                    GameObject countUI = Instantiate(countUIPrefabs[type], canvasParent.transform);
                    RectTransform uiRect = countUI.GetComponent<RectTransform>();
                    if (uiRect != null)
                    {
                        float newX = topRowStartX + (i * uiSpacing);
                        Vector2 newCanvasPos = new Vector2(newX, topRowY);
                        uiRect.anchoredPosition = newCanvasPos;
                        Debug.Log($"Instanciando {countUI.name} na posi��o local: {newCanvasPos}");

                        TextMeshProUGUI textComponent = countUI.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.text = potionCounts[type].count.ToString();
                        }
                        else
                        {
                            Debug.LogWarning($"TextMeshProUGUI n�o encontrado em {countUI.name}.");
                        }

                        uiRect.localScale = Vector3.zero;
                        uiRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                        instantiatedCountUIs.Add(countUI);
                    }
                    else
                    {
                        Debug.LogWarning($"RectTransform n�o encontrado em {countUI.name}.");
                        Destroy(countUI);
                    }
                }
            }

            float bottomRowWidth = (bottomRow.Count - 1) * uiSpacing;
            float bottomRowStartX = localCenterPos.x - (bottomRowWidth / 2f);
            float bottomRowY = localCenterPos.y - (verticalSpacing / 2f);

            for (int i = 0; i < bottomRow.Count; i++)
            {
                ItemType type = bottomRow[i];
                if (countUIPrefabs[type] != null)
                {
                    GameObject countUI = Instantiate(countUIPrefabs[type], canvasParent.transform);
                    RectTransform uiRect = countUI.GetComponent<RectTransform>();
                    if (uiRect != null)
                    {
                        float newX = bottomRowStartX + (i * uiSpacing);
                        Vector2 newCanvasPos = new Vector2(newX, bottomRowY);
                        uiRect.anchoredPosition = newCanvasPos;
                        Debug.Log($"Instanciando {countUI.name} na posi��o local: {newCanvasPos}");

                        TextMeshProUGUI textComponent = countUI.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.text = potionCounts[type].count.ToString();
                        }
                        else
                        {
                            Debug.LogWarning($"TextMeshProUGUI n�o encontrado em {countUI.name}.");
                        }

                        uiRect.localScale = Vector3.zero;
                        uiRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                        instantiatedCountUIs.Add(countUI);
                    }
                    else
                    {
                        Debug.LogWarning($"RectTransform n�o encontrado em {countUI.name}.");
                        Destroy(countUI);
                    }
                }
            }
        }
    }

    private void ClearInstantiatedElements()
    {
        foreach (GameObject countUI in instantiatedCountUIs)
        {
            if (countUI != null)
            {
                Destroy(countUI);
            }
        }
        instantiatedCountUIs.Clear();
    }

    private void OnDestroy()
    {
        ClearInstantiatedElements();
    }
}