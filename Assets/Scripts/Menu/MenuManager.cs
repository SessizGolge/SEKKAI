using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject statsMenu;
    [SerializeField] GameObject resetConfirmMenu;
    [SerializeField] GameObject statsMenuMain;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip bgMusic, selectSFX, chooseSFX, startGameSFX;
    [SerializeField] TMP_Text PRWaveValue, PRKillValue, coinValue;
    [SerializeField] int PRWave;
    [SerializeField] int PRKill;
    [SerializeField] int coin;
    public float masterVol;
    public float musicVol;
    public float SFXVol;

    void Start()
    {
        Cursor.visible = true;
        Screen.SetResolution(1600, 1200, true);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);

        PRWave = PlayerPrefs.GetInt("PR_WavesCompleted", 0);
        PRKill = PlayerPrefs.GetInt("PR_KilledEnemies", 0);
        masterVol = PlayerPrefs.GetFloat("SavedMasterVolume", 100);
        musicVol = PlayerPrefs.GetFloat("SavedMusicVolume", 50);
        SFXVol = PlayerPrefs.GetFloat("SavedSFXVolume", 50);
        coin = PlayerPrefs.GetInt("coin", 0);
    }

    public void StartGame() 
    {
        audioSource.PlayOneShot(startGameSFX);
        StartCoroutine(StartGameSoundWait());
    }

    IEnumerator StartGameSoundWait() 
    {
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator ExitSoundWait() 
    {
        yield return new WaitForSeconds(0.7f);
        Application.Quit();
        Debug.Log("Oyun kapatılıyor...");
    }

    public void OpenOptionsMenu() 
    {
        audioSource.PlayOneShot(chooseSFX);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu() 
    {
        audioSource.PlayOneShot(selectSFX);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void OpenStatsMenu() 
    {
        PRWaveValue.text = PRWave.ToString();
        PRKillValue.text = PRKill.ToString();
        coinValue.text = coin.ToString();
        audioSource.PlayOneShot(chooseSFX);
        statsMenu.SetActive(true);
        statsMenuMain.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseStatsMenu() 
    {
        audioSource.PlayOneShot(selectSFX);
        mainMenu.SetActive(true);
        statsMenuMain.SetActive(false);
        statsMenu.SetActive(false);
        resetConfirmMenu.SetActive(false);
        statsMenuMain.SetActive(false);
    }

    public void ExitGame() 
    {
        audioSource.PlayOneShot(selectSFX);
        StartCoroutine(ExitSoundWait());
    }

    public void OpenConfirmResetUI() 
    {
        audioSource.PlayOneShot(chooseSFX);
        resetConfirmMenu.SetActive(true);
        statsMenuMain.SetActive(false);
    }

    public void CloseConfirmResetUI() 
    {
        audioSource.PlayOneShot(selectSFX);
        resetConfirmMenu.SetActive(false);
        statsMenuMain.SetActive(true);
    }

    public void ResetStats() 
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("SavedMasterVolume", masterVol);
        PlayerPrefs.SetFloat("SavedMusicVolume", musicVol);
        PlayerPrefs.SetFloat("SavedSFXVolume", SFXVol);
        resetConfirmMenu.SetActive(false);
        statsMenuMain.SetActive(false);
        statsMenu.SetActive(false);
        audioSource.PlayOneShot(chooseSFX);
        Start();
    }
}
