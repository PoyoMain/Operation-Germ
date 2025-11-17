using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Spike : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileData;

    [Header("Audio")]
    [SerializeField] private SFXPlayer spikeShootSFX;

    public ProjectileSO ProjectileData => projectileData;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sRend;
    private bool readyToFire;
    private Vector2 launchDirection;
    private float launchPower;

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out anim);
        sRend = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.x < 0) sRend.flipY = true;
        else sRend.flipY = false;
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
        spikeShootSFX.Play();
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!readyToFire) return;

    //    OnHit(collision);
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!readyToFire) return;

    //    OnHit(collision);
    //}

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
