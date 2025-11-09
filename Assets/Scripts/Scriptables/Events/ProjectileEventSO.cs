using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ProjectileEventSO", menuName = "Scriptable Objects/Events/ProjectileEventSO")]
public class ProjectileEventSO : ScriptableObject
{
    public UnityAction<ProjectileSO> OnEventRaised;

    public void RaiseEvent(ProjectileSO projectile)
    {
        OnEventRaised?.Invoke(projectile);
    }
}
