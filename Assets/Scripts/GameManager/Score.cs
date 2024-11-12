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
    public ObjectiveBoardData orangeData;

    [SerializeField] private TextMeshProUGUI violetCount;
    [SerializeField] private TextMeshProUGUI greenCount;
    [SerializeField] private TextMeshProUGUI redCount;
    [SerializeField] private TextMeshProUGUI orangeCount;
  

    void Update()
    {
        violetCount.text = "x " + violetCountData.count.ToString();
        greenCount.text = "x " + greenCountData.count.ToString();
        redCount.text = "x " + redCountData.count.ToString();
        orangeCount.text = "x " + orangeData.count.ToString();
    }
}
