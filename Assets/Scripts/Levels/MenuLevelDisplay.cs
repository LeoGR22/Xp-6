using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuLevelDisplay : MonoBehaviour
{
    public LevelData LevelData;

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        text.text = LevelData.GetLevel().ToString();
    }
}
