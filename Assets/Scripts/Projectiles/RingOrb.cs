using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class RingOrb : MonoBehaviour
{
    private Rigidbody2D rb;

    public UnityAction OnDestroyed;

    private void Awake()
    {
        TryGetComponent(out rb);
    }

    public void SetVelocity(Vector2 value)
    {
        rb.linearVelocity = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnDestroyed.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        OnDestroyed.Invoke();
        Destroy(gameObject);
    }
}
