using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float time;
    public FloatSO timeData;
    public TextMeshProUGUI timerText;

    public TimerBooleanSO timerBool;

    public UnityEvent loseGame;
    public BooleanSO canLose;

    void Start()
    {
        canLose.value = true;
        time = timeData.value;
    }

    void Update()
    {
        timerText.text = time.ToString("F0");
        if (timerBool.value == true)
        {
            DecrementTimer();
        }
    }


    void DecrementTimer()
    {
        time -= 1f * Time.deltaTime;

        if (time < 0f && canLose.value == true) 
        {
            Lose();
        }
    }

    public void DecreaseMove()
    {
        time -= 1f;

        if (time <= 0f && canLose.value == true)
        {
            Lose();
        }
    }

    void Lose()
    {
        loseGame.Invoke();
    }
}
