using UnityEngine;

public class ClickEffectSpawner : MonoBehaviour
{
    public GameObject clickEffectPrefab; // O efeito que ser� instanciado

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 spawnPosition = GetWorldPosition(Input.GetTouch(0).position);
            Instantiate(clickEffectPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
        worldPosition.z = -1f; // Mant�m o Z fixo para aparecer na frente
        return worldPosition;
    }
}
