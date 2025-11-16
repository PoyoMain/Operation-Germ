using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KidneyStone : MonoBehaviour, IProjectile
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
        if (rb.linearVelocity.magnitude > 0.01f)
        {
            Vector2 direction = rb.linearVelocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void Initialize(Vector2 direction, float power)
    {
        rb.linearVelocity = direction * power;
        if (direction.x > 0) transform.eulerAngles = new(0, 0, 0);
        else if (direction.x < 0) transform.eulerAngles = new(0, 180, 0);
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
