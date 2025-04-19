using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Health player;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip bgMusic, selectSFX, chooseSFX, startGameSFX, gameOver;
    public bool isGameOver = true;
    public bool isDead;

    void Update()
    {
        isDead = player.IsDead;
        
        if (isDead && isGameOver) 
        {
            audioSource.PlayOneShot(gameOver);
            musicSource.Stop();
            isGameOver = false;
        }
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

