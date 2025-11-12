using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PoisonSpine : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileDataReference;

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
    public void Initialize(Vector2 direction, float power)
    {
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
