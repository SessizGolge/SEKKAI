using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] public int currentHealth, maxHealth;

    // Bu event'leri dışarıdan dinleyebiliriz (Animasyonlar, efektler vs.)
    public UnityEvent<GameObject> OnHit, OnDeath;

    [SerializeField] public bool IsDead = false;
    [SerializeField] public bool IsCursed = false;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] public AudioClip dieSFX, hitSFX;
    [SerializeField] public GameObject blood;

    // Sağlık başlatma
    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        IsDead = false;
    }
    void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }   
    }

    // Can kaybetme işlemi
    public void GetHit(int amount, GameObject sender, GameObject target)
    {
        audioSource = target.GetComponent<AudioSource>();
        if (IsDead)
            return;

        if (sender.layer == gameObject.layer)
            return;

        currentHealth -= amount;

        if (!IsDead) 
        {
            // Darbe ses efekti
            if (hitSFX != null)
            {
                audioSource.PlayOneShot(hitSFX);
            }

            // Kan efektini oluştur
            GameObject spawnedBlood = Instantiate(blood, transform.position, Quaternion.identity);
            StartCoroutine(DestroyBlood(spawnedBlood));
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            IsDead = true;
            OnDeath?.Invoke(sender);
            Die(target);
        }
        else
        {
            OnHit?.Invoke(sender);
        }
    }

    // Instantiate edilen kanı yok eden coroutine
    IEnumerator DestroyBlood(GameObject spawnedBlood)
    {
        yield return new WaitForSeconds(0.35f);
        Destroy(spawnedBlood);
    }

    // Ölüm işlemi
    public void Die(GameObject target)
    {
        audioSource = target.GetComponent<AudioSource>();
        if (IsDead)
        {
            if (target == null)
            {
                return;
            }

            if (CompareTag("Player")) 
            {
                PlayerController targetComponent = target.GetComponent<PlayerController>();
                if (targetComponent != null)
                {
                    targetComponent.UpdateAnimations(0, 0);
                    StartCoroutine(targetComponent.HandleDeath());
                }

                // Ölüm ses efekti
                if (dieSFX != null)
                {
                    audioSource.PlayOneShot(dieSFX);
                }
            }

            if (CompareTag("Enemy")) 
            {
                EnemyController targetComponent = target.GetComponent<EnemyController>();
                if (targetComponent != null)
                {
                    targetComponent.UpdateAnimations(0, 0);
                    StartCoroutine(targetComponent.HandleDeath());
                }

                // Ölüm ses efekti
                if (dieSFX != null)
                {
                    audioSource.PlayOneShot(dieSFX);
                }
            }
        }
    }



    // UI güncellemeleri için bir metot, örneğin sağlık çubuğunu güncellemek için
    public void UpdateHealthBar(UIElements uiElements)
    {
        if (uiElements.healthValue != null)
        {
            uiElements.healthValue.fillAmount = currentHealth / maxHealth;
        }
    }
}
