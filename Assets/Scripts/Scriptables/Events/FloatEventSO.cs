using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloatEventSO", menuName = "Scriptable Objects/Events/FloatEventSO")]
public class FloatEventSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;

    public void RaiseEvent(float value)
    {
        OnEventRaised?.Invoke(value);
    }
}
