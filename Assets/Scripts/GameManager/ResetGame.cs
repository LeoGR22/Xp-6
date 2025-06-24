using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    [SerializeField] private BooleanSO isTutorial;
    [SerializeField] private LevelData levelData;
    [SerializeField] private PlayerMoneySO playerMoney;
    [SerializeField] private FloatSO tutorialPart;
    void Start()
    {
        if (tutorialPart != null)
            tutorialPart.value = 1;
    }
}
