using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSettingsData : MonoBehaviour
{
    public GameObject settingsMenu;
    public SettingsPanel settingsPanel;

    void Start()
    {
        SettingsPanel sp = settingsMenu.GetComponent<SettingsPanel>();

        float volumeMusic = sp.GetMusicSlider().value;
        sp.GetAudioMixer().SetFloat("music", Mathf.Log10(volumeMusic) * 20);
        PlayerPrefs.SetFloat("musicVolume", volumeMusic);

        float volumeSFX = sp.GetSFXSlider().value;
        sp.GetAudioMixer().SetFloat("sfx", Mathf.Log10(volumeSFX) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volumeSFX);

        PlayerPrefs.Save();

        settingsMenu.SetActive(false);
    }
}
