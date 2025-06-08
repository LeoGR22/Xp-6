using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    [SerializeField] private BooleanSO isTutorial;
    [SerializeField] private LevelData levelData;
    [SerializeField] private PlayerMoneySO playerMoney;
    [SerializeField] private FloatSO tutoarialPart;
    void Start()
    {
        if(isTutorial !=  null) 
            isTutorial.value = true;
        if(levelData != null)
            levelData.level = 0;
        if (playerMoney != null)
            playerMoney.value = 0;
        if (tutoarialPart != null)
            tutoarialPart.value = 1;
    }
}
