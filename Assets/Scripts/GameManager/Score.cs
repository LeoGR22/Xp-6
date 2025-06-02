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
    public ObjectiveBoardData blueData;

    [SerializeField] private TextMeshPro violetCount;
    [SerializeField] private TextMeshPro greenCount;
    [SerializeField] private TextMeshPro redCount;
    [SerializeField] private TextMeshPro orangeCount;
    [SerializeField] private TextMeshPro blueCount;


    void Update()
    {
        violetCount.text = violetCountData.count.ToString();
        greenCount.text = greenCountData.count.ToString();
        redCount.text = redCountData.count.ToString();
        orangeCount.text = orangeData.count.ToString();
        blueCount.text = blueData.count.ToString();
    }
}
