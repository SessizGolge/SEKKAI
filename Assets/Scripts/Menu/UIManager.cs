using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject playerPauseScreen;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSFX, chooseSFX;

    void Start() 
    {
        Time.timeScale = 1f;
        playerPauseScreen.SetActive(false);
    }

    public void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            playerPauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ResumeButton() 
    {
        Time.timeScale = 1f;
        playerPauseScreen.SetActive(false);
    }

    public void MenuButton() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}
