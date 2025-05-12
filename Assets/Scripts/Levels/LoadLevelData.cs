using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

public class LoadLevelData : MonoBehaviour
{
    public SetTime timerData;
    public SetBoardSize widthData;
    public SetBoardSize heightData;
    public SetObjectives redObjective;
    public SetObjectives orangeObjective;
    public SetObjectives greenObjective;
    public SetObjectives violetObjective;
    public SetObjectives blueObjective;
    public SetLevel setLevel;

    public int levelInt;

    private string jsonFileName = "fases.json";

    public void LoadData()
    {
        levelInt = setLevel.GetLevel();
        StartCoroutine(LoadLevelDataFromJson());
    }

    IEnumerator LoadLevelDataFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string jsonString = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            jsonString = www.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Erro ao carregar JSON no Android: " + www.error);
            yield break;
        }
#else
        if (File.Exists(filePath))
        {
            jsonString = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado no path: " + filePath);
            yield break;
        }
#endif

        LevelList levelList = JsonUtility.FromJson<LevelList>(jsonString);
        LevelInfo level = System.Array.Find(levelList.levels, l => l.Level == levelInt);

        if (level != null)
        {
            timerData.SetterTime(level.Timer);
            heightData.SetHeigth(level.Altura);
            widthData.SetWidth(level.Largura);
            greenObjective.SetGreen(level.Green);
            orangeObjective.SetOrange(level.Orange);
            redObjective.SetRed(level.Red);
            violetObjective.SetViolet(level.Violet);
            blueObjective.SetBlue(level.Blue);

            Debug.Log("Dados do nível " + levelInt + " carregados com sucesso!");
        }
        else
        {
            Debug.LogWarning("Nível " + levelInt + " não encontrado no JSON!");
        }
    }
}


[System.Serializable]
public class LevelInfo
{
    public int Level;  
    public float Timer;
    public int Altura;
    public int Largura;
    public int Green;
    public int Orange;
    public int Red;
    public int Violet;
    public int Blue;
}

// Classe para armazenar a lista de níveis
[System.Serializable]
public class LevelList
{
    public LevelInfo[] levels;
}
