using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public int level;

    public int GetLevel()
    {
        return level;
    }

    public void ChangeLevel(int num)
    {
        level = num;
    }
}