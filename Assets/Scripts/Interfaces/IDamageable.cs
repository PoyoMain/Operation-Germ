using UnityEngine;

public interface IDamageable
{
    int Health { get; }
    float InvincibilityTime { get; }
    void Damage(int amount);
}
