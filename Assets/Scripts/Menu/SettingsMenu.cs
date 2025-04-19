// using UnityEngine;
// using UnityEngine.UI;

// public class SettingsMenu : MonoBehaviour
// {
//     public Slider musicSlider;
//     public Slider sfxSlider;

//     void Start()
//     {
//         // Slidere mevcut ses seviyelerini yükle
//         musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
//         sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
//     }

//     public void OnMusicVolumeChanged(float volume)
//     {
//         AudioManager.Instance.SetMusicVolume(volume);
//     }

//     public void OnSFXVolumeChanged(float volume)
//     {
//         AudioManager.Instance.SetSFXVolume(volume);
//     }
// }
