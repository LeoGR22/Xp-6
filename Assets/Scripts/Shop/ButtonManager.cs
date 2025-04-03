using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject monitorScroll;
    public GameObject keyboardScroll;
    public GameObject mouseScroll;

    public void OpenMonitorScroll()
    {
        monitorScroll.SetActive(true);
        keyboardScroll.SetActive(false);
        mouseScroll.SetActive(false);
    }

    public void OpenKeyboardScroll()
    {
        monitorScroll.SetActive(false);
        keyboardScroll.SetActive(true);
        mouseScroll.SetActive(false);
    }

    public void OpenMouseScroll()
    {
        monitorScroll.SetActive(false);
        keyboardScroll.SetActive(false);
        mouseScroll.SetActive(true);
    }
}
