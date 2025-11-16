using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int startHealth;
    [SerializeField] private float invincibilityTime;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventSO playerDamagedEventSO;
    [SerializeField] private VoidEventSO playerDeathEventSO;

    private int health;
    private float invincibleTimer;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private DamageFlash damageFlash;
    private DamageFlicker damageFlicker;

    public int Health => health;
    public float InvincibilityTime => invincibilityTime;
    private bool IsInvincible => invincibleTimer > 0;

    private void Awake()
    {
        TryGetComponent(out playerAttack);
        TryGetComponent(out playerMovement);
        TryGetComponent(out damageFlash);
        TryGetComponent(out damageFlicker);
    }

    private void Start()
    {
        health = startHealth;
    }

    private void FixedUpdate()
    {
        if (IsInvincible)
        {
            invincibleTimer -= Time.fixedDeltaTime;

            if (!IsInvincible) damageFlicker.EndDamageFlicker();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        TriggerBehavior(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        TriggerBehavior(collider);
    }

    private void TriggerBehavior(Collider2D collider)
    {
        if (collider.TryGetComponent(out DamageComponent damageComponent))
        {
            if (!IsInvincible && !playerAttack.IsParrying)
            {
                Damage(damageComponent.Damage);

                if (damageComponent.DealsKnockback) playerMovement.ApplyKnockback();
            }
        }
    }

    public void Damage(int amount)
    {
        health -= amount;
        invincibleTimer = invincibilityTime;
        if (damageFlash) damageFlash.CallDamageFlash();
        if (damageFlicker) damageFlicker.StartDamageFlicker();

        playerDamagedEventSO.RaiseEvent();

        if (health <= 0)
        {
            playerDeathEventSO.RaiseEvent();
        }
    }
}
