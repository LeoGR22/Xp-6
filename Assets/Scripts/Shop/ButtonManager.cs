using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject monitorScroll;
    public GameObject keyboardScroll;
    public GameObject mouseScroll;
    public GameObject decorScroll;

    [Header("Buttons")]
    public GameObject monitorButton;
    public GameObject keyboardButton;
    public GameObject mouseButton;
    public GameObject decorButton;

    private List<GameObject> scrolls;
    private List<GameObject> buttons;

    private void Awake()
    {
        scrolls = new List<GameObject> { monitorScroll, keyboardScroll, mouseScroll, decorScroll };
        buttons = new List<GameObject> { monitorButton, keyboardButton, mouseButton, decorButton };
    }

    private void SetActiveScroll(int index)
    {
        for (int i = 0; i < scrolls.Count; i++)
        {
            scrolls[i].SetActive(i == index);
            SetChildrenActive(buttons[i], i == index);
        }
    }

    private void SetChildrenActive(GameObject parent, bool active)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    public void OpenMonitorScroll() => SetActiveScroll(0);
    public void OpenKeyboardScroll() => SetActiveScroll(1);
    public void OpenMouseScroll() => SetActiveScroll(2);
    public void OpenDecorScroll() => SetActiveScroll(3);
}
