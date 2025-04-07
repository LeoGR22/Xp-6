using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowPrints : MonoBehaviour
{
    public TMP_Text debugTextUI;
    private Queue<string> logQueue = new Queue<string>();
    public int maxLines = 10;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logQueue.Count >= maxLines)
            logQueue.Dequeue();

        logQueue.Enqueue(logString);
        debugTextUI.text = string.Join("\n", logQueue.ToArray());
    }
}
