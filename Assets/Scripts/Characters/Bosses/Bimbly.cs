using System.Collections;
using UnityEngine;

public class Bimbly : MonoBehaviour
{
    [Header("General Boss Settings")]
    [SerializeField] private BossPart bossPart;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Wait Settings")]
    [SerializeField] private float waitTime;

    [Header("Creep Orb Settings")]
    [SerializeField] private ProjectileSO CreepOrbData;
    [SerializeField] private int minCreepOrbsToSpawn;
    [SerializeField] private int maxCreepOrbsToSpawn;

    [Header("RingOrbs Settings")]
    [SerializeField] private ProjectileSO RingOrbProjectileData;

    [Header("Mind Wave Settings")]
    [SerializeField] private ProjectileSO mindWaveData;
    [SerializeField] private float mindWaveTime;
    [SerializeField] private float mindWaveShootForce;

    [Header("Teleport Settings")]
    [SerializeField] private float teleportTime;

    private const string ANIM_PARAM_IS_ATTACKING = "IsAttacking";
    private const string ANIM_PARAM_CHOSEN_ATTACK = "ChosenAttack";
    private const int MIND_WAVE_ATTACK_INDEX = 1;
    private const int CREEP_ORB_ATTACK_INDEX = 2;
    private const int RING_ORB_ATTACK_INDEX = 3;

    private Animator anim;
    private BossHealth health;
    private bool isAttacking;
    private bool isTeleporting;

    #region Unity Methods
    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out health);
    }

    private void OnEnable()
    {
        health.OnBossDefeated += BossDefeated;
    }

    private void OnDisable()
    {
        health.OnBossDefeated -= BossDefeated;
    }

    private void Start()
    {
        mindWaveTimer = mindWaveTime;
        teleportTimer = teleportTime;
    }

    private void FixedUpdate()
    {
        MindWavesUpdate();
    }
    #endregion

    #region Creep Orbs
    private void CreepOrbsStart()
    {
        StartCoroutine(CreepOrbCoroutine());
    }

    private void CreepOrbsEnd()
    {
        
    }

    private IEnumerator CreepOrbCoroutine()
    {
        /// Set isattacking to true
        /// Set mind wave timer to 0
        /// If not in attack position, teleport
        /// Play animation
        /// Animation event determines when isattacking is set to false
        /// Wait a bit
        /// Teleport back to original position
        isAttacking = true;
        int numberToSpawn = Random.Range(minCreepOrbsToSpawn, maxCreepOrbsToSpawn + 1);

        yield break;
    }

    private void ShootCreepOrb()
    {
        
    }
    #endregion

    #region Ring Orbs
    private void RingOrbsStart()
    {
        StartCoroutine(RingOrbsCoroutine());
    }

    private void RingOrbsUpdate()
    {


    }

    private void RingOrbsEnd()
    {
        
    }

    private IEnumerator RingOrbsCoroutine()
    {
       
        yield break;
    }

    private void SpawnRingOrbsProjectiles()
    {
        
    }
    #endregion

    #region Mind Waves
    private float mindWaveTimer;
    private void MindWavesStart()
    {
        isAttacking = true;
        anim.SetInteger(ANIM_PARAM_CHOSEN_ATTACK, MIND_WAVE_ATTACK_INDEX);
        anim.SetBool(ANIM_PARAM_IS_ATTACKING, isAttacking);
    }

    private void MindWavesUpdate()
    {
        if (mindWaveTimer > 0 && !isAttacking)
        {
            mindWaveTimer -= Time.fixedDeltaTime;

            if (mindWaveTimer <= 0)
            {
                MindWavesStart();
                mindWaveTimer = mindWaveTime;
            }
        }
    }

    private void ShootMindWaves()
    {
        IProjectile projectile = Instantiate(mindWaveData.EnemyProjectile, projectileSpawnPoint.position, Quaternion.identity).GetComponent<IProjectile>();
        projectile.Initialize(transform.right, mindWaveShootForce);
    }
    #endregion

    #region Teleport
    private float teleportTimer;
    
    private void TeleportUpdate()
    {
        if (teleportTimer > 0 && !isAttacking)
        {
            teleportTimer -= Time.fixedDeltaTime;

            if (teleportTimer <= 0)
            {
                teleportTimer = teleportTime;
                Teleport();
            }
        }
    }

    private void Teleport()
    {
        StartCoroutine(TeleportCoroutine());
    }

    private IEnumerator TeleportCoroutine()
    {
        isTeleporting = true;

        isTeleporting = false;
        yield break;
    }
    #endregion

    private void EndAttack()
    {
        isAttacking = false;
        anim.SetBool(ANIM_PARAM_IS_ATTACKING, isAttacking);
        anim.SetInteger(ANIM_PARAM_CHOSEN_ATTACK, 0);
    }

    private void BossDefeated()
    {
        BossManager.Instance.DefeatBoss(bossPart);
    }
}
