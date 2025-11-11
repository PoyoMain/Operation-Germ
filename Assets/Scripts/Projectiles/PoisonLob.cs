using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PoisonLob : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileDataReference;
    [SerializeField] private GameObject poisonPuddlePrefab;
    
    private Rigidbody2D rb;
    private SpriteRenderer sRend;

    public ProjectileSO ProjectileData => projectileDataReference;

    private void Awake()
    {
        TryGetComponent(out rb);
        sRend = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0.01f)
        {
            Vector2 direction = rb.linearVelocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (direction.x < 0) sRend.flipY = true;
            else sRend.flipY = false;
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
        if (collision.collider.CompareTag("Ground"))
        {
            Instantiate(poisonPuddlePrefab, collision.GetContact(0).point, Quaternion.identity);
        }

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
