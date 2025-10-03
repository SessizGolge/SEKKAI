using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("AudioSources")]
    public AudioSource musicSource; // Örnek: BGM
    public AudioSource sfxSource;  // Örnek: Buton, efektler vs.

    [Header("Audio Clips")]
    public List<AudioClip> audioClips; // Tüm sesleri ekle

    private Dictionary<Sound, AudioClip> clipDictionary;

    public enum Sound
    {
        SelectSFX,
        ChooseSFX,
        CoinSFX,
        MenuMusic,
        StartGameSFX,
        PurchaseSFX,
        HitSFX,
        GameOver,
        NinjaStarSFX,
        NormalMessageSFX,
        ErrorMessageSFX,
        SwordSFX,
        KnifeSFX,
        ExplodeSFX,
        BloodSFX,
        Dash,
        GameMusic
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        clipDictionary = new Dictionary<Sound, AudioClip>();

        foreach (var clip in audioClips)
        {
            if (clip != null)
            {
                // Enum adı ile eşleştiriyoruz
                if (System.Enum.TryParse(clip.name, out Sound soundEnum))
                {
                    if (!clipDictionary.ContainsKey(soundEnum))
                        clipDictionary.Add(soundEnum, clip);
                }
            }
        }
    }

    // Ses çalma fonksiyonu
    public void Play(Sound sound, AudioSource source = null, float volume = 1f)
    {
        if (clipDictionary.TryGetValue(sound, out AudioClip clip))
        {
            if (source != null && clip != null)
                source.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning("SoundManager: Clip not found -> " + sound.ToString());
        }
    }
}
