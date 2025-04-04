using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaStar : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    public int starDamageValue;
    public Transform circleOrigin;
    public float radius;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip ninjaStarSFX;

    private Rigidbody2D rb;
    
    void Start()
    {
        audioSource.PlayOneShot(ninjaStarSFX);
        // Rigidbody2D bileşenini al
        rb = GetComponent<Rigidbody2D>();
        starDamageValue = playerController.weapons.starDamage;
    }

    void Update()
    {
        // Yıldız hareket ettikten sonra çarpışma kontrolünü sürekli yapmak için
        DetectColliders();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    // Yıldızın çevresindeki düşmanları algıla
    public void DetectColliders()
    {
        // Yıldızın çevresindeki düşmanları kontrol et
        foreach (Collider2D enemy in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            if (enemy.CompareTag("Enemy"))
            {

                // Düşman sağlığına saldır
                Health health;
                if (health = enemy.GetComponent<Health>())
                {
                    health.GetHit(starDamageValue, playerController.gameObject, enemy.gameObject);
                    Destroy(gameObject); // Yıldızın kendisini yok et
                }
            }
        }
    }

    // Yıldızın çarpışması
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DetectColliders();
    }
}
