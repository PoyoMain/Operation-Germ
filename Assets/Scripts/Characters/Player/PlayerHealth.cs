using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int startHealth;
    [SerializeField] private int invincibilityTime;
    [SerializeField] private Collider2D hurtboxCollider;

    private int health;
    private float invincibleTimer;
    private PlayerAttack playerAttack;

    public int Health => health;
    public float InvincibilityTime => invincibilityTime;
    private bool IsInvincible => invincibleTimer > 0;

    private void Awake()
    {
        TryGetComponent(out playerAttack);
    }

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
            if (collision.CompareTag("Enemy") && !IsInvincible && !playerAttack.IsParrying)
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
        print("Damaged");
    }
}
