using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    private float destroyTimer;

    private void OnEnable()
    {
        destroyTimer = destroyTime;
    }

    private void FixedUpdate()
    {
        destroyTimer -= Time.fixedDeltaTime;

        if (destroyTimer <= 0) Destroy(gameObject);
    }
}
