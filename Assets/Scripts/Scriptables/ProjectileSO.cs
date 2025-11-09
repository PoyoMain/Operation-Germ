using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSO", menuName = "Scriptable Objects/ProjectileSO")]
public class ProjectileSO : ScriptableObject
{
    [SerializeField] private int identifyAmount;
    [SerializeField] private float playerLaunchAngle;
    [SerializeField] private float playerLaunchForce;
    [SerializeField] private float playerCooldown;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private GameObject playerProjectile;

    public int IdentifyAmount => identifyAmount;
    public float PlayerLaunchAngle => playerLaunchAngle;
    public float PlayerLaunchForce => playerLaunchForce;
    public float PlayerCooldown => playerCooldown;
    public Sprite IconSprite => iconSprite;
    public GameObject EnemyProjectile => enemyProjectile;
    public GameObject PlayerProjectile => playerProjectile;
}
