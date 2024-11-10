using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectiveCount", menuName = "ObjectiveBoardData")]
public class ObjectiveBoardData : ScriptableObject
{
    public int count;

    public void SetCount(int value)
    {
        count = value;
    }
}
