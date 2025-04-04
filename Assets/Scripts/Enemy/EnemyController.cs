using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] Health healthComponent; // Health Script
    [SerializeField] int damage = 20;
    [SerializeField] float speed = 3f;
    [SerializeField] float detectionRange = 6f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 1.5f;

    [Header("Patrol Settings")]
    [SerializeField] float patrolDistance = 3f;
    [SerializeField] float patrolSpeed = 1.5f;

    [Header("References")]
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform kagenariVFX;
    private Vector2 patrolStartPoint;

    [SerializeField] bool isAttacking = false;
    private bool facingRight = true;
    private Transform closestPlayer;
    [SerializeField] PlayerController player;
    [SerializeField] GameObject shadow;
    [SerializeField] bool isPatrolling = false; // Yeni bir flag ekledik
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip knife;
    public Image enemyHealthValue;
    public Coroutine enemyHealthbarCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        patrolStartPoint = rb.position;
        healthComponent.OnDeath.AddListener(OnDeath); // Health Script'ten ölüm event'ini dinle
        StartCoroutine(Patrol());
    }

    void Update()
    {
        bool isDead = healthComponent.IsDead;

        if (isDead || isAttacking) return; // Eğer ölü veya saldırıyorsa, hiçbir şey yapma

        FindClosestPlayer(); // En yakın oyuncuyu bul

        if (closestPlayer != null && Vector2.Distance(transform.position, closestPlayer.position) <= detectionRange)
        {
            if (Vector2.Distance(transform.position, closestPlayer.position) <= attackRange)
            {
                if (!isDead)  // Oyuncu öldüyse saldırma
                {
                    StartCoroutine(Attack()); // Saldırı başlat
                }
            }
            else
            {
                MoveTowardsPlayer(); // Oyuncuya doğru hareket et
            }
        }
        else if (closestPlayer == null || Vector2.Distance(transform.position, closestPlayer.position) > detectionRange)
        {
            if (!isPatrolling)
            {
                rb.velocity = Vector2.zero;  // Hızı sıfırla
                StartCoroutine(Patrol());
                isPatrolling = true;
            }
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Tüm oyuncuları bul
        float closestDistance = Mathf.Infinity;
        closestPlayer = null;

        foreach (GameObject p in players)
        {
            float distance = Vector2.Distance(transform.position, p.transform.position);
            Health playerHealth = p.GetComponent<Health>();

            if (playerHealth != null && !playerHealth.IsDead)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = p.transform;
                }
            }
        }

        if (closestPlayer == null || Vector2.Distance(transform.position, closestPlayer.position) > detectionRange)
        {
            if (!isPatrolling) 
            {
                rb.velocity = Vector2.zero; // Hızı sıfırla
                StartCoroutine(Patrol());
                isPatrolling = true;
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (closestPlayer == null) 
        {
            anim.ResetTrigger("Attack");
            UpdateAnimations(0, 0);
            StartCoroutine(Patrol());
            return;
        }

        Vector2 direction = (closestPlayer.position - transform.position).normalized;
        rb.velocity = direction * speed;

        facingRight = direction.x > 0;
        UpdateAnimations(direction.x, direction.y);
    }

    IEnumerator Patrol()
    {
        if (isPatrolling) yield break; // Zaten patrol modundaysa tekrar başlama

        isPatrolling = true; // Patrol başlatıldı

        while (!healthComponent.IsDead && !isAttacking) // Eğer ölüyse veya saldırıyorsa patrol iptal edilir
        {
            Vector2 patrolTarget = patrolStartPoint + Random.insideUnitCircle * patrolDistance;

            Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
            rb.velocity = direction * patrolSpeed;
            facingRight = direction.x > 0;
            UpdateAnimations(direction.x, direction.y);

            while (Vector2.Distance(transform.position, patrolTarget) > 0.1f)
            {
                if (closestPlayer != null && Vector2.Distance(transform.position, closestPlayer.position) <= detectionRange)
                {
                    isPatrolling = false;
                    yield break; // Patrol'i durdur
                }
                yield return null;
            }

            rb.velocity = Vector2.zero;
            UpdateAnimations(0, 0);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }

        isPatrolling = false; // Patrol tamamlandı, yeniden çağrılabilir.
    }

    public void TakeDamage(GameObject source, int count)
    {
        bool isDead = healthComponent.IsDead;

        healthComponent.GetHit(count, source, gameObject); // Health Scripti ile hasar al

        // Sağlık çubuğunu güncelle
        float targetFill = (float)healthComponent.currentHealth / healthComponent.maxHealth;
        StartCoroutine(UpdateEnemyHealthBar(targetFill, 0.5f)); // Yavaşça güncelle (0.5 saniye süreyle)

        if (isDead)
        {
            UpdateAnimations(0, 0); // Öldüğünde animasyonları sıfırla
            StartCoroutine(HandleDeath()); // Ölme sürecini başlat
        }
    }


    public void UpdateAnimations(float moveX, float moveY)
    {
        bool isDead = healthComponent.IsDead;
        bool isWalking = moveX != 0 || moveY != 0;

        if (isDead)
        {
            anim.ResetTrigger("KagenariAttack");
            anim.SetBool("KagenariWalk", false);
            anim.SetBool("KagenariStand", false);
        }
        else
        {
            anim.ResetTrigger("KagenariAttack");
            kagenariVFX.transform.rotation = Quaternion.Euler(0, facingRight ? 0 : -180, 0); // Yüz yönünü değiştir
            anim.SetBool("KagenariWalk", isWalking);
            anim.SetBool("KagenariStand", !isWalking);
        }
    }


    public IEnumerator HandleDeath()
    {
        shadow.SetActive(false);
        anim.speed = 0.5f;
        anim.SetTrigger("KagenariDied");
        yield return new WaitForSeconds(0.7f); // Ölme animasyonu süresi
        Destroy(gameObject); // Düşmanı yok et
    }

    IEnumerator Attack()
    {
        bool isDead = healthComponent.IsDead;

        if (isDead) yield break; // Eğer ölü ise saldırıyı iptal et

        isAttacking = true;
        anim.SetTrigger("KagenariAttack");
        yield return new WaitForSeconds(0.5f);

        if (closestPlayer != null && !closestPlayer.GetComponent<Health>().IsDead)
        {
            Health playerHealth = closestPlayer.GetComponent<Health>();
            playerHealth.GetHit(damage, gameObject, closestPlayer.gameObject); // Oyuncuya hasar ver
            audioSource.PlayOneShot(knife);
        }

        yield return new WaitForSeconds(attackCooldown - 0.5f);
        
        isAttacking = false;
        anim.ResetTrigger("KagenariAttack");

        // Eğer saldırılan oyuncu öldüyse patrol başlat
        if (closestPlayer == null || closestPlayer.GetComponent<Health>().IsDead)
        {
            anim.ResetTrigger("KagenariAttack");
            UpdateAnimations(0, 0);
            StartCoroutine(Patrol());
        }
        else
        {
            // Eğer oyuncu hala hayatta ama detection range dışına çıktıysa patrol'e dön
            float distanceToPlayer = Vector2.Distance(transform.position, closestPlayer.position);
            if (distanceToPlayer > detectionRange + 1f) // Algılama mesafesinden biraz daha fazla mesafeye çıkarsa
            {
                anim.ResetTrigger("KagenariAttack");
                UpdateAnimations(0, 0);
                StartCoroutine(Patrol());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var starDamage = closestPlayer.GetComponent<PlayerController>().weapons.starDamage;
        if (collision.CompareTag("AkaiStar"))
        {
            TakeDamage(closestPlayer.gameObject, starDamage);
        }
    }

    // Ölüm event'ini dinleyen metot
    void OnDeath(GameObject sender)
    {
        // Ölüm anında sağlık çubuğunu sıfırla (smooth)
        StartCoroutine(UpdateEnemyHealthBar(0f, 0.5f)); // 0.5 saniyede sıfırlama

        // Ölüm anında yapılacak işlemler
        StopAllCoroutines();
        StartCoroutine(HandleDeath());
    }


    IEnumerator UpdateEnemyHealthBar(float targetFill, float duration = 0.2f)
    {
        float startFill = enemyHealthValue.fillAmount;
        float time = 0f;

        // Yumuşak geçiş işlemi
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            enemyHealthValue.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        // Son değeri ayarlayalım
        enemyHealthValue.fillAmount = targetFill;
    }
}
