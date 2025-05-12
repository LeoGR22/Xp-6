using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using SimpleJSON;

public class GoogleSheetsImporter : MonoBehaviour
{
    private string sheetID = "1fdBGsJ__-eEnmXuChiOZY1I7QIFapqmR2mPxwKnWiA4";
    private string sheetURL => $"https://docs.google.com/spreadsheets/d/{sheetID}/gviz/tq?tqx=out:json";
    private string savePath => Path.Combine(Application.dataPath, "StreamingAssets/fases.json");

    public void DownloadData()
    {
        StartCoroutine(FetchDataFromGoogleSheets());
    }

    private IEnumerator FetchDataFromGoogleSheets()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(sheetURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;

                // Limpa o JSON removendo o cabeçalho do Google Sheets
                rawJson = rawJson.Substring(rawJson.IndexOf('{'));
                rawJson = rawJson.TrimEnd(')');

                ProcessJson(rawJson);
            }
            else
            {
                Debug.LogError("Erro ao baixar os dados: " + request.error);
            }
        }
    }

    private void ProcessJson(string rawJson)
    {
        var json = JSON.Parse(rawJson);
        var rows = json["table"]["rows"].AsArray;

        JSONArray cleanData = new JSONArray();

        foreach (JSONNode row in rows)
        {
            var cells = row["c"].AsArray;
            if (cells.Count < 9) continue;

            JSONObject levelData = new JSONObject
            {
                ["Level"] = cells[0]?["v"] ?? 0,
                ["Tipo"] = cells[1]?["v"] ?? "",
                ["Timer"] = cells[2]?["v"] ?? 0,
                ["Altura"] = cells[3]?["v"] ?? 0,
                ["Largura"] = cells[4]?["v"] ?? 0,
                ["Green"] = cells[5]?["v"] ?? 0,
                ["Orange"] = cells[6]?["v"] ?? 0,
                ["Red"] = cells[7]?["v"] ?? 0,
                ["Violet"] = cells[8]?["v"] ?? 0,
                ["Blue"] = cells[9]?["v"] ?? 0
            };

            cleanData.Add(levelData);
        }

        // Criando o objeto final que envolve o array
        JSONObject finalJson = new JSONObject();
        finalJson["levels"] = cleanData;

        SaveJsonToFile(finalJson.ToString(2)); // Agora está no formato correto
    }

    private void SaveJsonToFile(string json)
    {
        string folderPath = Path.Combine(Application.dataPath, "StreamingAssets");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.WriteAllText(savePath, json);
        Debug.Log("Dados limpos salvos em: " + savePath);
    }
}
