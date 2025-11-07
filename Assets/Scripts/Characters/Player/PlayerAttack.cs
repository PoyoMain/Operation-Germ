using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputReaderSO inputReader;

    [Header("Stats")]
    [SerializeField] private PlayerAttackStats stats;

    [Header("Components")]
    [SerializeField] private Transform attackSpawnTransform;

    private const string ANIM_ATTACK_PARAM = "IsAttacking";
    private const string ANIM_PARRY_PARAM = "IsParrying";

    private bool attackDown;
    private bool attackOneDown;
    private bool attackTwoDown;
    private bool attackHeld;
    private bool attackOneHeld;
    private bool attackTwoHeld;
    private bool parryDown;
    private bool chargeHeld;
    private float attackInputTimer;
    private float parryInputTimer;
    private Animator anim;
    private PlayerMovement playerMovement;
    private (ProjectileSO, int) heldAttackOne = new();
    private (ProjectileSO, int) heldAttackTwo = new();

    private PlayerControls.GameplayActions Controls => inputReader.Controls;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out playerMovement);
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        attackOneHeld = Controls.AttackOne.IsPressed();
        attackTwoHeld = Controls.AttackTwo.IsPressed();
        attackHeld = (attackOneHeld || attackTwoHeld);
        chargeHeld = Controls.Charge.IsPressed() && attackHeld;

        if (!attackOneDown && !IsAttacking)
        {
            attackOneDown = Controls.AttackOne.WasPressedThisFrame();
        }
        if (!attackTwoDown && !IsAttacking)
        {
            attackTwoDown = Controls.AttackTwo.WasPressedThisFrame();
        }
        attackDown = (attackOneDown || attackTwoDown);
        if (attackDown) attackInputTimer = stats.InputBufferTime;

        if (!parryDown)
        {
            parryDown = Controls.Parry.IsPressed() && attackDown;
        }
        if (parryDown)  parryInputTimer = stats.InputBufferTime;
    }

    private void FixedUpdate()
    {
        HandleInputTimers();
        HandleParry();
        HandleCharge();
        HandleAttack();
    }

    private void HandleInputTimers()
    {
        if (attackInputTimer > 0)
        {
            attackInputTimer -= Time.fixedDeltaTime;
            if (attackInputTimer <= 0)
            {
                attackOneDown = false;
                attackTwoDown = false;
            }
        }

        if (parryInputTimer > 0)
        {
            parryInputTimer -= Time.fixedDeltaTime;
            if (parryInputTimer <= 0)
            {
                parryDown = false;
            }
        }
    }

    #region Parry
    private float parryTimer;
    private bool isParryingOne;
    private bool isParryingTwo;
    public bool IsParrying => isParryingOne || isParryingTwo;

    private void HandleParry()
    {
        if (parryTimer > 0)
        {
            parryTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!parryDown) return;

        if (!IsParrying) ExecuteParry();

        parryDown = false;
        parryInputTimer = 0;
    }

    private void ExecuteParry()
    {
        if (attackOneDown) isParryingOne = true;
        else if (attackTwoDown) isParryingTwo = true;
        anim.SetBool(ANIM_PARRY_PARAM, IsParrying);
        parryTimer = stats.TimeBetweenParries;
    }

    private void EndParry()
    {
        isParryingOne = false;
        isParryingTwo = false;
        anim.SetBool(ANIM_PARRY_PARAM, IsParrying);
    }
    #endregion

    #region Collisions
    private void OnTriggerEnter2D(Collider2D collider)
    {
        IProjectile projectile = collider.GetComponentInParent<IProjectile>();
        if (projectile != null)
        {
            if (isParryingOne)
            {
                if (heldAttackOne.Item1 == projectile.ProjectileData) heldAttackOne.Item2++;
                else
                {
                    heldAttackOne.Item1 = projectile.ProjectileData;
                    heldAttackOne.Item2 = 1;
                }
            }
            else if (isParryingTwo)
            {
                if (heldAttackTwo.Item1 == projectile.ProjectileData) heldAttackTwo.Item2++;
                else
                {
                    heldAttackTwo.Item1 = projectile.ProjectileData;
                    heldAttackTwo.Item2 = 1;
                }
            }
        }
    }
    #endregion

    #region Charge
    private bool isChargingOne = false;
    private bool isChargingTwo = false;
    public bool IsCharging => isChargingOne || isChargingTwo;
    private float chargeTimer;

    private void HandleCharge()
    {
        if (!chargeHeld && IsCharging)
        {
            isChargingOne = false;
            isChargingTwo = false;
            chargeTimer = 0;
        }

        if (IsParrying || IsAttacking) return;

        if (!chargeHeld) return;

        Charge();
    }

    private void Charge()
    {
        if (attackOneHeld && !isChargingTwo)
        { 
            if (heldAttackOne.Item1 == null) return;

            if (!isChargingOne) isChargingOne = true;

            if (chargeTimer > 0)
            {
                chargeTimer -= Time.fixedDeltaTime;

                if (chargeTimer <= 0)
                {
                    chargeTimer = stats.ChargeRate;
                    heldAttackOne.Item2++;
                    print(heldAttackOne.Item1.name + ": " + heldAttackOne.Item2);
                }
            }
            else if (chargeTimer <= 0) chargeTimer = stats.ChargeRate;
        }
        else if (attackTwoHeld && !isChargingOne)
        {
            if (heldAttackTwo.Item1 == null) return;

            if (!isChargingTwo) isChargingTwo = true;

            if (chargeTimer > 0)
            {
                chargeTimer -= Time.fixedDeltaTime;

                if (chargeTimer <= 0)
                {
                    chargeTimer = stats.ChargeRate;
                    heldAttackTwo.Item2++;
                    print(heldAttackTwo.Item1.name + ": " + heldAttackTwo.Item2);
                }
            }
            else if (chargeTimer <= 0) chargeTimer = stats.ChargeRate;
        }
    }
    #endregion

    #region Attack
    private float attackTimer;
    public bool IsAttacking { get; private set; }
    private void HandleAttack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!attackDown) return;

        if (!IsAttacking && !IsParrying && !IsCharging) ExecuteAttack();

        attackOneDown = false;
        attackTwoDown = false;
        attackDown = false;
        attackInputTimer = 0;
    }

    private void ExecuteAttack()
    {
        IsAttacking = true;
        anim.SetBool(ANIM_ATTACK_PARAM, IsAttacking);
        attackTimer = stats.TimeBetweenAttacks;

        if (attackOneDown)
        {
            if (heldAttackOne.Item1 == null || heldAttackOne.Item2 < heldAttackOne.Item1.IdentifyAmount) return;

            IProjectile projectile = Instantiate(heldAttackOne.Item1.PlayerProjectile, attackSpawnTransform.position, Quaternion.identity).GetComponent<IProjectile>();
            float angleRad = heldAttackOne.Item1.PlayerLaunchAngle * Mathf.Deg2Rad;
            Vector2 launchDirection = (transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;
            projectile.Initialize(launchDirection, heldAttackOne.Item1.PlayerLaunchForce);
        }
        else if (attackTwoDown)
        {
            if (heldAttackTwo.Item1 == null || heldAttackTwo.Item2 < heldAttackTwo.Item1.IdentifyAmount) return;

            IProjectile projectile = Instantiate(heldAttackTwo.Item1.PlayerProjectile, attackSpawnTransform.position, Quaternion.identity).GetComponent<IProjectile>();
            float angleRad = heldAttackTwo.Item1.PlayerLaunchAngle * Mathf.Deg2Rad;
            Vector2 launchDirection = (transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;
            projectile.Initialize(launchDirection, heldAttackTwo.Item1.PlayerLaunchForce);
        }
    }

    private void EndAttack()
    {
        IsAttacking = false;
        anim.SetBool(ANIM_ATTACK_PARAM, IsAttacking);
    }
    #endregion
}
