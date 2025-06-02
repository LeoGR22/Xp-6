using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro; // Para TextMeshProUGUI
using DG.Tweening; // Para animação com DOTween (opcional)

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

    public LoadLevelData loadLevel;

    private List<GameObject> instantiatedCountUIs = new List<GameObject>();

    void Start()
    {
        loadLevel.LoadData();
        AlignUIElements();
    }

    private void AlignUIElements()
    {
        ClearInstantiatedElements();

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

        // Converte a posição do centerObject para o espaço local do canvas
        Vector3 centerWorldPos = centerObject.transform.position;
        Vector2 centerCanvasPos = RectTransformUtility.WorldToScreenPoint(Camera.main, centerWorldPos);
        RectTransform canvasRect = canvasParent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, centerCanvasPos, null, out Vector2 localCenterPos);

        if (activeTypes.Count <= 2)
        {
            // Alinhamento em uma única linha horizontal
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
                        float currentY = uiRect.anchoredPosition.y;
                        float newX = uiStartX + (i * uiSpacing);
                        Vector2 newCanvasPos = new Vector2(newX, currentY);
                        uiRect.anchoredPosition = newCanvasPos;

                        // Atualiza o texto do TextMeshProUGUI
                        TextMeshProUGUI textComponent = countUI.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.text = potionCounts[type].count.ToString();
                        }
                        else
                        {
                            Debug.LogWarning($"TextMeshProUGUI não encontrado em {countUI.name}.");
                        }

                        // Aplica animação com DOTween (opcional)
                        uiRect.localScale = Vector3.zero;
                        uiRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                        instantiatedCountUIs.Add(countUI);
                        //Debug.Log($"Instanciado e posicionado contador {type} no Canvas em anchoredPosition: {newCanvasPos} com valor {potionCounts[type].count}");
                    }
                    else
                    {
                        Debug.LogWarning($"RectTransform não encontrado em {countUI.name}.");
                    }
                }
            }
        }
        else
        {
            // Alinhamento em duas linhas: 2 em cima, restantes em baixo
            List<ItemType> topRow = activeTypes.Take(2).ToList();
            List<ItemType> bottomRow = activeTypes.Skip(2).ToList();

            // Linha superior (até 2 contadores)
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

                        // Atualiza o texto do TextMeshProUGUI
                        TextMeshProUGUI textComponent = countUI.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.text = potionCounts[type].count.ToString();
                        }
                        else
                        {
                            Debug.LogWarning($"TextMeshProUGUI não encontrado em {countUI.name}.");
                        }

                        // Aplica animação com DOTween (opcional)
                        uiRect.localScale = Vector3.zero;
                        uiRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                        instantiatedCountUIs.Add(countUI);
                        //Debug.Log($"Instanciado e posicionado contador {type} na linha superior em anchoredPosition: {newCanvasPos} com valor {potionCounts[type].count}");
                    }
                    else
                    {
                        Debug.LogWarning($"RectTransform não encontrado em {countUI.name}.");
                    }
                }
            }

            // Linha inferior (restantes)
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

                        // Atualiza o texto do TextMeshProUGUI
                        TextMeshProUGUI textComponent = countUI.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.text = potionCounts[type].count.ToString();
                        }
                        else
                        {
                            Debug.LogWarning($"TextMeshProUGUI não encontrado em {countUI.name}.");
                        }

                        // Aplica animação com DOTween (opcional)
                        uiRect.localScale = Vector3.zero;
                        uiRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                        instantiatedCountUIs.Add(countUI);
                        //Debug.Log($"Instanciado e posicionado contador {type} na linha inferior em anchoredPosition: {newCanvasPos} com valor {potionCounts[type].count}");
                    }
                    else
                    {
                        Debug.LogWarning($"RectTransform não encontrado em {countUI.name}.");
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
