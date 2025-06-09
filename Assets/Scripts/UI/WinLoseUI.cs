using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseUI : MonoBehaviour
{
    public GameObject winUI;
    public GameObject loseUI;

    public BooleanSO isTuto;

    public Animator fadeUI;

    public GameObject conffetiPrefab;

    private GameObject potionBoard;

    private void Start()
    {
        winUI.SetActive(false);
        //loseUI.SetActive(false);
    }

    public void OpenWinUI()
    {
        winUI.SetActive(true);
        AudioManager.Instance.PlaySFX("Win");
        winUI.transform.localScale = Vector3.zero;
        winUI.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        potionBoard = GameObject.FindGameObjectWithTag("Board");
        potionBoard.GetComponent<PotionBoard>().WinGameBool(true);
    }

    public void CloseWinUI()
    {
        winUI.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            winUI.SetActive(false);
        });
    }

    public void BackToSetup()
    {
        StartCoroutine(GoTo("Game"));
    }
    public void NextLevel()
    {
        if(!isTuto.value)
            StartCoroutine(GoTo("Match3"));
    }

    private IEnumerator GoTo(string sceneName)
    {
        fadeUI.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    public void OpenLoseUI()
    {
        AudioManager.Instance.PlaySFX("Fail");
        loseUI.SetActive(true);
        loseUI.transform.localScale = Vector3.zero;
        loseUI.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void CloseLoseUI()
    {
        loseUI.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            loseUI.SetActive(false);
        });
    }
}
