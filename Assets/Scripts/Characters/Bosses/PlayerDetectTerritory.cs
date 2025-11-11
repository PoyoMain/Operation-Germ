using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerDetectTerritory : MonoBehaviour
{
    public bool PlayerDetected { get; private set; }
    public Transform PlayerTransform { get; private set; }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.parent.CompareTag("Player"))
        {
            PlayerDetected = true;
            PlayerTransform = collider.transform.parent;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.transform.parent.CompareTag("Player"))
        {
            PlayerDetected = false;
            PlayerTransform = collider.transform.parent;
        }
    }
}
