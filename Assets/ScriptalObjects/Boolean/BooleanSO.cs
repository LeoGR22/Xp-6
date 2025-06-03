using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBool", menuName = "BoolSO")]
public class BooleanSO : ScriptableObject
{
   public bool value;

    public void ChangeBool(bool v)
    {
        value = v;
    }
}
