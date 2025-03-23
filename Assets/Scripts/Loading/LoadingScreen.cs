using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public LevelLoader LevelLoader;
    public int sceneIndex;
    void Start()
    {
        LevelLoader.loadLevel(sceneIndex);
    }
}
