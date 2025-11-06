using UnityEngine;

public class PoisonSpine : MonoBehaviour, IProjectile
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
    }

    public void OnHit(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy")) return;

        Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
