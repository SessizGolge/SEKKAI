using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip bgMusic, selectSFX, chooseSFX, startGameSFX;

    void Update()
    {
        int waves = PlayerPrefs.GetInt("WavesCompleted", 0);
        int killed = PlayerPrefs.GetInt("KilledEnemies", 0);

        waveText.text = waves.ToString();
        enemiesKilledText.text = killed.ToString();
    }

    public void RetryButton() 
    {
        SceneManager.LoadScene("GameScene");
    }

    public void MenuButton() 
    {
        SceneManager.LoadScene("MenuScene");
    }
}

