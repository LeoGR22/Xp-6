using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoardSizeData", menuName = "BoardSizeData")]
public class BoardSizeData : ScriptableObject
{
    public int size;

    public void SetSize(int value)
    {
        size = value;
    }

    public int GetSize() { return size; }
}
