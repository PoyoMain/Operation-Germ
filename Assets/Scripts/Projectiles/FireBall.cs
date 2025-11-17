using UnityEngine;

public class FireBall : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileDataReference;


    public ProjectileSO ProjectileData => projectileDataReference;

    [SerializeField] private GameObject explosionPrefab;
    private float explosionRadius;
    private int numBounces;
    private Rigidbody2D rigidBody;
    private Animator anim;
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rigidBody.linearVelocity.sqrMagnitude > 0.01f)
        {
            Vector2 direction = rigidBody.linearVelocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void Explode()
    {
        
        Instantiate(explosionPrefab, transform.position, Quaternion.identity );
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        OnHit(collider);
    }


    public void Initialize(Vector2 direction, float power)
    {
        
        rigidBody.AddForce(direction * power, ForceMode2D.Impulse);
    }

    public void OnHit(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy")) return;

        if (collision.collider.CompareTag("Ground"))
        {
            anim.SetBool("HitFloor", true);
            numBounces++;
            if(numBounces == 3)
            {
                Explode();
            }
            else
            {
                return;

            }
            
            
        }
        

        Destroy();
    }

    public void ResetBounceAnim()
    {
        anim.SetBool("HitFloor", false);
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
