using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int startHealth;
    [SerializeField] private int invincibilityTime;
    [SerializeField] private Collider2D hurtboxCollider;

    private int health;
    private float invincibleTimer;

    public int Health => health;
    public float InvincibilityTime => invincibilityTime;
    private bool IsInvincible => invincibleTimer > 0;

    private void Start()
    {
        health = startHealth;
    }

    private void Update()
    {
        if (IsInvincible)
        {
            invincibleTimer -= Time.deltaTime;

            if (!IsInvincible) hurtboxCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DamageComponent damageComponent))
        {
            if (collision.CompareTag("Player") && !IsInvincible)
            {
                Damage(damageComponent.Damage);
                damageComponent.HitSomething();
            }
        }
    }

    public void Damage(int amount)
    {
        health -= amount;
        invincibleTimer = invincibilityTime;
        hurtboxCollider.enabled = false;
    }
}
