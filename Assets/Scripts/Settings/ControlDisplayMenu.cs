using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class ControlDisplayMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject playMenu;

    private void Start()
    {
        if(playMenu != null)
            playMenu.SetActive(false);

        if(settingsMenu != null)
            settingsMenu.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        settingsMenu.transform.localScale = Vector3.zero;
        settingsMenu.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void CloseSettings()
    {
        settingsMenu.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            settingsMenu.SetActive(false);
        });
    }

    public void OpenPlay()
    {
        playMenu.SetActive(true);
        playMenu.transform.localScale = Vector3.zero;
        playMenu.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void ClosePlay()
    {
        playMenu.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            playMenu.SetActive(false);
        });
    }
}
