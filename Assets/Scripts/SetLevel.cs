using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLevel : MonoBehaviour
{
   public LevelData levelData;

    public void PassLevel(int level)
    {
        levelData.level = level;
    }
}
