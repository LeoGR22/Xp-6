using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowPrints : MonoBehaviour
{
    public TMP_Text debugTextUI; // Para mensagens de log
    public TMP_Text fpsTextUI;   // Para exibir o FPS

    private Queue<string> logQueue = new Queue<string>();
    public int maxLines = 10;

    private float deltaTime = 0.0f;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (fpsTextUI != null)
        {
            float fps = 1.0f / deltaTime;
            fpsTextUI.text = $"FPS: {Mathf.Ceil(fps)}";
        }

        if (debugTextUI != null)
        {
            debugTextUI.text = string.Join("\n", logQueue.ToArray());
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (debugTextUI == null)
            return;

        if (logQueue.Count >= maxLines)
            logQueue.Dequeue();

        logQueue.Enqueue(logString);
    }
}
