using UnityEngine;

public interface IDamageable
{
    int Health { get; }
    void Damage();
}
