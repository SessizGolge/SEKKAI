using System.Collections;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    // [SerializeField] ObjectController objectController;
    [SerializeField] private Transform crosshair;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Health healthComponent;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip swordSFX;
    [SerializeField] private float offsetDistance = 0.3f; // Kılıcın normal duruş mesafesi
    [SerializeField] private float attackSpeed = 0.2f; // Saldırı süresi
    [SerializeField] private int attackDamage = 17;
    private bool attackBlocked = false;
    public bool isAttacking { get; private set; }
    private bool facingRight = true;
    public Transform circleOrigin;
    public float radius;

    public void ResetisAttacking() 
    {
        isAttacking = false;
    }

    void Update()
    {
        bool isDead = healthComponent.IsDead;

        if (!isDead) // && !objectController.isOpened
        {
            if (isAttacking) return;
            if (player == null || crosshair == null) return;

            bool shouldFaceRight = crosshair.position.x > player.transform.position.x;

            if (shouldFaceRight != facingRight)
            {
                facingRight = shouldFaceRight;
                transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1); // X ölçeğini ters çevir
            }

            if (!isAttacking)
            {
                // Kılıcı karakterin yanına sabit konumlandır
                float xOffset = facingRight ? offsetDistance : -offsetDistance;
                transform.position = player.transform.position + new Vector3(xOffset, -0.2f, 0);

                // Kılıcı mouse yönüne döndür
                Vector3 direction = (crosshair.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Eğer sola bakıyorsa açıyı ters çevir
                if (!facingRight)
                {
                    angle += 180f;
                }

                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // Mouse sol tık ile saldır
            if (Input.GetMouseButtonDown(0) && !isAttacking && spriteRenderer.enabled)
            {
                if (attackBlocked) return;
                anim.SetTrigger("Attack");
                isAttacking = true;
                attackBlocked = true;
                StartCoroutine(SlashAttack());
            }
        }
        else 
        {
            spriteRenderer.enabled = false;
        }
    }

    IEnumerator SlashAttack()
    {
        // Saldırı sonrası birkaç saniye bekle
        yield return new WaitForSeconds(attackSpeed);
        attackBlocked = false;
        audioSource.PlayOneShot(swordSFX);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    void OCollisionEnter2D(Collision2D collision)
    {
        DetectColliders();
    }
    public void DetectColliders() 
    {
        // Kılıç ile temas edilen tüm düşmanları kontrol et
        foreach (Collider2D enemy in Physics2D.OverlapCircleAll(circleOrigin.position, radius)) 
        {
            if (enemy.CompareTag("Enemy")) 
            {
                Health health;
                if (health = enemy.GetComponent<Health>()) 
                {
                    // Düşmanın sağlığını azalt
                    health.GetHit(attackDamage, player.gameObject, enemy.gameObject);
                }
            }
        }
    }
}
