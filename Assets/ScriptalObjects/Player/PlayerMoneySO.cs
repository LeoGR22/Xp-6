using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoneySO", menuName = "MoneySO")]
public class PlayerMoneySO : ScriptableObject
{
    public int value;

    public void ChangeMoney(int change)
    {
        value += change;
    }

    public int GetMoney()
    {
        return value;
    }
}
