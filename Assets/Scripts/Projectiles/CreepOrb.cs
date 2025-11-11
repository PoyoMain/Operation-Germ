using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CreepOrb : MonoBehaviour, IProjectile
{

    [SerializeField] private ProjectileSO projectileDataReference;
    [SerializeField] private TrackTarget trackTarget;
    [SerializeField] private float timeToMove;
    [SerializeField] private float timeBetweenMoves;
    [SerializeField] private float timesToMoveToPlayer;

    private Transform trackTransform;
    private float power;
    private Rigidbody2D rb;
    private float inBetweenMoveTimer;
    private float timesMoved;

    public ProjectileSO ProjectileData => projectileDataReference;

    private void Awake()
    {
        TryGetComponent(out rb);
    }

    private void Start()
    {
        inBetweenMoveTimer = timeBetweenMoves;
    }

    private void FixedUpdate()
    {
        if (inBetweenMoveTimer > 0)
        {
            inBetweenMoveTimer -= Time.fixedDeltaTime;

            if (inBetweenMoveTimer <= 0)
            {
                if (timesMoved >= timesToMoveToPlayer)
                {
                    Vector2 direction = (trackTransform.position - transform.position).normalized;
                    rb.linearVelocity = direction * power;
                }
                else
                {
                    StartCoroutine(MoveCoroutine());
                }
            }
        }
    }

    private IEnumerator MoveCoroutine()
    {
        Vector2 direction = (trackTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * power;

        yield return new WaitForSeconds(timeToMove);

        rb.linearVelocity = Vector2.zero;

        timesMoved++;
        inBetweenMoveTimer = timeBetweenMoves;

        yield break;
    }

    public void Initialize(Vector2 _, float power)
    {
        if (trackTarget == TrackTarget.Player) trackTransform = GameObject.FindGameObjectWithTag("Player").transform;
        else
        {
            GameObject[] targetOptions = GameObject.FindGameObjectsWithTag("EnemyBoss");

            GameObject closestTarget = targetOptions[0];
            float distanceToClosestTarget = Vector2.Distance(transform.position, closestTarget.transform.position);
            foreach (GameObject option in targetOptions)
            {
                float distance = Vector2.Distance(transform.position, option.transform.position);
                if (distance < distanceToClosestTarget)
                {
                    distanceToClosestTarget = distance;
                    closestTarget = option;
                }
            }

            trackTransform = closestTarget.transform;
        }
        this.power = power;
        inBetweenMoveTimer = timeBetweenMoves;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHit(collision);
    }

    public void OnHit(Collision2D collision)
    {
        Destroy();
    }

    public void OnHit(Collider2D collider)
    {
        Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
