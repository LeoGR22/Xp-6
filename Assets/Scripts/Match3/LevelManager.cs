using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private List<GameObject> prefabs;
    public LevelData levelData;

    void Start()
    {
        level = levelData.level;
        spawnBoard(level);
    }

    
    void Update()
    {
        
    }

    void spawnBoard(int level)
    {
        if (level >= 0 && level < prefabs.Count)
        {
            Instantiate(prefabs[level]);
        }
        else
        {
            Instantiate(prefabs[0]);
        }
    }
}
