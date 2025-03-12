using System.Collections;
using UnityEngine;

public class KagenariController : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 1.5f;

    [SerializeField] GameObject player;
    private Animator anim;
    private bool isAttacking = false;
    private bool facingRight = true;
    private bool isWalking = false;

    void Start()
    {
        // Player'ı bul
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
            }
            else
            {
                Debug.LogError("Player bulunamadı! Player objesinin tag'ı 'Player' mı?");
            }
        }

        // Animator'ı bul
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator bulunamadı! Doğru objeye eklediğinden emin ol.");
        }
    }

    void Update()
    {
        if (player == null || anim == null) return; // İkisinden biri yoksa devam etme!

        if (!isAttacking)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(Attack());
            }
            else if (distanceToPlayer <= detectionRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                StopMoving();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        isWalking = true;
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        // Karakterin yönünü belirle
        if (direction.x > 0 && !facingRight)
        {
            Flip(true);
        }
        else if (direction.x < 0 && facingRight)
        {
            Flip(false);
        }

        // Yürüme animasyonlarını oynat
        if (facingRight)
        {
            anim.SetBool("KagenariRightWalk", true);
            anim.SetBool("KagenariLeftWalk", false);
        }
        else
        {
            anim.SetBool("KagenariLeftWalk", true);
            anim.SetBool("KagenariRightWalk", false);
        }
    }

    void StopMoving()
    {
        isWalking = false;
        anim.SetBool("KagenariRightWalk", false);
        anim.SetBool("KagenariLeftWalk", false);

        if (facingRight)
        {
            anim.SetBool("KagenariRight", true);
            anim.SetBool("KagenariLeft", false);
        }
        else
        {
            anim.SetBool("KagenariLeft", true);
            anim.SetBool("KagenariRight", false);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        isWalking = false;

        anim.SetBool("KagenariRightWalk", false);
        anim.SetBool("KagenariLeftWalk", false);
        anim.SetBool("KagenariRight", false);
        anim.SetBool("KagenariLeft", false);

        // Saldırı animasyonu yönüne göre oynat
        if (facingRight)
        {
            anim.SetBool("KagenariAttackRight", true);
            Debug.Log("AttackRight animasyonu tetiklendi!");
        }
        else
        {
            anim.SetBool("KagenariAttackLeft", true);
            Debug.Log("AttackLeft animasyonu tetiklendi!");
        }


        yield return new WaitForSeconds(attackCooldown);  // Attack cooldown'ı
        isAttacking = false;
    }

    void Flip(bool faceRight)
    {
        facingRight = faceRight;
    }
}
