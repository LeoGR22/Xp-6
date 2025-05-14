using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItensSO", menuName = "ItensSO")]
public class PlayerItensSO : ScriptableObject
{
    [Header("Monitores")]
    public Sprite[] playerMonitorSprites; // Sprites de monitores possuídos
    public Sprite currentMonitor;

    [Header("Teclados")]
    public Sprite[] playerKeyboardSprites;
    public Sprite currentKeyboard;

    [Header("Mouses")]
    public Sprite[] playerMouseSprites;
    public Sprite currentMouse;

    [Header("Mousepads")]
    public Sprite[] playerMousepadSprites;
    public Sprite currentMousepad;

    [Header("Cups")]
    public Sprite[] playerCupSprites;
    public Sprite currentCup;

    [Header("Candles")]
    public Sprite[] playerCandleSprites;
    public Sprite currentCandle;

    [Header("WallDecors")]
    public Sprite[] playerWallDecorSprites;
    public Sprite currentWallDecor;

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
    }

    public bool IsSpriteInCategory(string name, Sprite sprite)
    {
        switch (name)
        {
            case "Monitor":
                return new List<Sprite>(playerMonitorSprites).Contains(sprite);
            case "Keyboard":
                return new List<Sprite>(playerKeyboardSprites).Contains(sprite);
            case "Mouse":
                return new List<Sprite>(playerMouseSprites).Contains(sprite);
            case "Mousepad":
                return new List<Sprite>(playerMousepadSprites).Contains(sprite);
            case "Cup":
                return new List<Sprite>(playerCupSprites).Contains(sprite);
            case "Candle":
                return new List<Sprite>(playerCandleSprites).Contains(sprite);
            case "WallDecor":
                return new List<Sprite>(playerWallDecorSprites).Contains(sprite);
            default:
                Debug.LogWarning("Categoria inválida: " + name);
                return false;
        }
    }

    public Sprite ReturnMonitorTexture() => currentMonitor != null ? currentMonitor : empty;
    public Sprite ReturnKeyboardTexture() => currentKeyboard != null ? currentKeyboard : empty;
    public Sprite ReturnMouseTexture() => currentMouse != null ? currentMouse : empty;
    public Sprite ReturnMousepadTexture() => currentMousepad != null ? currentMousepad : empty;
    public Sprite ReturnCupTexture() => currentCup != null ? currentCup : empty;
    public Sprite ReturnCandleTexture() => currentCandle != null ? currentCandle : empty;
    public Sprite ReturnWallDecorTexture() => currentWallDecor != null ? currentWallDecor : empty;
}