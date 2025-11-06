using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool destroyOnHit = false;
    public int Damage => damage;

    public void HitSomething()
    {
        if (destroyOnHit) Destroy(this.gameObject);
    }
}
