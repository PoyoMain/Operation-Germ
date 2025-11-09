using System.Collections;
using UnityEngine;

public class Bimbly : MonoBehaviour
{
    [Header("General Boss Settings")]
    [SerializeField] private BossPart bossPart;

    [Header("Wait Settings")]
    [SerializeField] private float waitTime;

    [Header("Creep Orb Settings")]
    [SerializeField] private ProjectileSO CreepOrbData;
    [SerializeField] private int minCreepOrbsToSpawn;
    [SerializeField] private int maxCreepOrbsToSpawn;

    [Header("RingOrbs Settings")]
    [SerializeField] private ProjectileSO RingOrbProjectileData;

    [Header("Mind Wave Settings")]
    [SerializeField] private ProjectileSO MindWaveData;

    private const string ANIM_IS_ATTACKING__PARAM = "IsAttacking";
    private const string ANIM_CHOSEN_ATTACK_PARAM = "ChosenAttack";
    private const int CREEP_ORB_ATTACK_INDEX = 1;
    private const int RING_ORB_ATTACK_INDEX = 2;
    private const int MIND_WAVE_ATTACK_INDEX = 3;

    private float waitTimer;
    private State state = State.Idle;
    private State previousState;
    private Animator anim;
    private BossHealth health;
    private bool isAttacking;

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
            case State.CreepOrbs:
                CreepOrbsEnd();
                break;
            case State.RingOrbs:
                RingOrbsEnd();
                break;
            case State.MindWaves:
                MindWavesEnd();
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
            case State.CreepOrbs:
                CreepOrbsStart();
                break;
            case State.RingOrbs:
                RingOrbsStart();
                break;
            case State.MindWaves:
                MindWavesStart();
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
            case State.CreepOrbs:
                CreepOrbsUpdate();
                break;
            case State.RingOrbs:
                RingOrbsUpdate();
                break;
            case State.MindWaves:
                MindWavesUpdate();
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
                ChangeState(State.CreepOrbs);
                break;
            case State.Waiting:
                break;
            case State.CreepOrbs:
                //ChangeState(State.RingOrbs);
                ChangeState(State.CreepOrbs);
                break;
            case State.RingOrbs:
                ChangeState(State.MindWaves);
                break;
            case State.MindWaves:
                ChangeState(State.CreepOrbs);
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

    #region Creep Orbs
    private void CreepOrbsStart()
    {
        StartCoroutine(CreepOrbCoroutine());
    }

    private void CreepOrbsUpdate()
    {

    }

    private void CreepOrbsEnd()
    {
        
    }

    private IEnumerator CreepOrbCoroutine()
    {
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
    private void MindWavesStart()
    {
        StartCoroutine(MindWavesCoroutine());
    }

    private void MindWavesUpdate()
    {


    }

    private void MindWavesEnd()
    {
        
    }

    private IEnumerator MindWavesCoroutine()
    {
        
        yield break;
    }

    private void ShootMindWaves()
    {
        
    }
    #endregion

    private void EndAttack()
    {
        isAttacking = false;
        anim.SetBool(ANIM_IS_ATTACKING__PARAM, isAttacking);
        anim.SetInteger(ANIM_CHOSEN_ATTACK_PARAM, 0);
    }

    private void BossDefeated()
    {
        BossManager.Instance.DefeatBoss(bossPart);
    }

    public enum State
    {
        Idle,
        Waiting,
        CreepOrbs,
        RingOrbs,
        MindWaves
    }
}
