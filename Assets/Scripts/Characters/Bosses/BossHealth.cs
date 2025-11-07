using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DamageFlash))]
public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int startHealth;
    [SerializeField] private float invincibilityTime;

    private int health;
    private float invincibleTimer;
    private DamageFlash damageFlash;

    public int Health => health;
    public float InvincibilityTime => invincibilityTime;
    private bool IsInvincible => invincibleTimer > 0;

    public UnityAction OnBossDefeated;

    private void Awake()
    {
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
            if (collision.CompareTag("Player") && !IsInvincible)
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

        if (health <= 0) OnBossDefeated?.Invoke();
    }
}
