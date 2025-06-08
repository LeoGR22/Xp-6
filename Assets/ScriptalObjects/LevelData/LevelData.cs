using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public int level;

    public float GetLevel()
    {
        return level;
    }

    public void ChangeLevel(int num)
    {
        level = num;
    }
}


