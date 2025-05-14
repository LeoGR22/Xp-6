using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetupItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private string itemType; 
    [SerializeField] private ColorPickerUI colorPickerUI; 
    [SerializeField] private PlayerItensSO playerItensSO;
    [SerializeField] private Slider slider;

    private bool isTouching;
    private int pointerId = -1; 

    public void OnPointerDown(PointerEventData eventData)
    {
        Image uiImage = GetComponent<Image>();

        if (uiImage != null && uiImage.sprite != null)
        {
            if (uiImage.sprite.name == "Empty")
            {
                return;
            }
        }

        if (slider.value != 1)
        {
            AudioManager.Instance.PlaySFX(itemType);
            return;
        }

        ShopDragHandler shopHandler = FindObjectOfType<ShopDragHandler>();
        if (shopHandler != null && shopHandler.IsShopOpen()) return;

        isTouching = true;
        pointerId = eventData.pointerId;

        Sprite currentSprite = null;
        switch (itemType)
        {
            case "Monitor": currentSprite = playerItensSO.ReturnMonitorTexture(); break;
            case "Keyboard": currentSprite = playerItensSO.ReturnKeyboardTexture(); break;
            case "Mouse": currentSprite = playerItensSO.ReturnMouseTexture(); break;
            case "Mousepad": currentSprite = playerItensSO.ReturnMousepadTexture(); break;
            case "Cup": currentSprite = playerItensSO.ReturnCupTexture(); break;
            case "Candle": currentSprite = playerItensSO.ReturnCandleTexture(); break;
            case "WallDecor": currentSprite = playerItensSO.ReturnWallDecorTexture(); break;
        }

        if (currentSprite != playerItensSO.empty)
        {
            colorPickerUI.Show(itemType, currentSprite, eventData.position);
            AudioManager.Instance.PlaySFX("Click2");
#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }
        else
        {
            isTouching = false;
            pointerId = -1;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isTouching || eventData.pointerId != pointerId) return;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isTouching || eventData.pointerId != pointerId) return;

        isTouching = false;
        pointerId = -1;

        colorPickerUI.TrySelectColor(eventData);
        colorPickerUI.Hide();
    }
}