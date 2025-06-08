using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingLevelManager : MonoBehaviour
{
    public float timeDelay;

    public string sceneName;

    public Animator anim;

    private void Start()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(timeDelay);
        anim.SetTrigger("FadeIn");
        AudioManager.Instance.PlaySFX("Click2");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}
