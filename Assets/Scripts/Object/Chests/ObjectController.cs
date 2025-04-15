using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public bool isOpened = false; // Sandığın açık mı olduğunu kontrol etmek için
    [SerializeField] private GameObject player; // Oyuncu
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerInteract playerInteractComponent;
    [SerializeField] private GameObject chestUI; // UI paneli
    [SerializeField] private KeyCode interactKey; // Etkileşim tuşu
    [SerializeField] private float interactionDistance = 2f; // Etkileşim mesafesi
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip openSFX;
    [SerializeField] Sprite closed;
    [SerializeField] Sprite opened;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isOpened = false;
        interactKey = player.GetComponent<PlayerInteract>().interactKey;
        // Başlangıçta UI kapalı olmalı
        chestUI.SetActive(false);
    }

    void Update()
    {
        // Debug.Log("Player mesafesi: " + Vector2.Distance(transform.position, player.transform.position));
        if (Vector2.Distance(transform.position, player.transform.position) <= interactionDistance) 
        {
            if (isOpened) 
            {
                isOpened = true;
                OpenChestUI();
                Debug.Log("Sandık açıldı!");
            }
            else
            {
                isOpened = false;
                CloseChestUI();
                Debug.Log("Sandık kapantıldı!");
            }
        }
    }

    // Sandığı açmak için UI'yi göster
    public void OpenChestUI()
    {
        spriteRenderer.sprite = opened;
        audioSource.PlayOneShot(openSFX);
        chestUI.SetActive(true);
        Cursor.visible = true;
    }

    public void CloseChestUI()
    {
        spriteRenderer.sprite = closed;
        chestUI.SetActive(false);
        Cursor.visible = false;
    }
}
