using UnityEngine;
using System.IO;

public class LoadLevelData : MonoBehaviour
{
    // Refer�ncias para os ScriptableObjects
    public SetTime timerData;       // ScriptableObject que armazena o tempo
    public SetBoardSize widthData;     // ScriptableObject que armazena a altura
    public SetBoardSize heightData;       // ScriptableObject que armazena a largura
    public SetObjectives redObjective;
    public SetObjectives orangeObjective;
    public SetObjectives greenObjective;
    public SetObjectives violetObjective;
    public SetLevel setLevel;
    // O n�vel atual que o jogador est� (voc� pode pegar do seu c�digo)
    public int levelInt;

    // Caminho do arquivo JSON
    private string jsonFilePath = "Assets/FasesDados/fases.json";  // Coloque o caminho correto do seu arquivo JSON

    // M�todo chamado pelo bot�o na Unity
    public void LoadData()
    {
        levelInt = setLevel.GetLevel();

        // Chama a fun��o para carregar os dados do JSON
        LoadLevelDataFromJson();
    }

    void LoadLevelDataFromJson()
    {
        // Verifica se o arquivo JSON existe
        if (File.Exists(jsonFilePath))
        {
            // L� o conte�do do arquivo JSON
            string jsonString = File.ReadAllText(jsonFilePath);

            // Converte o JSON para a classe LevelList
            LevelList levelList = JsonUtility.FromJson<LevelList>(jsonString);

            // Busca o n�vel baseado no levelInt (o n�vel atual do jogador)
            LevelInfo level = System.Array.Find(levelList.levels, l => l.Level == levelInt);

            if (level != null)
            {
                // Atualiza os ScriptableObjects com os dados do n�vel encontrado
                timerData.SetterTime(level.Timer);
                heightData.SetHeigth(level.Altura);
                widthData.SetWidth(level.Largura);
                greenObjective.SetGreen(level.Green);
                orangeObjective.SetOrange(level.Orange);
                redObjective.SetRed(level.Red);
                violetObjective.SetViolet(level.Violet);

                // Exibe uma mensagem no console para confirmar que os dados foram carregados
                Debug.Log("Dados do n�vel " + levelInt + " carregados com sucesso!");
            }
            else
            {
                // Se n�o encontrar o n�vel, exibe uma mensagem de erro
                Debug.LogError("N�vel " + levelInt + " n�o encontrado no JSON!");
            }
        }
        else
        {
            // Se o arquivo JSON n�o for encontrado, exibe um erro
            Debug.LogError("Arquivo JSON n�o encontrado!");
        }
    }
}

[System.Serializable]
public class LevelInfo
{
    public int Level;  // Corrigir para 'Level' (com L mai�sculo)
    public float Timer;
    public int Altura;
    public int Largura;
    public int Green;
    public int Orange;
    public int Red;
    public int Violet;
}

// Classe para armazenar a lista de n�veis
[System.Serializable]
public class LevelList
{
    public LevelInfo[] levels;
}
