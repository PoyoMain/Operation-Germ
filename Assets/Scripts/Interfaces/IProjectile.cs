using UnityEngine;

public interface IProjectile
{
    void OnHit(Collision2D collision);
    void Destroy();
}
