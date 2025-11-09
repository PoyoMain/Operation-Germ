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
    private const string ANIM_CHARGE_PARAM = "IsCharging";

    private bool attackDown;
    private bool attackOneDown;
    private bool attackTwoDown;
    private bool attackHeld;
    private bool attackOneHeld;
    private bool attackTwoHeld;
    private bool ReleaseDown;
    private bool chargeHeld;
    private float attackInputTimer;
    private float ReleaseInputTimer;
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
        if (playerMovement.IsGrounded)
        {
            attackOneHeld = Controls.AttackOne.IsPressed();
            attackTwoHeld = Controls.AttackTwo.IsPressed();
        }
        else
        {
            attackOneHeld = false;
            attackTwoHeld = false;
        }

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

        if (!ReleaseDown)
        {
            ReleaseDown = Controls.Release.IsPressed() && attackDown;
        }
        if (ReleaseDown)  ReleaseInputTimer = stats.InputBufferTime;
    }

    private void FixedUpdate()
    {
        HandleInputTimers();
        HandleRelease();
        HandleCharge();
        HandleParry();
        HandleAttack();
        HandleDraining();
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

        if (ReleaseInputTimer > 0)
        {
            ReleaseInputTimer -= Time.fixedDeltaTime;
            if (ReleaseInputTimer <= 0)
            {
                ReleaseDown = false;
            }
        }
    }

    #region Release
    private void HandleRelease()
    {
        if (playerMovement.IsInKnockback) return;
        if (!ReleaseDown) return;

        ExecuteRelease();

        ReleaseDown = false;
        ReleaseInputTimer = 0;
        attackOneDown = false;
        attackTwoDown = false;
        attackDown = false;
        attackInputTimer = 0;
    }

    private void ExecuteRelease()
    {
        if (attackOneDown) 
        {
            if (heldAttackOne.Item1 == null) return;

            heldAttackOne.Item1 = null;
            heldAttackOne.Item2 = 0;
        }
        else if (attackTwoDown)
        {
            if (heldAttackTwo.Item1 == null) return;

            heldAttackTwo.Item1 = null;
            heldAttackTwo.Item2 = 0;
        }
    }
    #endregion

    #region Collisions
    private void OnTriggerEnter2D(Collider2D collider)
    {
        TriggerBehavior(collider);

    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        TriggerBehavior(collider);
    }

    private void TriggerBehavior(Collider2D collider)
    {
        if (playerMovement.IsInKnockback) return;

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
        if (playerMovement.IsInKnockback) return;

        if (!chargeHeld && IsCharging)
        {
            isChargingOne = false;
            isChargingTwo = false;
            chargeTimer = 0;
            anim.SetBool(ANIM_CHARGE_PARAM, IsCharging);
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

            if (!isChargingOne)
            {
                isChargingOne = true;
                drainTimerOne = 0;
                anim.SetBool(ANIM_CHARGE_PARAM, IsCharging);
            }

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

            if (!isChargingTwo)
            {
                isChargingTwo = true;
                drainTimerTwo = 0;
                anim.SetBool(ANIM_CHARGE_PARAM, IsCharging);
            }

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

    #region Parry
    private float parryTimer;
    private bool isParryingOne;
    private bool isParryingTwo;
    public bool IsParrying => isParryingOne || isParryingTwo;

    private void HandleParry()
    {
        if (playerMovement.IsInKnockback) return;

        if (parryTimer > 0)
        {
            parryTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!attackDown) return;
        if (IsAttacking) return;
        if (attackOneDown && CanUseAttackOne) return;
        else if (attackTwoDown && CanUseAttackTwo) return;

        ExecuteParry();

        attackOneDown = false;
        attackTwoDown = false;
        attackDown = false;
        attackInputTimer = 0;
    }

    private void ExecuteParry()
    {
        if (IsParrying) return;

        if (attackOneDown) isParryingOne = true;
        else if (attackTwoDown) isParryingTwo = true;
        anim.SetBool(ANIM_PARRY_PARAM, IsParrying);

    }

    private void EndParry()
    {
        Invoke(nameof(EndParryNextFrame), Time.fixedDeltaTime);
    }

    private void EndParryNextFrame()
    {
        parryTimer = stats.TimeBetweenParries;
        isParryingOne = false;
        isParryingTwo = false;
        anim.SetBool(ANIM_PARRY_PARAM, IsParrying);
    }
    #endregion

    #region Attack
    private float attackTimer;
    private float attackCooldown;
    public bool IsAttacking { get; private set; }
    private bool CanUseAttackOne => heldAttackOne.Item1 != null && heldAttackOne.Item2 >= heldAttackOne.Item1.IdentifyAmount;
    private bool CanUseAttackTwo => heldAttackTwo.Item1 != null && heldAttackTwo.Item2 >= heldAttackTwo.Item1.IdentifyAmount;
    private void HandleAttack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
        }

        if (attackTimer <= 0 && attackDown && !IsAttacking && !playerMovement.IsInKnockback && !IsParrying && !chargeHeld) ExecuteAttack();

        attackOneDown = false;
        attackTwoDown = false;
        attackDown = false;
        attackInputTimer = 0;
    }

    private void ExecuteAttack()
    {
        IsAttacking = true;
        anim.SetBool(ANIM_ATTACK_PARAM, IsAttacking);

        if (attackOneDown)
        {
            if (!CanUseAttackOne) return;

            attackCooldown = heldAttackOne.Item1.PlayerCooldown;

            IProjectile projectile = Instantiate(heldAttackOne.Item1.PlayerProjectile, attackSpawnTransform.position, Quaternion.identity).GetComponent<IProjectile>();
            float angleRad = heldAttackOne.Item1.PlayerLaunchAngle * Mathf.Deg2Rad;
            Vector2 launchDirection = (transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;
            projectile.Initialize(launchDirection, heldAttackOne.Item1.PlayerLaunchForce);
        }
        else if (attackTwoDown)
        {
            if (!CanUseAttackTwo) return;

            attackCooldown = heldAttackTwo.Item1.PlayerCooldown;

            IProjectile projectile = Instantiate(heldAttackTwo.Item1.PlayerProjectile, attackSpawnTransform.position, Quaternion.identity).GetComponent<IProjectile>();
            float angleRad = heldAttackTwo.Item1.PlayerLaunchAngle * Mathf.Deg2Rad;
            Vector2 launchDirection = (transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;
            projectile.Initialize(launchDirection, heldAttackTwo.Item1.PlayerLaunchForce);
        }
    }

    private void EndAttack()
    {
        IsAttacking = false;
        attackTimer = attackCooldown;
        anim.SetBool(ANIM_ATTACK_PARAM, IsAttacking);
    }
    #endregion

    #region Drain
    private float drainTimerOne;
    private float drainTimerTwo;
    private void HandleDraining()
    {
        if (!isChargingOne && heldAttackOne.Item1 != null && heldAttackOne.Item2 < heldAttackOne.Item1.IdentifyAmount && heldAttackOne.Item2 > 0)
        {
            if (drainTimerOne > 0)
            {
                drainTimerOne -= Time.fixedDeltaTime;

                if (drainTimerOne <= 0)
                {
                    heldAttackOne.Item2--;
                    drainTimerOne = stats.DrainRate;
                    print(heldAttackOne.Item1.name + ": " + heldAttackOne.Item2);
                }
            }
            else drainTimerOne = stats.DrainRate;
        }

        if (!isChargingTwo && heldAttackTwo.Item1 != null && heldAttackTwo.Item2 < heldAttackTwo.Item1.IdentifyAmount && heldAttackTwo.Item2 > 0)
        {
            if (drainTimerTwo > 0)
            {
                drainTimerTwo -= Time.fixedDeltaTime;

                if (drainTimerTwo <= 0)
                {
                    heldAttackTwo.Item2--;
                    drainTimerTwo = stats.DrainRate;
                    print(heldAttackTwo.Item1.name + ": " + heldAttackTwo.Item2);
                }
            }
            else drainTimerTwo = stats.DrainRate;
        }
    }
    #endregion
}
