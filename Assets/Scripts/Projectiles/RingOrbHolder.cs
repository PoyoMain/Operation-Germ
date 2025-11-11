using UnityEngine;

public class RingOrbHolder : MonoBehaviour, IProjectile
{
    [SerializeField] private ProjectileSO projectileData;

    private int orbCount;

    public ProjectileSO ProjectileData => projectileData;

    private void Awake()
    {
        
    }

    public void Initialize(Vector2 _, float power)
    {
        RingOrb[] orbs = GetComponentsInChildren<RingOrb>();
        orbCount = orbs.Length;
        foreach (RingOrb orb in orbs)
        {
            Vector2 direction = (orb.transform.position - transform.position).normalized;
            orb.SetVelocity(direction * power);
            orb.OnDestroyed += SubtractOrbCount;
        }
    }

    private void SubtractOrbCount()
    {
        orbCount--;

        if (orbCount > 0) return;

        Destroy(gameObject);
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
