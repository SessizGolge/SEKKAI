using System.Collections;
using System.Collections.Generic;
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
    public float maxX = 6.5f;
    public float maxY = 5f;
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
public class HealthSettings
{
    public float health = 100f;
    public float maxHealth = 100f;
}

[System.Serializable]
public class WeaponObjects
{
    public GameObject sword;
    public GameObject ninjaStar;
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
}

public class PlayerController : MonoBehaviour
{
    // --------------------------
    [Header("Player Settings")]
    public MovementSettings movement;
    public StaminaSettings staminaSettings;
    public HealthSettings healthSettings;
    public CursedEnergySettings cursedEnergySettings;
    public WeaponObjects weapons;
    public UIElements uiElements;

    // --------------------------
    [Header("Player States")]
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 direction;
    public bool isDead = false;
    public bool isStarHolding = false;
    public bool isSwordHolding = false;
    public bool isCursed = false;

    private bool facingRight = true;
    private bool isDashing = false;
    private bool isSprinting = false;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        anim = GetComponent<Animator>();

        weapons.sword.GetComponent<SpriteRenderer>().enabled = false;
        weapons.ninjaStar.GetComponent<SpriteRenderer>().enabled = false;
        weapons.crosshair.SetActive(true);

        UpdateStaminaBar();
        UpdateHealthBar();
    }

    void Update()
    {
        if (healthSettings.health <= 0) 
        {
            isDead = true;
        }

        if (!isDead) 
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

            UpdateHealthBar();
            HandleMovementInput();
            HandleWeaponSwitching();
            HandleSprint();
            HandleDash();
            RegenerateStamina();
        }
    }

    void FixedUpdate()
    {
        if (!isDead && !isDashing) 
        {
            Vector2 newPosition = rb.position + direction * movement.speed * Time.fixedDeltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, -movement.maxX, movement.maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, -movement.maxY, movement.maxY);
            rb.MovePosition(newPosition);
        }
    }

    void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        if (moveHorizontal > 0) facingRight = true;
        else if (moveHorizontal < 0) facingRight = false;

        UpdateAnimations(moveHorizontal, moveVertical);
    }

    void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            weapons.sword.GetComponent<SpriteRenderer>().enabled = true;
            isSwordHolding = true;
            weapons.ninjaStar.GetComponent<SpriteRenderer>().enabled = false;
            isStarHolding = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            weapons.sword.GetComponent<SpriteRenderer>().enabled = false;
            isSwordHolding = false;
            weapons.ninjaStar.GetComponent<SpriteRenderer>().enabled = true;
            isStarHolding = true;
        }
    }

    void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && staminaSettings.stamina > staminaSettings.sprintDuration)
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
        if (Input.GetKeyDown(KeyCode.Space) && staminaSettings.stamina >= staminaSettings.dashCost && !isDashing)
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

    void UpdateAnimations(float moveX, float moveY)
    {
        bool isWalking = moveX != 0 || moveY != 0;

        if (facingRight && !isDashing)
        {
            anim.SetBool("AkaiRight", !isWalking);
            anim.SetBool("AkaiRightWalk", isWalking);
            anim.SetBool("AkaiLeft", false);
            anim.SetBool("AkaiLeftWalk", false);
        }
        else
        {
            anim.SetBool("AkaiLeft", !isWalking);
            anim.SetBool("AkaiLeftWalk", isWalking);
            anim.SetBool("AkaiRight", false);
            anim.SetBool("AkaiRightWalk", false);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        staminaSettings.stamina -= staminaSettings.dashCost;
        staminaSettings.stamina = Mathf.Clamp(staminaSettings.stamina, 0f, staminaSettings.maxStamina);
        UpdateStaminaBar();

        Vector2 dashDirection = direction == Vector2.zero ? (facingRight ? Vector2.right : Vector2.left) : direction;
        rb.velocity = dashDirection * movement.dashSpeed;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = facingRight ? uiElements.dashRight : uiElements.dashLeft;

        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        rb.velocity = Vector2.zero;
    }

    void UpdateStaminaBar()
    {
        if (uiElements.staminaValue != null)
        {
            uiElements.staminaValue.fillAmount = staminaSettings.stamina / staminaSettings.maxStamina;
        }
    }

    void UpdateHealthBar()
    {
        if (uiElements.healthValue != null)
        {
            uiElements.healthValue.fillAmount = healthSettings.health / healthSettings.maxHealth;
        }
    }
    
    void UpdateCursedEnergyBar() 
    {
        if (uiElements.cursedEnergyValue != null)
        {
            uiElements.cursedEnergyValue.fillAmount = cursedEnergySettings.cursedEnergy / cursedEnergySettings.maxCursedEnergy;
        }
    } 
}
