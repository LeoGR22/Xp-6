using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLevel : MonoBehaviour
{
   public LevelData levelData;

    public void PassLevel()
    {
        Debug.Log("PassLevel chamado!");
        levelData.level += 1;
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        playerManager.SavePlayer();
        print("SALVOUUUUUUUUUUU");
    }

    public int GetLevel()
    {
        return levelData.level;
    }

    public void SetterLevel(int level)
    {
        levelData.level = level;
    }
}
