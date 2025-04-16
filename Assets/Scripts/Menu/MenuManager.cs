using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;

    void Start()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);    
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenOptionsMenu() 
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu() 
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void ExitGame() 
    {
        Application.Quit();
        Debug.Log("Oyun kapatılıyor...");
    }
}
