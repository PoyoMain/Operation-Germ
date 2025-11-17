using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private bool dealKnockback = true;

    public int Damage => damage;
    public bool DealsKnockback => dealKnockback;
}
