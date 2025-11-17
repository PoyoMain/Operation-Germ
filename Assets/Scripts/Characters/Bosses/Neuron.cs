using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Neuron : MonoBehaviour
{
    [Header("Listen Events")]
    [SerializeField] private VoidEventSO neuronHitSO;

    [Header("Audio")]
    [SerializeField] private SFXPlayer hitSFX;

    private Animator anim;

    private void Awake()
    {
        TryGetComponent(out anim);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        hitSFX.Play();
        neuronHitSO.RaiseEvent();
        anim.SetTrigger("Activate");
    }
}
