using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DamageFlash))]
public class StagedEnemyHealth : MonoBehaviour
{
    [SerializeField] private Sprite[] healthSprites;
    [SerializeField] private int startHealth;
    [SerializeField] private float invincibilityTime;
    [SerializeField] private int dizzyInverval;

    public UnityAction onDizzyStageStarted;                                                       
    public UnityAction onHealthDepleted;                                                       

    private int health;
    private float invincibleTimer;
    private DamageFlash damageFlash;
    private SpriteRenderer sRend;

    public int Health => health;
    public float InvincibilityTime => invincibilityTime;
    private bool IsInvincible => invincibleTimer > 0;

    private void Awake()
    {
        TryGetComponent(out damageFlash);
        sRend = GetComponentInChildren<SpriteRenderer>();
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
            if (!IsInvincible)
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

        if (health <= 0) onHealthDepleted.Invoke();

        UpdateHealthSprite();
    }

    private void UpdateHealthSprite()
    {
        if (health <= 0) return;

        float healthPercentage = (float)health / startHealth;

        int spriteIndex = healthSprites.Length - Mathf.CeilToInt(healthPercentage * (healthSprites.Length - 1));

        if (sRend.sprite == healthSprites[spriteIndex]) return;

        sRend.sprite = healthSprites[spriteIndex];
        if (spriteIndex % dizzyInverval == 0) onDizzyStageStarted.Invoke();
    }
}
