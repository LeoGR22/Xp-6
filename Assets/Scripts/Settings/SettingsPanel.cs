using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    public GameObject settingsMenu;

    [Header("Buttons Sprites")]
    private bool _musicIsActive = true;
    [SerializeField] private Image musicButton;
    [SerializeField] private Sprite activeMusicSprite;
    [SerializeField] private Sprite deactiveMusicSprite;

    private bool _soundIsActive = true;
    [SerializeField] private Image soundButton;
    [SerializeField] private Sprite activeSoundSprite;
    [SerializeField] private Sprite deactiveSoundSprite;

    private bool _vibrateIsActive = true;
    [SerializeField] private Image vibrateButton;
    [SerializeField] private Sprite activeVibrateSprite;
    [SerializeField] private Sprite deactiveVibrateSprite;

    private float _lastMusicVolume = 1f;
    private float _lastSFXVolume = 1f;

    private void Start()
    {
        musicSlider.onValueChanged.AddListener(delegate { OnMusicSliderChanged(); });
        SFXSlider.onValueChanged.AddListener(delegate { OnSFXSliderChanged(); });

        if (PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("SFXVolume") && PlayerPrefs.HasKey("mouseSenseController"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }

        _musicIsActive = musicSlider.value > 0.0001f;
        _soundIsActive = SFXSlider.value > 0.0001f;

        UpdateMusicButtonImage();
        UpdateSoundButtonImage();
        UpdateVibrateButtonImage();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;

        if (volume <= 0.0001f)
        {
            myMixer.SetFloat("music", -80f);
        }
        else
        {
            myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;

        if (volume <= 0.0001f)
        {
            myMixer.SetFloat("sfx", -80f);
        }
        else
        {
            myMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        _lastMusicVolume = musicSlider.value;
        _lastSFXVolume = SFXSlider.value;

        SetMusicVolume();
        SetSFXVolume();
    }

    public void MusicButton()
    {
        _musicIsActive = !_musicIsActive;

        if (_musicIsActive)
        {
            musicSlider.value = _lastMusicVolume;
        }
        else
        {
            if (musicSlider.value > 0.0001f)
                _lastMusicVolume = musicSlider.value;

            musicSlider.value = 0f;
        }

        SetMusicVolume();
        UpdateMusicButtonImage();
    }

    public void SoundButton()
    {
        _soundIsActive = !_soundIsActive;

        if (_soundIsActive)
        {
            SFXSlider.value = _lastSFXVolume;
        }
        else
        {
            if (SFXSlider.value > 0.0001f)
                _lastSFXVolume = SFXSlider.value;

            SFXSlider.value = 0f;
        }

        SetSFXVolume();
        UpdateSoundButtonImage();
    }

    public void VibrateButton()
    {
        _vibrateIsActive = !_vibrateIsActive;
        UpdateVibrateButtonImage();
    }

    private void OnMusicSliderChanged()
    {
        SetMusicVolume();

        bool isNowActive = musicSlider.value > 0.0001f;
        if (isNowActive != _musicIsActive)
        {
            _musicIsActive = isNowActive;
            if (isNowActive) _lastMusicVolume = musicSlider.value;
            UpdateMusicButtonImage();
        }
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }

    private void OnSFXSliderChanged()
    {
        SetSFXVolume();

        bool isNowActive = SFXSlider.value > 0.0001f;
        if (isNowActive != _soundIsActive)
        {
            _soundIsActive = isNowActive;
            if (isNowActive) _lastSFXVolume = SFXSlider.value;
            UpdateSoundButtonImage();
        }
    }

    private void UpdateMusicButtonImage()
    {
        musicButton.sprite = _musicIsActive ? activeMusicSprite : deactiveMusicSprite;
    }

    private void UpdateSoundButtonImage()
    {
        soundButton.sprite = _soundIsActive ? activeSoundSprite : deactiveSoundSprite;
    }

    private void UpdateVibrateButtonImage()
    {
        vibrateButton.sprite = _vibrateIsActive ? activeVibrateSprite : deactiveVibrateSprite;
    }

    public AudioMixer GetAudioMixer() => myMixer;
    public Slider GetMusicSlider() => musicSlider;
    public Slider GetSFXSlider() => SFXSlider;
}
