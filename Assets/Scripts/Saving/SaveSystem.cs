using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SavePath = Application.persistentDataPath + "/playerV2.json";

    public static void SavePlayer(PlayerManager manager)
    {
        string path = SavePath;
        PlayerData data = new PlayerData(manager);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        // Forçar sincronização no Android
#if UNITY_ANDROID
        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            stream.Flush();
            stream.Close();
        }
#endif
        Debug.Log($"Jogador salvo em: {path}, Conteúdo: {json}");
    }

    public static PlayerData LoadPlayer(PlayerManager manager)
    {
        string path = SavePath;
        Debug.Log($"Tentando carregar save de: {path}, Existe: {File.Exists(path)}");

        if (File.Exists(path))
        {
            return null;
        }
        else
        {
            Debug.Log("Arquivo de save não encontrado. Criando novo save com valores padrão.");
            PlayerData newData = new PlayerData
            {
                money = 0,
                level = 0,
                isTutorial = true,
                tutorialPart = 1f,
                ownedItems = new PlayerData.OwnedItemCategory[]
                {
                    new PlayerData.OwnedItemCategory { category = "Monitor", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "Keyboard", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "Mouse", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "Mousepad", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "Cup", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "Candle", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "WallDecor", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "Mic", sprites = new string[0] },
                    new PlayerData.OwnedItemCategory { category = "Headset", sprites = new string[0] }
                },
                currentItems = new PlayerData.CurrentItem[]
                {
                    new PlayerData.CurrentItem { category = "Monitor", sprite = null },
                    new PlayerData.CurrentItem { category = "Keyboard", sprite = null },
                    new PlayerData.CurrentItem { category = "Mouse", sprite = null },
                    new PlayerData.CurrentItem { category = "Mousepad", sprite = null },
                    new PlayerData.CurrentItem { category = "Cup", sprite = null },
                    new PlayerData.CurrentItem { category = "Candle", sprite = null },
                    new PlayerData.CurrentItem { category = "WallDecor", sprite = null },
                    new PlayerData.CurrentItem { category = "Mic", sprite = null },
                    new PlayerData.CurrentItem { category = "Headset", sprite = null }
                }
            };

            string json = JsonUtility.ToJson(newData, true);
            File.WriteAllText(path, json);
            // Forçar sincronização no Android
#if UNITY_ANDROID
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Flush();
                stream.Close();
            }
#endif
            Debug.Log($"Novo save criado em: {path}, Conteúdo: {json}");
            return newData;
        }
    }

    public static void ClearSave()
    {
        string path = SavePath;
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Save anterior excluído: {path}");
        }
    }
}