using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDisplayMenu : MonoBehaviour
{
    public GameObject settingsMenu;

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }
}
