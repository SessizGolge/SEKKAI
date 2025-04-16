using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip bgMusic, selectSFX, chooseSFX, startGameSFX;

    void Start()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);    
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

    public void OpenOptionsMenu() 
    {
        audioSource.PlayOneShot(chooseSFX);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu() 
    {
        audioSource.PlayOneShot(chooseSFX);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void ExitGame() 
    {
        audioSource.PlayOneShot(chooseSFX);
        Application.Quit();
        Debug.Log("Oyun kapatılıyor...");
    }
}
