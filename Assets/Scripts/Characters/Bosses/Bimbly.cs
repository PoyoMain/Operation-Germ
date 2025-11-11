using System.Collections;
using UnityEngine;

public class Bimbly : MonoBehaviour
{
    [Header("General Boss Settings")]
    [SerializeField] private BossPart bossPart;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Creep Orb Settings")]
    [SerializeField] private ProjectileSO creepOrbData;
    [SerializeField] private int minCreepOrbsToSpawn;
    [SerializeField] private int maxCreepOrbsToSpawn;
    [SerializeField] private float timeBetweenCreepOrbs;
    [SerializeField] private float creepOrbMoveSpeed;

    [Header("RingOrbs Settings")]
    [SerializeField] private ProjectileSO RingOrbProjectileData;
    [SerializeField, Range(0, 1)] private float chanceToDoRingOrbs;

    [Header("Mind Wave Settings")]
    [SerializeField] private ProjectileSO mindWaveData;
    [SerializeField] private float mindWaveTime;
    [SerializeField] private float mindWaveShootForce;

    [Header("Teleport Settings")]
    [SerializeField] private float teleportTime;
    [SerializeField] private Transform teleportPositionLeft;
    [SerializeField] private Transform teleportPositionRight;
    [SerializeField] private Transform teleportPositionMiddle;

    private const string ANIM_PARAM_IS_ATTACKING = "IsAttacking";
    private const string ANIM_PARAM_CHOSEN_ATTACK = "ChosenAttack";
    private const int MIND_WAVE_ATTACK_INDEX = 1;
    private const int CREEP_ORB_ATTACK_INDEX = 2;
    private const int RING_ORB_ATTACK_INDEX = 3;

    private Animator anim;
    private BossHealth health;
    private bool isAttacking;
    private bool isTeleporting;
    private Location teleportLocation;

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
        //mindWaveTimer = mindWaveTime;
        //teleportTimer = teleportTime;
        CreepOrbsStart();
    }

    private void FixedUpdate()
    {
        MindWavesUpdate();
        TeleportUpdate();
    }
    #endregion

    private void OnNeuronHit()
    {
        float choice = Random.Range(0f, 1f);
        if (choice <= chanceToDoRingOrbs) RingOrbsStart();
        else CreepOrbsStart();
    }

    #region Creep Orbs
    private void CreepOrbsStart()
    {
        isAttacking = true;
        anim.SetInteger(ANIM_PARAM_CHOSEN_ATTACK, CREEP_ORB_ATTACK_INDEX);
        anim.SetBool(ANIM_PARAM_IS_ATTACKING, isAttacking);
    }


    private IEnumerator CreepOrbCoroutine()
    {
        int numberToSpawn = Random.Range(minCreepOrbsToSpawn, maxCreepOrbsToSpawn + 1);
        int numberSpawned = 0;

        while (numberSpawned < numberToSpawn)
        {
            ShootCreepOrb();
            numberSpawned++;

            if (numberSpawned != numberToSpawn) yield return new WaitForSeconds(timeBetweenCreepOrbs);
            
            yield return null;
        }

        EndAttack();
        yield break;
    }

    private void ActivateCreepOrbs()
    {
        StartCoroutine(CreepOrbCoroutine());
    }

    private void ShootCreepOrb()
    {
        IProjectile projectile = Instantiate(creepOrbData.EnemyProjectile, projectileSpawnPoint.position, Quaternion.identity).GetComponent<IProjectile>();
        projectile.Initialize(Vector2.zero, creepOrbMoveSpeed);
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
        if (mindWaveTimer > 0 && !isAttacking && !isTeleporting)
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
        if (teleportTimer > 0 && !isAttacking && !isTeleporting)
        {
            teleportTimer -= Time.fixedDeltaTime;

            if (teleportTimer <= 0)
            {
                teleportTimer = teleportTime;
                StartTeleportation(teleportToMiddle: false);
            }
        }
    }

    private void StartTeleportation(bool teleportToMiddle)
    {
        isTeleporting = true;

        if (teleportToMiddle)
        {
            if (teleportLocation == Location.Middle) return;

            teleportLocation = Location.Middle;
            anim.SetBool("IsTeleporting", isTeleporting);
        }
        else
        {
            if (teleportLocation == Location.Left)
            {
                teleportLocation = Location.Right;
                anim.SetBool("IsTeleporting", isTeleporting);
            }
            else if (teleportLocation == Location.Right)
            {
                teleportLocation = Location.Left;
                anim.SetBool("IsTeleporting", isTeleporting);
            }
        }
    }

    private void Teleport()
    {
        Invoke(nameof(Reappear), 1);
    }

    private void Reappear()
    {
        transform.position = teleportLocation switch
        {
            Location.Left => transform.position = teleportPositionLeft.position,
            Location.Right => transform.position = teleportPositionRight.position,
            Location.Middle => transform.position = teleportPositionMiddle.position,
            _ => throw new System.NotImplementedException(),
        };

        mindWaveTimer = mindWaveTime;
        isTeleporting = false;
        anim.SetBool("IsTeleporting", isTeleporting);
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

    public enum Location
    {
        Left,
        Right,
        Middle
    }
}
