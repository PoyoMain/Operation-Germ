using System.Collections;
using UnityEngine;

public class GorbBlorb : MonoBehaviour
{
    [Header("General Boss Settings")]
    [SerializeField] private BossPart bossPart;

    [Header("Wait Settings")]
    [SerializeField] private float waitTime;

    [Header("Poison Lob Settings")]
    [SerializeField] private int minNumberOfLobsToShoot;
    [SerializeField] private int maxNumberOfLobsToShoot;
    [SerializeField] private float timeBetweenPoisonLobs;
    [SerializeField] private ProjectileSO poisonLobData;
    [SerializeField] private Transform poisonLobFireTransform;
    [SerializeField] private float minLobLaunchAngle;
    [SerializeField] private float maxLobLaunchAngle;
    [SerializeField] private float minLobLaunchForce;
    [SerializeField] private float maxLobLaunchForce;

    [Header("Slam Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpAngle;
    [SerializeField] private float slamForce;
    [SerializeField] private ProjectileSO slamProjectileData;
    [SerializeField] private Transform slamProjectileSpawnPointFront;
    [SerializeField] private Transform slamProjectileSpawnPointBack;
    [SerializeField] private float slamProjectileLaunchForce;

    [Header("Poison Spine Settings")]
    [SerializeField] private int numberOfSpinesToShoot;
    [SerializeField] private float timeBetweenPoisonSpines;
    [SerializeField] private ProjectileSO poisonSpineData;
    [SerializeField] private Transform poisonSpineSpawnPoint;
    [SerializeField] private float spineLaunchForce;

    private const string ANIM_IS_ATTACKING__PARAM = "IsAttacking";
    private const string ANIM_ATTACK_AGAIN_PARAM = "AttackAgain";
    private const string ANIM_CHOSEN_ATTACK_PARAM = "ChosenAttack";
    private const int LOB_ATTACK_INDEX = 1;
    private const int SLAM_ATTACK_INDEX = 2;
    private const int SPINE_ATTACK_INDEX = 3;

    private float waitTimer;
    private float poisonLobTimer;
    private float poisonSpineTimer;
    private State state = State.Idle;
    private State previousState;
    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private BossHealth health;
    private bool isAttacking;

    #region Unity Methods
    private void Awake()
    {
        if (FindAnyObjectByType<PlayerMovement>() != null) player = FindAnyObjectByType<PlayerMovement>().transform;
        TryGetComponent(out rb);
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
        ChangeState(State.Waiting);
    }

    private void FixedUpdate()
    {
        OnStateUpdate();
    }
    #endregion

    #region State Management
    private void ChangeState(State newState)
    {
        previousState = state;

        // On Phase End
        switch (state)
        {
            case State.Idle:
                break;
            case State.Waiting:
                WaitEnd();
                break;
            case State.PoisonLobs:
                PoisonLobsEnd();
                break;
            case State.Slam:
                SlamEnd();
                break;
            case State.PoisonSpines:
                PoisonSpinesEnd();
                break;
        }

        // On Phase Start
        switch (newState)
        {
            case State.Idle:
                break;
            case State.Waiting:
                WaitStart();
                break;
            case State.PoisonLobs:
                PoisonLobsStart();
                break;
            case State.Slam:
                SlamStart();
                break;
            case State.PoisonSpines:
                PoisonSpinesStart();
                break;
        }

        state = newState;
    }

    private void OnStateUpdate()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Waiting:
                WaitUpdate();
                break;
            case State.PoisonLobs:
                PoisonLobsUpdate();
                break;
            case State.Slam:
                SlamUpdate();
                break;
            case State.PoisonSpines:
                PoisonSpinesUpdate();
                break;
        }
    }
    #endregion

    #region Decision
    public void MakeDecision()
    {
        switch (previousState)
        {
            case State.Idle:
                ChangeState(State.PoisonLobs);
                break;
            case State.Waiting:
                break;
            case State.PoisonLobs:
                ChangeState(State.Slam);
                break;
            case State.Slam:
                ChangeState(State.PoisonSpines);
                break;
            case State.PoisonSpines:
                ChangeState(State.PoisonLobs);
                break;
        }
    }
    #endregion

    #region Waiting
    private void WaitStart()
    {
        waitTimer = waitTime;
    }

    private void WaitUpdate()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;

            if (waitTimer <= 0) MakeDecision();
        }
    }

    private void WaitEnd()
    {

    }
    #endregion

    #region Poison Lobs
    private void PoisonLobsStart()
    {
        StartCoroutine(PoisonLobCoroutine());
    }

    private void PoisonLobsUpdate()
    {
      
    }

    private void PoisonLobsEnd()
    {
        FacePlayer();
    }

    private IEnumerator PoisonLobCoroutine()
    {
        poisonLobTimer = timeBetweenPoisonLobs;
        anim.SetInteger(ANIM_CHOSEN_ATTACK_PARAM, LOB_ATTACK_INDEX);
       

        int numberToShoot = Random.Range(minNumberOfLobsToShoot, maxNumberOfLobsToShoot + 1);
        for (int i = 0; i < numberToShoot; i++)
        {
            while(poisonLobTimer > 0)
            {
                poisonLobTimer -= Time.deltaTime;
                yield return null;
            }

            if (i < numberToShoot - 1)
            {
                poisonLobTimer = timeBetweenPoisonLobs;
                anim.SetBool(ANIM_ATTACK_AGAIN_PARAM, true);
            }
            else anim.SetBool(ANIM_ATTACK_AGAIN_PARAM, false);

            isAttacking = true;
            anim.SetBool(ANIM_IS_ATTACKING__PARAM, isAttacking);

            while (isAttacking) yield return null;
        }

        ChangeState(State.Waiting);
        yield break;
    }

    private void ShootPoisonLob()
    {
        IProjectile projectile = Instantiate(poisonLobData.EnemyProjectile, poisonLobFireTransform.position, Quaternion.identity).GetComponent<IProjectile>();
        float angle = Random.Range(minLobLaunchAngle, maxLobLaunchAngle);
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 launchDirection = (transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;
        float launchForce = Random.Range(minLobLaunchForce, maxLobLaunchForce);
        projectile.Initialize(launchDirection, launchForce);
    }
    #endregion

    #region Slam
    private void SlamStart()
    {
        StartCoroutine(SlamCoroutine());
    }

    private void SlamUpdate()
    {
        

    }

    private void SlamEnd()
    {
        FacePlayer();
    }

    private IEnumerator SlamCoroutine()
    {
        anim.SetInteger(ANIM_CHOSEN_ATTACK_PARAM, SLAM_ATTACK_INDEX);

        float angleRad = jumpAngle * Mathf.Deg2Rad;
        Vector2 jumpDirection = (transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);

        while (rb.linearVelocity.y > 0.1f) yield return null;

        anim.SetBool(ANIM_IS_ATTACKING__PARAM, true);
        rb.linearVelocity = new(0f, -slamForce);

        while (rb.linearVelocity.y < -0.1f) yield return null;

        SpawnSlamProjectiles();
        anim.SetBool(ANIM_IS_ATTACKING__PARAM, false);
        EndAttack();
        ChangeState(State.Waiting);
        yield break;
    }

    private void SpawnSlamProjectiles()
    {
        IProjectile projectile = Instantiate(slamProjectileData.EnemyProjectile, slamProjectileSpawnPointFront.position, Quaternion.identity).GetComponent<IProjectile>();
        projectile.Initialize(transform.right, slamProjectileLaunchForce);

        projectile = Instantiate(slamProjectileData.EnemyProjectile, slamProjectileSpawnPointBack.position, Quaternion.identity).GetComponent<IProjectile>();
        projectile.Initialize(-transform.right, slamProjectileLaunchForce);
    }
    #endregion

    #region Poison Spines
    private void PoisonSpinesStart()
    {
        StartCoroutine(PoisonSpinesCoroutine());
    }

    private void PoisonSpinesUpdate()
    {


    }

    private void PoisonSpinesEnd()
    {
        FacePlayer();
    }

    private IEnumerator PoisonSpinesCoroutine()
    {
        anim.SetInteger(ANIM_CHOSEN_ATTACK_PARAM, SPINE_ATTACK_INDEX);
        poisonSpineTimer = timeBetweenPoisonSpines;

        for (int i = 0; i < numberOfSpinesToShoot; i++)
        {
            while (poisonSpineTimer > 0)
            {
                poisonSpineTimer -= Time.deltaTime;
                yield return null;
            }

            if (i < numberOfSpinesToShoot - 1)
            {
                poisonSpineTimer = timeBetweenPoisonSpines;
                anim.SetBool(ANIM_ATTACK_AGAIN_PARAM, true);
            }
            else anim.SetBool(ANIM_ATTACK_AGAIN_PARAM, false);

            anim.SetBool(ANIM_IS_ATTACKING__PARAM, true);

            while (isAttacking) yield return null;
        }

        ChangeState(State.Waiting);
        yield break;
    }

    private void ShootPoisonSpines()
    {
        IProjectile projectile = Instantiate(poisonSpineData.EnemyProjectile, poisonSpineSpawnPoint.position, Quaternion.identity).GetComponent<IProjectile>();
        projectile.Initialize(transform.right, spineLaunchForce);
    }
    #endregion

    private void EndAttack()
    {
        isAttacking = false;
        anim.SetBool(ANIM_IS_ATTACKING__PARAM, isAttacking);
        anim.SetInteger(ANIM_CHOSEN_ATTACK_PARAM, 0);
    }

    private void FacePlayer()
    {
        if (!player) return;

        Vector3 eulers = transform.eulerAngles;
        if (player.position.x < transform.position.x) eulers.y = 180;
        else if (player.position.x > transform.position.x) eulers.y = 0;
        transform.eulerAngles = eulers;
    }

    private void BossDefeated()
    {
        BossManager.Instance.DefeatBoss(bossPart);
    }

    public enum State
    {
        Idle,
        Waiting,
        PoisonLobs,
        Slam,
        PoisonSpines
    }
}

public enum Phase
{
    One,
    Two,
}
