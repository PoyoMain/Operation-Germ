using UnityEngine;

public class PoisonSpine : MonoBehaviour, IProjectile
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
    }

    public void OnHit(Collision2D collision)
    {
        Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
