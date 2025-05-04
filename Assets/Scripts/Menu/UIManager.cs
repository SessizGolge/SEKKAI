using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] Health playerHealth;
    [SerializeField] GameObject playerPauseScreen;
    [SerializeField] GameObject playerOptionsScreen;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip selectSFX, chooseSFX;
    public bool isOptions;

    void Start() 
    {
        Time.timeScale = 1f;
        playerPauseScreen.SetActive(false);
        playerOptionsScreen.SetActive(false);
        isOptions = false;
    }

    public void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !playerHealth.IsDead && !isOptions)
        {
            playerPauseScreen.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void ResumeButton() 
    {
        Time.timeScale = 1f;
        playerPauseScreen.SetActive(false);
            Cursor.visible = false;
        sfxSource.PlayOneShot(selectSFX);
    }

    public void MenuButton() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
        sfxSource.PlayOneShot(chooseSFX);
    }

    public void OptionsButton() 
    {
        playerPauseScreen.SetActive(false);
        playerOptionsScreen.SetActive(true);
        Cursor.visible = true;
        isOptions = true;
        sfxSource.PlayOneShot(chooseSFX);
    }

    public void OptionsCancelButton() 
    {
        playerPauseScreen.SetActive(true);
        playerOptionsScreen.SetActive(false);
        isOptions = false;
        sfxSource.PlayOneShot(selectSFX);
    }
}
