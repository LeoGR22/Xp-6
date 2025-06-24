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

    public void SetMoney(int newValue)
    {
        value = Mathf.Max(0, newValue);
    }
}