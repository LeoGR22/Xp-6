using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PriceManager : MonoBehaviour
{
    public ItensSO itensSO;
    public PlayerItensSO playerItensSO;
    public Sprite buyButtonSprite;
    public Sprite equipButtonSprite;
    public Sprite equippedButtonSprite;

    private Sprite itemSprite;
    private string itemCategory;
    private Button button;
    private TMP_Text buttonText;
    private Image buttonImage;

    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("Componente Button n�o encontrado neste GameObject.");
            return;
        }

        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogWarning("Componente Image n�o encontrado neste GameObject.");
            return;
        }

        buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText == null)
        {
            Debug.LogWarning("Componente TMP_Text n�o encontrado nos filhos.");
            return;
        }

        Transform iconTransform = transform.parent.Find("Icon");
        if (iconTransform == null)
        {
            Debug.LogWarning("Objeto 'Icon' n�o encontrado como irm�o.");
            return;
        }

        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogWarning("Componente Image n�o encontrado no 'Icon'.");
            return;
        }

        itemSprite = iconImage.sprite;
        if (itemSprite == null)
        {
            Debug.LogWarning("Nenhum sprite atribu�do ao Image do 'Icon'.");
            return;
        }

        itemCategory = DetermineItemCategory();
        if (string.IsNullOrEmpty(itemCategory))
        {
            Debug.LogWarning("N�o foi poss�vel determinar a categoria do item. Bot�o n�o ser� inicializado.");
            return;
        }

        UpdateButtonState();

        button.onClick.AddListener(OnButtonClick);
    }

    private string DetermineItemCategory()
    {
        if (itensSO.monitors.Exists(item => item.sprite == itemSprite)) return "Monitor";
        if (itensSO.keyboards.Exists(item => item.sprite == itemSprite)) return "Keyboard";
        if (itensSO.mouses.Exists(item => item.sprite == itemSprite)) return "Mouse";
        if (itensSO.mousepads.Exists(item => item.sprite == itemSprite)) return "Mousepad";
        if (itensSO.cups.Exists(item => item.sprite == itemSprite)) return "Cup";
        if (itensSO.candles.Exists(item => item.sprite == itemSprite)) return "Candle";
        if (itensSO.wallDecors.Exists(item => item.sprite == itemSprite)) return "WallDecor";
        if (itensSO.mics.Exists(item => item.sprite == itemSprite)) return "Mic";
        if (itensSO.headsets.Exists(item => item.sprite == itemSprite)) return "Headset";
        Debug.LogWarning("N�o foi poss�vel determinar a categoria do sprite: " + itemSprite.name);
        return "";
    }

    private void UpdateButtonState()
    {
        if (playerItensSO == null || itemSprite == null || string.IsNullOrEmpty(itemCategory))
        {
            Debug.LogWarning("PlayerItensSO, itemSprite ou itemCategory n�o est�o configurados.");
            return;
        }

        bool isOwned = playerItensSO.IsSpriteInCategory(itemCategory, itemSprite);

        if (isOwned)
        {
            bool isEquipped = IsItemEquipped(itemCategory, itemSprite);

            if (isEquipped)
            {
                buttonText.text = "Equipped";
                buttonImage.sprite = equippedButtonSprite;
                button.interactable = false;
            }
            else
            {
                buttonText.text = "Equip";
                buttonImage.sprite = equipButtonSprite;
                button.interactable = true;
            }
        }
        else
        {
            int price = itensSO.GetPriceFromSprite(itemSprite);
            buttonText.text = "$" + price;
            buttonImage.sprite = buyButtonSprite;
            button.interactable = true;
        }
    }

    private bool IsItemEquipped(string category, Sprite sprite)
    {
        switch (category)
        {
            case "Monitor":
                return playerItensSO.ReturnMonitorTexture() == sprite;
            case "Keyboard":
                return playerItensSO.ReturnKeyboardTexture() == sprite;
            case "Mouse":
                return playerItensSO.ReturnMouseTexture() == sprite;
            case "Mousepad":
                return playerItensSO.ReturnMousepadTexture() == sprite;
            case "Cup":
                return playerItensSO.ReturnCupTexture() == sprite;
            case "Candle":
                return playerItensSO.ReturnCandleTexture() == sprite;
            case "WallDecor":
                return playerItensSO.ReturnWallDecorTexture() == sprite;
            case "Mic":
                return playerItensSO.ReturnMicTexture() == sprite;
            case "Headset":
                return playerItensSO.ReturnHeadsetTexture() == sprite;
            default:
                Debug.LogWarning("Categoria inv�lida: " + category);
                return false;
        }
    }

    private void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX("Click2");
        if (playerItensSO.IsSpriteInCategory(itemCategory, itemSprite))
        {
            PlayerManager playerManager = FindObjectOfType<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.ChangeCurrentSprite(itemCategory, itemSprite);
                playerManager.ApplyAllTextures();
                UpdateButtonState();
            }
        }
        else
        {
            PlayerManager playerManager = FindObjectOfType<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.VerifyItem(itemCategory, itemSprite);
            }
        }
    }

    public void RefreshButtonState()
    {
        UpdateButtonState();
    }
}