using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VoidEventSO", menuName = "Scriptable Objects/Events/VoidEventSO")]
public class VoidEventSO : ScriptableObject
{
    public event UnityAction OnEventRaised;

    [ContextMenu("Activate")]
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
