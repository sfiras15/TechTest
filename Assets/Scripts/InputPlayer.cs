using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles various action done by the player such as attacking,interacting with objects
/// Stores information about health/stamina 
/// </summary>
public class InputPlayer : MonoBehaviour
{


    [Header("References")]
    
    private Animator animator;
    private Camera cam;
    private PlayerMovement playerMovement;

    [Header("Attacking")]
    [SerializeField] private KeyCode attackKey;
    private bool attacking = false;
    private bool readyToAttack = true;

    //The size of the sphere that we are casting in the attack animation
    [SerializeField] private float hitsize = 0.12f;

    [SerializeField] private LayerMask whatIsEnemy;

    [Header("Gun")]
    [SerializeField] private KeyCode AimDownSight;
    [SerializeField] private float characterRotationSpeed = 5f;



    [Header("Using Item")]
    [SerializeField] private LayerMask whatIsItem;
    // affected by using potions atm
    private float damageMultiplier = 1f;
    // equipping weapons 
    [SerializeField] private EquipementManager equipementManager;

    // To prevent the player from attacking when he's browsing
    private bool isInventoryActive = false;

    [Header("Player's Health")]
    //[SerializeField] private Bar playerHealthBar;
    private Health playerHealth;


    [Header("Player's Stamina")]
    //[SerializeField] private Bar playerStaminaBar;
    private Stamina playerStamina;

    //Variables for stamina recovery/drain system
    [SerializeField] private float staminaRecoveryRate = 1f;
    private float lastRecovryTime;
    private bool recoveryTimerIsRunning = false;
    private bool drainTimerIsRunning = false;
    private float lastSprintTime;

    public static event Action<int,int> OnStaminaChange;
    public static event Action<int, int> OnHealthChange;






    private void Awake()
    {
        playerHealth = new Health(100);
        playerStamina = new Stamina(100);
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        cam = Camera.main;

    }
    private void OnEnable()
    {
        // Event for when the player is using a consumable
        Inventory.onConsumableUse += playerState;

        // Event for equiping/unequipping gear
        Inventory.onGearEquip += GearState;

        // Event for when the player is taking damage
        PlayerManager.onPlayerDamaged += UpdateHealth;

    }
    private void OnDisable()
    {
        Inventory.onConsumableUse -= playerState;
        PlayerManager.onPlayerDamaged -= UpdateHealth;
        Inventory.onGearEquip -= GearState;
    }

    // Boost the stats of the player when equipingg / unequipping an item
    private void GearState(Gear gear,bool equip)
    {
        Debug.Log("Gear + " + gear);
        Debug.Log("equip " + equip);
        if (equip)
        {
            damageMultiplier += gear.damageBoost / 100f;
            Debug.Log(damageMultiplier);
            playerHealth.SetMaxHealth = playerHealth.GetMaxHealth + gear.healthBoost;    
            OnHealthChange?.Invoke(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);

            playerMovement.IncreaseSpeed(playerMovement.GetSpeedMultiplier + gear.speedBoost / 100f);
        }
        else
        {
            damageMultiplier -= gear.damageBoost / 100f;
            playerHealth.SetMaxHealth = playerHealth.GetMaxHealth - gear.healthBoost;
            OnHealthChange?.Invoke(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);

            playerMovement.IncreaseSpeed(playerMovement.GetSpeedMultiplier - gear.speedBoost / 100f);
        }
    }

    private void Start()
    {
        //Update the stamina/healthbar UI
        OnHealthChange?.Invoke(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);
        OnStaminaChange?.Invoke(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
        //PlayerManager.instance.AddXp(0);

    }
    private void UpdateHealth(float damage)
    {
        playerHealth.Damage(damage);
        OnHealthChange?.Invoke(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);

        //Debug.Log(playerHealth.GetCurrentHealth);
    }


    private void playerState(Consumable_SO potion)
    {
        if (potion.type == Consumable_SO.ConsumableType.Healing)
        {
            playerHealth.Heal(potion.buffValue);
            OnHealthChange?.Invoke(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);
            //playerHealthBar.UpdateBar(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);
            Debug.Log("healing player");
        }
        else if (potion.type == Consumable_SO.ConsumableType.MsIncrease)
        {
            Debug.Log("Ms Increased");
            StartCoroutine(IncreaseMs(potion));
        }
        else
        {
            damageMultiplier += potion.buffValue / 100f;
            Debug.Log("Damage Increased");
            Invoke(nameof(ResetDamageMultiplier), potion.duration);
            
        }
    }

    private IEnumerator IncreaseMs(Consumable_SO potion)
    {
        float msMultiplier = 1f + potion.buffValue / 100f;
        playerMovement.IncreaseSpeed(msMultiplier);
        yield return new WaitForSeconds(potion.duration);
        playerMovement.IncreaseSpeed(msMultiplier - potion.buffValue / 100f);
    }

    private void ResetDamageMultiplier()
    {
        damageMultiplier = 1f;
    }
    private void UpdateInventoryUIState(bool isActive)
    {
        isInventoryActive = isActive;
    }
    // Update is called once per frame
    void Update()
    {
       
        //To test xp bar , remove later
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerManager.instance.AddXp(70);
        }
        StaminaManagement();
        //if (Input.GetKeyDown(collectKey))
        //{
        //    Collect();
        //}
        if (InventoryUI.isInventoryActive)
        {
            // If the inventory UI is active, return early to prevent the player from attacking,locking onto enemies
            return;
        }

        if (Input.GetKeyDown(attackKey) && readyToAttack && equipementManager.GetCurrentEquippedWeapon != null 
            && playerStamina.GetCurrentStamina >= equipementManager.GetCurrentEquippedWeapon.staminaConsumption && playerMovement.GetGroundedState)
        {
            if (equipementManager.GetCurrentEquippedWeapon.type == Weapon_SO.WeaponType.Sword) Attack();
        }

        

       
    }

    private void LateUpdate()
    {
        GunSystem();
    }
    private void GunSystem()
    {
        if (Input.GetKey(AimDownSight))
        {
            // Get mouse position
            Vector3 mousePos = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(mousePos);

            // Perform raycast to ground if necessary
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Calculate rotation angle
                Vector3 targetDir = hit.point - transform.position;
                float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;

                // Optionally smooth the rotation
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, characterRotationSpeed * Time.deltaTime);
            }
        }
    }
    // Handles the stamina recovery/drain of the player when he's walking/sprinting
    public void StaminaManagement()
    {
        if (playerMovement.state == PlayerMovement.MovementState.walking)
        {
            if (!recoveryTimerIsRunning)
            {
                recoveryTimerIsRunning = true;
                lastRecovryTime = Time.time;
            }

            if (Time.time - lastRecovryTime >= 0.5f && playerStamina.GetCurrentStamina < playerStamina.GetMaxStamina)
            {
                playerStamina.Recover(staminaRecoveryRate);
                OnStaminaChange?.Invoke(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
                //playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
                recoveryTimerIsRunning = false;
            }
        }
        else
        {
            recoveryTimerIsRunning = false; // Reset the recovery timer when not walking
        }

        if (playerMovement.state == PlayerMovement.MovementState.sprinting)
        {
            if (!drainTimerIsRunning)
            {
                drainTimerIsRunning = true;
                lastSprintTime = Time.time;
            }

            if (Time.time - lastSprintTime >= 0.2f)
            {
                if (playerStamina.GetCurrentStamina >= playerMovement.GetSprintConsumptionRate)
                {
                    playerStamina.Drain(playerMovement.GetSprintConsumptionRate);
                    Debug.Log("currentStamina = " + playerStamina.GetCurrentStamina);
                    OnStaminaChange?.Invoke(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
                    //playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
                    drainTimerIsRunning = false;
                }         
            }            
        }
        else
        {
            drainTimerIsRunning = false; // Reset the drain timer when not sprinting
        }
    }

    
    private void Attack()
    {
        if (!attacking)
        {
            attacking = true;
            animator.SetTrigger("Attacking");
            //Pass the appropriate weaponType to the Attack blend tree to intiate the right weaponAnimation
            animator.SetFloat("WeaponType", (int)equipementManager.GetCurrentEquippedWeapon.type);

            //Update the player's stamina accordingly
            playerStamina.Drain(equipementManager.GetCurrentEquippedWeapon.staminaConsumption);
            OnStaminaChange?.Invoke(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
            //playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);

            StartCoroutine(EndAttackMotion(equipementManager.GetCurrentEquippedWeapon.attackDuration));
        }
        playerMovement.attacking = true;
        StartCoroutine(ResetAttack(equipementManager.GetCurrentEquippedWeapon.attackDuration));
    }

    

    private IEnumerator ResetAttack(float duration)
    {
        readyToAttack = false;
        //AttackTime
        yield return new WaitForSeconds(duration);
        readyToAttack = true;

    }
    private IEnumerator EndAttackMotion(float duration)
    {
        //AttackTime
        yield return new WaitForSeconds(duration);
        attacking = false;
        playerMovement.attacking = false;
    }


    //Function inside the attackAnimation
    public void StartAttack()
    {
        Physics.SphereCast(transform.position + Vector3.up, hitsize, transform.forward,out var hitInfo, equipementManager.GetCurrentEquippedWeapon.attackRange,whatIsEnemy);
        if (hitInfo.collider != null)
        {
            Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
            if (enemy !=null)
            {
                enemy.EnemyDamaged(equipementManager.GetCurrentEquippedWeapon.damage * damageMultiplier);
               
            }
        }
        
    }


    public Stamina GetPlayerStamina
    {
        get { return playerStamina; }
    }
    
    

}

