using UnityEngine;

public class FollowWorldObjectUIOverlay : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset;
    public Camera worldCamera;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 worldPos = target.position + worldOffset;
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);
        rectTransform.position = screenPos;
    }
}
