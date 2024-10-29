using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float timeDelay = 2.0f;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(Delay(sceneName));
    }

    private IEnumerator Delay(string sceneName)
    {
        yield return new WaitForSeconds(timeDelay);
        SceneManager.LoadScene(sceneName);
    }
}
