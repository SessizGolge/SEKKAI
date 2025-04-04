using System.Collections;
using UnityEngine;
using TMPro;

public class NinjaStarController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Health healthComponent;
    [SerializeField] private Transform crosshair;
    [SerializeField] private GameObject ninjaStarPrefab;
    [SerializeField] private TMP_Text starCountText;
    [SerializeField] private Transform starSpawnPoint;
    [SerializeField] private float throwSpeed = 10f;
    [SerializeField] private float starCooldown = 0.3f;
    [SerializeField] private int maxStars = 5;
    [SerializeField] private float starReloadCooldown = 2f;
    [SerializeField] private float starDestroyTime = 1f;

    private int starCount;
    private bool isStarHolding = true;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        starCount = maxStars;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(StarRegeneration());
    }

    void Update()
    {
        bool isDead = healthComponent.IsDead;

        if (!isDead)
        {
            starCountText.text = starCount.ToString();
            isStarHolding = player.isStarHolding;

            if (isStarHolding)
            {
                if (player == null || crosshair == null) return;

                bool shouldFaceRight = crosshair.position.x > player.transform.position.x;

                if (shouldFaceRight != facingRight)
                {
                    facingRight = shouldFaceRight;
                }

                // Yıldızın karakterin yanında durması
                float xOffset = facingRight ? 0.3f : -0.3f;
                transform.position = player.transform.position + new Vector3(xOffset, -0.2f, 0);

                Vector3 direction = (crosshair.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);

                if (Input.GetMouseButtonDown(0) && starCount > 0 && spriteRenderer.enabled)
                {
                    ThrowStar();
                }
            }
            else 
            {
                spriteRenderer.enabled = false; // Yıldız saklanıyor
            }
        }
        else 
        {
            spriteRenderer.enabled = false; // Ölüm durumunda yıldız görünmez
        }
    }

    void ThrowStar()
    {
        // Yıldızı görünmez yap
        spriteRenderer.enabled = false;

        GameObject newStar = Instantiate(ninjaStarPrefab, starSpawnPoint.position, Quaternion.identity);
        newStar.GetComponent<Rigidbody2D>().velocity = (crosshair.position - starSpawnPoint.position).normalized * throwSpeed;
        newStar.transform.rotation = Quaternion.LookRotation(Vector3.forward, crosshair.position - starSpawnPoint.position);
        starCount--;

        Destroy(newStar, starDestroyTime);
        StartCoroutine(ReturnStar());
    }

    IEnumerator ReturnStar()
    {
        // Yıldız atıldıktan sonra kısa bir bekleme süresi
        yield return new WaitForSeconds(starCooldown);

        // Yıldız geri geldiğinde, oyuncunun hala yıldızı tutup tutmadığını kontrol et
        if (starCount <= 0)
        {
            spriteRenderer.enabled = false; // Yıldız yoksa görünmez
        }
        else
        {
            spriteRenderer.enabled = true; // Yıldız varsa görünür
        }
    }

    IEnumerator StarRegeneration()
    {
        // Yıldızların zamanla yenilenmesini sağla
        while (true)
        {
            yield return new WaitForSeconds(starReloadCooldown);

            if (starCount < maxStars)
            {
                starCount++;

                // Eğer oyuncu hala yıldızı tutuyorsa, görünürlük aç
                if (starCount > 0 && player.isStarHolding) 
                {
                    spriteRenderer.enabled = true;
                }
            }
        }
    }
}
