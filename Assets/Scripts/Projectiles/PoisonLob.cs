using UnityEngine;

public class PoisonLob : MonoBehaviour, IProjectile
{
    [SerializeField] private GameObject poisonPuddlePrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
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

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
