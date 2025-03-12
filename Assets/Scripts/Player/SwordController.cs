using System.Collections;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform crosshair;
    [SerializeField] private float offsetDistance = 0.3f; // Kılıcın normal duruş mesafesi
    [SerializeField] private float attackArc = 90f; // Kılıcın süpürme açısı (derece cinsinden)
    [SerializeField] private float attackSpeed = 0.15f; // Saldırı süresi

    private bool isAttacking = false;
    private bool facingRight = true;
    private Quaternion originalRotation;

    void Update()
    {
        if (player == null || crosshair == null) return;

        bool shouldFaceRight = crosshair.position.x > player.transform.position.x;

        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
        }

        if (!isAttacking)
        {
            // Kılıcı karakterin yanına sabit konumlandır
            float xOffset = facingRight ? offsetDistance : -offsetDistance;
            transform.position = player.transform.position + new Vector3(xOffset, -0.2f, 0);

            // Kılıcı mouse yönüne döndür
            Vector3 direction = (crosshair.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Mouse sol tık ile saldır
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(SlashAttack());
        }
    }

    IEnumerator SlashAttack()
    {
        isAttacking = true;
        originalRotation = transform.rotation;

        // **Saldırı hareketi (yay çizme)**
        float elapsedTime = 0;
        float startAngle = facingRight ? -attackArc / 2 : attackArc / 2;
        float endAngle = facingRight ? attackArc / 2 : -attackArc / 2;

        while (elapsedTime < attackSpeed)
        {
            float t = elapsedTime / attackSpeed;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            transform.rotation = originalRotation * Quaternion.Euler(0, 0, currentAngle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = originalRotation; // Saldırı bitince eski duruşa dön
        isAttacking = false;
    }
}
