using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MindWave : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileDataReference;
    [SerializeField] private TrackTarget trackTarget;

    private Rigidbody2D rb;
    

    public ProjectileSO ProjectileData => projectileDataReference;

    private void Awake()
    {
        TryGetComponent(out rb);
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0.01f) // Only rotate if moving
        {
            Vector2 movementDirection = rb.linearVelocity;
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    public void Initialize(Vector2 _, float power)
    {
        Transform trackTransform;
        if (trackTarget == TrackTarget.Player) trackTransform = GameObject.FindGameObjectWithTag("Player").transform;
        else trackTransform = GameObject.FindGameObjectWithTag("Enemy Boss").transform;

        Vector2 direction = (trackTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * power;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        OnHit(collider);
    }

    public void OnHit(Collision2D _)
    {
        Destroy();
    }

    public void OnHit(Collider2D _)
    {
        Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
