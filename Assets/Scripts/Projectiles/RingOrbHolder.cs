using UnityEngine;

public class RingOrbHolder : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileData;

    private Rigidbody2D[] orbRBs;
    public ProjectileSO ProjectileData => throw new System.NotImplementedException();

    private void Awake()
    {
        orbRBs = GetComponentsInChildren<Rigidbody2D>();
    }

    public void Initialize(Vector2 _, float power)
    {
        foreach (Rigidbody2D rb in orbRBs)
        {
            Vector2 direction = (rb.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * power;
        }
    }

    public void OnHit(Collision2D collision)
    {
        throw new System.NotImplementedException();
    }

    public void OnHit(Collider2D collider)
    {
        throw new System.NotImplementedException();
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
}
