using Unity.VisualScripting;
using UnityEngine;

public class FireMissile : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileDataReference;
    public ProjectileSO ProjectileData => projectileDataReference;
    [SerializeField] private float minDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float missileLifeTime;
    [SerializeField] private TrackTarget trackTarget; 

    private Rigidbody2D rigidBody;
    private Transform trackTransform;
    
    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Initialize(Vector2 direction, float power)
    {
        if(trackTarget == TrackTarget.Player)
        {
            trackTransform = FindAnyObjectByType<PlayerMovement>().transform;
        }
        else if(trackTarget == TrackTarget.Enemy)
        {
            trackTransform = FindAnyObjectByType<HeartBurn>().transform;
        }
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
    void Awake()
    {
        
        rigidBody = GetComponent<Rigidbody2D>();
        

        Destroy(gameObject, missileLifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidBody.linearVelocity = transform.up * moveSpeed * Time.fixedDeltaTime * 10f;
        Vector2 direction = (trackTransform.position -transform.position).normalized;
        float rotationSteer = Vector3.Cross(transform.up, direction).z;
        rigidBody.angularVelocity = rotationSteer * rotateSpeed *10f;

    }

    
}
