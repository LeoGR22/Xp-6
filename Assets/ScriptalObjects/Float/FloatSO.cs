using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloat", menuName = "FloatSO")]
public class FloatSO : ScriptableObject
{
   public float value;

    public float GetFloat()
    {
        return value;
    }

    public void SetFloat(float var)
    {
        value = var;
    }
}
