using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public ObjectiveBoardData violetCountData;
    public ObjectiveBoardData greenCountData;
    public ObjectiveBoardData redCountData;
    public ObjectiveBoardData orangeData;
    public ObjectiveBoardData blueData;

    [SerializeField] private TextMeshPro violetCount;
    [SerializeField] private TextMeshPro greenCount;
    [SerializeField] private TextMeshPro redCount;
    [SerializeField] private TextMeshPro orangeCount;
    [SerializeField] private TextMeshPro blueCount;

    [SerializeField] private Sprite checkmarkSprite; // Sprite de checkmark único pra todos

    // Flags pra rastrear se cada objetivo foi completado
    private bool violetCompleted = false;
    private bool greenCompleted = false;
    private bool redCompleted = false;
    private bool orangeCompleted = false;
    private bool blueCompleted = false;

    // Referências pros GameObjects filhos com SpriteRenderer
    private GameObject violetCheckmark;
    private GameObject greenCheckmark;
    private GameObject redCheckmark;
    private GameObject orangeCheckmark;
    private GameObject blueCheckmark;

    void Update()
    {
        UpdateObjective(violetCountData, violetCount, ref violetCompleted, ref violetCheckmark);
        UpdateObjective(greenCountData, greenCount, ref greenCompleted, ref greenCheckmark);
        UpdateObjective(redCountData, redCount, ref redCompleted, ref redCheckmark);
        UpdateObjective(orangeData, orangeCount, ref orangeCompleted, ref orangeCheckmark);
        UpdateObjective(blueData, blueCount, ref blueCompleted, ref blueCheckmark);
    }

    private void UpdateObjective(ObjectiveBoardData data, TextMeshPro textMesh, ref bool isCompleted, ref GameObject checkmarkObject)
    {
        if (isCompleted || textMesh == null) return; // Pula se já completou ou textMesh é nulo

        // Pega o MeshRenderer do GameObject do TextMeshPro
        MeshRenderer textMeshRenderer = textMesh.gameObject.GetComponent<MeshRenderer>();
        if (textMeshRenderer == null)
        {
            Debug.LogError($"MeshRenderer não encontrado em {textMesh.gameObject.name}!");
            return;
        }

        if (data.count > 0)
        {
            // Mostra o texto com o contador
            textMesh.text = data.count.ToString();
            if (checkmarkObject != null) checkmarkObject.SetActive(false); // Desativa o checkmark se existir
        }
        else
        {
            // Contador zerou, vira checkmark
            isCompleted = true;
            textMesh.enabled = false; // Desativa o TextMeshPro

            if (checkmarkSprite == null)
            {
                Debug.LogError("Sprite de checkmark não tá configurado no Inspector!");
                return;
            }

            // Cria ou ativa o GameObject filho com o SpriteRenderer
            if (checkmarkObject == null)
            {
                checkmarkObject = new GameObject(textMesh.gameObject.name + "_Checkmark");
                checkmarkObject.transform.SetParent(textMesh.transform);
                checkmarkObject.transform.localPosition = new Vector3(-.77f, 0.1f, 0f); // Offset pra esquerda
                checkmarkObject.transform.localScale = new Vector3(0.3f, 0.3f, 1f); // Escala menor

                SpriteRenderer spriteRenderer = checkmarkObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = checkmarkSprite;
                spriteRenderer.sortingOrder = textMeshRenderer.sortingOrder; // Usa o sortingOrder do MeshRenderer
                spriteRenderer.sortingLayerName = textMeshRenderer.sortingLayerName; // Usa o sortingLayerName do MeshRenderer
            }
            else
            {
                checkmarkObject.SetActive(true); // Ativa o checkmark se já existe
            }
        }
    }
}