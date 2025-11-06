using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.ShaderGraph;
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
    private bool parryDown;
    private float attackInputTimer;
    private float parryInputTimer;
    private Animator anim;
    private (ProjectileSO, int) heldAttackOne = new();
    private (ProjectileSO, int) heldAttackTwo = new();

    public bool IsParrying => isParryingOne || isParryingTwo;
    private PlayerControls.GameplayActions Controls => inputReader.Controls;

    private void Awake()
    {
        TryGetComponent(out anim);
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if (!attackOneDown && !isAttacking)
        {
            attackOneDown = Controls.AttackOne.WasPressedThisFrame();
        }
        if (!attackTwoDown && !isAttacking)
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
        if (attackOneDown) { isParryingOne = true; print("One"); }
        else if (attackTwoDown) { isParryingTwo = true; print("Two"); }
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

    #region Attack
    private float attackTimer;
    private bool isAttacking;
    private void HandleAttack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!attackDown) return;

        if (!isAttacking && !IsParrying) ExecuteAttack();

        attackOneDown = false;
        attackTwoDown = false;
        attackDown = false;
        attackInputTimer = 0;
    }

    private void ExecuteAttack()
    {
        isAttacking = true;
        anim.SetBool(ANIM_ATTACK_PARAM, isAttacking);
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
        isAttacking = false;
        anim.SetBool(ANIM_ATTACK_PARAM, isAttacking);
    }
    #endregion
}
