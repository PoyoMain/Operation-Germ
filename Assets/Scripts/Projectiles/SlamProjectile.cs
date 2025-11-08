using UnityEngine;

public class SlamProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileDataReference;
    [SerializeField] private float timeToSlowDown;

    private Rigidbody2D rb;
    private SpriteRenderer sRend;
    private float timer;
    private Vector2 initialVelocity;

    public ProjectileSO ProjectileData => projectileDataReference;

    private void Awake()
    {
        TryGetComponent(out rb);
        sRend = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Vector2 movementDirection = rb.linearVelocity;

        if (movementDirection.magnitude > 0.01f) // Only rotate if moving
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        if (movementDirection.x < 0) sRend.flipY = true;
        else sRend.flipY = false;

        timer += Time.fixedDeltaTime;
        movementDirection = Vector2.Lerp(initialVelocity, Vector2.zero, timer / timeToSlowDown);
        if (movementDirection == Vector2.zero) Destroy();
        else rb.linearVelocity = movementDirection;
    }
    public void Initialize(Vector2 direction, float power)
    {
        initialVelocity = rb.linearVelocity = direction * power;
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
        if (collision.collider.CompareTag("Enemy")) return;

        Destroy();
    }

    public void OnHit(Collider2D collider)
    {
        if (collider.CompareTag("Enemy")) return;

        Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
