using UnityEngine;

public class FollowWorldObjectUIOverlay : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset;
    public Camera worldCamera;
    public bool onlyAffectY = false;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Vector3 worldPos = target.position + worldOffset;
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        if (onlyAffectY)
            rectTransform.position = new Vector3(rectTransform.position.x, screenPos.y, rectTransform.position.z);
        else
            rectTransform.position = screenPos;
    }
}
