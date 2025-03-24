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
        SpawnBoard(level);
    }

    void SpawnBoard(int level)
    {
        if (prefabs != null)
        {
            Instantiate(prefabs[0]);
        }
    }
}
