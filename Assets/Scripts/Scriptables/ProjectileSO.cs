using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSO", menuName = "Scriptable Objects/ProjectileSO")]
public class ProjectileSO : ScriptableObject
{
    [SerializeField] private int identifyAmount;
    [SerializeField] private int playerLaunchAngle;
    [SerializeField] private int playerLaunchForce;
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private GameObject playerProjectile;

    public int IdentifyAmount => identifyAmount;
    public int PlayerLaunchAngle => playerLaunchAngle;
    public int PlayerLaunchForce => playerLaunchForce;
    public GameObject EnemyProjectile => enemyProjectile;
    public GameObject PlayerProjectile => playerProjectile;
}
