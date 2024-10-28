using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private List<GameObject> prefabs;

    void Start()
    {
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

    public void setLevel(int value)
    {
        level = value;
    }
}
