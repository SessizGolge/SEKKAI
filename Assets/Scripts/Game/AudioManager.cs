using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour 
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;

    public void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("SavedMasterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("SavedMusicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SavedSFXVolume");
    }

    public void SetMasterVolume(float _value) 
    {
        if(_value < 1) 
        {
            _value = .001f;
        }

        RefreshMasterSlider(_value);
        PlayerPrefs.SetFloat("SavedMasterVolume", _value);
        audioMixer.SetFloat("Master", Mathf.Log10(_value / 100) * 20f);
    }

    public void SetMusicVolume(float _value) 
    {
        if(_value < 1) 
        {
            _value = .001f;
        }

        RefreshMusicSlider(_value);
        PlayerPrefs.SetFloat("SavedMusicVolume", _value);
        audioMixer.SetFloat("Music", Mathf.Log10(_value / 100) * 20f);
    }

    public void SetSFXVolume(float _value) 
    {
        if(_value < 1) 
        {
            _value = .001f;
        }

        RefreshSFXSlider(_value);
        PlayerPrefs.SetFloat("SavedSFXVolume", _value);
        audioMixer.SetFloat("SFX", Mathf.Log10(_value / 100) * 20f);
    }

    public void SetMasterFromSlider() 
    {
        SetMasterVolume(masterSlider.value);
    }

    public void SetMusicFromSlider() 
    {
        SetMusicVolume(musicSlider.value);
    }

    public void SetSFXFromSlider() 
    {
        SetSFXVolume(SFXSlider.value);
    }

    public void RefreshMasterSlider(float _value) 
    {
        masterSlider.value = _value;
    }

    public void RefreshMusicSlider(float _value) 
    {
        musicSlider.value = _value;
    }

    public void RefreshSFXSlider(float _value) 
    {
        SFXSlider.value = _value;
    }
}