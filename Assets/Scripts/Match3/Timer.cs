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

    private bool pauseTimer = false;

    void Start()
    {
        canLose.value = true;
        time = timeData.value;
    }

    void Update()
    {
        if (!pauseTimer)
        {
            timerText.text = time.ToString("F0");
            if (timerBool.value == true)
            {
                DecrementTimer();
            }
        }
    }

    public void ChangeMoves(int num)
    {
        time = num;
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
    }
    public void Reset()
    {
        hasTriggeredLose = false;
        canLose.value = true;
        time = timeData.value;
        pauseTimer = false;
    }

    private bool hasTriggeredLose = false; 

    public void CheckLose()
    {
        if (time <= 0f && canLose.value && !hasTriggeredLose)
        {
            Lose();
        }
    }

    void Lose()
    {
        hasTriggeredLose = true;
        loseGame.Invoke();
    }

    public void PauseTimer()
    {
        pauseTimer = true;
    }

    public float GetMovesLeft()
    {
        return time;
    }
}
