using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyConfirmation : MonoBehaviour
{
    public Image targetImage;

    public void SetSprite(Sprite sprite)
    {
        targetImage.preserveAspect = true;
        targetImage.sprite = sprite;
    }
}
