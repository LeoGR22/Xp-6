using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoardSize : MonoBehaviour
{
    public BoardSizeData width;
    public BoardSizeData heigth;

    public void SetWidth(int value)
    {
        width.size = value;
    }

    public void SetHeigth(int value)
    {
        heigth.size = value;
    }
}
