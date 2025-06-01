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

    [SerializeField] private float squashAmountY = 0.8f; // Escala Y mínima durante compressão
    [SerializeField] private float stretchAmountX = 1.2f; // Escala X máxima durante compressão
    [SerializeField] private float squashDuration = 0.2f; // Duração total do efeito de compressão

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

        transform.position = _targetPos;

        isMoving = false;

        StartCoroutine(SquashEffect());
    }

    private IEnumerator SquashEffect()
    {
        Vector3 originalScale = new Vector3(0.8f, 0.8f, 0.8f); 
        Vector3 squashedScale = new Vector3(stretchAmountX * 0.8f, squashAmountY * 0.8f, 0.8f); 

        float compressTime = squashDuration * 0.4f; 
        float elapsed = 0f;

        while (elapsed < compressTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / compressTime;
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t);
            yield return null;
        }
        transform.localScale = squashedScale;

        float returnTime = squashDuration * 0.6f; 
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
    Orange,
    Blue
}