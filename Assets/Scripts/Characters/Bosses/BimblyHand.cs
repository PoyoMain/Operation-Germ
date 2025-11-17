using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class BimblyHand : MonoBehaviour
{
    [Header("Move & Idle Setting")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform originPoint;
    [SerializeField] private float idleAttackTime;

    [Header("Tracking Settings")]
    [SerializeField] private float trackingAttackTime;
    [SerializeField, Range(0, 1)] private float chanceToHitNeuronWhenTracking;

    [Header("Slam Stats")]
    [SerializeField] private float slamForce;
    [SerializeField] private Transform slamStartingPoint;
    [SerializeField] private LayerMask hitLayers;

    [Header("Components")]
    [SerializeField] private PlayerDetectTerritory detectTerritory;

    [Header("Audio")]
    [SerializeField] private SFXPlayer hitGroundSFX;

    private State state = State.Idle;
    private Rigidbody2D rb;
    private Animator anim;

    private const string ANIM_PARAM_SLAM = "IsSlamming";

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out anim);
    }

    private void Start()
    {
        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (state == State.Idle && detectTerritory.PlayerDetected) ChangeState(State.Tracking);
        else if (state == State.Tracking && !detectTerritory.PlayerDetected) ChangeState(State.Idle);

        StateUpdate();
    }

    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.Idle:
                idleAttackTimer = idleAttackTime;
                break;
            case State.Tracking:
                trackingAttackTimer = trackingAttackTime;
                break;
            case State.AttackingPlayer:
                AttackPlayer();
                break;
            case State.AttackingNeuron:
                if (state == State.Idle) AttackNeuron(() => ChangeState(State.Idle));
                if (state == State.Tracking) AttackNeuron(() => ChangeState(State.Tracking));
                break;
        }

        state = newState;
    }

    private void StateUpdate()
    {
        switch (state)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Tracking:
                TrackingUpdate();
                break;
            case State.AttackingPlayer:
                break;
            case State.AttackingNeuron:
                break;
        }
    }

    #region Idle
    private float idleAttackTimer;
    private void IdleUpdate()
    {
        if (idleAttackTimer > 0)
        {
            idleAttackTimer -= Time.deltaTime;
            if (idleAttackTimer <= 0) ChangeState(State.AttackingNeuron);
        }

        if (Vector2.Distance(originPoint.position, transform.position) < 0.1f) return;

        transform.position = Vector2.Lerp(transform.position, originPoint.position, moveSpeed * Time.deltaTime);
    }
    #endregion

    #region Tracking
    private float trackingAttackTimer;
    private void TrackingUpdate()
    {
        if (trackingAttackTimer > 0)
        {
            trackingAttackTimer -= Time.deltaTime;

            if (trackingAttackTimer <= 0)
            {
                float choice = Random.Range(0f, 1f);
                if (choice <= chanceToHitNeuronWhenTracking) ChangeState(State.AttackingNeuron);
                else ChangeState(State.AttackingPlayer);
            }
        }

        Vector2 targetPosition = new (detectTerritory.PlayerTransform.position.x, originPoint.position.y);
        if (Vector2.Distance(targetPosition, transform.position) < 0.1f) return;

        transform.position = Vector2.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    #endregion

    #region Attacking
    private void AttackNeuron(Action onAttackEnd)
    {
        StartCoroutine(AttackNeuronCoroutine(onAttackEnd));
    }

    private IEnumerator AttackNeuronCoroutine(Action onAttackEnd)
    {
        Vector2 abovePos = slamStartingPoint.position;
        yield return transform.DOMove(abovePos, 1f).WaitForCompletion();

        RaycastHit2D hit;
        Vector2 rayOrigin = transform.position;
        float raycastDistance = 200f;

        hit = Physics2D.Raycast(rayOrigin, Vector2.down, raycastDistance, hitLayers);
        if (hit)
        {
            Vector2 slamPos = hit.point;
            slamPos.y -= 0.02f;

            Tweener tween = transform.DOMoveY(slamPos.y, 0.5f).SetEase(Ease.InExpo);
            bool halfwayDone = false;

            tween.OnUpdate(() =>
            {
                if (tween.ElapsedPercentage() >= 0.75f && !halfwayDone)
                {
                    anim.SetBool(ANIM_PARAM_SLAM, true);
                }
            });

            yield return tween.WaitForCompletion();
            anim.SetBool(ANIM_PARAM_SLAM, false);
            yield return new WaitForSeconds(1f);
            yield return transform.DOMove(abovePos, 0.5f).SetEase(Ease.InOutQuart).WaitForCompletion();
        }

        onAttackEnd?.Invoke();
        yield break;
    }

    private void AttackPlayer()
    {
        StartCoroutine(AttackPlayerCoroutine());
    }

    private IEnumerator AttackPlayerCoroutine()
    {
        float aboveY = slamStartingPoint.position.y;
        yield return transform.DOMoveY(aboveY, 0.25f).WaitForCompletion();

        RaycastHit2D hit;
        Vector2 rayOrigin = transform.position;
        float raycastDistance = 200f;

        hit = Physics2D.Raycast(rayOrigin, Vector2.down, raycastDistance, hitLayers);
        if (hit)
        {
            Vector2 slamPos = hit.point;

            Tweener tween = transform.DOMoveY(slamPos.y, 0.5f).SetEase(Ease.InExpo);
            bool halfwayDone = false;

            tween.OnUpdate(() =>
            {
                if (tween.ElapsedPercentage() >= 0.75f && !halfwayDone)
                {
                    anim.SetBool(ANIM_PARAM_SLAM, true);
                }
            });

            yield return tween.WaitForCompletion();
            hitGroundSFX.Play();
            anim.SetBool(ANIM_PARAM_SLAM, false);
            yield return new WaitForSeconds(1f);
            yield return transform.DOMoveY(aboveY, 0.5f).SetEase(Ease.InOutQuart).WaitForCompletion();
        }

        ChangeState(State.Tracking);
        yield break;
    }
    #endregion

    public enum State
    {
        Idle,
        Tracking,
        AttackingPlayer,
        AttackingNeuron
    }
}
