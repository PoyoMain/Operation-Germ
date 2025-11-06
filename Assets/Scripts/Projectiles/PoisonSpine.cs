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
