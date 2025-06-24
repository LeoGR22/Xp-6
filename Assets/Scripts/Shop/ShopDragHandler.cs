using UnityEngine;
using UnityEngine.EventSystems;

public class ShopDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Vector2 touchStartPos;
    private bool isDragging;
    private float dragThreshold = 250f; 
    private float maxDragAngle = 45f; 

    private Animator shopAnimator;

    void Start()
    {
        shopAnimator = GetComponent<Animator>();
        if (shopAnimator == null)
        {
            Debug.LogError("Animator não encontrado no painel da loja!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartPos = eventData.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !IsShopOpen()) return;

        Vector2 currentPos = eventData.position;
        Vector2 dragVector = currentPos - touchStartPos;

        float dragDistance = dragVector.magnitude;
        float dragAngle = Vector2.Angle(dragVector, Vector2.down);

        if (dragDistance > dragThreshold && dragAngle < maxDragAngle)
        {
            CloseShop();
            isDragging = false;
        }
    }

    public void CloseShop()
    {
        if (!IsShopOpen()) return;

        shopAnimator.Play("CloseShop");
    }

    public void OpenShop()
    {
        shopAnimator.Play("OpenShop");
        AudioManager.Instance.PlaySFX("OpenShop"); 
    }

    public bool IsShopOpen()
    {
        if (shopAnimator == null) return false;

        AnimatorStateInfo stateInfo = shopAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("OpenShop") || stateInfo.IsName("OpenShopIdle");
    }
}