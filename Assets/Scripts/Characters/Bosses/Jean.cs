using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class Jean : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private BossPart bossPart;

    [Header("Armor")]
    [SerializeField] private StagedEnemyHealth armorHealth;
    [SerializeField] private GameObject armorHitboxParent;

    [Header("Components")]
    [SerializeField] private GameObject hitboxParent;
    [SerializeField] private GameObject hurtboxParent;

    [Header("Throw Stats")]
    [SerializeField] private float throwRate;
    [SerializeField] private float throwRateNoArmor;
    [SerializeField] private ProjectileSO kidneyStoneProjectileData;
    [SerializeField] private float minThrowAngle;
    [SerializeField] private float maxThrowAngle;
    [SerializeField] private float minThrowForce;
    [SerializeField] private float maxThrowForce;
    [SerializeField] private Transform throwSpawnPoint;

    [Header("Spike Stats")]
    [SerializeField] private SpikeSpawner spikeSpawner;
    [SerializeField] private float spikeRate;
    [SerializeField] private float spikeRateNoArmor;

    [Header("Dizzy Stats")]
    [SerializeField] private float dizzyTime;

    [Header("Audio")]
    [SerializeField] private SFXPlayer throwSFX;
    [SerializeField] private SFXPlayer dizzySFX;

    private const string ANIM_PARAM_DIZZY = "IsDizzy";
    private const string ANIM_PARAM_REMOVE_ARMOR = "ArmorRemoved";
    private const string ANIM_PARAM_THROW = "IsThrowing";

    private Animator anim;
    private BossHealth health;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out health);
    }

    private void OnEnable()
    {
        armorHealth.onDizzyStageStarted += StartDizzyPhase;
        armorHealth.onHealthDepleted += RemoveArmor;
        health.OnBossDefeated += BossDefeated;
    }

    private void OnDisable()
    {
        armorHealth.onDizzyStageStarted -= StartDizzyPhase;
        armorHealth.onHealthDepleted -= RemoveArmor;
        health.OnBossDefeated -= BossDefeated;
    }

    private void FixedUpdate()
    {
        HandleThrowTimer();
        HandleSpikeTimer();
    }

    #region Dizzy
    private bool isDizzy;
    private void StartDizzyPhase()
    {
        isDizzy = true;
        anim.SetBool(ANIM_PARAM_DIZZY, isDizzy);
        anim.SetBool(ANIM_PARAM_THROW, false);

        armorHitboxParent.SetActive(false);
        hitboxParent.SetActive(true);
        hurtboxParent.SetActive(true);
        dizzySFX.Play(looping: true);

        Invoke(nameof(EndDizzyPhase), dizzyTime);
    }

    private void EndDizzyPhase()
    {
        isDizzy = false;
        anim.SetBool(ANIM_PARAM_DIZZY, isDizzy);

        armorHitboxParent.SetActive(true);
        hitboxParent.SetActive(false);
        hurtboxParent.SetActive(false);
        dizzySFX.Stop();
    }
    #endregion

    #region Armor
    private bool armorRemoved;
    private void RemoveArmor()
    {
        armorRemoved = true;
        anim.SetBool(ANIM_PARAM_REMOVE_ARMOR, armorRemoved);
        anim.SetBool(ANIM_PARAM_THROW, false);

        armorHitboxParent.SetActive(false);
        hitboxParent.SetActive(true);
        hurtboxParent.SetActive(true);
    }
    #endregion

    #region Throw
    private float throwTimer;

    private void HandleThrowTimer()
    {
        if (isDizzy) return;

        if (throwTimer > 0)
        {
            throwTimer -= Time.fixedDeltaTime;

            if (throwTimer <= 0)
            {
                throwTimer = armorRemoved ? throwRateNoArmor : throwRate;
                anim.SetBool(ANIM_PARAM_THROW, true);
            }
        }
        else throwTimer = armorRemoved ? throwRateNoArmor : throwRate;
    }

    private void Throw()
    {
        float angle = Random.Range(minThrowAngle, maxThrowAngle);
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 launchDirection = (-transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;

        float force = Random.Range(minThrowForce, maxThrowForce);

        IProjectile projectile = Instantiate(kidneyStoneProjectileData.EnemyProjectile, throwSpawnPoint.position, Quaternion.identity).GetComponent<IProjectile>();
        projectile.Initialize(launchDirection, force);

        throwSFX.Play();

        EndThrow();
    }

    private void EndThrow()
    {
        anim.SetBool(ANIM_PARAM_THROW, false);
    }
    #endregion

    #region Spikes
    private float spikeTimer;

    private void HandleSpikeTimer()
    {
        if (isDizzy) return;

        if (spikeTimer > 0)
        {
            spikeTimer -= Time.fixedDeltaTime;

            if (spikeTimer <= 0)
            {
                spikeTimer = armorRemoved ? spikeRateNoArmor : spikeRate;
                spikeSpawner.StartSpikeSpawn();
            }
        }
        else spikeTimer = armorRemoved ? spikeRateNoArmor : spikeRate;
    }
    #endregion

    #region General Methods
    private void BossDefeated()
    {
        BossManager.Instance.DefeatBoss(bossPart);
        Destroy(transform.parent.gameObject);
    }
    #endregion
}
