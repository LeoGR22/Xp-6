using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public ObjectiveBoardData violetCountData;
    public ObjectiveBoardData greenCountData;
    public ObjectiveBoardData redCountData;

    [SerializeField] private TextMeshProUGUI violetCount;
    [SerializeField] private TextMeshProUGUI greenCount;
    [SerializeField] private TextMeshProUGUI redCount;
  

    void Update()
    {
        violetCount.text = "x " + violetCountData.count.ToString();
        greenCount.text = "x " + greenCountData.count.ToString();
        redCount.text = "x " + redCountData.count.ToString();
    }
}
