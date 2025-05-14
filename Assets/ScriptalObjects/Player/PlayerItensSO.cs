using System;
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
    [Header("Mousepads")]
    public Sprite[] playerMousepadSprites;
    public Sprite currentMousepad;

    [Space(10)]
    [Header("Cups")]
    public Sprite[] playerCupSprites;
    public Sprite currentCup;

    [Space(10)]
    [Header("Candles")]
    public Sprite[] playerCandleSprites;
    public Sprite currentCandle;

    [Space(10)]
    [Header("WallDecors")]
    public Sprite[] playerWallDecorSprites;
    public Sprite currentWallDecor;

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
                    Array.Resize(ref playerMonitorSprites, playerMonitorSprites.Length + 1);
                    playerMonitorSprites[playerMonitorSprites.Length - 1] = newTexture;
                }
                break;

            case "Keyboard":
                if (!new List<Sprite>(playerKeyboardSprites).Contains(newTexture))
                {
                    Array.Resize(ref playerKeyboardSprites, playerKeyboardSprites.Length + 1);
                    playerKeyboardSprites[playerKeyboardSprites.Length - 1] = newTexture;
                }
                break;

            case "Mouse":
                if (!new List<Sprite>(playerMouseSprites).Contains(newTexture))
                {
                    Array.Resize(ref playerMouseSprites, playerMouseSprites.Length + 1);
                    playerMouseSprites[playerMouseSprites.Length - 1] = newTexture;
                }
                break;

            case "Mousepad":
                if (!new List<Sprite>(playerMousepadSprites).Contains(newTexture))
                {
                    Array.Resize(ref playerMousepadSprites, playerMousepadSprites.Length + 1);
                    playerMousepadSprites[playerMousepadSprites.Length - 1] = newTexture;
                }
                break;

            case "Cup":
                if (!new List<Sprite>(playerCupSprites).Contains(newTexture))
                {
                    Array.Resize(ref playerCupSprites, playerCupSprites.Length + 1);
                    playerCupSprites[playerCupSprites.Length - 1] = newTexture;
                }
                break;

            case "Candle":
                if (!new List<Sprite>(playerCandleSprites).Contains(newTexture))
                {
                    Array.Resize(ref playerCandleSprites, playerCandleSprites.Length + 1);
                    playerCandleSprites[playerCandleSprites.Length - 1] = newTexture;
                }
                break;

            case "WallDecor":
                if (!new List<Sprite>(playerWallDecorSprites).Contains(newTexture))
                {
                    Array.Resize(ref playerWallDecorSprites, playerWallDecorSprites.Length + 1);
                    playerWallDecorSprites[playerWallDecorSprites.Length - 1] = newTexture;
                }
                break;

            default:
                Debug.LogWarning("Tipo de item inválido: " + itemType);
                break;
        }
    }

    public void SetCurrentSprite(string name, Sprite sprite)
    {
        switch (name)
        {
            case "Monitor":
                currentMonitor = sprite;
                break;
            case "Keyboard":
                currentKeyboard = sprite;
                break;
            case "Mouse":
                currentMouse = sprite;
                break;
            case "Mousepad":
                currentMousepad = sprite;
                break;
            case "Cup":
                currentCup = sprite;
                break;
            case "Candle":
                currentCandle = sprite;
                break;
            case "WallDecor":
                currentWallDecor = sprite;
                break;
        }

        // Opcional: Salvamento (descomente se quiser usar)
        /*
        PlayerPrefs.SetString($"Current{name}", sprite ? sprite.name : "");
        PlayerPrefs.Save();
        */
    }

    public bool IsSpriteInCategory(string name, Sprite sprite)
    {
        switch (name)
        {
            case "Monitor":
                foreach (var s in playerMonitorSprites)
                {
                    if (s == sprite)
                        return true;
                }
                break;
            case "Keyboard":
                foreach (var s in playerKeyboardSprites)
                {
                    if (s == sprite)
                        return true;
                }
                break;
            case "Mouse":
                foreach (var s in playerMouseSprites)
                {
                    if (s == sprite)
                        return true;
                }
                break;
            case "Mousepad":
                foreach (var s in playerMousepadSprites)
                {
                    if (s == sprite)
                        return true;
                }
                break;
            case "Cup":
                foreach (var s in playerCupSprites)
                {
                    if (s == sprite)
                        return true;
                }
                break;
            case "Candle":
                foreach (var s in playerCandleSprites) 
                {
                    if (s == sprite)
                        return true;
                }
                break;
            case "WallDecor":
                foreach (var s in playerWallDecorSprites) 
                {
                    if (s == sprite)
                        return true;
                }
                break;
        }
        return false;
    }

    public Sprite ReturnMonitorTexture()
    {
        return currentMonitor != null ? currentMonitor : empty;
    }

    public Sprite ReturnKeyboardTexture()
    {
        return currentKeyboard != null ? currentKeyboard : empty;
    }

    public Sprite ReturnMouseTexture()
    {
        return currentMouse != null ? currentMouse : empty;
    }

    public Sprite ReturnMousepadTexture()
    {
        return currentMousepad != null ? currentMousepad : empty;
    }

    public Sprite ReturnCupTexture()
    {
        return currentCup != null ? currentCup : empty;
    }

    public Sprite ReturnCandleTexture()
    {
        return currentCandle != null ? currentCandle : empty;
    }

    public Sprite ReturnWallDecorTexture()
    {
        return currentWallDecor != null ? currentWallDecor : empty;
    }

    // Opcional: Carregamento (descomente se quiser usar)
    /*
    public void LoadPlayerItems()
    {
        string[] categories = { "Monitor", "Keyboard", "Mouse", "Mousepad", "Cup", "Candle", "WallDecor" };
        foreach (var category in categories)
        {
            string spriteName = PlayerPrefs.GetString($"Current{category}", "");
            if (!string.IsNullOrEmpty(spriteName))
            {
                Sprite sprite = null;
                switch (category)
                {
                    case "Monitor":
                        sprite = playerMonitorSprites.FirstOrDefault(s => s.name == spriteName);
                        currentMonitor = sprite;
                        break;
                    case "Keyboard":
                        sprite = playerKeyboardSprites.FirstOrDefault(s => s.name == spriteName);
                        currentKeyboard = sprite;
                        break;
                    case "Mouse":
                        sprite = playerMouseSprites.FirstOrDefault(s => s.name == spriteName);
                        currentMouse = sprite;
                        break;
                    case "Mousepad":
                        sprite = playerMousepadSprites.FirstOrDefault(s => s.name == spriteName);
                        currentMousepad = sprite;
                        break;
                    case "Cup":
                        sprite = playerCupSprites.FirstOrDefault(s => s.name == spriteName);
                        currentCup = sprite;
                        break;
                    case "Candle":
                        sprite = playerCandleSprites.FirstOrDefault(s => s.name == spriteName);
                        currentCandle = sprite;
                        break;
                    case "WallDecor":
                        sprite = playerWallDecorSprites.FirstOrDefault(s => s.name == spriteName);
                        currentWallDecor = sprite;
                        break;
                }
            }
        }
    }
    */
}