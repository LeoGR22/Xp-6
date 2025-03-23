using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;

    public void loadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        float startTime = Time.time;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            slider.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        float elapsedTime = Time.time - startTime;
        if (elapsedTime < 1f)
            yield return new WaitForSeconds(1f - elapsedTime);

        slider.value = 1f;
        operation.allowSceneActivation = true;
    }
}
