using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform crosshair;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Health healthComponent;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip swordSFX;
    [SerializeField] private float offsetDistance = 0.3f;
    [SerializeField] private float attackSpeed = 0.2f;
    [SerializeField] private int attackDamageValue = 17;
    private bool attackBlocked = false;
    public bool isAttacking { get; private set; }
    private bool facingRight = true;
    public Transform circleOrigin;
    public float radius;

    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

    public void ResetisAttacking()
    {
        isAttacking = false;
    }

    void Update()
    {
        bool isDead = healthComponent.IsDead;
        attackDamageValue = player.weapons.attackDamage;

        if (!isDead)
        {
            if (isAttacking) return;
            if (player == null || crosshair == null) return;

            bool shouldFaceRight = crosshair.position.x > player.transform.position.x;

            if (shouldFaceRight != facingRight)
            {
                facingRight = shouldFaceRight;
                transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
            }

            if (!isAttacking)
            {
                float xOffset = facingRight ? offsetDistance : -offsetDistance;
                transform.position = player.transform.position + new Vector3(xOffset, -0.2f, 0);

                Vector3 direction = (crosshair.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (!facingRight)
                {
                    angle += 180f;
                }

                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

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
        DetectColliders(); // hasar animasyonla senkron değilse buraya
        audioSource.PlayOneShot(swordSFX);
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
        attackBlocked = false;
        damagedEnemies.Clear(); // saldırı bitince temizle
    }

    public void DetectColliders()
    {
        foreach (Collider2D enemy in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            if (enemy.CompareTag("Enemy"))
            {
                GameObject enemyObj = enemy.gameObject;
                if (!damagedEnemies.Contains(enemyObj))
                {
                    Health health = enemyObj.GetComponent<Health>();
                    if (health != null)
                    {
                        health.GetHit(attackDamageValue, player.gameObject, enemyObj);
                        damagedEnemies.Add(enemyObj);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    // Bu yanlış yazılmış, büyük ihtimalle gereksiz
    // void OCollisionEnter2D(Collision2D collision)
    // {
    //     DetectColliders();
    // }
}
