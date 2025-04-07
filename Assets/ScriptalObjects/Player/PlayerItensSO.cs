using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItensSO", menuName = "ItensSO")]
public class PlayerItensSO : ScriptableObject
{
    [Header("Monitores")]
    public Sprite[] playerMonitorSprites;
    public Sprite currentMonitor;

    [Space(10)]
    [Header("Teclados")]
    public Sprite[] playerKeyboardSprites;
    public Sprite currentKeyboard;

    [Space(10)]
    [Header("Mouses")]
    public Sprite[] playerMouseSprites;
    public Sprite currentMouse;

    [Space(10)]
    [Header("Empty")]
    public Sprite empty;

    public void AddPlayerItemTexture(string itemType, Sprite newTexture)
    {
        switch (itemType)
        {
            case "Monitor":
                if (!new List<Sprite>(playerMonitorSprites).Contains(newTexture))
                {
                    List<Sprite> tempMonitorList = new List<Sprite>(playerMonitorSprites);
                    tempMonitorList.Add(newTexture);
                    playerMonitorSprites = tempMonitorList.ToArray();
                }
                break;

            case "Keyboard":
                if (!new List<Sprite>(playerKeyboardSprites).Contains(newTexture))
                {
                    List<Sprite> tempKeyboardList = new List<Sprite>(playerKeyboardSprites);
                    tempKeyboardList.Add(newTexture);
                    playerKeyboardSprites = tempKeyboardList.ToArray();
                }
                break;

            case "Mouse":
                if (!new List<Sprite>(playerMouseSprites).Contains(newTexture))
                {
                    List<Sprite> tempMouseList = new List<Sprite>(playerMouseSprites);
                    tempMouseList.Add(newTexture);
                    playerMouseSprites = tempMouseList.ToArray();
                }
                break;

            default:
                Debug.LogWarning("Tipo de item inválido: " + itemType);
                break;
        }
    }

    public void SetCurrentSprite(string name, Sprite sprite)
    {
        if(name == "Monitor")
        {
            currentMonitor = sprite;
        }else if(name == "Keyboard")
        {
            currentKeyboard = sprite;
        }else if(name == "Mouse")
        {
            currentMouse = sprite;
        }
    }
    public bool IsSpriteInCategory(string name, Sprite sprite)
    {
        if (name == "Monitor")
        {
            foreach (var s in playerMonitorSprites)
            {
                if (s == sprite)
                    return true;
            }
        }
        else if (name == "Keyboard")
        {
            foreach (var s in playerKeyboardSprites)
            {
                if (s == sprite)
                    return true;
            }
        }
        else if (name == "Mouse")
        {
            foreach (var s in playerMouseSprites)
            {
                if (s == sprite)
                    return true;
            }
        }

        return false;
    }

    public Sprite ReturnMonitorTexture()
    {
        if(currentMonitor != null)
            return currentMonitor;
        else return empty;
    }
    public Sprite ReturnKeyboardTexture()
    {
        if (currentKeyboard != null)
            return currentKeyboard;
        else return empty;
    }
    public Sprite ReturnMouseTexture()
    {
        if (currentMouse != null)
            return currentMouse;
        else return empty;
    }
}
