using UnityEngine;
using System.IO;

public class LoadLevelData : MonoBehaviour
{
    // Referências para os ScriptableObjects
    public SetTime timerData;       // ScriptableObject que armazena o tempo
    public SetBoardSize widthData;     // ScriptableObject que armazena a altura
    public SetBoardSize heightData;       // ScriptableObject que armazena a largura
    public SetObjectives redObjective;
    public SetObjectives orangeObjective;
    public SetObjectives greenObjective;
    public SetObjectives violetObjective;
    public SetLevel setLevel;
    // O nível atual que o jogador está (você pode pegar do seu código)
    public int levelInt;

    // Caminho do arquivo JSON
    private string jsonFilePath = "Assets/FasesDados/fases.json";  // Coloque o caminho correto do seu arquivo JSON

    // Método chamado pelo botão na Unity
    public void LoadData()
    {
        levelInt = setLevel.GetLevel();

        // Chama a função para carregar os dados do JSON
        LoadLevelDataFromJson();
    }

    void LoadLevelDataFromJson()
    {
        // Verifica se o arquivo JSON existe
        if (File.Exists(jsonFilePath))
        {
            // Lê o conteúdo do arquivo JSON
            string jsonString = File.ReadAllText(jsonFilePath);

            // Converte o JSON para a classe LevelList
            LevelList levelList = JsonUtility.FromJson<LevelList>(jsonString);

            // Busca o nível baseado no levelInt (o nível atual do jogador)
            LevelInfo level = System.Array.Find(levelList.levels, l => l.Level == levelInt);

            if (level != null)
            {
                // Atualiza os ScriptableObjects com os dados do nível encontrado
                timerData.SetterTime(level.Timer);
                heightData.SetHeigth(level.Altura);
                widthData.SetWidth(level.Largura);
                greenObjective.SetGreen(level.Green);
                orangeObjective.SetOrange(level.Orange);
                redObjective.SetRed(level.Red);
                violetObjective.SetViolet(level.Violet);

                // Exibe uma mensagem no console para confirmar que os dados foram carregados
                Debug.Log("Dados do nível " + levelInt + " carregados com sucesso!");
            }
            else
            {
                // Se não encontrar o nível, exibe uma mensagem de erro
                Debug.LogError("Nível " + levelInt + " não encontrado no JSON!");
            }
        }
        else
        {
            // Se o arquivo JSON não for encontrado, exibe um erro
            Debug.LogError("Arquivo JSON não encontrado!");
        }
    }
}

[System.Serializable]
public class LevelInfo
{
    public int Level;  // Corrigir para 'Level' (com L maiúsculo)
    public float Timer;
    public int Altura;
    public int Largura;
    public int Green;
    public int Orange;
    public int Red;
    public int Violet;
}

// Classe para armazenar a lista de níveis
[System.Serializable]
public class LevelList
{
    public LevelInfo[] levels;
}
