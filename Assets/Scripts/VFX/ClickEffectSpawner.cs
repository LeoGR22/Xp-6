using UnityEngine;

public class ClickEffectSpawner : MonoBehaviour
{
    public GameObject clickEffectPrefab; 
    public RectTransform canvasTransform; 
    public Camera uiCamera; 

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SpawnEffect(Input.GetTouch(0).position);
        }
    }

    void SpawnEffect(Vector2 screenPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            screenPosition,
            uiCamera,
            out localPoint
        );

        GameObject effect = Instantiate(clickEffectPrefab, canvasTransform);
        RectTransform effectTransform = effect.GetComponent<RectTransform>();

        if (effectTransform != null)
        {
            effectTransform.anchoredPosition3D = new Vector3(localPoint.x, localPoint.y, -31f); 
            effectTransform.localScale = Vector3.one; 
            effectTransform.SetAsLastSibling();
        }
    }
}
