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
        if (rb.linearVelocity.magnitude > 0.01f) // Only rotate if moving
        {
            // Calculate the direction of movement from the current velocity
            Vector2 direction = rb.linearVelocity;

            // Calculate the angle in degrees
            // Mathf.Atan2 returns the angle in radians between the x-axis and a 2D vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply the rotation to the transform
            // Quaternion.Euler creates a rotation from Euler angles (around the Z-axis for 2D)
            // Add the offset if needed so the sprite's "forward" direction aligns with the arc
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
        if (collision.collider.CompareTag("Enemy")) return;

        if (collision.collider.CompareTag("Ground"))
        {
            Instantiate(poisonPuddlePrefab, collision.GetContact(0).point, Quaternion.identity);
        }

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
