using UnityEngine;
using UnityEngine.UIElements;

public class FireLog : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileDataReference;

    public ProjectileSO ProjectileData => projectileDataReference;

    [SerializeField] private GameObject log;
    [SerializeField] private GameObject log02;
    [SerializeField] private Transform center;
    [SerializeField] private float spinSpeed;
    private Rigidbody2D rigidBody;
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Initialize(Vector2 direction, float power)
    {
        rigidBody.AddForce(direction * power, ForceMode2D.Impulse);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        OnHit(collider);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Rotate the log around the pivot
        log.transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        log02.transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }
}
