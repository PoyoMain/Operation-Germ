using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Spike : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileData;

    public ProjectileSO ProjectileData => projectileData;

    private Rigidbody2D rb;
    private Animator anim;
    private bool readyToFire;
    private Vector2 launchDirection;
    private float launchPower;

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out anim);
    }

    public void Initialize(Vector2 direction, float power)
    {
        launchDirection = direction;
        launchPower = power;
    }

    private void Launch()
    {
        readyToFire = true;
        rb.linearVelocity = launchDirection * launchPower;
        anim.SetBool("ReadyToFire", readyToFire);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!readyToFire) return;

        OnHit(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!readyToFire) return;

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
