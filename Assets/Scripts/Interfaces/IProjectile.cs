using UnityEngine;

public interface IProjectile
{
    ProjectileSO ProjectileData { get; }
    void Initialize(Vector2 direction, float power);
    void OnHit(Collision2D collision);
    void OnHit(Collider2D collider);
    void Destroy();
}
