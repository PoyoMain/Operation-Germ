using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int startHealth;
    [SerializeField] private float invincibilityTime;

    private int health;
    private float invincibleTimer;
    private PlayerAttack playerAttack;
    private DamageFlash damageFlash;

    public int Health => health;
    public float InvincibilityTime => invincibilityTime;
    private bool IsInvincible => invincibleTimer > 0;

    private void Awake()
    {
        TryGetComponent(out playerAttack);
        TryGetComponent(out damageFlash);
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DamageComponent damageComponent))
        {
            if (!IsInvincible && !playerAttack.IsParrying)
            {
                Damage(damageComponent.Damage);
            }
        }
    }

    public void Damage(int amount)
    {
        health -= amount;
        invincibleTimer = invincibilityTime;
        damageFlash.CallDamageFlash();
    }
}
