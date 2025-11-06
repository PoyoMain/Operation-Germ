using System.Collections;
using UnityEngine;

public class GorbBlorb : MonoBehaviour
{
    [Header("Wait Settings")]
    [SerializeField] private float waitTime;

    [Header("Poison Lob Settings")]
    [SerializeField] private int minNumberOfLobsToShoot;
    [SerializeField] private int maxNumberOfLobsToShoot;
    [SerializeField] private float timeBetweenPoisonLobs;
    [SerializeField] private Rigidbody2D poisonLobPrefab;
    [SerializeField] private Transform poisonLobFireTransform;
    [SerializeField] private float lobLaunchAngle;
    [SerializeField] private float lobLaunchForce;

    [Header("Slam Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpAngle;
    [SerializeField] private float slamForce;

    [Header("Poison Spine Settings")]
    [SerializeField] private int numberOfSpinesToShoot;
    [SerializeField] private float timeBetweenPoisonSpines;
    [SerializeField] private Rigidbody2D poisonSpinePrefab;
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
    private bool isAttacking;

    #region Unity Methods
    private void Awake()
    {
        if (FindAnyObjectByType<PlayerMovement>() != null) player = FindAnyObjectByType<PlayerMovement>().transform;
        TryGetComponent(out rb);
        TryGetComponent(out anim);
    }

    private void Start()
    { 
        ChangeState(State.PoisonSpines);
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
        Rigidbody2D projectile = Instantiate(poisonLobPrefab, poisonLobFireTransform.position, Quaternion.identity);
        float angleRad = lobLaunchAngle * Mathf.Deg2Rad;
        Vector2 launchDirection = (transform.right * Mathf.Cos(angleRad) + transform.up * Mathf.Sin(angleRad)).normalized;
        projectile.linearVelocity = launchDirection * lobLaunchForce;
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

        anim.SetBool(ANIM_IS_ATTACKING__PARAM, false);
        EndAttack();
        ChangeState(State.Waiting);
        yield break;
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
        Rigidbody2D projectile = Instantiate(poisonSpinePrefab, poisonSpineSpawnPoint.position, Quaternion.identity);
        projectile.linearVelocity = transform.right * spineLaunchForce;
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
