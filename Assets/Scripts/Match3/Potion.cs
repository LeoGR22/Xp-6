using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public ItemType potionType;

    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;

    public bool isMoving;

    // Par�metros para o efeito de compress�o (ajust�veis no Inspector)
    [SerializeField] private float squashAmountY = 0.8f; // Escala Y m�nima durante compress�o
    [SerializeField] private float stretchAmountX = 1.2f; // Escala X m�xima durante compress�o
    [SerializeField] private float squashDuration = 0.2f; // Dura��o total do efeito de compress�o

    public Potion(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    // MoveToTarget
    public void MoveToTarget(Vector2 _targetPos, float duration = 0.15f)
    {
        StartCoroutine(MoveCoroutine(_targetPos, duration));
    }

    // MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 _targetPos, float duration)
    {
        isMoving = true;

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(startPosition, _targetPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que a po��o esteja na posi��o final
        transform.position = _targetPos;

        // Marca a po��o como n�o mais em movimento
        isMoving = false;

        // Inicia a anima��o de compress�o sem aguard�-la
        StartCoroutine(SquashEffect());
    }

    // Coroutine para o efeito de compress�o
    private IEnumerator SquashEffect()
    {
        Vector3 originalScale = Vector3.one; // Escala normal (1, 1, 1)
        Vector3 squashedScale = new Vector3(stretchAmountX, squashAmountY, 1f); // Escala comprimida

        // Fase 1: Comprimir (squash) rapidamente
        float compressTime = squashDuration * 0.4f; // 40% do tempo para compress�o
        float elapsed = 0f;

        while (elapsed < compressTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / compressTime;
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t);
            yield return null;
        }
        transform.localScale = squashedScale;

        // Fase 2: Voltar ao normal suavemente
        float returnTime = squashDuration * 0.6f; // 60% do tempo para voltar
        elapsed = 0f;

        while (elapsed < returnTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnTime;
            transform.localScale = Vector3.Lerp(squashedScale, originalScale, t);
            yield return null;
        }
        transform.localScale = originalScale;
    }
}

public enum ItemType
{
    Green,
    Red,
    Violet,
    Orange
}