using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float time;
    public FloatSO timeData;
    public TextMeshProUGUI timerText;



    void Start()
    {
        time = timeData.value;
    }

    void Update()
    {
        timerText.text = time.ToString("F0");
        DecrementTimer();
    }


    void DecrementTimer()
    {
        time -= 1f * Time.deltaTime;

        if (time < 0f)
        {
            Lose();
        }
    }

    void Lose()
    {
        return;
    }
}
