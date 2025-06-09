using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private TMP_Text levelText;
    public LevelData levelData;

    void Start()
    {
        level = levelData.level;
        SpawnBoard(level);
        levelText.text = $"Level {levelData.level}";
    }

    void SpawnBoard(int level)
    {
        if (prefabs != null)
        {
            Instantiate(prefabs[0]);
        }
    }
}
