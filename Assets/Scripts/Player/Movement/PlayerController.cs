using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ------------------------------
// ---- Ayar Grupları -----------
// ------------------------------

[System.Serializable]
public class MovementSettings
{
    public float speed = 5f;
    public float runSpeed = 7.5f;
    public float dashSpeed = 10f;
    // public float maxX = 6.5f;
    // public float maxY = 5f;
}

[System.Serializable]
public class StaminaSettings
{
    public float stamina = 5f;
    public float maxStamina = 5f;
    public float regenRate = 1f;
    public float dashCost = 2f;
    public float sprintCost = 1f;
    public float sprintDuration = 0f; // Cooldown gibi
}

[System.Serializable]
public class CursedEnergySettings 
{
    public float cursedEnergy = 0;
    public float maxCursedEnergy = 100;
}

[System.Serializable]
public class WeaponObjects
{
    public GameObject sword;
    public GameObject ninjaStar;
    [SerializeField] public int starDamage = 25;
    [SerializeField] public int attackDamage = 20;
    public GameObject crosshair;
}

[System.Serializable]
public class UIElements
{
    public Sprite dashLeft;
    public Sprite dashRight;

    public Image UIBar;
    public Image staminaValue;
    public Image healthValue;
    public Image cursedEnergyBar;
    public Image cursedEnergyValue;
    public TMP_Text healthNumber, staminaNumber, cursedEnergyNumber, coinNumber, killStateValue, waveStateValue;
}

[System.Serializable]
public class SFX 
{
    public AudioSource audioSource;
    public AudioClip hitSFX, dashSFX;
}

public class PlayerController : MonoBehaviour
{
    // --------------------------
    [Header("Player Settings")]
    public MovementSettings movement;
    public StaminaSettings staminaSettings;
    public CursedEnergySettings cursedEnergySettings;
    public WeaponObjects weapons;
    public UIElements uiElements;
    public SFX sfx;

    // --------------------------
    [Header("Player States")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject playerVFX;
    [SerializeField] GameObject gameOverScreen;
    
    // [SerializeField] ObjectController objectController;
    [SerializeField] Animator anim;
    [SerializeField] Health healthComponent;
    private Vector2 direction;
    public bool isStarHolding = false;
    public bool isSwordHolding = false;
    public bool isCursed = false;
    private bool facingRight = true;
    private bool isDashing = false;
    private bool isSprinting = false;
    public string dieAnim = "AkaiDied";
    public string cursedDieAnim = "AkaiCursedDied";
    private Coroutine healthBarCoroutine, staminaBarCoroutine, cursedEnergyCoroutine;
    [SerializeField] private float slowDuration = 0.5f;
    [SerializeField] private float slowMultiplier = 0.5f;
    private bool isSlowed = false;
    private float originalSpeed;
    public bool shouldMove = true;
    
    
    void Start()
    {
        gameOverScreen.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        healthComponent = GetComponent<Health>();
        
        weapons.sword.GetComponent<SpriteRenderer>().enabled = false;
        weapons.ninjaStar.GetComponent<SpriteRenderer>().enabled = false;
        weapons.crosshair.SetActive(true);

        UpdateStaminaBar();
        UpdateHealthBar();
    }

    void Update()
    {
        bool isDead = healthComponent.IsDead;

        if (!isDead && shouldMove) // && !objectController.isOpened
        {
            if (isCursed) 
            {
                uiElements.cursedEnergyBar.gameObject.SetActive(true);
                UpdateCursedEnergyBar();
            }
            else 
            {
                uiElements.cursedEnergyBar.gameObject.SetActive(false);
            }

            UpdateCoins();
            UpdateStates();
            UpdateHealthBar();
            HandleMovementInput();
            HandleWeaponSwitching();
            HandleSprint();
            HandleDash();
            RegenerateStamina();
        }
        else if (isDead) {uiElements.UIBar.gameObject.SetActive(false);}
    }

    private void UpdateStates()
    {
        uiElements.killStateValue.text = PlayerPrefs.GetInt("KilledEnemies").ToString();
        uiElements.waveStateValue.text = PlayerPrefs.GetInt("WavesCompleted").ToString();
    }

    public void TakeDamage(GameObject source, int damage)
    {
        healthComponent.GetHit(damage, source, gameObject);
        sfx.audioSource.PlayOneShot(sfx.hitSFX);

        if (!isSlowed)
        {
            StartCoroutine(SlowDown());
        }

        // Ölüm gerçekleştiğinde animasyon tetiklenebilir
        if (healthComponent.IsDead)
        {
            anim.SetTrigger("Die");
        }
    }

    private IEnumerator SlowDown()
    {
        isSlowed = true;

        originalSpeed = movement.speed; // veya hangi değişkenle karakteri hareket ettiriyorsan
        movement.speed *= slowMultiplier;

        yield return new WaitForSeconds(slowDuration);

        movement.speed = originalSpeed;
        isSlowed = false;
    }


    public void HandItemSwitch() 
    {
        if (isSwordHolding && !isStarHolding) 
        {
            weapons.sword.GetComponent<SpriteRenderer>().enabled = true;
            isSwordHolding = true;
            weapons.ninjaStar.GetComponent<SpriteRenderer>().enabled = false;
            isStarHolding = false;
        }
        if (isStarHolding && !isSwordHolding) 
        {
            weapons.sword.GetComponent<SpriteRenderer>().enabled = false;
            isSwordHolding = false;
            weapons.ninjaStar.GetComponent<SpriteRenderer>().enabled = true;
            isStarHolding = true;
        }
    }

    void FixedUpdate()
    {
        bool isDead = healthComponent.IsDead;

        if (!isDead && !isDashing) 
        {
            Vector2 newPosition = rb.position + direction * movement.speed * Time.fixedDeltaTime;
            // newPosition.x = Mathf.Clamp(newPosition.x, -movement.maxX, movement.maxX);
            // newPosition.y = Mathf.Clamp(newPosition.y, -movement.maxY, movement.maxY);
            rb.MovePosition(newPosition);
        }
    }

    void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(moveHorizontal, moveVertical).normalized;
        if (shouldMove) 
        {
            if (moveHorizontal > 0) facingRight = true;
            else if (moveHorizontal < 0) facingRight = false;
        }
        
        UpdateAnimations(moveHorizontal, moveVertical);
    }

    void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            isSwordHolding = true;
            isStarHolding = false;
            HandItemSwitch();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            isSwordHolding = false;
            isStarHolding = true;
            HandItemSwitch();
        }
    }

    void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && staminaSettings.stamina > staminaSettings.sprintDuration && shouldMove)
        {
            isSprinting = true;
            movement.speed = movement.runSpeed;
            staminaSettings.stamina -= staminaSettings.sprintCost * Time.deltaTime;
            staminaSettings.stamina = Mathf.Clamp(staminaSettings.stamina, 0f, staminaSettings.maxStamina);
            anim.speed = 1.5f;

            if (staminaSettings.stamina <= 0) 
            {
                staminaSettings.sprintDuration = 2f;
            }
            else 
            {
                staminaSettings.sprintDuration = 0f;
            }
            UpdateStaminaBar();
        }
        else
        {
            isSprinting = false;
            movement.speed = 5f;
            anim.speed = 1f;
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && staminaSettings.stamina >= staminaSettings.dashCost && !isDashing && shouldMove)
        {
            StartCoroutine(Dash());
        }
    }

    void RegenerateStamina()
    {
        if (!isSprinting && !isDashing)
        {
            staminaSettings.stamina += staminaSettings.regenRate * Time.deltaTime;
            staminaSettings.stamina = Mathf.Clamp(staminaSettings.stamina, 0f, staminaSettings.maxStamina);
            UpdateStaminaBar();
        }
    }

    public void UpdateAnimations(float moveX, float moveY)
    {
        bool isDead = healthComponent.IsDead;
        bool isWalking = moveX != 0 || moveY != 0;

        if (isDead) 
        {
            anim.SetBool("AkaiWalk", false);
            anim.SetBool("AkaiStand", false);
        }

        if (facingRight && !isDashing)
        {
            playerVFX.transform.rotation = Quaternion.Euler(0, 0, 0);
            anim.SetBool("AkaiWalk", isWalking);
            anim.SetBool("AkaiStand", !isWalking);
        }
        else if (!facingRight)
        {
            playerVFX.transform.rotation = Quaternion.Euler(0, -180, 0);
            anim.SetBool("AkaiStand", !isWalking);
            anim.SetBool("AkaiWalk", isWalking);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        staminaSettings.stamina -= staminaSettings.dashCost;
        staminaSettings.stamina = Mathf.Clamp(staminaSettings.stamina, 0f, staminaSettings.maxStamina);
        sfx.audioSource.PlayOneShot(sfx.dashSFX);
        UpdateStaminaBar();

        Vector2 dashDirection = direction == Vector2.zero ? (facingRight ? Vector2.right : Vector2.left) : direction;
        rb.velocity = dashDirection * movement.dashSpeed;

        SpriteRenderer spriteRenderer = playerVFX.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = facingRight ? uiElements.dashRight : uiElements.dashLeft;

        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        rb.velocity = Vector2.zero;
    }

    public IEnumerator HandleDeath()
    {
        anim.speed = 0.5f;

        if (!isCursed)
        {
            anim.SetTrigger("AkaiDied");
        }
        else
        {
            anim.SetTrigger("AkaiCursedDied");
        }
        
        weapons.crosshair.SetActive(false);
        Cursor.visible = true;
        gameOverScreen.SetActive(true);
        yield return new WaitForSeconds(0.7f); // Ölme animasyonunun süresine göre ayarla
        Destroy(gameObject);
    }

    IEnumerator SmoothHealthBar(float targetFill, float duration = 0.2f)
    {
        float startFill = uiElements.healthValue.fillAmount;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            uiElements.healthValue.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        uiElements.healthValue.fillAmount = targetFill; // Son değeri sabitle
    }

    void UpdateHealthBar()
    {
        if (uiElements.healthValue != null)
        {
            float targetFill = (float)healthComponent.currentHealth / (float)healthComponent.maxHealth;

            if (healthBarCoroutine != null)
            {
                StopCoroutine(healthBarCoroutine);
            }

            healthBarCoroutine = StartCoroutine(SmoothHealthBar(targetFill));
        }

        if (uiElements.healthNumber != null)
        {
            uiElements.healthNumber.text = healthComponent.currentHealth.ToString();
        }
    }


    IEnumerator SmoothStaminaBar(float targetFill, float duration = 0.2f)
    {
        float startFill = uiElements.staminaValue.fillAmount;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            uiElements.staminaValue.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        uiElements.staminaValue.fillAmount = targetFill; // Son değeri sabitle
    }


    void UpdateStaminaBar()
    {
        if (uiElements.staminaValue != null)
        {
            float targetFill = staminaSettings.stamina / staminaSettings.maxStamina;

            if (staminaBarCoroutine != null)
            {
                StopCoroutine(staminaBarCoroutine);
            }

            staminaBarCoroutine = StartCoroutine(SmoothStaminaBar(targetFill));
        }

        if (uiElements.staminaNumber != null)
        {
            uiElements.staminaNumber.text = Mathf.RoundToInt(staminaSettings.stamina).ToString();
        }
    }

    IEnumerator SmoothCursedEnergyBar(float targetFill, float duration = 0.2f)
    {
        float startFill = uiElements.cursedEnergyValue.fillAmount;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            uiElements.cursedEnergyValue.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        uiElements.cursedEnergyValue.fillAmount = targetFill;
    }

    void UpdateCursedEnergyBar()
    {
        if (uiElements.cursedEnergyValue != null)
        {
            float targetFill = cursedEnergySettings.cursedEnergy / cursedEnergySettings.maxCursedEnergy;

            if (cursedEnergyCoroutine != null)
            {
                StopCoroutine(cursedEnergyCoroutine);
            }

            cursedEnergyCoroutine = StartCoroutine(SmoothCursedEnergyBar(targetFill));
        }

        if (uiElements.cursedEnergyNumber != null)
        {
            uiElements.cursedEnergyNumber.text = cursedEnergySettings.cursedEnergy.ToString();
        }
    }
    void UpdateCoins() 
    {
        uiElements.coinNumber.text = PlayerPrefs.GetInt("coin").ToString();
    }
}
