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

    public void AddPlayerMonitorTexture(Sprite newTexture)
    {
        List<Sprite> tempList = new List<Sprite>(playerMonitorSprites);
        tempList.Add(newTexture);
        playerMonitorSprites = tempList.ToArray();
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
